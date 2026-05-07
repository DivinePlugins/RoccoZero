using Divine.Numerics;

namespace Divine.Core.Extensions
{
    public static class RectangleFExtensions
    {
        public static RectangleF MultiplyBy(this RectangleF rect, float scale)
        {
            return new RectangleF(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
        }

        public static RectangleF MultiplyBy(this RectangleF rect, float scaleX, float scaleY)
        {
            return new RectangleF(rect.X * scaleX, rect.Y * scaleY, rect.Width * scaleX, rect.Height * scaleY);
        }
    }
}