using Prometheus.Infrastructure.Component.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Component
{
    public abstract class ComponentOptions<TOptions> where TOptions : ComponentOptions<TOptions>
    {
        public virtual string OptionsName => this.GetType().Name;// { get; }

        public DbConfiguration DbConfiguration { get; set; }

        public TOptions Initialize(TOptions options)
        {
            var opts = SetOptionsProperties(options);
            opts.DbConfiguration = options.DbConfiguration;
            return opts;
        }

        public abstract TOptions SetOptionsProperties(TOptions options);
    }
}
