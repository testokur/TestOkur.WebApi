﻿namespace TestOkur.Notification.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using TestOkur.Contracts.User;
    using TestOkur.Notification.Infrastructure;
    using TestOkur.Notification.Models;

    internal class ResetPasswordTokenGeneratedConsumer
        : IConsumer<IResetPasswordTokenGenerated>
    {
        private readonly INotificationFacade _notificationFacade;

        public ResetPasswordTokenGeneratedConsumer(INotificationFacade notificationFacade)
        {
            _notificationFacade = notificationFacade;
        }

        public Task Consume(ConsumeContext<IResetPasswordTokenGenerated> context)
        {
            return _notificationFacade.SendEmailAsync(
                context.Message,
                Template.PasswordResetEmailUser,
                context.Message.Email);
        }
    }
}
