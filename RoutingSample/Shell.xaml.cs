using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using ReactiveUI;
using ReactiveUI.Routing;
using ReactiveUI.Xaml;

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
        public Shell()
        {
            ConfigIoc();  
            DataContext = Router = new ShellViewModel();
            InitializeComponent();

            ViewHost.Router = Router;

            //this.BindCommand(ViewModel, x => x.NavigateBack, v=> v.NavigateBack, "Click");
            Router.Navigate.Go<WelcomeViewModel>();
            Router.Navigate.Go<NextPage1ViewModel>();
        }

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ShellViewModel)value; }
        }

        public IRoutingState Router { get; private set; }

        protected void ConfigIoc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WelcomeViewModel>();
            builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>();

            builder.RegisterType<NextPage1ViewModel>();
            builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>();

            builder.RegisterInstance(this).As<IScreen>();

            var container = builder.Build();

            RxApp.ConfigureServiceLocator(
                            (iface, contract) =>
                            {
                                if (contract != null) return container.ResolveNamed(contract, iface);
                                return container.Resolve(iface);
                            },
                            (iface, contract) =>
                            {
                                Type constructed = typeof(IEnumerable<>).MakeGenericType(new[] { iface });

                                if (contract != null) return container.ResolveNamed(contract, constructed) as IEnumerable<object>;
                                return container.Resolve(constructed) as IEnumerable<object>;
                            },
                            (realClass, iface, contract) =>
                            {
                                var b = new ContainerBuilder();

                                if (contract != null)
                                    b.RegisterType(realClass).Named(contract, iface);
                                else
                                    b.RegisterType(realClass).As(iface);

                                b.Update(container);
                            });
            
        }
    }
}
