using Microsoft.AspNetCore.WebUtilities;
using Prometheus.Sms.Service.Abstractions;
using Prometheus.Sms.Service.DataModel;
using Prometheus.Sms.Service.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Prometheus.Sms.Service.SmsApiClient
{
    public class SmsClient : ISmsClient
    {
        private HttpClient _httpClient;
        private readonly SmsServiceOptions _options;

        public SmsClient(SmsServiceOptions options)
        {
            _httpClient = new HttpClient();
            _options = options;
        }

        public async void SendSms(SmsInfo smsInfo)
        {
            var url = _options.SendSmsApiUrl;
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
            {
                { "test", "1" },
                { "project", _options.ProjectName },
                { "apikey", _options.ApiKey },
                { "sender", "mainsms" /*_options.SenderName*/ },
                { "message", $"Уважаемый {smsInfo.Name}, ваш багаж {smsInfo.BaggageStatus}" },
                { "recipients", smsInfo.Phone.Replace("+7", "8") }
            });
            request.RequestUri = new Uri(url);            
            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responceContent = await response.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(responceContent))
                {

                }
            }
        }
    }
}
