using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Autofac;
using Autofac.Core.Lifetime;
using NLog;
using Reactive.EventAggregator;
using ReactiveUI;
using ReactiveUI.Routing;
using System.Linq;

namespace RoutingSample
{
    public class OpenNewLifetimeScopeCommand
    {
        private readonly Action<ContainerBuilder> _action;

        public OpenNewLifetimeScopeCommand(Action<ContainerBuilder> action)
        {
            _action = action;
        }

        public Action<ContainerBuilder> Action { get { return _action; } }
    }

    public class NewLifetimeScopeCreated
    {}

    public class AppBootstrapper : IDisposable
    {
        private IEventAggregator _eventAggregator = new EventAggregator();
        private UseAutofacServiceLocator _configServiceLocator;
        private readonly Logger _logger;

        public IObservable<ILifetimeScope> OnStartUp { get; private set; }

        public AppBootstrapper(IContainer testContainer = null)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            OnStartUp = Observable.Create<ILifetimeScope>(observer =>
            {
                try
                {
                    var container = testContainer ?? CreateRootContainer();
                    _configServiceLocator = new UseAutofacServiceLocator(container);
                    var d = new CompositeDisposable(RootSubscriptions(container));
                    
                    observer.OnNext(container);
                    return d;
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return Disposable.Empty;
                }
            });
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

        private IEnumerable<IDisposable> RootSubscriptions(IContainer container)
        {
            yield return _configServiceLocator.OnRegisted(
                Observer.Create<IList<ComponentRegistrationValue>>(buffer => _eventAggregator.Publish(new RxAppServiceLocatorChangingEvent(buffer))));

            yield return _eventAggregator.GetEvent<OpenNewLifetimeScopeCommand>().Subscribe(command =>
            {
                _configServiceLocator.Upate(container.BeginLifetimeScope(command.Action));
                _logger.Info(string.Format("RxApp ServiceLocator setup success on thread {0}.", Thread.CurrentThread));
            });

            yield return _eventAggregator.GetEvent<RxAppServiceLocatorChangingEvent>().Subscribe(e =>
            {
                e.Update(container);
                _logger.Info(string.Format("RxApp ServiceLocator Registrations updated on thread {0}.", Thread.CurrentThread));

            });

            yield return Disposable.Create(() =>
            {
                container.Dispose();
                _logger.Info("Startup root container disposed");
            });
        }

    
        public void Dispose()
        {

        }

    }
}