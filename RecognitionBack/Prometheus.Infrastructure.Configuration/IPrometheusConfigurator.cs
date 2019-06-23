using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Configuration
{

    public interface IPrometheusConfigurator
    {
        IPrometheusDbConfigurator GetDbConfigurator();
    }
}
