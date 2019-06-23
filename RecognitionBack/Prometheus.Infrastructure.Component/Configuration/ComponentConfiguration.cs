using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Component
{
    /// <summary>
    /// Описание структуры конфигурации компонента из файла конфигурации appsettings.json
    /// </summary>
    public class ComponentConfiguration
    {
        public string ComponentName { get; set; }

        public string AssemblyName { get; set; }

        public ComponentConfiguration[] Dependencies { get; set; }
    }
}
