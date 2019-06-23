using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Model
{
    public class SearchParticipant
    {
        public Guid SearchParticipantId { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser Participant { get; set; }

        public Guid SearchTicketId { get; set; }

        public virtual SearchTicket Ticket { get; set; }

        //видел
        //не видел, но помогу
        //не видел и занят
        public int ParticipantSearchStatus { get; set; }
    }
}
