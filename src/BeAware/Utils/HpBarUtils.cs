namespace Divine.Core.Utils;

using Divine.Helpers;
using Divine.Numerics;

public static class HpBarUtils
{
    public static Vector2 HpBarValuePosition(Vector2 hpBarPosition, float size)
    {
        return hpBarPosition + new Vector2((HUDInfo.HpBarSizeX / 2) - size * 0.5f, 1 * HUDInfo.RatioPercentage);
    }
}