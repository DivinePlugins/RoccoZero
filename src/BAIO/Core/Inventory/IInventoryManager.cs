namespace Ensage.SDK.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Components;
    using Divine.Game;

    using Inventory = Divine.Entity.Entities.Units.Components.Inventory;

    public interface IInventoryManager : INotifyCollectionChanged, IDisposable
    {
        Inventory Inventory { get; }

        HashSet<InventoryItem> Items { get; }

        void Attach(object target);

        void Detach(object target);

        ItemStockInfo GetStockInfo(AbilityId id, Team team = Team.Undefined);
    }
}