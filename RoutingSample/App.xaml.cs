using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Autofac;
using Autofac.Core.Lifetime;
using Reactive.EventAggregator;
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
            Application.Current.Dispatcher.UnhandledException += (sender, args) =>
            {
                var log = log4net.LogManager.GetLogger(typeof(Shell));
                log.Error(args.Exception);
            };

            var bootstrapper = new AppBootstrapper();

            bootstrapper.OnStartUp.ObserveOnDispatcher().Subscribe(c =>
            {
                c.Resolve<Shell>().Show();
            });
        }


    }
}
