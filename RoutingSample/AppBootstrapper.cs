using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Autofac;
using Autofac.Core.Lifetime;
using Reactive.EventAggregator;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class AppBootstrapper : IDisposable, IObserver<BatchComponentsRegistedEvent>
    {
        private readonly EventAggregator _eventAggregator;
        private static IContainer _container;

        public static IObservable<EventPattern<LifetimeScopeBeginningEventArgs>> LifetimeScopeBeginningSource;

        static AppBootstrapper()
        {
            _container = new ContainerBuilder().Build();

            LifetimeScopeBeginningSource = Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(
                ev => _container.ChildLifetimeScopeBeginning += ev,
                ev => _container.ChildLifetimeScopeBeginning -= ev);
        }

        public AppBootstrapper(IContainer testContainer = null)
        {
            _eventAggregator = new EventAggregator();

            Observable.Create<IContainer>(observer =>
            {
                try
                {
                    var root = testContainer ?? CreateDefaultContainer();

                    _eventAggregator.GetEvent<BatchComponentsRegistedEvent>().Subscribe(this);

                    LifetimeScopeBeginningSource.ObserveOn(Scheduler.Default).Subscribe(new LifetimeScopeContainer(new BatchComponentsRegistration(_eventAggregator)));

                    observer.OnNext(root);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                return Disposable.Empty;
            }).SubscribeOn(Scheduler.Default)
            .Subscribe(c =>
            {
                c.BeginLifetimeScope();
            });
        }

        IContainer CreateDefaultContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(_eventAggregator);
            builder.RegisterInstance(new RoutingState()).As<IRoutingState>();
            builder.RegisterType<ShellViewModel>().As<IScreen>().As<ShellViewModel>();

            builder.Update(_container);

            return _container;
        }

        public void Dispose()
        {

        }

        public void OnNext(BatchComponentsRegistedEvent value)
        {
            value.Update(_container);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
