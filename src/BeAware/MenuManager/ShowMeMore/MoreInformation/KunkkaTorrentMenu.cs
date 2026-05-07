namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class KunkkaTorrentMenu
{
    public KunkkaTorrentMenu(Menu moreInformationMenu)
    {
        var kunkkaTorrentMenu = moreInformationMenu.AddMenu("Kunkka Torrent").SetImage(AbilityId.kunkka_torrent);
        EnableItem = kunkkaTorrentMenu.AddSwitcher("Enable");
        RedItem = kunkkaTorrentMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = kunkkaTorrentMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = kunkkaTorrentMenu.AddSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = kunkkaTorrentMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = kunkkaTorrentMenu.AddSwitcher("Side Message", false);
        SoundItem = kunkkaTorrentMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = kunkkaTorrentMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = kunkkaTorrentMenu.AddSwitcher("Draw On World");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSwitcher WhenIsVisibleItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }
}