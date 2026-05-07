using System;
using System.Runtime.Serialization;

namespace Divine.Core.Managers.Orbwalker
{
    [Serializable]
    public class OrbwalkerException : Exception
    {
        public OrbwalkerException()
        {
        }

        public OrbwalkerException(string message)
          : base(message)
        {
        }

        public OrbwalkerException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected OrbwalkerException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
