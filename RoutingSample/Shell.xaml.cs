using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Autofac;
using Autofac.Core.Lifetime;
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
            
            GoToDefaultPages = Observable.Create<Unit>(observer => Scheduler.Default.Schedule(() =>
           {
               try
               {
                   var builder = new ContainerBuilder();

                   builder.RegisterType<WelcomeViewModel>().InstancePerLifetimeScope();
                   builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>().InstancePerLifetimeScope();

                   builder.RegisterType<NextPage1ViewModel>().InstancePerLifetimeScope();
                   builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>().InstancePerLifetimeScope();

                   eventAggregator.Publish(builder);

                   _router.Navigate.Go<WelcomeViewModel>();
                   _router.Navigate.Go<NextPage1ViewModel>();

                   observer.OnCompleted();
               }
               catch (Exception ex)
               {
                   observer.OnError(ex);
               }
           }));
        }

        public IObservable<Unit> GoToDefaultPages { get; private set; }

        public IRoutingState Router { get { return _router; } }
    }



    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IViewFor<ShellViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(null));

        public Shell()
        {
            InitializeComponent();

            ViewHost.Router = RxApp.GetService<IRoutingState>();
            
            RxApp.GetService<ShellViewModel>().GoToDefaultPages.Retry().Subscribe(x =>
            {
                
            });
        }

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (ShellViewModel)value; } }

    }
}