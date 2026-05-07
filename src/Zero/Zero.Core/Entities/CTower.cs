namespace Divine.Core.Entities;

using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units.Buildings;

public sealed class CTower : CBuilding
{
    internal CTower(Tower tower)
        : base(tower)
    {
        Base = tower;
    }

    public new Tower Base { get; }

    public override CUnit AttackTarget
    {
        get
        {
            var attackTarget = Base.AttackTarget;
            if (attackTarget != null)
            {
                return UnitManager.GetUnitByHandle(attackTarget.Handle);
            }

            return null;
        }
    }

    public static implicit operator Tower(CTower tower)
    {
        return tower.Base;
    }

    public static explicit operator CTower(Entity entity)
    {
        return (CTower)UnitManager.GetUnitByEntity(entity);
    }
}