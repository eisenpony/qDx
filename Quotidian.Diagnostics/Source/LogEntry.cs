using System;

namespace Quotidian.Diagnostics.Source
{
    public class LogEntry<T> : ILogEntry<T>
    {
        public LogEntry(TraceLevel level, Lazy<T> data)
            : this(level)
        {
            LazyData = data;
        }

        public LogEntry(TraceLevel level, T data)
            : this(level)
        {
            this.data = data;
        }

        private LogEntry(TraceLevel level)
        {
            Level = level;
            TimeStamp = DateTimeOffset.Now;
        }

        public TraceLevel Level { get; }
        public DateTimeOffset TimeStamp { get; set; }
        public Enum Code { get; set; }
        public string CorrelationId { get; set; }

        private T data;
        public T Data
        {
            get
            {
                if (LazyData != null && !LazyData.IsValueCreated)
                    data = LazyData.Value;

                return data;
            }
        }

        Lazy<T> LazyData { get; }

        public void TraceTo(ITrace trace)
        {
            trace.Log(this);
        }
    }
}