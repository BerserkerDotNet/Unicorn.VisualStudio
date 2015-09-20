using System;
using System.Runtime.Serialization;

namespace Unicorn.VS.Exceptions
{
    [Serializable]
    public class MalformedPackageReceivedException : Exception
    {
        public MalformedPackageReceivedException()
        {
        }

        public MalformedPackageReceivedException(string message) : base(message)
        {
        }

        public MalformedPackageReceivedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MalformedPackageReceivedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}