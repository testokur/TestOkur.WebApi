﻿using TestOkur.Report.Integration.Tests.Common;

namespace TestOkur.Report.Integration.Tests.Consumers
{
    using AutoFixture;
    using FluentAssertions;
    using MassTransit;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TestOkur.Contracts.Classroom;
    using TestOkur.Optic.Form;
    using TestOkur.Report.Consumers;
    using TestOkur.Report.Infrastructure.Repositories;
    using TestOkur.Serialization;
    using TestOkur.Test.Common;
    using Xunit;

    public class ClassroomUpdatedConsumerShould : ConsumerTest, IClassFixture<WebApplicationFactory>
    {
        private readonly WebApplicationFactory _webApplicationFactory;

        public ClassroomUpdatedConsumerShould(WebApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Theory(Skip = "Fix it later")]
        [TestOkurAutoData]
        public async Task UpdateClassroomName(int userId, int examId, int classroomId,
            string classroom, int newGrade, string newClassName, IFixture fixture)
        {
            var client = _webApplicationFactory.CreateClientWithUserId(userId);
            var forms = GenerateStudentForms(fixture, examId, userId, classroomId, classroom);
            var response = await client.PostAsync(ApiPath, forms.ToJsonContent());
            response.EnsureSuccessStatusCode();
            var studentOpticalForms = await GetListAsync<StudentOpticalForm>(client, examId);
            studentOpticalForms.Should()
                .HaveCount(2)
                .And
                .NotContain(s => s.ClassroomId != classroomId);
            var repository = _webApplicationFactory.Services.GetService(typeof(IStudentOpticalFormRepository));
            var consumer = new ClassroomUpdatedConsumer(repository as IStudentOpticalFormRepository);
            var context = Substitute.For<ConsumeContext<IClassroomUpdated>>();
            context.Message.ClassroomId.Returns(classroomId);
            context.Message.Grade.Returns(newGrade);
            context.Message.Name.Returns(newClassName);
            await consumer.Consume(context);
            studentOpticalForms = await GetListAsync<StudentOpticalForm>(client, examId);
            studentOpticalForms.Select(s => s.Classroom)
                .Distinct()
                .Should()
                .HaveCount(1)
                .And
                .Contain($"{newGrade}/{newClassName}");
        }

        private List<StudentOpticalForm> GenerateStudentForms(
            IFixture fixture,
            int examId,
            int userId,
            int classroomId,
            string classroom)
        {
            var forms = new List<StudentOpticalForm>
            {
                GenerateStudentForm(fixture, examId, userId),
                GenerateStudentForm(fixture, examId, userId),
            };

            foreach (var form in forms)
            {
                form.ClassroomId = classroomId;
                form.Classroom = classroom;
            }

            return forms;
        }
    }
}
