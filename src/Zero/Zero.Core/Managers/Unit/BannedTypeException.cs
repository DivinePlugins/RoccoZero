using System;
using System.Runtime.Serialization;

namespace Divine.Core.Managers.Unit
{
    [Serializable]
    public class BannedTypeException : Exception
    {
        public BannedTypeException()
        {
        }

        public BannedTypeException(string message)
            : base(message)
        {
        }

        public BannedTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BannedTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}