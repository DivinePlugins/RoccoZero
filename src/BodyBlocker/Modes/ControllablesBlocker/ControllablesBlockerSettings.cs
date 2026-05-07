namespace BodyBlocker.Modes.ControllablesBlocker;

using Divine.Menu;
using Divine.Menu.Items;

internal class ControllablesBlockerSettings
{
    public ControllablesBlockerSettings(Menu root)
    {
        var menu = root.AddMenu("Controllables blocker");
        Key = menu.AddHoldKey("Hotkey").SetTooltip("Block enemy hero with controllable units (Press)");
        BlockSensitivity = menu.AddSlider("Block sensitivity", 150, 100, 200).SetTooltip("Bigger value will result in smaller block, but with higher success rate");
        ControllablesCount = menu.AddSlider("Units", 2, 1, 5).SetTooltip("Number of units to use");
        SpreadUnits = menu.AddSwitcher("Spread units").SetTooltip("If enabled units will try to form an arc, otherwise they all will run in front of the hero");
    }

    public MenuSlider BlockSensitivity { get; }

    public MenuSlider ControllablesCount { get; }

    public MenuHoldKey Key { get; }

    public MenuSwitcher SpreadUnits { get; }
}