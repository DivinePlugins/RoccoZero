namespace BeAware.MenuManager.ShowMeMore;

using Divine.Menu;
using Divine.Menu.Items;
using Divine.Renderer;

public class CheckRuneMenu
{
    public CheckRuneMenu(Menu showMeMoreMenu)
    {
        var checkRuneMenu = showMeMoreMenu.AddMenu("Check Rune").SetImage("rune_regen", ImageType.Ability);
        EnableItem = checkRuneMenu.AddSwitcher("Enable");
        SideMessageItem = checkRuneMenu.AddSwitcher("Side Message");
        PlaySoundItem = checkRuneMenu.AddSwitcher("Play Sound");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher PlaySoundItem { get; }
}