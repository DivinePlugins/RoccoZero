namespace CreepsAggro;

using System;

using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;

internal class Config : IDisposable
{
    public Config()
    {
        var factory = MenuManager.CreateRootMenu("Creeps Aggro");
        Aggro = factory.CreateHoldKey("Aggro", Key.None);
        UnAggro = factory.CreateHoldKey("Drop aggro", Key.None);
    }

    public MenuHoldKey Aggro { get; }

    public MenuHoldKey UnAggro { get; }

    public MenuSwitcher MoveToMousePosition { get; }

    public void Dispose()
    {
        //factory?.Dispose();
    }
}