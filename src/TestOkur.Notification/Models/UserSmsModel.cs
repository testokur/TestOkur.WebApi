﻿namespace TestOkur.Notification.Models
{
    using System;
    using TestOkur.Notification.Dtos;

    public class UserSmsModel
    {
        public UserSmsModel(Sms sms)
        {
            Phone = sms.Phone;
            Subject = sms.Subject;
            Body = sms.Body;
            Credit = sms.Credit;
            RequestDateTimeUtc = sms.RequestDateTimeUtc;
            ResponseDateTimeUtc = sms.ResponseDateTimeUtc;
            CreatedOnDateTimeUtc = sms.CreatedOnDateTimeUtc;
            Status = sms.Status;
        }

        public UserSmsModel()
        {
        }

        public DateTime CreatedOnDateTimeUtc { get; set; }

        public string Phone { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public int Credit { get; set; }

        public DateTime RequestDateTimeUtc { get; set; }

        public DateTime ResponseDateTimeUtc { get; set; }

        public SmsStatus Status { get; set; }
    }
}
