namespace DotaMapPlus;

using Divine.Menu;
using Divine.Numerics;

internal sealed class Context
{
    //private readonly ZoomHack ZoomHack;

    private readonly ConsoleCommands ConsoleCommands;

    private readonly WeatherHack WeatherHack;

    public Context()
    {
        var rootMenu = MenuManager.CreateRootMenu("DotaMapPlus");
        rootMenu.SetFontColor(Color.Aqua);

        //ZoomHack = new ZoomHack(rootMenu);

        ConsoleCommands = new ConsoleCommands(rootMenu);

        WeatherHack = new WeatherHack(rootMenu);
    }

    public void Dispose()
    {
        //ZoomHack.Dispose();
        ConsoleCommands.Dispose();
        WeatherHack.Dispose();
    }
}