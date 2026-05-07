namespace Divine.Core.Entities;

using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units.Heroes;

public sealed class CIllusion : CHero
{
    internal CIllusion(Hero hero)
        : base(hero)
    {
    }

    public static implicit operator Hero(CIllusion illusion)
    {
        return illusion.Base;
    }

    public static explicit operator CIllusion(Entity entity)
    {
        return (CIllusion)UnitManager.GetUnitByEntity(entity);
    }
}