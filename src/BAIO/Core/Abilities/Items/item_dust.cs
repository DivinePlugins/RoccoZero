// <copyright file="item_dust.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_dust : ActiveAbility, IAreaOfEffectAbility, IHasTargetModifier
    {
        public item_dust(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.ProvidesVision;

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_dustofappearance";
    }
}