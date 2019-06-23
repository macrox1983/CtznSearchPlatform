using Microsoft.Extensions.Configuration;
using Prometheus.Infrastructure.Component;

namespace Prometheus.CtznSearch.ApiService.Options
{
    public class CtznSearchApiServiceConfigureOptions : ComponentConfigureOptions<CtznSearchApiServiceOptions>
    {
        public CtznSearchApiServiceConfigureOptions(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
