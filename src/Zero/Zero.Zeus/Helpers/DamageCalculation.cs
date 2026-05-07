using System;

using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Entity.Entities.Abilities.Components;

namespace Divine.Zeus.Helpers
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
            // Veil
            var veil = Abilities.Veil;
            if (veil != null && KillStealMenu.AbilitiesSelection[veil.Id])
            {
                DamageAbilities.Add(veil);
            }

            // Ethereal
            var ethereal = Abilities.Ethereal;
            if (ethereal != null && KillStealMenu.AbilitiesSelection[ethereal.Id])
            {
                DamageAbilities.Add(ethereal);
            }

            // Shivas
            var shivas = Abilities.Shivas;
            if (shivas != null && KillStealMenu.AbilitiesSelection[shivas.Id])
            {
                DamageAbilities.Add(shivas);
            }

            // LightningBolt
            var lightningBolt = Abilities.LightningBolt;
            if (lightningBolt.Level > 0 && KillStealMenu.AbilitiesSelection[lightningBolt.Id])
            {
                DamageAbilities.Add(lightningBolt);
            }

            // Dagon
            var dagon = Abilities.Dagon;
            if (dagon != null && KillStealMenu.AbilitiesSelection[AbilityId.item_dagon_5])
            {
                DamageAbilities.Add(dagon);
            }

            // ArcLightning
            var arcLightning = Abilities.ArcLightning;
            if (arcLightning.Level > 0 && KillStealMenu.AbilitiesSelection[arcLightning.Id])
            {
                DamageAbilities.Add(arcLightning);
            }

            // Nimbus
            var nimbus = Abilities.Nimbus;
            if (nimbus.Level > 0 && KillStealMenu.AbilitiesSelection[nimbus.Id])
            {
                DamageAbilities.Add(nimbus);
            }

            // ThundergodsWrath
            var thundergodsWrath = Abilities.ThundergodsWrath;
            if (thundergodsWrath.Level > 0 && KillStealMenu.AbilitiesSelection[thundergodsWrath.Id])
            {
                DamageAbilities.Add(thundergodsWrath);
            }
        }

        protected override float MagicalDamageHealth(CAbility ability, CHero hero, float magicalDamageReduction, float health, float magicalDamage)
        {
            //var staticField = Abilities.StaticField;
            //if (CanBeCasted(staticField) && !ability.IsItem)
            //{
            //    return staticField.GetDamage(hero, magicalDamageReduction - 1, Math.Max(0, health - magicalDamage));
            //}

            return 0;
        }
    }
}
