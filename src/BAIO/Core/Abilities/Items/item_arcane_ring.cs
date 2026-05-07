// <copyright file="item_arcane_ring.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_arcane_ring : ActiveAbility, IAreaOfEffectAbility, IHasManaRestore
    {
        public item_arcane_ring(Item item)
            : base(item)
        {
        }

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("mana_restore");
            }
        }
    }
}