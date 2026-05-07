// <copyright file="item_royal_jelly.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_royal_jelly : RangedAbility, IHasTargetModifier
    {
        public item_royal_jelly(Item item)
            : base(item)
        {
        }

        public override bool CanHit(params Unit[] targets)
        {
            if (!targets.Any())
            {
                return true;
            }

            return targets.All(x => x.Distance2D(this.Owner) < CastRange && !x.HasModifier(TargetModifierName));
        }

        public string TargetModifierName { get; } = "modifier_royal_jelly";
    }
}