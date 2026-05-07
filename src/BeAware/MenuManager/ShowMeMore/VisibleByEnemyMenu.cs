//namespace BeAware.MenuManager.ShowMeMore;

//using Divine.Menu.Items;

//internal sealed class VisibleByEnemyMenu
//{
//    private readonly string[] EffectTypeNames =
//    {
//        "Default",
//        "Default MOD",
//        "VBE",
//        "Omniknight",
//        "Assault",
//        "Arrow",
//        "Mark",
//        "Glyph",
//        "Coin",
//        "Lightning",
//        "Energy Orb",
//        "Pentagon",
//        "Axis",
//        "Beam Jagged",
//        "Beam Rainbow",
//        "Walnut Statue",
//        "Thin Thick",
//        "Ring Wave",
//        "Visible"
//    };

//    public VisibleByEnemyMenu(Menu showMeMoreMenu)
//    {
//        var visibleByEnemyMenu = showMeMoreMenu.AddMenu("Visible By Enemy");
//        EnableItem = visibleByEnemyMenu.AddSwitcher("Enable");
//        EffectTypeItem = visibleByEnemyMenu.AddSelector("Effect Type", EffectTypeNames);
//        RedItem = visibleByEnemyMenu.AddSlider("Red:", 255, 0, 255);
//        GreenItem = visibleByEnemyMenu.AddSlider("Green:", 255, 0, 255);
//        BlueItem = visibleByEnemyMenu.AddSlider("Blue:", 255, 0, 255);
//        AlphaItem = visibleByEnemyMenu.AddSlider("Alpha:", 255, 0, 255);
//        AlliedHeroesItem = visibleByEnemyMenu.AddSwitcher("Allied Heroes");
//        WardsItem = visibleByEnemyMenu.AddSwitcher("Wards");
//        MinesItem = visibleByEnemyMenu.AddSwitcher("Mines");
//        OutpostsItem = visibleByEnemyMenu.AddSwitcher("Outposts");
//        NeutralsItem = visibleByEnemyMenu.AddSwitcher("Neutrals");
//        UnitsItem = visibleByEnemyMenu.AddSwitcher("Units");
//        BuildingsItem = visibleByEnemyMenu.AddSwitcher("Buildings");
//    }

//    public MenuSwitcher EnableItem  { get; }

//    public MenuSelector EffectTypeItem  { get; }

//    public MenuSlider RedItem { get; }

//    public MenuSlider GreenItem { get; }

//    public MenuSlider BlueItem { get; }

//    public MenuSlider AlphaItem { get; }

//    public MenuSwitcher AlliedHeroesItem  { get; }

//    public MenuSwitcher WardsItem  { get; }

//    public MenuSwitcher MinesItem  { get; }

//    public MenuSwitcher OutpostsItem { get; }

//    public MenuSwitcher NeutralsItem  { get; }

//    public MenuSwitcher UnitsItem  { get; }

//    public MenuSwitcher BuildingsItem  { get; }
//}