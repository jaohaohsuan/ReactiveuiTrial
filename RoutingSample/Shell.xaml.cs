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

namespace RoutingSample
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IScreen
    {
        public Shell()
        {
            ConfigIoc();
            InitializeComponent();
            Router = new RoutingState();
            ViewHost.Router = Router;

            Router.Navigate.Go<WelcomeViewModel>();
        }

        public IRoutingState Router { get; private set; }

        protected void ConfigIoc()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WelcomeViewModel>();
            builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>();
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
