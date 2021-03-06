﻿namespace TestOkur.Notification.Infrastructure.Data
{
    using MongoDB.Driver;
    using TestOkur.Notification.Configuration;
    using TestOkur.Notification.Dtos;
    using TestOkur.Notification.Models;

    public class TestOkurContext
    {
        private readonly IMongoDatabase _database = null;

        public TestOkurContext(ApplicationConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            _database = client.GetDatabase(configuration.Database);
        }

        public IMongoCollection<Sms> Smses => _database.GetCollection<Sms>("Smses");

        public IMongoCollection<EMail> Emails => _database.GetCollection<EMail>("Emails");

        public IMongoCollection<SmsLog> SmsLogs => _database.GetCollection<SmsLog>("SmsLogs");
    }
}
