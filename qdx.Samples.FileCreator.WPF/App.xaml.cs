using qdx.Samples.FileCreator.Diagnostics;
using qdx.Samples.FileCreator.WPF.Diagnostics;
using qdx.Samples.FileCreator.WPF.View;
using qdx.Samples.FileCreator.WPF.ViewModel;
using Quotidian.Diagnostics;
using Quotidian.Diagnostics.Source;
using Quotidian.Diagnostics.Source.Application;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace qdx.Samples.FileCreator.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ITrace Trace { get; }

        static App()
        {
            Trace =
                new ReallyBasicErrorLogger(
                    e => e.Level <= TraceLevel.Error);

            AppDomain.CurrentDomain.UnhandledException += UnhandledException; // Adding logging to AppDomain.UnhandledException helps when troubleshooting startup errors
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Trace.Critical(ex);
            Environment.FailFast(ex.Message, ex);
        }

        protected override void OnStartup(StartupEventArgs a)
        {
            try
            {
                base.OnStartup(a);

                // Composition root
                var app = new FileMakerView();
                app.Context =
                    new FileMakerViewModel(
                        new FileCreator.Creator(
                            new CompositeTrace(
                                new ErrorPrompterTrace(e => e.Level <= app.Context.TraceLevel),
                                QdxSystemTraceAdapter.ForType<FileMakerView>(),
                                new ReallyBasicErrorLogger(e => e.Level <= app.Context.TraceLevel)))
                        {
                            SecureTrace = new ReallyBasicErrorLogger(e => e.Level <= app.Context.TraceLevel)
                        })
                    {
                        NumberFiles = 10,
                        SelectedDirectory = $"{Directory.GetCurrentDirectory().TrimEnd('\\')}\\",
                        TraceLevel = TraceLevel.Error
                    };

                app.InitializeComponent();
                app.Show();
            }
            catch (Exception e) // App entry point is one of the few places a Pokemon handler is OK
            {
                Trace.Critical(e);
                Environment.FailFast(e.Message, e);
            }
        }
    }
}
