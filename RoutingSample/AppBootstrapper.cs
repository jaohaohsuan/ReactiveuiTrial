using System;
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
    public class OpenNewLifetimeScope
    {
        private ContainerBuilder _builder;

        public OpenNewLifetimeScope(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public ContainerBuilder Builder { get { return _builder; } }
    }

    public class NewLifetimeScopeCreated
    {}

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
                    var lifetimeScope = Config(testContainer ?? CreateRootContainer()).BeginLifetimeScope("root");

                    _configServiceLocator.Upate(lifetimeScope);

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

            _configServiceLocator.OnRegisted.Subscribe(buffer => _eventAggregator.Publish(new RxAppServiceLocatorChangingEvent(buffer)));
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
            _eventAggregator.GetEvent<OpenNewLifetimeScope>().Subscribe(x =>
            {
                 x.Builder.Update(root);
                _configServiceLocator.Upate(root.BeginLifetimeScope(x.Builder));
                _logger.Info(string.Format("RxApp ServiceLocator setup success on thread {0}.", Thread.CurrentThread));
                //_eventAggregator.Publish(new NewLifetimeScopeCreated());
            });

            //components registration handling
            _eventAggregator.GetEvent<RxAppServiceLocatorChangingEvent>().Subscribe(e =>
            {
                e.Update(root);
                _logger.Info(string.Format("RxApp ServiceLocator Registrations updated on thread {0}.", Thread.CurrentThread));

            });

            return root;
        }
        public void Dispose()
        {

        }

    }
}