// <copyright file="rattletrap_power_cogs.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_rattletrap
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class rattletrap_power_cogs : ActiveAbility, IAreaOfEffectAbility, IHasTargetModifier
    {
        public rattletrap_power_cogs(Ability ability)
            : base(ability)
        {
        }

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("cogs_radius");
            }
        }

        public string TargetModifierName { get; } = "modifier_rattletrap_cog_push";
    }
}