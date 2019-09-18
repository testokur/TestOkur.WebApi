﻿namespace TestOkur.WebApi.Application.Contact
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Paramore.Brighter;
    using TestOkur.Data;
    using TestOkur.Domain.Model.StudentModel;
    using TestOkur.Infrastructure.Cqrs;

    public sealed class DeleteContactCommandHandler : RequestHandlerAsync<DeleteContactCommand>
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;

        public DeleteContactCommandHandler(IApplicationDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        [ClearCache(2)]
        public override async Task<DeleteContactCommand> HandleAsync(
            DeleteContactCommand command,
            CancellationToken cancellationToken = default)
        {
            using (var dbContext = _dbContextFactory.Create(command.UserId))
            {
                var contact = await GetContactAsync(dbContext, command, cancellationToken);

                if (contact != null)
                {
                    dbContext.Remove(contact);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<Contact> GetContactAsync(
            ApplicationDbContext dbContext,
            DeleteContactCommand command,
            CancellationToken cancellationToken)
        {
            return await dbContext.Contacts.FirstOrDefaultAsync(
                l => l.Id == command.ContactId && EF.Property<int>(l, "CreatedBy") == command.UserId,
                cancellationToken);
        }
    }
}
