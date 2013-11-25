using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Autofac;
using Autofac.Core.Lifetime;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class AppBootstrapper : IScreen, IDisposable
    {
        private static IContainer _container;

        private readonly IRoutingState _router;
        private readonly IObservable<EventPattern<LifetimeScopeBeginningEventArgs>> _lifetimeScopeObs;
        public IRoutingState Router { get { return _router; } }

        public AppBootstrapper(IContainer testContainer = null, IRoutingState testRouter = null)
        {
            _router = testRouter ?? new RoutingState();

            _container = testContainer ?? CreateDefaultContainer();

            _lifetimeScopeObs = Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(
                ev => _container.ChildLifetimeScopeBeginning += ev,
                ev => _container.ChildLifetimeScopeBeginning -= ev);

            _lifetimeScopeObs.Subscribe(new LifetimeScopeContainer(new BatchComponentsRegistration(_container)));
        }
        
        public static IContainer Container { get { return _container; } }


        IContainer CreateDefaultContainer()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterInstance<IScreen>(this);

            return builder.Build();
        }

        public void Dispose()
        {

        }
    }
}
