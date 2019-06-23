using Prometheus.Sms.Service.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Sms.Service.Abstractions
{
    public interface ISmsClient
    {
        void SendSms(SmsInfo smsInfo);
    }
}
