namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal sealed class SpellsMenu
{
    public SpellsMenu(Menu overlayMenu)
    {
        var spellsMenu = overlayMenu.AddMenu("Spells");
        AllyOverlayItem = spellsMenu.AddSwitcher("Ally", false);
        EnemyOverlayItem = spellsMenu.AddSwitcher("Enemy");
        ModeItem = spellsMenu.AddSelector("Mode:", ["Default", "Without Texture", "Low"]);
        ExtraSizeItem = spellsMenu.AddSlider("Extra Size:", 0, -10, 10);
        ExtraPosXItem = spellsMenu.AddSlider("Extra Pos X:", 0, -150, 150);
        ExtraPosYItem = spellsMenu.AddSlider("Extra Pos Y:", 0, -150, 150);
    }

    public MenuSwitcher AllyOverlayItem { get; }

    public MenuSwitcher EnemyOverlayItem { get; }

    public MenuSelector ModeItem { get; }

    public MenuSlider ExtraSizeItem { get; }

    public MenuSlider ExtraPosXItem { get; }

    public MenuSlider ExtraPosYItem { get; }
}