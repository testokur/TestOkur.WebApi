﻿namespace TestOkur.WebApi.Application.Contact
{
    using System;
    using System.Collections.Generic;
    using TestOkur.Infrastructure.CommandsQueries;

    public class GetUserContactsQuery :
        QueryBase<IReadOnlyCollection<ContactReadModel>>,
        ICacheResult
    {
        public GetUserContactsQuery(int userId)
            : base(userId)
        {
        }

        public GetUserContactsQuery()
        {
        }

        public string CacheKey => $"Contacts_{UserId}";

        public TimeSpan CacheDuration => TimeSpan.FromHours(4);
    }
}
