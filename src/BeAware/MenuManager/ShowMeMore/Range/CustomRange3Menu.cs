namespace BeAware.MenuManager.ShowMeMore.Range;

using Divine.Menu.Items;

internal sealed class CustomRange3Menu
{
    public CustomRange3Menu(Menu rangeMenu)
    {
        var customRange3Menu = rangeMenu.AddMenu("Custom Range 3");
        EnableItem = customRange3Menu.AddSwitcher("Enable", false);
        RangeItem = customRange3Menu.AddSlider("Range: ------------------------------------------", 700, 0, 5000);
        RedItem = customRange3Menu.AddSlider("Red:", 255, 0, 255);
        GreenItem = customRange3Menu.AddSlider("Green:", 0, 0, 255);
        BlueItem = customRange3Menu.AddSlider("Blue:", 0, 0, 255);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RangeItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }
}