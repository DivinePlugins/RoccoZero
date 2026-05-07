namespace Divine.Core.Utils;

using Divine.Numerics;

public static class ItemPanelUtils
{
    public static Vector2 ItemPanelBack(float size)
    {
        var sizeX = 20 + size * 0.4f;
        return new Vector2(sizeX, sizeX / 1.30f);  //TODO
    }

    public static Vector2 ItemPanelSize(float size)
    {
        var sizeX = 20 + size * 0.4f;
        return new Vector2(sizeX, sizeX / 1.30f);
    }
}