using Quotidian.Diagnostics.Source;
using System;
using System.Text;
using System.Windows;

namespace qdx.Samples.FileCreator.WPF.Diagnostics
{
    // This is an example of an ITrace adapter.
    // It connects qdx with the WPF MessageBox, allowing error and notification messages created by a module using qdx to be displayed as a Windows prompt.

    // This is an example only. I don't recommend using this in production as there are several shortcomings.
    internal class ErrorPrompterTrace : ITrace
    {
        public ErrorPrompterTrace(Func<ILogEntry, bool> filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public Func<ILogEntry, bool> Filter { get; }

        public void Log<T>(ILogEntry<T> entry)
        {
            if (!Filter(entry))
                return;

            var builder = new StringBuilder()
                .Append($"{entry.Level}: ")
                .AppendLine(GetDescription(entry))
                .Append("Check error.log for more details.");

            MessageBox.Show(builder.ToString(), entry.Level.ToString(), MessageBoxButton.OK, ImageForLevel(entry.Level));
        }

        private string GetDescription<T>(ILogEntry<T> entry)
        {
            if (entry.Data is Exception e)
                return e.Message;

            return entry.Data.ToString();
        }

        private MessageBoxImage ImageForLevel(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Critical: return MessageBoxImage.Stop;
                case TraceLevel.Error: return MessageBoxImage.Error;
                case TraceLevel.Warning: return MessageBoxImage.Warning;
                default: return MessageBoxImage.Information;
            }
        }
    }
}
