namespace Divine.Core.Entities.Metadata;

using System;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SpellAttribute : System.Attribute
{
    public SpellAttribute(AbilityId abilityId, HeroId heroId)
    {
        AbilityId = abilityId;
        HeroId = heroId;
        UnitName = heroId.ToString();
    }

    public SpellAttribute(AbilityId abilityId, HeroId heroId, string unitName)
    {
        AbilityId = abilityId;
        HeroId = heroId;
        UnitName = unitName;
    }

    public AbilityId AbilityId { get; }

    public HeroId HeroId { get; }

    public string UnitName { get; }
}