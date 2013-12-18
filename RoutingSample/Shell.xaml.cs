using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IViewFor<ShellViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(null));

        public Shell(IRoutingState routingState)
        {
            InitializeComponent();

            ViewHost.Router = routingState;

            Loaded += (sender, e) =>
            {
                ViewModel.OnCompomentsRegisted.ObserveOnDispatcher().Subscribe(_ =>
                {
                    ViewModel.GoToDefaultPage();
                });
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