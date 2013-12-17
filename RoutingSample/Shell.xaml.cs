using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
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

            RegisterCompoments = Observable.Create<Unit>(observer => Scheduler.Default.Schedule(() =>
            {
                try
                {
                    var builder = new ContainerBuilder();

                    builder.RegisterType<WelcomeViewModel>().InstancePerLifetimeScope();
                    builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>().InstancePerLifetimeScope();

                    builder.RegisterType<NextPage1ViewModel>().InstancePerLifetimeScope();
                    builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>().InstancePerLifetimeScope();

                    eventAggregator.Publish(new BatchComponentsRegistedEvent(builder));

                    observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }));

            GoToDefaultPage = Observable.Create<Unit>(observer => Scheduler.CurrentThread.Schedule(() =>
                {
                    {
                        try
                        {
                            _router.Navigate.Go<WelcomeViewModel>();
                            _router.Navigate.Go<NextPage1ViewModel>();
                            observer.OnCompleted();
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                        }
                    }
                })
            );
        }

        public IObservable<Unit> RegisterCompoments { get; private set; }

        public IObservable<Unit> GoToDefaultPage { get; set; }

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
            Application.Current.Dispatcher.UnhandledException += (sender, e) =>
            {
                var log = log4net.LogManager.GetLogger(typeof(Shell));
                log.Error(e.Exception);
            };

            InitializeComponent();

            //Observable.Create(observer => {  });

            
            AppBootstrapper.LifetimeScopeBeginningSource.ObserveOnDispatcher().Subscribe(_ =>
            { 
                //_.EventArgs.LifetimeScope.Resolve<IRoutingState>(); // RxApp.GetService<IRoutingState>();
                ViewHost.Router = _.EventArgs.LifetimeScope.Resolve<IRoutingState>();
                var vm = _.EventArgs.LifetimeScope.Resolve<ShellViewModel>(); //_.EventArgs.LifetimeScope.Resolve<ShellViewModel>();

                    vm.RegisterCompoments.CombineLatest(vm.GoToDefaultPage, (unit, unit1) =>
                    {
                        return Unit.Default;
                    }).Retry().Subscribe(x =>
                    {

                    });

                //try
                //{
                    
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
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