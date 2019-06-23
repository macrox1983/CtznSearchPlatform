using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Model
{
    public class FaceRecognitionHistory
    {
        public Guid FaceRecognitionHistoryId { get; set; }

        public Guid SearchTicketId { get; set; }

        public string Message { get; set; }
    }
}
