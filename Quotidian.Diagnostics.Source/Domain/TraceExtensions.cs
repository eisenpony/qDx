using System;

namespace Quotidian.Diagnostics.Source.Domain
{
    public static class TraceExtensions
    {
        public static ITrace Warning<T>(this ITrace trace, Enum code, T data, string correlation = null) => Log(TraceLevel.Warning, trace, code, data, correlation);
        public static ITrace Warning<T>(this ITrace trace, Enum code, Func<T> factory, string correlation = null) => Log(TraceLevel.Warning, trace, code, factory, correlation);
        public static ITrace Information<T>(this ITrace trace, Enum code, T data, string correlation = null) => Log(TraceLevel.Information, trace, code, data, correlation);
        public static ITrace Information<T>(this ITrace trace, Enum code, Func<T> factory, string correlation = null) => Log(TraceLevel.Information, trace, code, factory, correlation);
        public static ITrace Verbose<T>(this ITrace trace, Enum code, T data, string correlation = null) => Log(TraceLevel.Verbose, trace, code, data, correlation);
        public static ITrace Verbose<T>(this ITrace trace, Enum code, Func<T> factory, string correlation = null) => Log(TraceLevel.Verbose, trace, code, factory, correlation);

        public static ITrace Warning<T>(this ITrace trace, T data, string correlation = null) => Log(TraceLevel.Warning, trace, null, data, correlation);
        public static ITrace Warning<T>(this ITrace trace, Func<T> factory, string correlation = null) => Log(TraceLevel.Warning, trace, null, factory, correlation);
        public static ITrace Information<T>(this ITrace trace, T data, string correlation = null) => Log(TraceLevel.Information, trace, null, data, correlation);
        public static ITrace Information<T>(this ITrace trace, Func<T> factory, string correlation = null) => Log(TraceLevel.Information, trace, null, factory, correlation);
        public static ITrace Verbose<T>(this ITrace trace, T data, string correlation = null) => Log(TraceLevel.Verbose, trace, null, data, correlation);
        public static ITrace Verbose<T>(this ITrace trace, Func<T> factory, string correlation = null) => Log(TraceLevel.Verbose, trace, null, factory, correlation);

        private static ITrace Log<T>(TraceLevel level, ITrace trace, Enum code, T data, string correlation = null)
        {
            var entry = new LogEntry<T>(trace.Name, level, data)
            {
                Code = code,
                CorrelationId = correlation
            };

            trace.Log(entry);
            return trace;
        }

        private static ITrace Log<T>(TraceLevel level, ITrace trace, Enum code, Func<T> data, string correlation = null)
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