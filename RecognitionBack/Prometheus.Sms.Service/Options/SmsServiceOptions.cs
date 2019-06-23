using Prometheus.Infrastructure.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Sms.Service.Options
{
    public class SmsServiceOptions : ComponentOptions<SmsServiceOptions>
    {
        public override string OptionsName => nameof(SmsServiceOptions);

        public string SendSmsApiUrl { get; set; }

        public string ProjectName { get; set; }

        public string SenderName { get; set; }

        public string ApiKey { get; set; }

        public override SmsServiceOptions SetOptionsProperties(SmsServiceOptions options)
        {
            SenderName = options.SenderName;
            SendSmsApiUrl = options.SendSmsApiUrl;
            ProjectName = options.ProjectName;
            ApiKey = options.ApiKey;
            return this;
        }
    }
}
