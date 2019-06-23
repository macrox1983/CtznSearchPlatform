using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Component.Configuration
{
    /// <summary>
    /// Класс для вычитывания настроек из конфигурационного файла приложения appsettings.json
    /// </summary>
    public class DbConfiguration
    {
        public string MigrationAssembly { get; set; }

        public string ConnectionString { get; set; }

        public string DbConfigurator { get; set; }
    }
}
