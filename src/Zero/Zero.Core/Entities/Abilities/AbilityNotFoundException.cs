using System;
using System.Runtime.Serialization;

namespace Divine.Core.Entities.Abilities
{
    [Serializable]
    public class AbilityNotFoundException : Exception
    {
        public AbilityNotFoundException()
        {
        }

        public AbilityNotFoundException(string message)
            : base(message)
        {
        }

        public AbilityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AbilityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}