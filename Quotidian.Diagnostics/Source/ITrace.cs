namespace Quotidian.Diagnostics.Source
{
    public interface ITrace
    {
        void Log<T>(ILogEntry<T> entry);
    }
}