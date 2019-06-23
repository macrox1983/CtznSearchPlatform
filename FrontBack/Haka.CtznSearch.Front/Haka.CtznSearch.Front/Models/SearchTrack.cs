using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Haka.CtznSearch.Front.Models
{
    public class SearchTrack
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DateTime { get; set; }
    }
}
