// <copyright file="Necronomicon.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Aggregation
{
    using Divine.Entity.Entities.Abilities.Items;

    public abstract class Necronomicon : ActiveAbility
    {
        protected Necronomicon(Item item)
            : base(item)
        {
        }
    }
}