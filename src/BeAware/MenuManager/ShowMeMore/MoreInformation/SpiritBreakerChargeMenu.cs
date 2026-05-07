namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal class SpiritBreakerChargeMenu
{
    public SpiritBreakerChargeMenu(Menu moreInformationMenu)
    {
        var spiritBreakerChargeMenu = moreInformationMenu.AddMenu("Spirit Breaker Charge").SetImage(AbilityId.spirit_breaker_charge_of_darkness);
        EnableItem = spiritBreakerChargeMenu.AddSwitcher("Enable");
        RedItem = spiritBreakerChargeMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = spiritBreakerChargeMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = spiritBreakerChargeMenu.AddSlider("Blue:", 0, 0, 255);
        AlphaItem = spiritBreakerChargeMenu.AddSlider("Alpha:", 40, 0, 255);
        SideMessageItem = spiritBreakerChargeMenu.AddSwitcher("Side Message");
        SoundItem = spiritBreakerChargeMenu.AddSwitcher("Play Sound");
        OnMinimapItem = spiritBreakerChargeMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = spiritBreakerChargeMenu.AddSwitcher("Draw On World");
        WriteOnChatItem = spiritBreakerChargeMenu.AddSwitcher("Write On Chat", false);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider AlphaItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }

    public MenuSwitcher WriteOnChatItem { get; }
}