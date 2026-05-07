namespace Ensage.SDK.Abilities.npc_dota_hero_rattletrap
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;

    using Ensage.SDK.Helpers;

    public class rattletrap_rocket_flare : CircleAbility
    {
        public rattletrap_rocket_flare(Ability ability)
            : base(ability)
        {
        }

        protected override float RawDamage
        {
            get
            {
                var damage = base.RawDamage;

                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_clockwerk_2);
                if (talent?.Level > 0)
                {
                    damage += talent.GetAbilitySpecialData("value");
                }

                return damage;
            }
        }

        public override float GetDamage(params Unit[] targets)
        {
            var totalDamage = 0.0f;

            var damage = this.RawDamage;
            var amplify = this.Owner.GetSpellAmplification();
            foreach (var target in targets)
            {
                var reduction = this.Ability.GetDamageReduction(target, this.DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }
    }
}