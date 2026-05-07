namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal class InvokerSunStrikeMenu
{
    public InvokerSunStrikeMenu(Menu moreInformationMenu)
    {
        var invokerSunStrikeMenu = moreInformationMenu.AddMenu("Invoker Sun Strike").SetImage(AbilityId.invoker_sun_strike);
        EnableItem = invokerSunStrikeMenu.AddSwitcher("Enable");
        RedItem = invokerSunStrikeMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = invokerSunStrikeMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = invokerSunStrikeMenu.AddSlider("Blue:", 0, 0, 255);
        SideMessageItem = invokerSunStrikeMenu.AddSwitcher("Side Message");
        SoundItem = invokerSunStrikeMenu.AddSwitcher("Play Sound");
        OnMinimapItem = invokerSunStrikeMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = invokerSunStrikeMenu.AddSwitcher("Draw On World");
        WriteOnChatItem = invokerSunStrikeMenu.AddSwitcher("Write On Chat", false);
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