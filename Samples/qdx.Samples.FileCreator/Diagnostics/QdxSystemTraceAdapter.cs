using Quotidian.Diagnostics.Source;
using System;
using System.Diagnostics;

namespace qdx.Samples.FileCreator.Diagnostics
{
    // This is an example of an ITrace adapter.
    // It connects qdx with the old System.Diagnostics Trace system, allowing error and notification messages created by a module using qdx to be repeated to TraceSources.
    // The forwarded messages can eventually be recieved by TraceListeners, wired up through the app.config file.

    // This is an example only. I don't recommend using the System.Diagnostics Trace system, because I think there are many great alternatives.
    public class QdxSystemTraceAdapter : ITrace
    {
        public static QdxSystemTraceAdapter ForType<T>() => ForType(typeof(T));
        public static QdxSystemTraceAdapter ForType(Type t) => new QdxSystemTraceAdapter(t.FullName); // Using an objects qualified name is a common trace pattern allowing for advanced filtering.

        public QdxSystemTraceAdapter(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            Trace = new TraceSource(name);
        }

        public TraceSource Trace { get; }

        public void Log<T>(ILogEntry<T> entry)
        {
            var traceEventType = GetTraceEventTypeFromQdxLevel(entry.Level);
            Trace.TraceData(traceEventType, entry.Code?.GetHashCode() ?? 0, entry.Data);
        }

        private TraceEventType GetTraceEventTypeFromQdxLevel(Quotidian.Diagnostics.Source.TraceLevel qdxLevel)
        {
            switch (qdxLevel)
            {
                case Quotidian.Diagnostics.Source.TraceLevel.Critical: return TraceEventType.Critical;
                case Quotidian.Diagnostics.Source.TraceLevel.Error: return TraceEventType.Error;
                case Quotidian.Diagnostics.Source.TraceLevel.Warning: return TraceEventType.Warning;
                case Quotidian.Diagnostics.Source.TraceLevel.Information: return TraceEventType.Information;
                case Quotidian.Diagnostics.Source.TraceLevel.Verbose: return TraceEventType.Verbose;
                case Quotidian.Diagnostics.Source.TraceLevel.Off: return TraceEventType.Verbose;
                default: return TraceEventType.Error;
            }
        }
    }
}
