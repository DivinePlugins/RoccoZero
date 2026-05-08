namespace Divine.Core.Managers.Ability;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Helpers;
using Divine.Core.Managers.Ability.Delegates;
using Divine.Core.Managers.Unit;
using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Exceptions;
using Divine.Helpers;
using Divine.Update;

using DivineAbility = Entity.Entities.Abilities.Ability;

public sealed class AbilityManager
{
    private static readonly Dictionary<uint, CAbility> spells = new Dictionary<uint, CAbility>();

    private static readonly Dictionary<uint, CItem> items = new Dictionary<uint, CItem>();

    static AbilityManager()
    {
        new UnitManager();

        try
        {
            foreach (var item in EntityManager.GetEntities<Item>())
            {
                if (!item.IsValid || item.Id != AbilityId.item_tpscroll)
                {
                    continue;
                }

                var owner = item.Owner;
                if (owner == null)
                {
                    continue;
                }

                var unit = UnitManager.GetUnitByHandle(owner.Handle);
                if (unit == null)
                {
                    continue;
                }

                ClassTownPortalScroll(unit, item, out var classItem);

                items[classItem.Handle] = classItem;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }

        UnitManager<CUnit>.UnitAdded += UnitAdded;
        UnitManager<CUnit>.UnitRemoved += UnitRemoved;

        //UpdateManager.Subscribe(500, OnInventoryUpdate);

        Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
    }

    private static SpellEventHandler spellAdded;

    public static event SpellEventHandler SpellAdded
    {
        add
        {
            foreach (var spell in Spells)
            {
                try
                {
                    value.Invoke(spell);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }

            spellAdded += value;
        }

        remove
        {
            spellAdded -= value;
        }
    }

    public static event SpellEventHandler SpellRemoved;

    private static ItemEventHandler itemAdded;

    public static event ItemEventHandler ItemAdded
    {
        add
        {
            foreach (var item in Items)
            {
                try
                {
                    value.Invoke(item);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }

            itemAdded += value;
        }

        remove
        {
            itemAdded -= value;
        }
    }

    public static event ItemEventHandler ItemRemoved;

    private static void UnitAdded(CUnit unit)
    {
        RefreshSpells(unit);
        RefreshItems(unit);
    }

    private static void UnitRemoved(CUnit unit)
    {
        foreach (var spell in unit.GetSpells())
        {
            spells.Remove(spell.Handle);
            SpellRemoved?.Invoke(spell);
        }

        foreach (var item in unit.GetItems())
        {
            items.Remove(item.Handle);
            ItemRemoved?.Invoke(item);
        }
    }

    private static readonly ZSleeper sleeper = new ZSleeper();

    internal void AbilityAdded(DivineAbility ability)
    {
        if (ability is Item item)
        {
            if (item.Id != AbilityId.item_tpscroll)
            {
                return;
            }

            var unit = UnitManager.GetUnitByHandle(item.Owner.Handle);
            if (unit == null)
            {
                return;
            }

            ClassTownPortalScroll(unit, item, out var classItem);

            items[classItem.Handle] = classItem;
            itemAdded?.Invoke(classItem);
        }
        else
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var owner = ability.Owner;
            if (owner == null)
            {
                return;
            }

            sleeper.Sleep(100);

            var unit = UnitManager.GetUnitByHandle(owner.Handle);
            if (unit == null)
            {
                return;
            }

            RefreshSpells(unit);
        }
    }

    internal void AbilityRemoved(DivineAbility ability)
    {
        var item = ability as Item;
        if (item == null || item.Id != AbilityId.item_tpscroll)
        {
            return;
        }

        var handle = item.Handle;
        if (!items.TryGetValue(handle, out var classItem))
        {
            return;
        }

        classItem.Dispose();

        items.Remove(handle);
        ItemRemoved?.Invoke(classItem);
    }

    private static void OnInventoryUpdate()  // TODO Temporarily Soon Remove and only use OnInt32PropertyChange
    {
        foreach (var unit in UnitManager<CUnit>.Units)
        {
            RefreshItems(unit);
        }
    }

    private static void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.PropertyName != "m_iParity" || e.NewValue.GetInt32() == e.OldValue.GetInt32())
        {
            return;
        }

        UpdateManager.BeginInvoke(100, () =>
        {
            var unit = UnitManager.GetUnitByHandle(sender.Handle);
            if (unit == null)
            {
                return;
            }

            RefreshItems(unit);
        });
    }

    private static readonly HashSet<string> IgnoreSpells = new HashSet<string>
    {
        "seasonal_ti9_shovel",
        "seasonal_ti9_monkey",
        "seasonal_ti9_instruments",
        "seasonal_summon_ti9_balloon",
        "high_five",
        "seasonal_ti9_banner",
        "seasonal_ti10_portal",
        "seasonal_ti10_soccer_ball",
        "seasonal_ti10_hot_potato",
        "seasonal_ti10_high_five",
        "seasonal_ti10_disco_ball"
    };

    private static void RefreshSpells(CUnit unit)
    {
        var newSpells = new List<CAbility>();

        try
        {
            foreach (var spell in unit.Spellbook.Spells)
            {
                if (IgnoreSpells.Contains(spell.Name))
                {
                    continue;
                }

                ClassSpell(unit, spell, out var classSpell);

                newSpells.Add(classSpell);
            }
        }
        catch (EntityNotFoundException)
        {
            // Ignore
        }

        unit.RefreshSpells(newSpells, out var oldSpells);

        foreach (var spell in newSpells.Except(oldSpells))
        {
            spells[spell.Handle] = spell;
            spellAdded?.Invoke(spell);
        }

        foreach (var spell in oldSpells.Except(newSpells))
        {
            spells.Remove(spell.Handle);
            SpellRemoved?.Invoke(spell);
        }
    }

    private static void RefreshItems(CUnit unit)
    {
        if (!unit.HasInventory)
        {
            return;
        }

        var newItems = new List<CItem>();

        foreach (var item in unit.Inventory.Items)
        {
            ClassItem(unit, item, out var classItem);

            newItems.Add(classItem);
        }

        unit.RefreshItems(newItems, out var oldItems);

        foreach (var item in newItems.Except(oldItems))
        {
            items[item.Handle] = item;
            itemAdded?.Invoke(item);
        }

        foreach (var item in oldItems.Except(newItems))
        {
            item.Dispose();

            items.Remove(item.Handle);
            ItemRemoved?.Invoke(item);
        }
    }

    private static void ClassSpell(CUnit unit, DivineAbility ability, out CAbility classSpell)
    {
        if (spells.TryGetValue(ability.Handle, out classSpell))
        {
            return;
        }

        if (AbilityContainer.TryGetType(ability.Id, out var type))
        {
            classSpell = (CAbility)Activator.CreateInstance(type, ability);
        }
        else
        {
            classSpell = new CAbility(ability);
        }

        classSpell.Owner = unit;
    }

    private static void ClassItem(CUnit unit, Item item, out CItem classItem)
    {
        if (items.TryGetValue(item.Handle, out classItem))
        {
            return;
        }

        if (AbilityContainer.TryGetType(item.Id, out var type))
        {
            classItem = (CItem)Activator.CreateInstance(type, item);
        }
        else
        {
            classItem = new CItem(item);
        }

        classItem.Owner = unit;
    }

    private static void ClassTownPortalScroll(CUnit unit, Item item, out CItem classItem)
    {
        if (items.TryGetValue(item.Handle, out classItem))
        {
            return;
        }

        if (AbilityContainer.TryGetType(AbilityId.item_tpscroll, out var type))
        {
            classItem = (CItem)Activator.CreateInstance(type, item);
        }

        classItem.Owner = unit;
    }

    public static CAbility GetSpellByEntity(Entity entity)
    {
        if (entity == null)
        {
            return null;
        }

        if (spells.TryGetValue(entity.Handle, out var spell))
        {
            return spell;
        }

        return null;
    }

    public static CAbility GetSpellByHandle(uint handle)
    {
        if (spells.TryGetValue(handle, out var spell))
        {
            return spell;
        }

        return null;
    }

    public static CItem GetItemByEntity(Entity entity)
    {
        if (entity == null)
        {
            return null;
        }

        if (items.TryGetValue(entity.Handle, out var item))
        {
            return item;
        }

        return null;
    }

    public static CItem GetItemByHandle(uint handle)
    {
        if (items.TryGetValue(handle, out var item))
        {
            return item;
        }

        return null;
    }

    public static CAbility GetAbilityByEntity(Entity entity)
    {
        if (entity == null)
        {
            return null;
        }

        var handle = entity.Handle;
        if (spells.TryGetValue(handle, out var spell))
        {
            return spell;
        }

        if (items.TryGetValue(handle, out var item))
        {
            return item;
        }

        return null;
    }

    public static CAbility GetAbilityByHandle(uint handle)
    {
        if (spells.TryGetValue(handle, out var spell))
        {
            return spell;
        }

        if (items.TryGetValue(handle, out var item))
        {
            return item;
        }

        return null;
    }

    public static IEnumerable<CAbility> Spells
    {
        get
        {
            return spells.Values.Where(x => x.IsValid);
        }
    }

    public static IEnumerable<CItem> Items
    {
        get
        {
            return items.Values.Where(x => x.IsValid);
        }
    }

    public static IEnumerable<CAbility> Abilities
    {
        get
        {
            return Spells.Concat(Items);
        }
    }
}
