﻿namespace TestOkur.Notification.Models
{
    public class Template
    {
        public static readonly Template DailyReportEmailAdmin =
            new Template(
                "GunlukRapor_Yonetici_Email",
                "TestOkur Gunluk Veriler - Yeni Sistem",
                "Admin_Daily_Report.html");

        public static readonly Template UserErrorAlertEmail =
            new Template(
                "UserErrorAlertEmail",
                "Yeni Kullanici Hata Mesaji Iletildi",
                "NewErrorMessageReceived.html");

        public static readonly Template AccountRegistrationEmailAdmin =
            new Template(
                "YeniLisansKaydi_Yonetici_Email",
                "Yeni Lisans Kaydı",
                "Admin_New_User.html");

        public static readonly Template SmsFailureEmailAdmin =
            new Template(
                "SmsHatasi_Yonetici_Email",
                "SMS Hatasi",
                "Admin_SMS_Failure.html");

        public static readonly Template LicenseExpirationNoticeEmailUser =
            new Template(
                "LisansSonaErmeBildirim_Kullanici_Email",
                "TestOkur Lisans Süreniz Dolmak Üzere",
                "LicenseExpirationNotice.html");

        public static readonly Template UserSubscriptionExtendedEmail =
            new Template(
                "UserSubscriptionExtendedEmail",
                "Lisans Yenileme",
                "UserSubscriptionExtended.html");

        public static readonly Template UserSubscriptionExtendedSms =
            new Template(
                "UserSubscriptionExtendedSms",
                "TestOkur",
                "UserSubscriptionExtended.html");

        public static readonly Template LicenseExpirationNoticeSms =
            new Template(
                "LicenseExpirationNoticeSms",
                "TestOkur",
                "LicenseExpirationNotice.html");

        public static readonly Template AccountActivationEmailUser =
            new Template(
                "LisansAktiflestirme_Kullanici_Email",
                "Lisans Bilgileriniz",
                "User_MembershipActivation.html");

        public static readonly Template AccountActivationSmsUser =
            new Template(
                "LisansAktiflestirme_Kullanici_Sms",
                "TestOkur",
                "User_MembershipActivation.html");

        public static readonly Template AccountRegistrationEmailUser =
            new Template(
                "YeniLisansKaydi_Kullanici_Email",
                "Lisans Kaydınız Alındı",
                "User_New_User.html");

        public static readonly Template AccountRegistrationSmsUser =
            new Template(
                "YeniLisansKaydi_Kullanici_Sms",
                "TestOkur",
                "User_New_User.html");

        public static readonly Template PasswordResetEmailUser =
            new Template(
                "ParolaSifirlama_Kullanici_Email",
                "TestOkur Parola Sifirla",
                "User_ResetPassword.html");

        public static readonly Template SmsCreditAddedEmailUser =
            new Template(
                "SmsPaketi_Kullanici_Email",
                "SMS Paketiniz",
                "User_SMSOrder.html");

        public static readonly Template SmsCreditAddedSmsUser =
            new Template(
                "SmsPaketi_Kullanici_Sms",
                "TestOkur",
                "User_SMSOrder.html");

        public static readonly Template ReferrerSmsCreditsAddedSms =
            new Template(
                "ReferrerSmsCreditsAddedSms",
                "TestOkur",
                "Referrer_SmsCredits_Added.html");

        public static readonly Template RefereeSmsCreditsAddedSms =
            new Template(
                "RefereeSmsCreditsAddedSms",
                "TestOkur",
                "Referee_SmsCredits_Added.html");

        public static readonly Template ReferrerSmsCreditsAddedEmail =
            new Template(
                "ReferrerSmsCreditsAddedSms",
                "TestOkur'dan Hediye SMS Kazandiniz!",
                "Referrer_SmsCredits_Added.html");

        public static readonly Template RefereeSmsCreditsAddedEmail =
            new Template(
                "RefereeSmsCreditsAddedSms",
                "TestOkur'dan Hediye SMS Kazandiniz!",
                "Referee_SmsCredits_Added.html");

        public Template(string name, string subject, string bodyPath)
        {
            Name = name;
            Subject = subject;
            BodyPath = bodyPath;
        }

        public string Name { get; }

        public string Subject { get; }

        public string BodyPath { get; }
    }
}
