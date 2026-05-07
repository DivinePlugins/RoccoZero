using System;

using Divine.Core.Entities;
using Divine.Extensions;
using Divine.Numerics;

namespace Divine.Core.Extensions
{
    public static class Vector2Extensions
    {
        public static bool IsInRange(this Vector2 sourcePosition, CEntity target, float range)
        {
            return target.Position.IsInRange(sourcePosition, range);
        }

        public static bool IsUnderRectangle(this Vector2 position, RectangleF rectangleF)
        {
            return position.IsUnderRectangle(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
        }

        public static bool IsUnderRectangle(this Vector2 position, float x, float y, float width, float height)
        {
            return position.X > x && position.X <= x + width && position.Y > y && position.Y <= y + height;
        }

        public static bool MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta, out Vector2 result)
        {
            var x = target.X - current.X;
            var y = target.Y - current.Y;

            var sqdist = x * x + y * y;
            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
            {
                result = target;
                return true;
            }

            var dist = (float)Math.Sqrt(sqdist);
            result = new Vector2(current.X + x / dist * maxDistanceDelta, current.Y + y / dist * maxDistanceDelta);
            return false;
        }

        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            MoveTowards(current, target, maxDistanceDelta, out var result);
            return result;
        }
    }
}