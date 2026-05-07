namespace Divine.Core.Entities;

using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;

public class CCourier : CUnit
{
    internal CCourier(Courier courier)
        : base(courier)
    {
        Base = courier;
    }

    public new Courier Base { get; }

    public bool IsFlying
    {
        get
        {
            return Base.IsFlying;
        }
    }

    public CourierState State
    {
        get
        {
            return Base.State;
        }
    }

    public Hero StateIssuer
    {
        get
        {
            return Base.StateIssuer;
        }
    }

    public float RespawnTime
    {
        get
        {
            return Base.RespawnTime;
        }
    }

    public static implicit operator Courier(CCourier courier)
    {
        return courier.Base;
    }

    public static explicit operator CCourier(Entity entity)
    {
        return (CCourier)UnitManager.GetUnitByEntity(entity);
    }
}