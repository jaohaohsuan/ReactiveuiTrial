using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl, IViewFor<WelcomeViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (WelcomeViewModel), typeof (Welcome),
                new PropertyMetadata(null));

        public Welcome()
        {
            InitializeComponent();
            this.OneWayBind(ViewModel, x => x.Greeting, v => v.Greeting.Text); //won't throw exception
            this.Bind(ViewModel, x => x.Indiactor, v => v.Indicator.Value);
            //this.OneWayBind(ViewModel, x => x.Greeting);
            
        }

        public WelcomeViewModel ViewModel
        {
            get { return (WelcomeViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (WelcomeViewModel) value; } }
    }

    public class WelcomeViewModel : ReactiveObject, IRoutableViewModel
    {
        private double _indiactor;

        public WelcomeViewModel(IScreen screen)
        {
            HostScreen = screen;
            Greeting = "Hello Reactiveui!";
            Indiactor = 0.001;
        }

        public double Indiactor
        {
            get { return _indiactor; }
            set { this.RaiseAndSetIfChanged(ref _indiactor, value); }
        }

        public string Greeting { get; private set; }

        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
    }
}