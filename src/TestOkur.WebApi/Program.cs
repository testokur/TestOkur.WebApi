﻿namespace TestOkur.WebApi
{
	using System;
	using System.Net;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Logging;
	using TestOkur.Data;
	using TestOkur.Infrastructure.Extensions;

	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var host = BuildWebHost(args);
			await host.MigrateDbContextAsync<ApplicationDbContext>(async (context, services) =>
			{
				await DbInitializer.CreateLogTableAsync(services);
				await DbInitializer.SeedAsync(context);
			});

			host.Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSentry(options =>
				{
					options.Release = "qa";
					options.MaxBreadcrumbs = 200;
					options.HttpProxy = null;
					options.DecompressionMethods = DecompressionMethods.None;
					options.MaxQueueItems = 100;
					options.ShutdownTimeout = TimeSpan.FromSeconds(5);
				})
				.ConfigureLogging((hostingContext, logging) =>
				 {
					 logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
					 logging.AddConsole();
					 logging.AddDebug();
				 }).Build();
	}
}
