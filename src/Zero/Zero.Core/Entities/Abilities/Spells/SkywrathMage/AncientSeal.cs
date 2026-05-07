namespace Divine.Core.Entities.Abilities.Spells.SkywrathMage;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

[Spell(AbilityId.skywrath_mage_ancient_seal, HeroId.npc_dota_hero_skywrath_mage)]
public sealed class AncientSeal : RangedSpell, IHasTargetModifier, IHasDamageAmplifier
{
    public AncientSeal(Ability ability)
        : base(ability)
    {
    }

    public DamageType AmplifierType { get; } = DamageType.Magical;

    public override UnitState AppliesUnitState { get; } = UnitState.Silenced;

    public float DamageAmplification
    {
        get
        {
            var amplification = GetAbilitySpecialData("resist_debuff") / -100.0f;

            var talent = Owner.GetAbilityById(AbilityId.special_bonus_unique_skywrath_3);
            if (talent?.Level > 0)
            {
                amplification += talent.GetAbilitySpecialData("value") / -100.0f;
            }

            return amplification;
        }
    }

    public string TargetModifierName { get; } = "modifier_skywrath_mage_ancient_seal";
}