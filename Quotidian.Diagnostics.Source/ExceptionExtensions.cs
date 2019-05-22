using System;
using System.Text;

namespace Quotidian.Diagnostics
{
    public static class ExceptionExtensions
    {
        public static string ToTrace(this Exception e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var trace = new StringBuilder();
            ToTrace(e, new StringBuilder(), trace);

            return trace.ToString();
        }

        private static void ToTrace(Exception e, StringBuilder indent, StringBuilder trace)
        {
            if (e == null)
            {
                if (indent.Length >= 2)
                    indent.Length -= 2;
                return;
            }

            trace.Append(indent)
                 .Append(e.Message)
                 .AppendLine();

            if (e.StackTrace != null)
                foreach (var l in e.StackTrace.Split('\n'))
                    trace.Append(indent)
                         .Append(l)
                         .AppendLine();

            indent.Append("  ");

            if (e is AggregateException ae)
                foreach (var i in ae.InnerExceptions)
                    ToTrace(i, indent, trace);
            else
                ToTrace(e.InnerException, indent, trace);
        }
    }
}