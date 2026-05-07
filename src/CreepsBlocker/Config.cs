namespace CreepsBlocker;

using System;

using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;

internal class Config : IDisposable
{
    public Config()
    {
        var factory = MenuManager.CreateRootMenu("Creeps Blocker");
        MenuKey = factory.CreateHoldKey("Hotkey", Key.None);
        BlockRangedCreep = factory.CreateSwitcher("Block ranged creep");
        BlockSensitivity = factory.CreateSlider("Block sensitivity", 550, 500, 700);
        BlockSensitivity.SetTooltip("Bigger value will result in smaller block, but with higher success rate");
        CenterCamera = factory.CreateSwitcher("Center camera", true);
    }

    public MenuSwitcher BlockRangedCreep { get; }

    public MenuSlider BlockSensitivity { get; }

    public MenuSwitcher CenterCamera { get; }

    public MenuHoldKey MenuKey { get; }

    public void Dispose()
    {
        //factory?.Dispose();
    }
}