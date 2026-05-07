namespace BeAware.MenuManager;

using BeAware.MenuManager.Overlay;
using BeAware.MenuManager.PartialMapHack;
using BeAware.MenuManager.ShowMeMore;

using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

internal sealed class MenuConfig
{
    public MenuConfig()
    {
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.divinebeaware.png");
        var rootMenu = MenuManager.InformationMenu.AddMenu("BeAware").SetImage("BeAware.Resources.Textures.divinebeaware.png").SetDisplayTextFontColor(Color.Aqua);

        //OverlayMenu = new OverlayMenu(rootMenu);
        ShowMeMoreMenu = new ShowMeMoreMenu(rootMenu);
        PartialMapHackMenu = new PartialMapHackMenu(rootMenu);

        FullyDisableSoundsItem = rootMenu.AddSwitcher("Fully Disable Sounds", false);
        VolumeItem = rootMenu.AddSlider("Volume", 100, 0, 100);
        DefaultSoundItem = rootMenu.AddSwitcher("Default Sound", false).SetTooltip("All Sounds Becomes Default");
        LanguageItem = rootMenu.AddSelector("Language", ["EN", "RU"]);
    }

    public OverlayMenu OverlayMenu { get; }

    public ShowMeMoreMenu ShowMeMoreMenu { get; }

    public PartialMapHackMenu PartialMapHackMenu { get; }

    public MenuSwitcher FullyDisableSoundsItem { get; }

    public MenuSlider VolumeItem { get; }

    public MenuSwitcher DefaultSoundItem { get; }

    public MenuSelector LanguageItem { get; }

    public void Dispose()
    {

    }
}