using Prometheus.Sms.Service.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Sms.Service
{
    public class MockDataRepository
    {
        public List<SmsInfo> GetSmsInfos()
        {
            return new List<SmsInfo>
            {
                new SmsInfo { Phone = "+79659069229", Name = "Сергей", BaggageStatus = "Поступил в багажное отделение"},
                new SmsInfo { Phone = "+79659069229", Name = "Сергей", BaggageStatus = "Прошел досмотр"},
                new SmsInfo { Phone = "+79659069229", Name = "Сергей", BaggageStatus = "Прошел сортировку"},
                new SmsInfo { Phone = "+79659069229", Name = "Сергей", BaggageStatus = "Погружен в ВС"},
            };
        }
    }
}
