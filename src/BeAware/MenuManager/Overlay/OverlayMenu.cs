namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class OverlayMenu
{
    public OverlayMenu(Menu rootMenu)
    {
        var overlayMenu = rootMenu.AddMenu("Overlay");

        TopPanelMenu = new TopPanelMenu(overlayMenu);
        HpBarMenu = new HpBarMenu(overlayMenu);
        ManaBarMenu = new ManaBarMenu(overlayMenu);
        SpellsMenu = new SpellsMenu(overlayMenu);
        ItemsMenu = new ItemsMenu(overlayMenu);
        TownPortalScrollMenu = new TownPortalScrollMenu(overlayMenu);
        EmemyItemPanelMenu = new EmemyItemPanelMenu(overlayMenu);
        NetworthPanelMenu = new NetworthPanelMenu(overlayMenu);
    }

    public TopPanelMenu TopPanelMenu { get; }

    public HpBarMenu HpBarMenu { get; }

    public ManaBarMenu ManaBarMenu { get; }

    public SpellsMenu SpellsMenu { get; }

    public ItemsMenu ItemsMenu { get; }

    public TownPortalScrollMenu TownPortalScrollMenu { get; }

    public EmemyItemPanelMenu EmemyItemPanelMenu { get; }

    public NetworthPanelMenu NetworthPanelMenu { get; }
}