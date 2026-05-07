namespace Divine.Core.Entities.Utilities;

using System.Collections.Generic;

using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

public sealed class CachedInfo
{
    private static readonly HashSet<CachedInfo> CachedInfos = new HashSet<CachedInfo>();

    private readonly CUnit Unit;

    public CachedInfo(CUnit unit)
    {
        Unit = unit;

        CachedInfos.Add(this);
    }

    static CachedInfo()
    {
        RendererManager.Draw += OnUpdate;
    }

    private static void OnUpdate()
    {
        var rawGameTime = GameManager.RawGameTime;

        foreach (var cachedInfo in CachedInfos)
        {
            var unit = cachedInfo.Unit;
            var isVisible = unit.Base.IsVisible;
            var position = unit.Base.Position;

            cachedInfo.IsCached = true;

            if (isVisible)
            {
                cachedInfo.LastVisibleTime = rawGameTime;
            }

            cachedInfo.Position = position;
            cachedInfo.ScreenPosition = RendererManager.WorldToScreen(position);
            cachedInfo.HealthBarPosition = RendererManager.WorldToScreen(position + new Vector3(0, 0, unit.HealthBarOffset));
        }
    }

    public static CachedInfo Run(CUnit unit)
    {
        return unit.CachedInfo;
    }

    public void Dispose()
    {
        CachedInfos.Remove(this);
    }

    public bool IsCached { get; private set; }

    public float LastVisibleTime { get; private set; } = GameManager.RawGameTime - 0.1f;

    public Vector3 Position { get; private set; }

    public Vector2 ScreenPosition { get; private set; }

    public Vector2 HealthBarPosition { get; private set; }
}