using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Divine.Core.Entities;
using Divine.Core.Entities.Metadata;
using Divine.Core.Managers.Ability;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Update;

namespace Divine.Core.Managers
{
    public static class ActionManager
    {
        private static readonly Dictionary<uint, Action<object>> ActionIds = new Dictionary<uint, Action<object>>();

        static ActionManager()
        {
            AbilityManager.SpellAdded += AbilityAdded;
            AbilityManager.SpellRemoved += AbilityRemoved;

            AbilityManager.ItemAdded += AbilityAdded;
            AbilityManager.ItemRemoved += AbilityRemoved;
        }

        private static void AbilityAdded(CAbility ability)
        {
            if (!ActionIds.TryGetValue(ability.Owner.Handle ^ (uint)ability.Id, out var action))
            {
                return;
            }

            action?.Invoke(ability);
            UpdateManager.BeginInvoke(() => action?.Invoke(ability));
        }

        private static void AbilityRemoved(CAbility ability)
        {
            if (!ActionIds.TryGetValue(ability.Owner.Handle ^ (uint)ability.Id, out var action))
            {
                return;
            }

            action?.Invoke(null);
        }

        private static AbilityId[] TypeToIds<T>()
        {
            var type = typeof(T);
            var spellAttribute = type.GetCustomAttribute<SpellAttribute>();
            if (spellAttribute != null)
            {
                return new[] { spellAttribute.AbilityId };
            }

            var ItemAttributes = (ItemAttribute[])type.GetCustomAttributes<ItemAttribute>();
            var abilityIds = new AbilityId[ItemAttributes.Length];
            for (var i = 0; i < ItemAttributes.Length; i++)
            {
                abilityIds[i] = ItemAttributes[i].AbilityId;
            }

            return abilityIds;
        }

        public static void ActionAdd<T>(Action<T> action)
            where T : CAbility
        {
            ActionAdd(UnitManager.Owner, action);
        }

        public static void ActionAdd<T>(CUnit unit, Action<T> action)
            where T : CAbility
        {
            foreach (var id in TypeToIds<T>())
            {
                ActionAdd(unit, id, action);
            }
        }

        public static void ActionAdd<T>(AbilityId id, Action<T> action)
            where T : CAbility
        {
            ActionAdd(UnitManager.Owner, id, action);
        }

        public static void ActionAdd<T>(CUnit unit, AbilityId id, Action<T> action)
            where T : CAbility
        {
            var ability = unit.Abilities.FirstOrDefault(x => x.Id == id);  //TODO TEST
            if (ability != null)
            {
                action?.Invoke((T)ability);
            }

            ActionIds[unit.Handle ^ (uint)id] = new Action<object>(x => action((T)x));
        }

        public static void ActionRemove<T>()
            where T : CAbility
        {
            ActionRemove<T>(UnitManager.Owner);
        }

        public static void ActionRemove<T>(CUnit unit)
            where T : CAbility
        {
            foreach (var id in TypeToIds<T>())
            {
                ActionRemove<T>(unit, id);
            }
        }

        public static void ActionRemove<T>(CUnit unit, AbilityId id)
            where T : CAbility
        {
            ActionIds.Remove(unit.Handle ^ (uint)id);
        }

        public static void ActionRemove(object o)
        {
            ActionRemove(o, UnitManager.Owner);
        }

        public static void ActionRemove(object o, CUnit unit)
        {
            foreach (var method in o.GetType().GetMethods().Where(x => typeof(CAbility).IsAssignableFrom(x.ReturnType)))
            {
                var name = method.ReturnType.Name;

                if (name.Contains("Aggregations"))
                {
                    continue;
                }

                if (!Enum.TryParse(name, out AbilityId id))
                {
                    continue;
                }

                ActionIds.Remove(unit.Handle ^ (uint)id);
            }
        }
    }
}