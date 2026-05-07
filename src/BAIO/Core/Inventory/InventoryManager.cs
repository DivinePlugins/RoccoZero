namespace Ensage.SDK.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;

    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Units;
    using Divine.Game;
    using Divine.Update;
    using Divine.Zero.Log;

    using Ensage.SDK.Abilities;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Service;

    using Inventory = Divine.Entity.Entities.Units.Components.Inventory;

    public sealed class InventoryManager : IInventoryManager
    {
        private Dictionary<Type, BaseAbility> itemCache = new Dictionary<Type, BaseAbility>();

        private HashSet<InventoryItem> items;

        public InventoryManager(IServiceContext context)
        {
            this.Owner = context.Owner;
            this.LastItems = new HashSet<InventoryItem>();

            if (this.Owner.HasInventory)
            {
                this.OnInventoryUpdate();
                Entity.NetworkPropertyChanged += this.OnNetworkPropertyChanged;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Inventory Inventory => this.Owner.Inventory;

        public HashSet<InventoryItem> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = this.Inventory.Items.Select(item => new InventoryItem(item)).ToHashSet();
                }

                return this.items;
            }
        }

        public Unit Owner { get; }

        private List<ItemBindingHandler> Bindings { get; } = new List<ItemBindingHandler>();

        private HashSet<InventoryItem> LastItems { get; set; }

        public void Dispose()
        {
            Entity.NetworkPropertyChanged -= this.OnNetworkPropertyChanged;
            this.items = null;
        }

        public void Attach(object target)
        {
            var targetType = target.GetType();

            foreach (var property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try
                {
                    var itemType = property.PropertyType;
                    var attribute = property.GetCustomAttribute<ItemBindingAttribute>();

                    if (attribute != null)
                    {
                        var id = (AbilityId)Enum.Parse(typeof(AbilityId), itemType.Name);

                        // get or create handler
                        var handler = this.Bindings.FirstOrDefault(e => e.Type == itemType);
                        if (handler == null)
                        {
                            handler = new ItemBindingHandler(id, itemType);
                            this.Bindings.Add(handler);
                        }

                        // create binding
                        handler.Add(property, target);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Warn(e);
                }
            }

            foreach (var change in this.Items)
            {
                var item = this.GetOrCreateItem(change.Item);

                foreach (var handler in this.Bindings.Where(e => e.Id == change.Id))
                {
                    handler.UpdateBindings(item);
                }
            }
        }

        public void Detach(object target)
        {
            foreach (var binding in this.Bindings)
            {
                binding.Remove(target);
            }
        }

        public ItemStockInfo GetStockInfo(AbilityId id, Team team = Team.Undefined)
        {
            if (team == Team.Undefined)
            {
                team = this.Owner.Team;
            }

            return GameManager.ItemStockInfos.FirstOrDefault(e => e.AbilityId == id && e.Team == team);
        }

        private object GetOrCreateItem(Item item)
        {
            var typeName = $"Ensage.SDK.Abilities.Items.{item.Id}";
            var type = Type.GetType(typeName);

            if (type == null)
            {
                return null;
            }

            BaseAbility cacheItem = null;

            if (this.itemCache.TryGetValue(type, out cacheItem))
            {
                if (cacheItem?.Item?.IsValid == false)
                {
                    cacheItem = null;
                }
            }

            if (cacheItem == null)
            {
                cacheItem = (BaseAbility)Activator.CreateInstance(type, item);
                this.itemCache[type] = cacheItem;
            }

            return cacheItem;
        }

        private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_iParity")
            {
                return;
            }

            if (e.NewValue.GetInt32() == e.OldValue.GetInt32())
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                if (sender == this.Owner)
                {
                    this.items = null;
                    UpdateManager.BeginInvoke(this.OnInventoryUpdate);
                }
            });
        }

        private void OnInventoryUpdate()
        {
            // inventory diff
            var added = this.Items.Except(this.LastItems).ToList();
            var removed = this.LastItems.Except(this.Items).ToList();

            this.UpdateBindings(added, removed);

            // update events
            if (this.CollectionChanged != null)
            {
                if (removed.Count > 0)
                {
                    this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
                }

                if (added.Count > 0)
                {
                    this.CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added));
                }
            }

            this.LastItems = this.Items;
        }

        private void UpdateBindings(IEnumerable<InventoryItem> added, IEnumerable<InventoryItem> removed)
        {
            // update bindings
            foreach (var change in added)
            {
                var item = this.GetOrCreateItem(change.Item);

                foreach (var handler in this.Bindings.Where(e => e.Id == change.Id))
                {
                    handler.UpdateBindings(item);
                }
            }

            foreach (var change in removed)
            {
                foreach (var handler in this.Bindings.Where(e => e.Id == change.Id))
                {
                    handler.ResetBindings();
                }
            }
        }
    }
}