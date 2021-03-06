﻿namespace TestOkur.WebApi
{
    using CacheManager.Core;
    using Dapper;
    using Dapper.FluentMap;
    using FluentValidation.AspNetCore;
    using HealthChecks.UI.Client;
    using IdentityModel;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Polly;
    using Polly.Extensions.Http;
    using Prometheus;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Reflection;
    using TestOkur.Common;
    using TestOkur.Common.Configuration;
    using TestOkur.Data;
    using TestOkur.Domain.Model.SmsModel;
    using TestOkur.Infrastructure.CommandsQueries;
    using TestOkur.Infrastructure.CommandsQueries.Extensions;
    using TestOkur.Infrastructure.Mvc;
    using TestOkur.Infrastructure.Mvc.Extensions;
    using TestOkur.Infrastructure.Mvc.Threading;
    using TestOkur.Serialization;
    using TestOkur.WebApi.Application.Captcha;
    using TestOkur.WebApi.Application.User.Clients;
    using TestOkur.WebApi.Configuration;
    using TestOkur.WebApi.Extensions;
    using TestOkur.WebApi.Infrastructure;
    using ConfigurationBuilder = CacheManager.Core.ConfigurationBuilder;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            Configuration.GetSection("RabbitMqConfiguration").Bind(RabbitMqConfiguration);
            Configuration.GetSection("OAuthConfiguration").Bind(OAuthConfiguration);
            Configuration.GetSection("ApplicationConfiguration").Bind(ApplicationConfiguration);
        }

        public IWebHostEnvironment Environment { get; }

        private IConfiguration Configuration { get; }

        private RabbitMqConfiguration RabbitMqConfiguration { get; } = new RabbitMqConfiguration();

        private OAuthConfiguration OAuthConfiguration { get; } = new OAuthConfiguration();

        private ApplicationConfiguration ApplicationConfiguration { get; } = new ApplicationConfiguration();

        public void ConfigureServices(IServiceCollection services)
        {
            AddOptions(services);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });
            services.AddMemoryCache();
            services.AddControllers(options =>
                {
                    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                    options.Filters.Add(new ValidateInputFilter());
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddCommandsAndQueries(Assembly.GetExecutingAssembly());
            AddHealthChecks(services);
            AddCache(services);
            AddDatabase(services);
            AddAuthentication(services);
            AddPolicies(services);
            AddMessageBus(services);
            AddHttpClients(services);
            RegisterServices(services);
            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseHttpMetrics();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics("/metrics-core");
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                });
                endpoints.MapDefaultControllerRoute();
            });
            InitializeFluentMappings();
        }

        private void AddMessageBus(IServiceCollection services)
        {
            var configure = services.BuildServiceProvider().GetService<Action<IRabbitMqReceiveEndpointConfigurator>>();

            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(
                    cfg =>
                    {
                        var uriStr = $"rabbitmq://{RabbitMqConfiguration.Uri}/{RabbitMqConfiguration.Vhost}";
                        cfg.Host(new Uri(uriStr), hc =>
                        {
                            hc.Username(RabbitMqConfiguration.Username);
                            hc.Password(RabbitMqConfiguration.Password);
                        });
                        if (configure != null)
                        {
                            cfg.ReceiveEndpoint(configure);
                        }
                    }));
            });

            if (Environment.IsDevelopment())
            {
                services.BuildServiceProvider()
                    .GetService<IBusControl>().Start();
            }
        }

        private void AddDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Postgres");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .EnableSensitiveDataLogging()
                .UseNpgsql(
                    connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly)).Options;
            services.AddSingleton(dbContextOptions);
            services.AddSingleton<IApplicationDbContextFactory, ApplicationDbContextFactory>();
            services.AddDbContext<ApplicationDbContext>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                return;
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = OAuthConfiguration.Authority;
                    options.RequireHttpsMetadata = OAuthConfiguration.RequireHttpsMetadata;
                    options.ApiName = OAuthConfiguration.ApiName;
                    options.JwtValidationClockSkew = TimeSpan.FromHours(24);
                });
        }

        private void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
            services.ConfigureAndValidate<ApplicationConfiguration>(Configuration);
            services.ConfigureAndValidate<OAuthConfiguration>(Configuration);

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ApplicationConfiguration>>().Value);

            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<OAuthConfiguration>>().Value);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ISmsCreditCalculator, SmsCreditCalculator>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddSingleton<ICommandQueryLogger, CommandQueryLogger>();
            services.AddHttpContextAccessor();
        }

        private void AddHealthChecks(IServiceCollection services)
        {
            var rabbitMqUri = $@"amqp://{RabbitMqConfiguration.Username}:{RabbitMqConfiguration.Password}@{RabbitMqConfiguration.Uri}/{RabbitMqConfiguration.Vhost}";
            services.AddHealthChecks()
                .AddNpgSql(Configuration.GetConnectionString("Postgres"))
                .AddUrlGroup(new Uri(ApplicationConfiguration.SeqUrl), "Seq")
                .AddUrlGroup(new Uri(ApplicationConfiguration.CaptchaServiceUrl + "hc"), "CaptchaService")
                .AddIdentityServer(new Uri(OAuthConfiguration.Authority))
                .AddRabbitMQ(rabbitMqUri, null, "rabbitmq");
        }

        private void AddCache(IServiceCollection services)
        {
            var cacheManagerConfig =
                ConfigurationBuilder.BuildConfiguration(cfg =>
                {
                    cfg.WithGzJsonSerializer()
                        .WithMicrosoftMemoryCacheHandle("runTimeMemory");
                });

            services.AddSingleton(cacheManagerConfig);
            services.AddCacheManager();

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("DataProtection-Keys"));
        }

        private void InitializeFluentMappings()
        {
            using (CrossProcessLockFactory.CreateCrossProcessLock())
            {
                DefaultTypeMap.MatchNamesWithUnderscores = true;
                if (FluentMapper.EntityMaps.IsEmpty)
                {
                    FluentMapper.Initialize(config =>
                    {
                        config.AddMapFromCurrentAssembly();
                    });
                }
            }
        }

        private void AddPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AuthorizationPolicies.Public,
                    policy => policy.RequireAssertion(context => context.User.Identity.IsAuthenticated));

                options.AddPolicy(
                    AuthorizationPolicies.Private,
                    policy => policy.RequireAssertion(context =>
                        context.User.IsInRole(Roles.Admin) ||
                        context.User.HasClaim(c => c.Type == JwtClaimTypes.ClientId &&
                                                   c.Value == Clients.Private)));
                options.AddPolicy(
                    AuthorizationPolicies.Customer,
                    policy => policy.RequireAssertion(context =>
                        context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Customer)));

                options.AddPolicy(
                    AuthorizationPolicies.Admin,
                    policy => policy.RequireRole(Roles.Admin));

                options.AddPolicy(
                    AuthorizationPolicies.Distributor,
                    policy => policy.RequireAssertion(context =>
                        context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Distributor)));
            });
        }

        private void AddHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<ICaptchaService, CaptchaService>(client =>
                {
                    client.BaseAddress = new Uri(ApplicationConfiguration.CaptchaServiceUrl);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All,
                });

            services.AddHttpClient<ISabitClient, SabitClient>(client =>
            {
                client.BaseAddress = new Uri(ApplicationConfiguration.SabitApiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(OAuthConfiguration.Authority);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
