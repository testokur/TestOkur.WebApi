﻿namespace TestOkur.Report.Models
{
    using System;

    public class ReportRequest
    {
        public int LessonId { get; set; }

        public int StudentId { get; set; }

        public string StudentFullName { get; set; }

        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public DateTime RequestDateTimeUtc { get; set; }

        public DateTime ResponseDateTimeUtc { get; set; }

        public string ReportType { get; set; }

        public string ExamName { get; set; }

        public string ExamType { get; set; }

        public int ExamId { get; set; }

        public string Classroom { get; set; }

        public string Booklet { get; set; }

        public string ExportType { get; set; }
    }
}
