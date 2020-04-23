using qdx.Samples.FileCreator.Console.Diagnostics;
using qdx.Samples.FileCreator.Diagnostics;
using Quotidian.Diagnostics;
using Quotidian.Diagnostics.Source;
using System;

namespace qdx.Samples.FileCreator.Console
{
    class TraceBuilder
    {
        ITrace ConsoleLogger { get; set; }
        ITrace ReallyBasicErrorLogger { get; set; }
        Func<ILogEntry, bool> Filter { get; set; }

        public TraceBuilder()
        {
            Filter = e => e.Level <= TraceLevel.Error;

            ConsoleLogger = new ConsoleLogger(Filter);
            ReallyBasicErrorLogger = new ReallyBasicErrorLogger(Filter);
        }

        public ITrace BuildTraceFor<T>()
        {
            return
                new CompositeTrace(
                    ConsoleLogger, // A singleton -- all components log through here
                    ReallyBasicErrorLogger, // A singleton -- all components log through here
                    QdxSystemTraceAdapter.ForType<T>()); // New for each component.
        }

        public void SetLogFilter(Func<ILogEntry, bool> filter)
        {
            Filter = filter;

            ConsoleLogger = new ConsoleLogger(Filter); // Reset the singleton loggers
            ReallyBasicErrorLogger = new ReallyBasicErrorLogger(Filter); // Reset the singleton loggers
        }
    }
}