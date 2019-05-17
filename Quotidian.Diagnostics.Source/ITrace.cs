namespace Quotidian.Diagnostics.Source
{
    public interface ITrace
    {
        string Name { get; }
        void Log<T>(ILogEntry<T> entry);
    }
}