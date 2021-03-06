﻿namespace TestOkur.WebApi.Application.Exam.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Paramore.Brighter;
    using TestOkur.Data;
    using TestOkur.Infrastructure.CommandsQueries;

    public sealed class DeleteExamCommandHandler : RequestHandlerAsync<DeleteExamCommand>
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IBus _bus;

        public DeleteExamCommandHandler(IBus bus, IApplicationDbContextFactory dbContextFactory)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _dbContextFactory = dbContextFactory;
        }

        [ClearCache(2)]
        public override async Task<DeleteExamCommand> HandleAsync(
            DeleteExamCommand command,
            CancellationToken cancellationToken = default)
        {
            await using (var dbContext = _dbContextFactory.Create(command.UserId))
            {
                var exam = await GetAsync(dbContext, command, cancellationToken);

                if (exam != null)
                {
                    dbContext.Remove(exam);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    await PublishEventAsync(command.ExamId, cancellationToken);
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private Task PublishEventAsync(int id, CancellationToken cancellationToken)
        {
            return _bus.Publish(
                new ExamDeleted(id),
                cancellationToken);
        }

        private Task<Domain.Model.ExamModel.Exam> GetAsync(
            ApplicationDbContext dbContext,
            DeleteExamCommand command,
            CancellationToken cancellationToken)
        {
            return dbContext.Exams.FirstOrDefaultAsync(
                l => l.Id == command.ExamId &&
                     EF.Property<int>(l, "CreatedBy") == command.UserId,
                cancellationToken);
        }
    }
}
