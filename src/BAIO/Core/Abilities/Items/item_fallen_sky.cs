// <copyright file="item_fallen_sky.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_fallen_sky : RangedAbility, IHasTargetModifier, IHasModifier
    {
        public item_fallen_sky(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_fallen_sky_burn";
        public string ModifierName { get; } = "modifier_item_fallen_sky_land";
    }
}