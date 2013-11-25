using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Autofac;
using Autofac.Core.Lifetime;
using ReactiveUI;
using ReactiveUI.Routing;

namespace RoutingSample
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new AppBootstrapper();

            AppBootstrapper.Container.BeginLifetimeScope(builder =>
            {
                builder.RegisterType<WelcomeViewModel>().InstancePerLifetimeScope();
                builder.RegisterType<Welcome>().As<IViewFor<WelcomeViewModel>>().InstancePerLifetimeScope();

                builder.RegisterType<NextPage1ViewModel>().InstancePerLifetimeScope();
                builder.RegisterType<NextPage1>().As<IViewFor<NextPage1ViewModel>>().InstancePerLifetimeScope();
            });
        }
    }
}
