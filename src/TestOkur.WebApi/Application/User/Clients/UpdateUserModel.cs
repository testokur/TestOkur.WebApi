﻿namespace TestOkur.WebApi.Application.User.Clients
{
    using System;

    public class UpdateUserModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public int MaxAllowedDeviceCount { get; set; }

        public int MaxAllowedStudentCount { get; set; }

        public bool CanScan { get; set; }

        public int LicenseTypeId { get; set; }

        public DateTime? ExpiryDateUtc { get; set; }

        public bool? Active { get; set; }
    }
}
