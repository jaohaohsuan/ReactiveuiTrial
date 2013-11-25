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
    public class ShellViewModel
    {
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

            this.Loaded += (sender, e)
                =>
            {
                DataContext = new ShellViewModel();

                ViewHost.Router = RxApp.GetService<IScreen>().Router;

                ViewHost.Router.Navigate.Go<WelcomeViewModel>();
                ViewHost.Router.Navigate.Go<NextPage1ViewModel>();
            };
        }

        

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (ShellViewModel)value; } }

    }

   
}