// <copyright file="item_havoc_hammer.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_havoc_hammer : ActiveAbility, IHasTargetModifier
    {
        public item_havoc_hammer(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_havoc_hammer_slow";

        public override float CastRange
        {
            get
            {
                return Ability.GetAbilitySpecialData("range");
            }
        }

        public float KnockbackDistance
        {
            get
            {
                return Ability.GetAbilitySpecialData("knockback_distance");
            }
        }
    }
}