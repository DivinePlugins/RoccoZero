namespace Ensage.SDK.Orbwalker
{
    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Units;
    using Divine.Numerics;

    public interface IOrbwalker
    {
        bool Attack(Unit target);

        bool CanAttack(Unit target);

        bool CanMove();

        float GetTurnTime(Entity unit);

        bool Move(Vector3 position);

        bool OrbwalkTo(Unit target);
    }
}