namespace Quotidian.Diagnostics.Source.ImplicitDependency
{
    public abstract class TraceSource
    {
        public ITrace For<T>(T source)
        {
            return For<T>();
        }

        public ITrace For<T>()
        {
            return Named(typeof(T).FullName);
        }

        public abstract ITrace Named(string name);
    }
}