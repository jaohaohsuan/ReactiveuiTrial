using System;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using Autofac.Core.Lifetime;
using Reactive.EventAggregator;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class ContainerUpdatedEvent
    {
        
    }

    public class AppBootstrapper : IDisposable
    {
        private IEventAggregator _eventAggregator =  new EventAggregator();
        private readonly UseAutofacServiceLocator _configServiceLocator = new UseAutofacServiceLocator();

        public AppBootstrapper(IContainer testContainer = null)
        {
            OnStartUp = Observable.Create<ILifetimeScope>(observer => Scheduler.Default.Schedule(() =>
            {
                try
                {
                    var root = testContainer ?? CreateRootContainer();

                    //sync new components registration
                    _eventAggregator.GetEvent<ComponentsRegistedEvent>().Subscribe(e =>
                    {
                        e.Update(root);
                        _eventAggregator.Publish(new ContainerUpdatedEvent());
                    });
                    
                    Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(h => root.ChildLifetimeScopeBeginning += h, h => root.ChildLifetimeScopeBeginning -= h)
                              .Select(o=> o.EventArgs.LifetimeScope)
                              .ObserveOn(Scheduler.Default).Subscribe(_configServiceLocator);

                    observer.OnNext(root.BeginLifetimeScope());
                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }));

            _configServiceLocator.OnRegisted.Subscribe(buffer => _eventAggregator.Publish(new ComponentsRegistedEvent(buffer)));
        }

        public IObservable<ILifetimeScope> OnStartUp { get; private set; }

        public void Dispose()
        {
        }

        private IContainer CreateRootContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(_eventAggregator).As<IEventAggregator>().As<EventAggregator>();
            builder.RegisterInstance(new RoutingState()).As<IRoutingState>();
            builder.RegisterType<ShellViewModel>().As<IScreen>().As<ShellViewModel>();
            builder.RegisterType<Shell>().PropertiesAutowired();

            return builder.Build();
        }
    }
}