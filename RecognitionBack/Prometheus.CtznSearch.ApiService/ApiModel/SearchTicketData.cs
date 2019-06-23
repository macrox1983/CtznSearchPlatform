using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.CtznSearch.ApiService.ApiModel
{
    public class SearchTicketData
    {
        public byte[] LostPersonPhoto { get; set; }

        public string LostPersonName { get; set; }

        public DateTime LostPersonBirthDate { get; set; }

        public bool IsMale { get; set; }

        public int LostPersonGrowth { get; set; } //рост

        public int LostPersonWeight { get; set; } //вес

        public string LostPersonEyeColor { get; set; }

        public string LostPersonHairColor { get; set; }

        public string LostPersonDescription { get; set; }

        public decimal LastLongitude { get; set; }

        public decimal LastLatitude { get; set; }

        public DateTime LostDateTime { get; set; }

        public Guid UserId { get; set; }
    }
}
