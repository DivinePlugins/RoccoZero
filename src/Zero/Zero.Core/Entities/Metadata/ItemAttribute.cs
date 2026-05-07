using System;

using Divine.Entity.Entities.Abilities.Components;

namespace Divine.Core.Entities.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ItemAttribute : System.Attribute
    {
        public ItemAttribute(AbilityId abilityId)
        {
            AbilityId = abilityId;
        }

        public AbilityId AbilityId { get; }
    }
}