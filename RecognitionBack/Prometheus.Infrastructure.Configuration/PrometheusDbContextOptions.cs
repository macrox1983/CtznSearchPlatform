using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Configuration
{
    public class PrometheusDbContextOptions
    {
        public PrometheusDbContextOptions(DbContextOptions dbContextOptions)
        {
            CurrentDbContextOptions = dbContextOptions;
        }

        public DbContextOptions CurrentDbContextOptions { get; }
    }
}
