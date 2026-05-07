namespace BeAware.MenuManager.ShowMeMore;

using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Menu;
using Divine.Menu.Items;

public class CheckHandOfMidasMenu
{
    public CheckHandOfMidasMenu(Menu showMeMoreMenu)
    {
        var checkHandOfMidasMenu = showMeMoreMenu.AddMenu("Check Hand Of Midas").SetImage(ItemId.item_hand_of_midas);
        EnableItem = checkHandOfMidasMenu.AddSwitcher("Enable");
        SideMessageItem = checkHandOfMidasMenu.AddSwitcher("Side Message");
        PlaySoundItem = checkHandOfMidasMenu.AddSwitcher("Play Sound");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher PlaySoundItem { get; }
}