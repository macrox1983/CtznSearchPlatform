using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Haka.CtznSearch.Front.Models
{
    public class SearchMessage
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
