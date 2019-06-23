using Microsoft.AspNetCore.Mvc;
using Prometheus.Host.ViewModels;
using Prometheus.Infrastructure.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Host.Controllers
{
    [Microsoft.AspNetCore.Mvc.Controller]
    [Microsoft.AspNetCore.Mvc.Route("Components")]
    public class ComponentsController : Controller
    {
        private IList<IComponent> _components;
        public ComponentsController(IList<IComponent> components)
        {
            _components = components;
        }

        public string GetState()
        {
            return "Host available";
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("WorkedComponents")]
        public IActionResult Components()
        {
            return View(_components.Select(c => new ComponentVm { ComponentName = c.ComponentName, ComponentDescription = c.ComponentDescription, ComponentState = c.GetComponentState() }).ToList());
        }

        [HttpGet]        
        [Microsoft.AspNetCore.Mvc.Route("GenerateComponentKey")]
        public IActionResult GenerateComponentKey(string componentName)
        {
            return View(new GenerateKeyVm { ComponentName = componentName});
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Mvc.Route("GenerateComponentKey")]
        public IActionResult GenerateComponentKey([FromForm] GenerateKeyVm gen)
        {
            if(gen.ComponentName != null)
            {
                gen.ApiKey = "kdsjg";
            }
            return RedirectToAction(nameof(Components));
        }

        [Microsoft.AspNetCore.Mvc.Route("PauseComponent")]
        public async Task<IActionResult> PauseComponent(string componentName)
        {
            await _components.FirstOrDefault(c => c.ComponentName == componentName)?.Pause();
            return RedirectToAction(nameof(Components));
        }

        [Microsoft.AspNetCore.Mvc.Route("StopComponent")]
        public async Task<IActionResult> StopComponent(string componentName)
        {
            await _components.FirstOrDefault(c => c.ComponentName == componentName)?.Stop();
            return RedirectToAction(nameof(Components));
        }
    }
}
