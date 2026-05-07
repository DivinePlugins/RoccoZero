// <copyright file="item_faerie_fire.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_faerie_fire : ActiveAbility, IHasHealthRestore
    {
        public item_faerie_fire(Item item)
            : base(item)
        {
        }

        public float TotalHealthRestore
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("hp_restore");
            }
        }
    }
}