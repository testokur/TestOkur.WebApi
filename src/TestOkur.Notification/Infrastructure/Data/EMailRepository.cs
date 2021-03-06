﻿namespace TestOkur.Notification.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using TestOkur.Notification.Configuration;
    using TestOkur.Notification.Models;

    public class EMailRepository : IEMailRepository
    {
        private readonly TestOkurContext _context;

        public EMailRepository(ApplicationConfiguration configuration)
        {
            _context = new TestOkurContext(configuration);
        }

        public Task AddAsync(EMail email)
        {
            return _context.Emails.InsertOneAsync(email);
        }

        public Task<List<EMail>> GetEmailsAsync(DateTime from, DateTime to)
        {
            var filter = Builders<EMail>.Filter.Gte(e => e.SentOnUtc, from.ToUniversalTime());
            filter &= Builders<EMail>.Filter.Lte(e => e.SentOnUtc, to.ToUniversalTime());

            return _context.Emails.Find(filter)
                .SortByDescending(e => e.SentOnUtc)
                .ToListAsync();
        }
    }
}
