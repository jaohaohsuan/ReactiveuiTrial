using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Autofac;
using Autofac.Core.Lifetime;
using Reactive.EventAggregator;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class AppBootstrapper : IDisposable
    {
        private readonly EventAggregator _eventAggregator;

        public AppBootstrapper(IContainer testContainer = null)
        {
            _eventAggregator = new EventAggregator();
            
            var container = testContainer ?? CreateDefaultContainer();

            _eventAggregator.GetEvent<ContainerBuilder>().Subscribe(builder =>
            {
                builder.Update(container);
            });
            
            var lifetimeScopeObs = Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(
                ev => container.ChildLifetimeScopeBeginning += ev,
                ev => container.ChildLifetimeScopeBeginning -= ev);

            lifetimeScopeObs.Subscribe(new LifetimeScopeContainer(new BatchComponentsRegistration(_eventAggregator)));

            container.BeginLifetimeScope();
        }

        IContainer CreateDefaultContainer()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterInstance(_eventAggregator);
            builder.RegisterInstance(new RoutingState()).As<IRoutingState>();
            builder.RegisterType<ShellViewModel>().As<IScreen>().As<ShellViewModel>();

            return builder.Build();
        }

        public void Dispose()
        {

        }
    }
}
