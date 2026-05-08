using Divine.Numerics;

namespace Divine.Core.Extensions
{
    public static class RectangleFExtensions
    {
        public static Rect MultiplyBy(this Rect rect, float scale)
        {
            return new Rect(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
        }

        public static Rect MultiplyBy(this Rect rect, float scaleX, float scaleY)
        {
            return new Rect(rect.X * scaleX, rect.Y * scaleY, rect.Width * scaleX, rect.Height * scaleY);
        }
    }
}