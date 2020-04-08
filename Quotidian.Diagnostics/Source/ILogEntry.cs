using System;

namespace Quotidian.Diagnostics.Source
{
    public interface ILogEntry
    {
        Enum Code { get; set; }
        string CorrelationId { get; set; }
        TraceLevel Level { get; }
        DateTimeOffset TimeStamp { get; }

        void TraceTo(ITrace trace);
    }

    public interface ILogEntry<out T> : ILogEntry
    {
        T Data { get; }
    }
}