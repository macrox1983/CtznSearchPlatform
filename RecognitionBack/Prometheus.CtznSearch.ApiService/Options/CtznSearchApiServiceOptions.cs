using Prometheus.Infrastructure.Component;

namespace Prometheus.CtznSearch.ApiService.Options
{
    public class CtznSearchApiServiceOptions : ComponentOptions<CtznSearchApiServiceOptions>
    {     
        public string SearchTicketImagesFolder { get; set; }

        public string RequestUriAfterDetect { get; set; }

        public override string OptionsName => "CtznSearchApiServiceOptions";

        public override CtznSearchApiServiceOptions SetOptionsProperties(CtznSearchApiServiceOptions options)
        {
            DbConfiguration = options.DbConfiguration;
            SearchTicketImagesFolder = options.SearchTicketImagesFolder;
            RequestUriAfterDetect = options.RequestUriAfterDetect;
            return this;
        }
    }
}
