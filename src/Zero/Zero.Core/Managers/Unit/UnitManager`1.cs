using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Managers.Unit.Delegates;
using Divine.Common.Log;

namespace Divine.Core.Managers.Unit
{
    public static class UnitManager<TUnit>
        where TUnit : CUnit
    {
        private static readonly HashSet<TUnit> units = new HashSet<TUnit>();

        static UnitManager()
        {
            UnitManager.UnitAdded += OnAdded;
            UnitManager.UnitRemoved += OnRemoved;
        }

        private static UnitEventHandler<TUnit> onUnitAdded;

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

                onUnitAdded += value;
            }

            remove
            {
                onUnitAdded -= value;
            }
        }

        public static event UnitEventHandler<TUnit> UnitRemoved;

        private static void OnAdded(CUnit unit)
        {
            var type = unit as TUnit;
            if (type == null)
            {
                return;
            }

            units.Add(type);
            onUnitAdded?.Invoke(type);
        }

        private static void OnRemoved(CUnit unit)
        {
            var type = unit as TUnit;
            if (type == null)
            {
                return;
            }

            units.Remove(type);
            UnitRemoved?.Invoke(type);
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