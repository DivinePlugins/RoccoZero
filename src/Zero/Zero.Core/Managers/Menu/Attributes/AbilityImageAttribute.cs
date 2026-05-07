namespace Ensage.SDK.Menu.Attributes
{
    using System;

    using Divine.Entity.Entities.Abilities.Components;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class AbilityImageAttribute : ImageAttribute
    {
        public AbilityImageAttribute(AbilityId abilityId)
        {
            AbilityId = abilityId;
        }

        public AbilityId AbilityId { get; }
    }
}