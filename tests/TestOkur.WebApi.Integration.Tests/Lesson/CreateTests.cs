﻿namespace TestOkur.WebApi.Integration.Tests.Lesson
{
    using FluentAssertions;
    using System.Threading.Tasks;
    using TestOkur.Common;
    using TestOkur.Serialization;
    using TestOkur.TestHelper.Extensions;
    using Xunit;

    public class CreateTests : LessonTest
    {
       [Fact(Skip = "Fix later")]
        public async Task When_Lesson_Exists_Then_BadRequestShouldBeReturned()
        {
            using var testServer = await CreateWithUserAsync();
            var client = testServer.CreateClient();
            var command = await CreateLessonAsync(client);
            var response = await client.PostAsync(ApiPath, command.ToJsonContent());
            await response.Should().BeBadRequestAsync(ErrorCodes.LessonExists);
        }

       [Fact(Skip = "Fix later")]
        public async Task When_ValidValuesArePosted_Then_LessonShouldBeCreated()
        {
            using var testServer = await CreateWithUserAsync();
            var client = testServer.CreateClient();
            var command = await CreateLessonAsync(client);

            (await GetLessonListAsync(client)).Should()
                .Contain(l => l.Name == command.Name);
        }
    }
}
