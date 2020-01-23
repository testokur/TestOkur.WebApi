﻿namespace TestOkur.Report.Domain.Optic.Answerkey
{
    public class QuestionAnswer
    {
        public int QuestionNo { get; set; }

        public int QuestionNoBookletB { get; set; }

        public int QuestionNoBookletC { get; set; }

        public int QuestionNoBookletD { get; set; }

        public byte Answer { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public QuestionAnswerCancelAction QuestionAnswerCancelAction { get; set; }
    }
}
