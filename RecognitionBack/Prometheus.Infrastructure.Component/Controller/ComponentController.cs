using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public abstract class ComponentController:Controller
    {

    }
}
