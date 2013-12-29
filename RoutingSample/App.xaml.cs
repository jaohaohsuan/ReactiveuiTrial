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
using NLog;
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
        private IDisposable _openShellViewSubscription;
        private Logger _logger;


        protected override void OnStartup(StartupEventArgs e)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();

            Application.Current.Dispatcher.UnhandledException += (sender, args) =>
            {
                _logger.FatalException("UnhandledException", args.Exception);
            };

            var bootstrapper = new AppBootstrapper();

            _openShellViewSubscription = bootstrapper.OnStartUp.ObserveOnDispatcher().Subscribe(c =>
            {
                c.Resolve<Shell>().Show();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _openShellViewSubscription.Dispose();
            base.OnExit(e);
        }
    }
}
