﻿namespace TestOkur.Notification.Infrastructure.Data
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TestOkur.Notification.Configuration;
    using TestOkur.Notification.Dtos;

    internal class SmsRepository : ISmsRepository
    {
        private readonly TestOkurContext _context;

        public SmsRepository(ApplicationConfiguration configuration)
        {
            _context = new TestOkurContext(configuration);
        }

        public Task AddManyAsync(IEnumerable<Sms> list) => _context.Smses.InsertManyAsync(list);

        public Task AddAsync(Sms sms) => _context.Smses.InsertOneAsync(sms);

        public Task UpdateSmsAsync(Sms sms)
        {
            var filter = Builders<Sms>.Filter.Eq(s => s.Id, sms.Id);
            return _context.Smses.ReplaceOneAsync(filter, sms);
        }

        public Task<List<Sms>> GetTodaysSmsesAsync()
        {
            var filter = Builders<Sms>.Filter.Gte(x => x.CreatedOnDateTimeUtc, DateTime.UtcNow.Date);

            return _context.Smses
                .Find(filter)
                .SortByDescending(e => e.CreatedOnDateTimeUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sms>> GetUserSmsesAsync(string userSubjectId)
        {
            var filter = Builders<Sms>.Filter.Eq(x => x.UserSubjectId, userSubjectId);

            return (await _context.Smses.Find(filter).ToListAsync()).OrderByDescending(s => s.CreatedOnDateTimeUtc);
        }

        public Task<List<Sms>> GetPendingOrFailedSmsesAsync()
        {
            var filter = Builders<Sms>.Filter.Gte(x => x.CreatedOnDateTimeUtc, DateTime.UtcNow.Date.AddDays(-2));
            filter &= Builders<Sms>.Filter.In(x => x.Status, new[] { SmsStatus.Pending, SmsStatus.Failed });

            return _context.Smses
                .Find(filter)
                .ToListAsync();
        }

        public Task<List<SmsLog>> GetUserSmsLogsAsync(int userId)
        {
            var filter = Builders<SmsLog>.Filter.Eq(x => x.UserId, userId);

            return _context.SmsLogs.Find(filter).ToListAsync();
        }
    }
}
