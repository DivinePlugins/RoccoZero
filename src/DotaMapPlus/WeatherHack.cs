namespace DotaMapPlus;

using System;

using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

internal sealed class WeatherHack
{
    private readonly MenuSelector WeatherItem;

    public WeatherHack(RootMenu rootMenu)
    {
        var weatherHackMenu = rootMenu.CreateMenu("Weather Hack");
        WeatherItem = weatherHackMenu.CreateSelector("Selected", WeatherNames);
        WeatherItem.ValueChanged += OnWeatherValueChanged;
    }

    public void Dispose()
    {
        GameConsoleManager.SetValue("cl_weather", 0);

        WeatherItem.ValueChanged -= OnWeatherValueChanged;
    }

    private void OnWeatherValueChanged(MenuSelector selector, SelectorEventArgs e)
    {
        GameConsoleManager.SetValue("cl_weather", Array.IndexOf(WeatherNames, e.NewValue));
    }

    private static readonly string[] WeatherNames =
    {
        "Default",
        "Snow",
        "Rain",
        "Moonbeam",
        "Pestilence",
        "Harvest",
        "Sirocco",
        "Ash",
        "Aurora"
    };
}