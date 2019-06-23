using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Haka.CtznSearch.Front.Models
{
    public enum TicketStatusEnum
    {        
        Open=0,
        Close=10
    }

    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Ages { get; set; }
        public string Description { get; set; }
        public DateTime Last { get; set; }
        public DateTime Created { get; set; }
        public int CreatorId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Zone { get; set; }
        public TicketStatusEnum Status { get; set; }

    }
}
