﻿namespace TestOkur.Notification.Infrastructure.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using TestOkur.Notification.Dtos;
    using TestOkur.Notification.Infrastructure.Data;

    public class SmsServiceLoggingHandler : DelegatingHandler
    {
        private readonly ISmsRepository _smsRepository;

        public SmsServiceLoggingHandler(ISmsRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var sms = (Sms)request.Properties["sms"];
            var requestDateTimeUtc = DateTime.UtcNow;
            var serviceRequest = request.Content != null 
                ? await request.Content.ReadAsStringAsync(cancellationToken) 
                : request.RequestUri.ToString();
            var response = await base.SendAsync(request, cancellationToken);
            sms.ServiceRequest = serviceRequest;
            sms.ServiceResponse = await response.Content.ReadAsStringAsync();
            sms.RequestDateTimeUtc = requestDateTimeUtc;
            sms.ResponseDateTimeUtc = DateTime.UtcNow;
            sms.Status = response.IsSuccessStatusCode ? SmsStatus.Successful : SmsStatus.Failed;

            await _smsRepository.UpdateSmsAsync(sms);
            return response;
        }
    }
}
