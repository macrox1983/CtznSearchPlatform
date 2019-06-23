using System;

namespace Prometheus.Presentation
{
    public class ErrorVm
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}