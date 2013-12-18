using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Autofac;
using Reactive.EventAggregator;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class ShellViewModel : IScreen
    {
        private readonly IRoutingState _router;

        public ShellViewModel(EventAggregator eventAggregator, IRoutingState router)
        {
            _router = router;

            OnCompomentsRegisted = Observable.Create<Unit>(observer => Scheduler.Default.Schedule(() =>
            {
                eventAggregator.GetEvent<ContainerUpdatedEvent>().Subscribe(_ =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                var builder = new ContainerBuilder();

                builder.RegisterType<WelcomeViewModel>().InstancePerLifetimeScope();
                builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>().InstancePerLifetimeScope();

                builder.RegisterType<NextPage1ViewModel>().InstancePerLifetimeScope();
                builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>().InstancePerLifetimeScope();

                eventAggregator.Publish(new ComponentsRegistedEvent(builder));
            }));
        }

        public IObservable<Unit> OnCompomentsRegisted { get; private set; }

        public void GoToDefaultPage()
        {
            _router.Navigate.Go<WelcomeViewModel>();
            _router.Navigate.Go<NextPage1ViewModel>();
        }

        public IRoutingState Router { get { return _router; } }
    }
}