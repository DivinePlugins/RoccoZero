namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class BloodseekerRuptureMenu
{
    public BloodseekerRuptureMenu(Menu moreInformationMenu)
    {
        var bloodseekerRuptureMenu = moreInformationMenu.AddMenu("Bloodseeker Rupture").SetImage(AbilityId.bloodseeker_rupture);
        EnableItem = bloodseekerRuptureMenu.AddSwitcher("Enable");
        AutoStopItem = bloodseekerRuptureMenu.AddSwitcher("Auto Stop");
        RedItem = bloodseekerRuptureMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = bloodseekerRuptureMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = bloodseekerRuptureMenu.AddSlider("Blue:", 0, 0, 255);
        AlphaItem = bloodseekerRuptureMenu.AddSlider("Alpha:", 40, 0, 255);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSwitcher AutoStopItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider AlphaItem { get; }
}