using qdx.Samples.FileCreator.Diagnostics;
using Quotidian.Diagnostics.Source;
using Quotidian.Diagnostics.Source.Application;
using System;
using System.IO;
using System.Linq;

namespace qdx.Samples.FileCreator.Console
{
    internal class Program
    {
        private static ITrace Trace { get; }
        private static TraceBuilder TraceBuilder { get; }

        static Program()
        {
            TraceBuilder = new TraceBuilder();
            Trace = TraceBuilder.BuildTraceFor<Program>();

            AppDomain.CurrentDomain.UnhandledException += UnhandledException; // Adding logging to AppDomain.UnhandledException helps when troubleshooting startup errors
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Trace.Critical(ex);
            Environment.FailFast(ex.Message, ex);
        }

        private static void Main(string[] args)
        {
            try
            {
                // Parsing arguments
                if (args.Length < 1 || !int.TryParse(args[0], out var num))
                {
                    ShowUsage();
                    return;
                }

                foreach (var arg in args.Skip(1))
                    if (arg.ToLowerInvariant() == "-verbose")
                        TraceBuilder.SetLogFilter(e => true);


                // Composition Root
                var app =
                    new Creator(
                        TraceBuilder.BuildTraceFor<Creator>());


                // Do the work
                app.MakeFiles(Directory.GetCurrentDirectory(), num);
            }
            catch (Exception e) // App entry point is one of the few places a Pokemon handler is OK
            {
                Trace.Critical(e);
                Environment.FailFast(e.Message, e);
            }
        }

        private static void ShowUsage()
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("FileMaker # [-verbose]");
            System.Console.WriteLine("  where # is the number of files to create");
        }
    }
}
