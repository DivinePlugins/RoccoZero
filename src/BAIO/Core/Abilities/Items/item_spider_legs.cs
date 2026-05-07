// <copyright file="item_spider_legs.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_spider_legs : ActiveAbility, IHasModifier
    {
        public item_spider_legs(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_spider_legs_active";
    }
}