using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Component
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class ComponentConfigureOptions<TOptions> : IConfigureOptions<TOptions> where TOptions: ComponentOptions<TOptions>
    {
        private readonly IConfiguration _configuration;

        public ComponentConfigureOptions(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public void Configure(TOptions options)
        {
            var configOptions = _configuration.GetSection(options.OptionsName).Get<TOptions>();
            options.Initialize(configOptions);
        }
    }
}
