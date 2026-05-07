namespace BeAware.MenuManager.ShowMeMore;

using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Renderer;

internal sealed class RoshanMenu
{
    public RoshanMenu(Menu showMeMoreMenu)
    {
        var roshanMenu = showMeMoreMenu.AddMenu("Roshan").SetImage("npc_dota_hero_roshan", ImageType.Unit);
        PanelItem = roshanMenu.AddSwitcher("Panel");
        AegisItem = roshanMenu.AddSwitcher("Aegis").SetImage(ItemId.item_aegis);
        SideMessageItem = roshanMenu.AddSwitcher("Side Message");
        PlaySoundItem = roshanMenu.AddSwitcher("Play Sound");
    }

    public MenuSwitcher PanelItem { get; }

    public MenuSwitcher AegisItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher PlaySoundItem { get; }
}