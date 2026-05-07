namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal class AncientApparitionIceBlastMenu
{
    public AncientApparitionIceBlastMenu(Menu moreInformationMenu)
    {
        var ancientApparitionIceBlastMenu = moreInformationMenu.AddMenu("Ancient Apparition Ice Blast").SetImage(AbilityId.ancient_apparition_ice_blast);
        EnableItem = ancientApparitionIceBlastMenu.AddSwitcher("Enable");
        RedItem = ancientApparitionIceBlastMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = ancientApparitionIceBlastMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = ancientApparitionIceBlastMenu.AddSlider("Blue:", 0, 0, 255);
        LineRedItem = ancientApparitionIceBlastMenu.AddSlider("Line Red:", 139, 0, 255);
        LineGreenItem = ancientApparitionIceBlastMenu.AddSlider("Line Green:", 0, 0, 255);
        LineBlueItem = ancientApparitionIceBlastMenu.AddSlider("Line Blue:", 0, 0, 255);
        SideMessageItem = ancientApparitionIceBlastMenu.AddSwitcher("Side Message");
        SoundItem = ancientApparitionIceBlastMenu.AddSwitcher("Play Sound");
        OnMinimapItem = ancientApparitionIceBlastMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = ancientApparitionIceBlastMenu.AddSwitcher("Draw On World");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider LineRedItem { get; }

    public MenuSlider LineGreenItem { get; }

    public MenuSlider LineBlueItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }

    public MenuSwitcher WriteOnChatItem { get; }
}