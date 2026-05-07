using System;

using Divine.Core.Entities;
using Divine.Extensions;
using Divine.Numerics;

namespace Divine.Core.Extensions
{
    public static class Vector3Extensions
    {
        public static float AngleBetween(this Vector3 start, Vector3 center, Vector3 end)
        {
            return (center - start).AngleBetween(end - center);
        }

        public static bool IsInRange(this Vector3 sourcePosition, CEntity target, float range)
        {
            return target.Position.IsInRange(sourcePosition, range);
        }

        public static Vector2 To2D(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static float Distance2D(this Vector3 value1, Vector3 value2)
        {
            return value1.To2D().Distance(value2.To2D());
        }

        public static bool MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta, out Vector3 result)
        {
            var x = target.X - current.X;
            var y = target.Y - current.Y;
            var z = target.Z - current.Z;

            var sqdist = x * x + y * y + z * z;
            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
            {
                result = target;
                return true;
            }

            var dist = (float)Math.Sqrt(sqdist);
            result = new Vector3(current.X + x / dist * maxDistanceDelta, current.Y + y / dist * maxDistanceDelta, current.Z + z / dist * maxDistanceDelta);
            return false;
        }

        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            current.MoveTowards(target, maxDistanceDelta, out var result);
            return result;
        }

        public static bool MoveTowards2D(this Vector3 current, Vector3 target, float maxDistanceDelta, out Vector3 result)
        {
            var x = target.X - current.X;
            var y = target.Y - current.Y;

            var sqdist = x * x + y * y;
            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
            {
                result = new Vector3(target.X, target.Y, 0);
                return true;
            }

            var dist = (float)Math.Sqrt(sqdist);
            result = new Vector3(current.X + x / dist * maxDistanceDelta, current.Y + y / dist * maxDistanceDelta, 0);
            return false;
        }

        public static Vector3 MoveTowards2D(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            current.MoveTowards2D(target, maxDistanceDelta, out var result);
            return result;
        }
    }
}