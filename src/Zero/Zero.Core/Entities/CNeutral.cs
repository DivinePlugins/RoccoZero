namespace Divine.Core.Entities;

using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities;

public class CNeutral : CCreep
{
    internal CNeutral(Creep creep)
        : base(creep)
    {
    }

    public static implicit operator Creep(CNeutral neutral)
    {
        return neutral.Base;
    }

    public static explicit operator CNeutral(Entity entity)
    {
        return (CNeutral)UnitManager.GetUnitByEntity(entity);
    }
}