// <copyright file="item_shadow_amulet.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;
    using Ensage.SDK.Abilities.Components;

    public class item_shadow_amulet : RangedAbility, IHasTargetModifier
    {
        public item_shadow_amulet(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invisible;

        public string TargetModifierName { get; } = "modifier_item_shadow_amulet_fade";
    }
}