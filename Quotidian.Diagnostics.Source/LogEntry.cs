using System;

namespace Quotidian.Diagnostics.Source
{
    public class LogEntry<T> : ILogEntry<T>
    {
        public LogEntry(string sourceName, TraceLevel level, Lazy<T> data)
            : this(sourceName, level)
        {
            LazyData = data;
        }

        public LogEntry(string sourceName, TraceLevel level, T data)
            : this(sourceName, level)
        {
            this.data = data;
        }

        private LogEntry(string sourceName, TraceLevel level)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
                throw new ArgumentException("Cannot be empty", nameof(sourceName));

            SourceName = sourceName;
            Level = level;
            TimeStamp = DateTimeOffset.Now;
        }

        public string SourceName { get; }
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