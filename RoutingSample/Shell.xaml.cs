using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Autofac;
using Autofac.Core.Lifetime;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    public class ShellViewModel : RoutingState
    {
    }

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IScreen, IViewFor<ShellViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(null));

        public Shell()
        {

            DataContext = Router = new ShellViewModel();

            var builder = new ContainerBuilder();

            builder.RegisterType<WelcomeViewModel>();
            builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>();

            builder.RegisterType<NextPage1ViewModel>();
            builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>();

            builder.RegisterInstance(this).As<IScreen>();

            var container = builder.Build();

            var lifetimeScopeObs = Observable.FromEventPattern<LifetimeScopeBeginningEventArgs>(
                ev => container.ChildLifetimeScopeBeginning += ev,
                ev => container.ChildLifetimeScopeBeginning -= ev);

          
            var registerTypesObserver = new Subject<Tuple<Type, Type, string>>();
            registerTypesObserver.Buffer(TimeSpan.FromMilliseconds(5)).Where(buffer => buffer.Any()).Select(buffer=>
                {
                    var b = new ContainerBuilder();

                    foreach (var tuple in buffer)
                    {
                        var key = tuple.Item3;
                        var concreteType = tuple.Item1;
                        var interfaceType = tuple.Item2;

                        if (key != null)
                            b.RegisterType(concreteType).Named(key, interfaceType);
                        else
                            b.RegisterType(concreteType).As(interfaceType);
                    }
                    return b;
                }).Subscribe(b=> b.Update(container));

            lifetimeScopeObs.Subscribe(new LifetimeScopeContainer(registerTypesObserver));
             
          
            container.BeginLifetimeScope();

            InitializeComponent();
            ViewHost.Router = Router;
            
            Router.Navigate.Go<WelcomeViewModel>();
            Router.Navigate.Go<NextPage1ViewModel>();
        }

        public IRoutingState Router { get; private set; }

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (ShellViewModel)value; } }

    }

   
}