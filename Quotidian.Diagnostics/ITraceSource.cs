using Quotidian.Diagnostics.Source;

namespace Quotidian.Diagnostics
{
    public interface ITraceSource
    {
        ITrace For<T>(T @this);
    }
}
