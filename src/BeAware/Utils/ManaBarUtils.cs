namespace Divine.Core.Utils;

using Divine.Helpers;
using Divine.Numerics;

public static class ManaBarUtils
{
    private static bool IsValueManaBar { get; set; }

    public static Vector2 ManaBarPosition(Vector2 hpBarPosition)
    {
        return hpBarPosition + new Vector2(1, HUDInfo.HpBarSizeY + 1.5f); ;
    }

    public static Vector2 ManaBarBackPosition(Vector2 manaBarPosition)
    {
        return manaBarPosition - new Vector2(0.5f, 1.07f);
    }

    public static float ManaBarSizeX(float manaPerc)
    {
        return (HUDInfo.HpBarSizeX - 3) * manaPerc;
    }

    public static float ManaBarSizeY { get; } = HUDInfo.HpBarSizeY / 2.8f;

    public static Vector2 ManaBarValuePosition(Vector2 manaBarPosition, float size)
    {
        IsValueManaBar = true;
        return manaBarPosition + new Vector2((HUDInfo.HpBarSizeX / 2) - size * 0.5f, -1 * HUDInfo.RatioPercentage);
    }
}