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
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl, IViewFor<WelcomeViewModel>
    {
        public Welcome()
        {
            InitializeComponent();
            //this.OneWayBind(ViewModel, x => x.Greeting, v=> v.Greeting.Text); //won't throw exception
            this.OneWayBind(ViewModel, x => x.Greeting);

        }

        public WelcomeViewModel ViewModel
        {
            get { return (WelcomeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(WelcomeViewModel), typeof(Welcome), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (WelcomeViewModel)value; }
        }
    }

    public class WelcomeViewModel : ReactiveUI.ReactiveObject ,IRoutableViewModel
    {
        public WelcomeViewModel(IScreen screen)
        {
            HostScreen = screen;
            Greeting = "Hello Reactiveui!";
        }

        public string Greeting { get; private set; }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }

    }
}
