using System;
using System.Collections.Generic;
using System.Linq;
using Quotidian.Diagnostics.Source;
using Quotidian.Diagnostics.Source.Application;

namespace Quotidian.Diagnostics
{
    public class CompositeTrace : ITrace
    {
        public List<ITrace> Traces { get; }

        public CompositeTrace(params ITrace[] traces)
        {
            Traces = traces?.ToList() ?? throw new ArgumentNullException(nameof(traces));
        }

        public void Log<T>(ILogEntry<T> entry)
        {
            var errors = new Queue<Exception>();

            foreach (var trace in Traces)
                try
                {
                    trace.Log(entry);
                }
                catch (Exception e)
                {
                    errors.Enqueue(e);
                }

            if (errors.Any())
                LogFailure(errors, new List<ITrace>(Traces));
        }

        private void LogFailure(Queue<Exception> errors, List<ITrace> traces)
        {
            if (!errors.Any())
                return;

            for (var error = errors.Dequeue(); errors.Any(); error = errors.Dequeue())
            {
                var i = 0;
                while (i < traces.Count)
                {
                    try
                    {
                        traces[i].Error(error);
                        i++;
                    }
                    catch (Exception e)
                    {
                        errors.Enqueue(e);
                        traces.RemoveAt(i);
                    }
                }
            }

        }
    }
}
