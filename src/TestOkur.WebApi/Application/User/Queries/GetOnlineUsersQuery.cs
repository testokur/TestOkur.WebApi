﻿namespace TestOkur.WebApi.Application.User.Queries
{
    using System.Collections.Generic;
    using TestOkur.Infrastructure.CommandsQueries;

    public class GetOnlineUsersQuery :
        QueryBase<IEnumerable<string>>,
        ISkipLogging
    {
        private GetOnlineUsersQuery()
        {
        }

        public static GetOnlineUsersQuery Default { get; } = new GetOnlineUsersQuery();
    }
}
