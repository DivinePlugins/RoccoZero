namespace Divine.Core.Entities;

using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units.Buildings;

public class CBuilding : CUnit
{
    internal CBuilding(Building building)
        : base(building)
    {
        Base = building;
    }

    public new Building Base { get; }

    public static implicit operator Building(CBuilding building)
    {
        return building.Base;
    }

    public static explicit operator CBuilding(Entity entity)
    {
        return (CBuilding)UnitManager.GetUnitByEntity(entity);
    }
}