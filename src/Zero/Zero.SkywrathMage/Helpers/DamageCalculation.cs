using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Spells.SkywrathMage;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;

namespace Divine.SkywrathMage.Helpers
{
    internal sealed class DamageCalculation : BaseDamageCalculation
    {
        private readonly BaseKillStealMenu KillStealMenu;

        private readonly Abilities Abilities;

        public DamageCalculation(Common common)
            : base(common.MenuConfig)
        {
            KillStealMenu = common.MenuConfig.KillStealMenu;

            Abilities = (Abilities)common.Abilities;
        }

        protected override void AbilityCheck()
        {
            // AncientSeal
            var ancientSeal = Abilities.AncientSeal;
            if (ancientSeal.Level > 0 && KillStealMenu.AbilitiesSelection[ancientSeal.Id])
            {
                DamageAbilities.Add(ancientSeal);
            }

            // Veil
            var veil = Abilities.Veil;
            if (veil != null && veil.IsValid && KillStealMenu.AbilitiesSelection[veil.Id])
            {
                DamageAbilities.Add(veil);
            }

            // Ethereal
            var ethereal = Abilities.Ethereal;
            if (ethereal != null && ethereal.IsValid && KillStealMenu.AbilitiesSelection[ethereal.Id])
            {
                DamageAbilities.Add(ethereal);
            }

            // Shivas
            var shivas = Abilities.Shivas;
            if (shivas != null && shivas.IsValid && KillStealMenu.AbilitiesSelection[shivas.Id])
            {
                DamageAbilities.Add(shivas);
            }

            // ConcussiveShot
            var concussiveShot = Abilities.ConcussiveShot;
            if (concussiveShot.Level > 0 && KillStealMenu.AbilitiesSelection[concussiveShot.Id])
            {
                DamageAbilities.Add(concussiveShot);
            }

            // ArcaneBolt
            var arcaneBolt = Abilities.ArcaneBolt;
            if (arcaneBolt.Level > 0 && KillStealMenu.AbilitiesSelection[arcaneBolt.Id])
            {
                DamageAbilities.Add(arcaneBolt);
            }

            // MysticFlare
            var mysticFlare = Abilities.MysticFlare;
            if (mysticFlare.Level > 0 && KillStealMenu.AbilitiesSelection[mysticFlare.Id])
            {
                DamageAbilities.Add(mysticFlare);
            }

            // Dagon
            var dagon = Abilities.Dagon;
            if (dagon != null && dagon.IsValid && KillStealMenu.AbilitiesSelection[AbilityId.item_dagon_5])
            {
                DamageAbilities.Add(dagon);
            }
        }

        protected override bool AbilityControl(CHero target, CAbility ability)
        {
            var concussiveShot = Abilities.ConcussiveShot;
            if (ability == concussiveShot && target != concussiveShot.TargetHit)
            {
                return true;
            }

            return false;
        }

        protected override bool IsHitTime(CHero target, CAbility ability)
        {
            if (ability == Abilities.Ethereal)
            {
                return MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_{ability.Name}");
            }

            if (ability == Abilities.ConcussiveShot)
            {
                return MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_{ability.Name}");
            }

            if (ability == Abilities.ArcaneBolt)
            {
                return MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_{ability.Name}");
            }

            return false;
        }

        protected override float GetMagicalDamage(CAbility ability, CHero hero, float magicalDamageReduction, float magicalCurrentHealth, bool canHitActive)
        {
            var mysticFlare = (ability as MysticFlare);
            if (mysticFlare != null)
            {
                if (!canHitActive)
                {
                    return DamageHelpers.GetSpellDamage(mysticFlare.GetTotalDamage(hero), magicalDamageReduction - 1);
                }

                return 0;
            }

            return ability.GetDamage(hero, magicalDamageReduction - 1, magicalCurrentHealth);
        }
    }
}
