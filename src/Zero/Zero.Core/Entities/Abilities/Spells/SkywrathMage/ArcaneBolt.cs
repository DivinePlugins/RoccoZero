using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

namespace Divine.Core.Entities.Abilities.Spells.SkywrathMage
{
    [Spell(AbilityId.skywrath_mage_arcane_bolt, HeroId.npc_dota_hero_skywrath_mage)]
    public sealed class ArcaneBolt : RangedSpell
    {
        public ArcaneBolt(Ability ability)
            : base(ability)
        {
        }

        public override float Speed
        {
            get
            {
                return GetAbilitySpecialData("bolt_speed");
            }
        }

        protected override float RawDamage
        {
            get
            {
                var damage = GetAbilitySpecialData("bolt_damage");
                var multiplier = GetAbilitySpecialData("int_multiplier");

                var hero = Owner as CHero;
                if (hero != null)
                {
                    damage += hero.TotalIntelligence * multiplier;
                }

                return damage;
            }
        }
    }
}