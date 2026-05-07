using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Managers.Unit.Delegates;
using Divine.Common.Log;

namespace Divine.Core.Managers.Unit
{
    public static class UnitManager<TUnit, T>
        where TUnit : CUnit
        where T : IType
    {
        private static readonly HashSet<TUnit> units = new HashSet<TUnit>();

        private static readonly IType type = TypeHelper.GetType<T>();

        static UnitManager()
        {
            UnitManager<TUnit>.UnitAdded += OnAdded;
            UnitManager<TUnit>.UnitRemoved += OnRemoved;

            LogManager.Info($"================ Activate UnitManager<{typeof(TUnit).Name}, {typeof(T).Name}>");
        }

        private static UnitEventHandler<TUnit> unitAdded;

        public static event UnitEventHandler<TUnit> UnitAdded
        {
            add
            {
                foreach (var unit in Units)
                {
                    try
                    {
                        value.Invoke(unit);
                    }
                    catch (Exception e)
                    {
                        LogManager.Error(e);
                    }
                }

                unitAdded += value;
            }

            remove
            {
                unitAdded -= value;
            }
        }

        public static event UnitEventHandler<TUnit> UnitRemoved;

        private static void OnAdded(TUnit unit)
        {
            if (!type.GetControl(unit))
            {
                return;
            }

            units.Add(unit);
            unitAdded?.Invoke(unit);
        }

        private static void OnRemoved(TUnit unit)
        {
            if (!type.GetControl(unit))
            {
                return;
            }

            units.Remove(unit);
            UnitRemoved?.Invoke(unit);
        }

        public static IEnumerable<TUnit> Units
        {
            get
            {
                return units.Where(x => x.IsValid);
            }
        }
    }
}