namespace BodyBlocker.Modes.HeroBlocker;

using Divine.Menu;
using Divine.Menu.Items;

internal class HeroBlockerSettings
{
    public HeroBlockerSettings(Menu root)
    {
        var subFactory = root.AddMenu("Hero blocker");
        Key = subFactory.AddHoldKey("Hotkey").SetTooltip("Block enemy hero with your hero  (Hold)");
        BlockSensitivity = subFactory.AddSlider("Block sensitivity", 150, 100, 200).SetTooltip("Bigger value will result in smaller block, but with higher success rate");
        CenterCamera = subFactory.AddSwitcher("Center camera");
    }

    public MenuSlider BlockSensitivity { get; }

    public MenuSwitcher CenterCamera { get; }

    public MenuHoldKey Key { get; }
}