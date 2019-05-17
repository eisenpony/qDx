using System;

namespace Quotidian.Diagnostics.Source.ImplicitDependency
{
    public static class TraceManager
    {
        static TraceSource source;
        public static TraceSource Source
        {
            get
            {
                if (source == null)
                    throw new InvalidOperationException("The trace source must be set before use.");
                return source;
            }
            set
            {
                if (source != null)
                    throw new InvalidOperationException("The trace source has already been set and cannot be changed");
                source = value;
            }
        }
    }
}
