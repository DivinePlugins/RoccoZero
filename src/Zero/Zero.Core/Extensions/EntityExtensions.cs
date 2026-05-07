using System;

using Divine.Core.Entities;
using Divine.Extensions;
using Divine.Numerics;

namespace Divine.Core.Extensions
{
    public static class EntityExtensions
    {
        public static float Distance2D(this CUnit entity, CUnit other, bool fromCenterToCenter = false)
        {
            return entity.Distance2D(other.Position) - (fromCenterToCenter ? 0f : entity.HullRadius + other.HullRadius);
        }

        public static float Distance2D(this CEntity entity, CEntity other)
        {
            return entity.Distance2D(other.Position);
        }

        public static float Distance2D(this CEntity entity, Vector3 position)
        {
            var entityPosition = entity.Position;
            return (float)Math.Sqrt(Math.Pow(entityPosition.X - position.X, 2) + Math.Pow(entityPosition.Y - position.Y, 2));
        }

        public static bool IsInRange(this CEntity source, CEntity target, float range)
        {
            return source.Position.IsInRange(target, range);
        }

        public static bool IsInRange(this CEntity source, Vector2 targetPosition, float range)
        {
            return source.Position.IsInRange(targetPosition, range);
        }

        public static bool IsInRange(this CEntity source, Vector3 targetPosition, float range)
        {
            return source.Position.IsInRange(targetPosition, range);
        }
    }
}
