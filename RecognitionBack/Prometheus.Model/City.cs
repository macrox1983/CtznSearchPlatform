using System;
using System.Collections.Generic;
using Prometheus.Infrastructure.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prometheus.Model
{
    /// <summary>
    /// Города
    /// </summary>
    public class City : IEntity<Guid>
    {
        public City()
        {

        }
        /// <summary>
        /// Код города
        /// </summary>
        [Key]
        [Required]
        public Guid CityId { get; set; }

        public Guid Key => CityId;

        [MaxLength(3)]
        public string CityCode { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отметка об удалении (в справочниках отображаться не будет)
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
