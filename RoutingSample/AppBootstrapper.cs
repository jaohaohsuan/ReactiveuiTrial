using System;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Core.Lifetime;
using NLog;
using Reactive.EventAggregator;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class ContainerUpdatedEvent
    {
        public ContainerUpdatedEvent()
        {

        }
    }

    public class OpenNewLifetimeScope
    {
        
    }

    public class AppBootstrapper : IDisposable
    {
        private IEventAggregator _eventAggregator = new EventAggregator();
        private readonly UseAutofacServiceLocator _configServiceLocator = new UseAutofacServiceLocator();
        private readonly Logger _logger;

        public IObservable<ILifetimeScope> OnStartUp { get; private set; }

        public AppBootstrapper(IContainer testContainer = null)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            OnStartUp = Observable.Create<ILifetimeScope>(observer =>
            {
                var d = new CompositeDisposable();
                try
                {
                    var lifetimeScope = Config(testContainer ?? CreateRootContainer()).BeginLifetimeScope();

                    d.Add(Disposable.Create(() =>
                    {
                        lifetimeScope.Dispose();
                        _logger.Info("Startup lifetimeScope disposed");
                    }));

                    observer.OnNext(lifetimeScope);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                return d;
            });

            _configServiceLocator.OnRegisted.Subscribe(buffer => _eventAggregator.Publish(new ComponentsRegistedEvent(buffer)));
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

        private IContainer Config(IContainer root)
        {
            _eventAggregator.GetEvent<OpenNewLifetimeScope>().ObserveOn(Scheduler.Default).Subscribe(_ =>
            {
                root.BeginLifetimeScope();
            });

            //components registration handling
            _eventAggregator.GetEvent<ComponentsRegistedEvent>().Subscribe(e =>
            {
                e.Update(root);
                if(e.RaiseContainerEUpdateEvent)
                    _eventAggregator.Publish(new ContainerUpdatedEvent());
            });

            //child lifetimeScope beginning handling
            Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(h => root.ChildLifetimeScopeBeginning += h, h => root.ChildLifetimeScopeBeginning -= h)
                      .Select(o => o.EventArgs.LifetimeScope)
                      .ObserveOn(Scheduler.Default).Subscribe(_configServiceLocator);

            return root;
        }
        public void Dispose()
        {

        }

    }
}