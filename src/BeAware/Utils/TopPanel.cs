namespace BeAware.Utils;

using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Helpers;
using Divine.Numerics;
using Divine.Renderer;

public sealed class TopPanel
{
    public Vector2 IconPosition { get; }

    public float SizeX { get; } = HUDInfo.TopPanelSizeX;

    public float SizeY { get; } = HUDInfo.TopPanelSizeY;

    public TopPanel(Hero hero)
    {
        IconPosition = HUDInfo.GetTopPanelPosition(hero);

        var ratioPercentage = HUDInfo.RatioPercentage;
        HelthManaBackPosition = IconPosition + new Vector2(0.5f, SizeY);
        HelthManaBackSize = new Vector2(SizeX, (4 * ratioPercentage) + HealthSizeY + ManaSizeY);

        HealthPosition = HelthManaBackPosition + new Vector2(1, 1.21f * ratioPercentage);
        ManaPosition = HealthPosition + new Vector2(0, 1.5f + HealthSizeY);

        var midle = (SizeX / 2) - (21.565f * ratioPercentage);
        UltimateBackPosition = HelthManaBackPosition + new Vector2(midle, HelthManaBackSize.Y + (8 * ratioPercentage));
        UltimateSpellPosition = HelthManaBackPosition + new Vector2(midle + 3.6f, HelthManaBackSize.Y + (11.4f * ratioPercentage));

        if (hero.Team == Team.Radiant)
        {
            VisibleIconName = "hero_visible_left";
        }
        else
        {
            VisibleIconName = "hero_visible_right";
        }

        VisibleIconRectangle = new RectangleF(IconPosition.X - 4, IconPosition.Y + 4.5f, SizeX + 8.5f, SizeY - 5);
    }

    public Vector2 HelthManaBackPosition { get; }

    public Vector2 HelthManaBackSize { get; }

    public Vector2 HealthPosition { get; }

    public float HealthSizeX(float healthPerc)
    {
        return (SizeX - 2) * healthPerc;
    }

    public float HealthSizeY { get; } = 4.84f * HUDInfo.RatioPercentage;

    public Vector2 ManaPosition { get; }

    public float ManaSizeX(float manaPerc)
    {
        return (SizeX - 2) * manaPerc;
    }

    public float ManaSizeY { get; } = 4.84f * HUDInfo.RatioPercentage;

    public Vector2 UltimateBackPosition { get; }

    public Vector2 UltimateBackSize { get; } = new Vector2(43.13f * HUDInfo.RatioPercentage);

    public Vector2 UltimateSpellPosition { get; }

    public Vector2 UltimateSpellSize { get; } = new Vector2(36.5f * HUDInfo.RatioPercentage);

    public Vector2 UltimateCooldownPosition(string cooldownText)
    {
        var measureText = RendererManager.MeasureText(cooldownText, UltimateCooldownSize);
        return UltimateSpellPosition + new Vector2(((UltimateSpellSize.X - 3) / 2) - (measureText.X / 2), (UltimateSpellSize.Y * 0.1f)); //TODO
    }

    public float UltimateCooldownSize { get; } = 26.58f * HUDInfo.RatioPercentage;

    public string VisibleIconName { get; }

    public RectangleF VisibleIconRectangle { get; }
}