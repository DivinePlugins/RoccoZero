namespace BeAware.MenuManager.ShowMeMore.Range;

using Divine.Menu.Items;

internal sealed class CustomRange2Menu
{
    public CustomRange2Menu(Menu rangeMenu)
    {
        var customRange2Menu = rangeMenu.AddMenu("Custom Range 2");
        EnableItem = customRange2Menu.AddSwitcher("Enable", false);
        RangeItem = customRange2Menu.AddSlider("Range: ------------------------------------------", 600, 0, 5000);
        RedItem = customRange2Menu.AddSlider("Red:", 255, 0, 255);
        GreenItem = customRange2Menu.AddSlider("Green:", 0, 0, 255);
        BlueItem = customRange2Menu.AddSlider("Blue:", 0, 0, 255);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RangeItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }
}