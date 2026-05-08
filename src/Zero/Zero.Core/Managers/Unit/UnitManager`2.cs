using Divine.Core.Entities;
using Divine.Core.Managers.Unit.Delegates;

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

            Logger.LogInformation($"================ Activate UnitManager<{typeof(TUnit).Name}, {typeof(T).Name}>");
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
                        Logger.LogError(e);
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
