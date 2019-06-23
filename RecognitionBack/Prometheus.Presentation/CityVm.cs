using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Presentation
{
    public class CityVm
    {
        /// <summary>
        /// Код города
        /// </summary>
        public Guid CityId { get; set; }
        //public List<CityVm> listCityVm { get; set; }

        /// <summary>
        ////Код города ИАТА
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        public string Name { get; set; }       
    }
}
