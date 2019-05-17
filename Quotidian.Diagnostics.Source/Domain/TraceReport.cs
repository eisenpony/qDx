using System;
using System.Collections.Generic;

namespace Quotidian.Diagnostics.Source.Domain
{
    public static class ReportExtensions
    {
        public static TraceReport Report(this ITrace trace, Enum code, string correlation)
        {
            return new TraceReport(trace, code, correlation);
        }
        public static TraceReport Report(this ITrace trace, string correlation)
        {
            return new TraceReport(trace, null, correlation);
        }
        public static TraceReport Report(this ITrace trace, Enum code)
        {
            return new TraceReport(trace, code, null);
        }


        public static TraceReport Warning<T>(this TraceReport trace, T data) => Log(TraceLevel.Warning, trace, data);
        public static TraceReport Warning<T>(this TraceReport trace, Func<T> factory) => Log(TraceLevel.Warning, trace, factory);
        public static TraceReport Information<T>(this TraceReport trace, T data) => Log(TraceLevel.Information, trace, data);
        public static TraceReport Information<T>(this TraceReport trace, Func<T> factory) => Log(TraceLevel.Information, trace, factory);
        public static TraceReport Verbose<T>(this TraceReport trace, T data) => Log(TraceLevel.Verbose, trace, data);
        public static TraceReport Verbose<T>(this TraceReport trace, Func<T> factory) => Log(TraceLevel.Verbose, trace, factory);

        private static TraceReport Log<T>(TraceLevel level, TraceReport trace, T data)
        {
            var entry = new LogEntry<T>(trace.Name, level, data)
            {
                Code = trace.Code,
                CorrelationId = trace.Correlation
            };

            trace.Log(entry);
            return trace;
        }

        private static TraceReport Log<T>(TraceLevel level, TraceReport trace, Func<T> data)
        {
            var entry = new LogEntry<T>(trace.Name, level, new Lazy<T>(data))
            {
                Code = trace.Code,
                CorrelationId = trace.Correlation
            };

            trace.Log(entry);
            return trace;
        }
    }

    public class TraceReport : IDisposable
    {
        public TraceReport(ITrace innerTrace, Enum code, string correlation)
        {
            Entries = new List<ILogEntry>();
            InnerTrace = innerTrace ?? throw new ArgumentNullException(nameof(innerTrace));
            Code = code;
            Correlation = correlation;
        }

        List<ILogEntry> Entries { get; }
        ITrace InnerTrace { get; }
        public Enum Code { get; }
        public string Correlation { get; }

        public string Name => InnerTrace.Name;

        public void Log<T>(ILogEntry<T> entry)
        {
            Entries.Add(entry);
        }

        public void Dispose()
        {
            foreach (var e in Entries)
            {
                if (Code != null)
                    e.Code = Code;
                if (Correlation != null)
                    e.CorrelationId = Correlation;

                e.TraceTo(InnerTrace);
            }
        }
    }
}
