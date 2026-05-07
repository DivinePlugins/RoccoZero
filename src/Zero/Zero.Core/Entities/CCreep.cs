namespace Divine.Core.Entities;

using Divine.Core.Managers;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units.Creeps;

public class CCreep : CUnit
{
    internal CCreep(Creep creep)
        : base(creep)
    {
        Base = creep;
    }

    public new Creep Base { get; }

    public static implicit operator Creep(CCreep creep)
    {
        return creep.Base;
    }

    public static explicit operator CCreep(Entity entity)
    {
        return (CCreep)UnitManager.GetUnitByEntity(entity);
    }
}