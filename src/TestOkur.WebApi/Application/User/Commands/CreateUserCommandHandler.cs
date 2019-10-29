﻿namespace TestOkur.WebApi.Application.User.Commands
{
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Paramore.Brighter;
    using Paramore.Darker;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TestOkur.Common;
    using TestOkur.Data;
    using TestOkur.Infrastructure.CommandsQueries;
    using TestOkur.WebApi.Application.Captcha;
    using TestOkur.WebApi.Application.User.Clients;
    using TestOkur.WebApi.Application.User.Events;
    using TestOkur.WebApi.Application.User.Queries;

    public sealed class CreateUserCommandHandler : RequestHandlerAsync<CreateUserCommand>
    {
        private readonly ICaptchaService _captchaService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IIdentityClient _identityClient;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly ISabitClient _sabitClient;

        public CreateUserCommandHandler(
            ICaptchaService captchaService,
            IPublishEndpoint publishEndpoint,
            IIdentityClient identityClient,
            IQueryProcessor queryProcessor,
            IApplicationDbContextFactory dbContextFactory,
            ISabitClient sabitClient)
        {
            _captchaService = captchaService ?? throw new ArgumentNullException(nameof(captchaService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _identityClient = identityClient ?? throw new ArgumentNullException(nameof(identityClient));
            _queryProcessor = queryProcessor;
            _dbContextFactory = dbContextFactory;
            _sabitClient = sabitClient;
        }

        [Idempotent(1)]
        [ClearCache(2)]
        public override async Task<CreateUserCommand> HandleAsync(
            CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            ValidateCaptcha(command);

            await using (var dbContext = _dbContextFactory.Create(command.UserId))
            {
                await ValidateReferrerAsync(dbContext, command);
                await EnsureUserDoesNotExistAsync(command, cancellationToken);
                await SaveToDatabaseAsync(dbContext, command, cancellationToken);
            }

            await RegisterUserAsync(command, cancellationToken);
            await PublishEventAsync(command, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task RegisterUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var licenseType = (await _sabitClient.GetLicenseTypesAsync())
                .First(l => l.Id == command.LicenseTypeId);
            var model = new CreateCustomerUserModel()
            {
                Email = command.Email,
                Id = command.Id.ToString(),
                CanScan = licenseType.CanScan,
                LicenseTypeId = command.LicenseTypeId,
                MaxAllowedDeviceCount = licenseType.MaxAllowedDeviceCount,
                MaxAllowedStudentCount = licenseType.MaxAllowedRecordCount,
                Password = command.Password,
            };
            await _identityClient.RegisterUserAsync(model, cancellationToken);
        }

        private async Task SaveToDatabaseAsync(
            ApplicationDbContext dbContext,
            CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            var city = await dbContext.Cities
                .Include(c => c.Districts)
                .FirstAsync(c => c.Id == command.CityId, cancellationToken);
            var district = city.Districts.First(d => d.Id == command.DistrictId);

            dbContext.Users.Add(command.ToDomainModel(city, district));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task EnsureUserDoesNotExistAsync(
            CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var users = await _queryProcessor.ExecuteAsync(new GetAllUsersQuery(), cancellationToken);
            if (users.Any(l => l.Email == command.Email))
            {
                throw new ValidationException(ErrorCodes.UserAlreadyExists);
            }
        }

        private async Task PublishEventAsync(
            CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish(
                new NewUserRegistered(
                    command.Email,
                    command.RegistrarFullName,
                    command.RegistrarPhone,
                    command.UserFirstName,
                    command.UserLastName,
                    command.SchoolName,
                    command.UserPhone,
                    command.LicenseTypeName,
                    command.DistrictName,
                    command.CityName,
                    command.Password,
                    command.Referrer), cancellationToken);
        }

        private async Task ValidateReferrerAsync(ApplicationDbContext dbContext, CreateUserCommand command)
        {
            if (string.IsNullOrEmpty(command.Referrer))
            {
                return;
            }

            if (!await dbContext.Users.AnyAsync(u => u.Email.Value == command.Referrer))
            {
                throw new ValidationException(ErrorCodes.ReferrerDoesNotExist);
            }

            if (command.Email == command.Referrer)
            {
                throw new ValidationException(ErrorCodes.SelfReferrerNotAllowed);
            }
        }

        private void ValidateCaptcha(CreateUserCommand command)
        {
            if (!_captchaService.Validate(command.CaptchaId, command.CaptchaCode))
            {
                throw new ValidationException(ErrorCodes.InvalidCaptcha);
            }
        }
    }
}
