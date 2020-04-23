using Quotidian.Diagnostics.Source;
using System;

namespace qdx.Samples.FileCreator.Console.Diagnostics
{
    // This is an example of an ITrace adapter.
    // It connects qdx with the console, allowing error and notification messages created by a module using qdx to be displayed on the console.
    public class ConsoleLogger : ITrace
    {
        public ConsoleLogger(Func<ILogEntry, bool> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        Func<ILogEntry, bool> Filter { get; }

        public void Log<T>(ILogEntry<T> entry)
        {
            var message = FormatMessage(entry);

            if (Filter(entry))
                System.Console.Error.WriteLine(message);
        }

        private string FormatMessage<T>(ILogEntry<T> entry)
        {
            if (entry.Data is Exception e)
                return $"{entry.Level} ({entry.Code}) : {e.Message}";

            return $"{entry.Level} ({entry.Code}) : {entry.Data}";
        }
    }
}
