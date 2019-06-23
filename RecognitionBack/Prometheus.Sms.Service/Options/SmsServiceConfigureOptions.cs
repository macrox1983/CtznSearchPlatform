using Microsoft.Extensions.Configuration;
using Prometheus.Infrastructure.Component;

namespace Prometheus.Sms.Service.Options
{
    public class SmsServiceConfigureOptions : ComponentConfigureOptions<SmsServiceOptions>
    {
        public SmsServiceConfigureOptions(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
