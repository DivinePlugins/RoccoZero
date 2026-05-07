namespace BeAware.MenuManager.ShowMeMore.Range;

using Divine.Menu.Items;

internal sealed class CustomRangeMenu
{
    public CustomRangeMenu(Menu rangeMenu)
    {
        var customRangeMenu = rangeMenu.AddMenu("Custom Range");
        EnableItem = customRangeMenu.AddSwitcher("Enable", false);
        RangeItem = customRangeMenu.AddSlider("Range: ------------------------------------------", 500, 0, 5000);
        RedItem = customRangeMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = customRangeMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = customRangeMenu.AddSlider("Blue:", 0, 0, 255);
    }

    public MenuSwitcher EnableItem  { get; }

    public MenuSlider RangeItem  { get; }

    public MenuSlider RedItem  { get; }

    public MenuSlider GreenItem  { get; }

    public MenuSlider BlueItem  { get; }
}