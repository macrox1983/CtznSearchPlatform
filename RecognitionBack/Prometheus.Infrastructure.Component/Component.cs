using Autofac;
using Autofac.Features.AttributeFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component
{
    public abstract class Component : Autofac.Module, IComponent
    {
        public abstract string ComponentName { get; }

        public abstract string ComponentDescription { get; }

        public List<IComponentHostedService> HostedServices { get; set; }

        public Assembly ComponentAssembly { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            ConfigureComponent(new ComponentBuilder(builder));
            builder.RegisterInstance(this).Named<Component>(this.ComponentName);
            builder.Register(context=> 
            {
                var component = context.ResolveNamed<Component>(this.ComponentName);
                component.HostedServices = context.ResolveNamed<IEnumerable<IComponentHostedService>>(component.GetType().Name).ToList();                
                return component;
            }).As<IComponent>();
        }

        protected abstract void ConfigureComponent(IComponentBuilder builder);

        public string GetComponentState()
        {
            return HostedServices.Aggregate("", (str, hs)=>str+hs.GetState()+Environment.NewLine);
        }

        public async Task Pause()
        {
            HostedServices.ForEach(async hs=> await hs.PauseAsync());
        }

        public async Task Stop()
        {
            HostedServices.ForEach(async hs=>await hs.StopAsync(CancellationToken.None));
        }

        public async Task Start()
        {
            HostedServices.ForEach(async hs=>await hs.StartAsync(CancellationToken.None));
        }
    }
}
