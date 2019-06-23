using System;
using System.Collections.Generic;
using Prometheus.Infrastructure.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prometheus.Model
{
    public class Constant : IEntity<Guid>
    {
        [Key]
        public Guid ConstantId { get; set; }

        public string ConstantName { get; set; }

        public string ConstantValue { get; set; }

        public bool ReadOnly { get; set; }

        public Guid Key => ConstantId;
    }
}
