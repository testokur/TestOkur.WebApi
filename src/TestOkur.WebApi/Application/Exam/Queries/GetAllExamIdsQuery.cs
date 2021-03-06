﻿namespace TestOkur.WebApi.Application.Exam.Queries
{
    using System.Collections.Generic;
    using TestOkur.Infrastructure.CommandsQueries;

    public class GetAllExamIdsQuery : QueryBase<IEnumerable<int>>
    {
        private GetAllExamIdsQuery()
        {
        }

        public static GetAllExamIdsQuery Default { get; } = new GetAllExamIdsQuery();
    }
}
