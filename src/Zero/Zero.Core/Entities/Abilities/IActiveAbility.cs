using Divine.Numerics;

namespace Divine.Core.Entities.Abilities
{
    public interface IActiveAbility
    {
        float CastPoint { get; }

        bool CanHit(CUnit target);

        int GetCastDelay(CUnit target);

        int GetCastDelay(Vector3 position);

        int GetCastDelay();

        int GetHitTime(CUnit target);

        int GetHitTime(Vector3 position);
    }
}
