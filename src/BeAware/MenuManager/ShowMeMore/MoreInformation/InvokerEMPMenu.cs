namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class InvokerEMPMenu
{
    public InvokerEMPMenu(Menu moreInformationMenu)
    {
        var invokerEMPMenu = moreInformationMenu.AddMenu("Invoker EMP").SetImage(AbilityId.invoker_emp);
        EnableItem = invokerEMPMenu.AddSwitcher("Enable");
        RedItem = invokerEMPMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = invokerEMPMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = invokerEMPMenu.AddSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = invokerEMPMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = invokerEMPMenu.AddSwitcher("Side Message", false);
        SoundItem = invokerEMPMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = invokerEMPMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = invokerEMPMenu.AddSwitcher("Draw On World");
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