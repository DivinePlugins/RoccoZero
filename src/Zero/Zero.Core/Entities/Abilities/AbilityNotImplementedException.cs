using System;
using System.Runtime.Serialization;

namespace Divine.Core.Entities.Abilities
{
    [Serializable]
    public class AbilityNotImplementedException : Exception
    {
        public AbilityNotImplementedException()
        {
        }

        public AbilityNotImplementedException(string message)
            : base(message)
        {
        }

        public AbilityNotImplementedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AbilityNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}