using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Haka.CtznSearch.Front.Models
{
    public class SearchCameraMatch
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
