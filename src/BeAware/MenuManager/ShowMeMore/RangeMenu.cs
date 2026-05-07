namespace BeAware.MenuManager.ShowMeMore;

using BeAware.MenuManager.ShowMeMore.Range;

using Divine.Menu.Items;

internal sealed class RangeMenu
{
    public RangeMenu(Menu showMeMoreMenu)
    {
        var rangeMenu = showMeMoreMenu.AddMenu("Range");
        CustomRangeMenu = new CustomRangeMenu(rangeMenu);
        CustomRange2Menu = new CustomRange2Menu(rangeMenu);
        CustomRange3Menu = new CustomRange3Menu(rangeMenu);
    }

    public CustomRangeMenu CustomRangeMenu { get; }

    public CustomRange2Menu CustomRange2Menu { get; }

    public CustomRange3Menu CustomRange3Menu { get; }
}