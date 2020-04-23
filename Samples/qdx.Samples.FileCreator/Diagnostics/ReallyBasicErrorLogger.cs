using Quotidian.Diagnostics;
using Quotidian.Diagnostics.Source;
using System;
using System.IO;
using System.Text;

namespace qdx.Samples.FileCreator.Diagnostics
{
    // This is an example of an ITrace adapter.
    // It connects qdx with a file, writing error and notification messages created by a module using qdx to disk.

    // This is an example only. I don't recommend using this in production as there are several shortcomings.
    public class ReallyBasicErrorLogger : ITrace
    {
        public ReallyBasicErrorLogger(Func<ILogEntry, bool> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        private Func<ILogEntry, bool> Filter { get; }

        public void Log<T>(ILogEntry<T> entry)
        {
            if (!Filter(entry))
                return;

            try
            {
                File.AppendAllText("error.log", FormatMessage(entry));
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is NotSupportedException)
            { } // Best effort logging only. Ignore any file-write problems.
        }

        private string FormatMessage<T>(ILogEntry<T> entry)
        {
            var builder = new StringBuilder()
                .Append($"{entry.TimeStamp} {entry.Level} ");

            if (entry.Code != null)
                builder.Append($"({entry.Code}) ");

            if (entry.CorrelationId != null)
                builder.Append($"[{entry.CorrelationId}] ");

            if (entry.Data is Exception ex)
                builder.AppendLine(ex.ToTrace());
            else
                builder.AppendLine(entry.Data.ToString());

            return builder.ToString();
        }
    }
}
