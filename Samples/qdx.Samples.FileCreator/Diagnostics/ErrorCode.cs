using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace qdx.Samples.FileCreator.Diagnostics
{

    // This class supports identifying exceptions raised by the domain classes with specific identifiers.
    // This allows exceptions to be identified by failure of contract rather than by type, which avoids the need for a wrapping class to couple its error handling to the specifics of a dependency.
    // It also provides a layer of indrection for error messages, so they may be intercepted and/or translated dynamically.
    // This my recommended pattern for domain errors. Low level or general use modules and frameworks should continue to use plain exceptions.

    // ErrorCode may become a part of qdx at a later time, but there are many details to work out first.
    [Serializable]
    public class ErrorCode : Exception
    {
        public Enum Code { get; set; }

        public ErrorCode(Enum code)
            : base(code.ToString())
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public ErrorCode(Enum code, Exception inner)
            : base(code.ToString(), inner)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        protected ErrorCode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Code = (Enum)info.GetValue(nameof(Code), typeof(Enum));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Code), Code);
            base.GetObjectData(info, context);
        }
    }
}
