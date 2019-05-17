using System;

namespace Quotidian.Diagnostics.Source.Application
{
    public static class TraceExtensions
    {
        public static ITrace Critical<T>(this ITrace trace, T data, Enum code = null, string correlation = null) => Log(TraceLevel.Critical, trace, data, code, correlation);
        public static ITrace Critical<T>(this ITrace trace, Func<T> factory, Enum code = null, string correlation = null) => Log(TraceLevel.Critical, trace, factory, code, correlation);
        public static ITrace Error<T>(this ITrace trace, T data, Enum code = null, string correlation = null) => Log(TraceLevel.Error, trace, data, code, correlation);
        public static ITrace Error<T>(this ITrace trace, Func<T> factory, Enum code = null, string correlation = null) => Log(TraceLevel.Error, trace, factory, code, correlation);

        private static ITrace Log<T>(TraceLevel level, ITrace trace, T data, Enum code = null, string correlation = null)
        {
            var entry = new LogEntry<T>(trace.Name, level, data)
            {
                Code = code,
                CorrelationId = correlation
            };
            trace.Log(entry);
            return trace;
        }

        private static ITrace Log<T>(TraceLevel level, ITrace trace, Func<T> data, Enum code = null, string correlation = null)
        {
            var entry = new LogEntry<T>(trace.Name, level, new Lazy<T>(data))
            {
                Code = code,
                CorrelationId = correlation
            };
            trace.Log(entry);
            return trace;
        }
    }
}