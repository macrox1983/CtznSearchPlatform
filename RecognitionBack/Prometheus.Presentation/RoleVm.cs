using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Presentation
{
    public class RoleVm
    {
        /// <summary>
        /// Ид роли
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Описание роли
        /// </summary>
        public string RoleDescription { get; set; }
    }
}
