using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prometheus.Host.ViewModels
{
    public class GenerateKeyVm
    {
        public GenerateKeyVm()
        {

        }

        [BindRequired]
        public string ComponentName { get; set; }

        [BindRequired]
        public string ApiKey { get; set; }
    }
}
