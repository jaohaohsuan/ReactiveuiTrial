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
    /// Interaction logic for NextPage1.xaml
    /// </summary>
    public partial class NextPage1 : UserControl, IViewFor<NextPage1ViewModel>
    {
        public NextPage1()
        {
            InitializeComponent();
        }

        public NextPage1ViewModel ViewModel
        {
            get { return (NextPage1ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(NextPage1ViewModel), typeof(NextPage1), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NextPage1ViewModel)value; }
        }
    }

    public class NextPage1ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get; private set; }
        public IScreen HostScreen { get; private set; }
    }
}
