namespace BeAware.MenuManager.ShowMeMore;

using Divine.Menu.Items;

internal sealed class ShowMeMoreMenu
{
    public ShowMeMoreMenu(Menu rootMenu)
    {
        var showMeMoreMenu = rootMenu.AddMenu("Show Me More");

        MoreInformationMenu = new MoreInformationMenu(showMeMoreMenu);
        //VisibleByEnemyMenu = new VisibleByEnemyMenu(showMeMoreMenu);
        //TrueSightVisionMenu = new TrueSightVisionMenu(showMeMoreMenu);
        //TowerHelperMenu = new TowerHelperMenu(showMeMoreMenu);
        //IllusionShowMenu = new IllusionShowMenu(showMeMoreMenu);
        //LinkenShowMenu = new LinkenShowMenu(showMeMoreMenu);
        RangeMenu = new RangeMenu(showMeMoreMenu);
        RoshanMenu = new RoshanMenu(showMeMoreMenu);
        CheckRuneMenu = new CheckRuneMenu(showMeMoreMenu);
        CheckHandOfMidasMenu = new CheckHandOfMidasMenu(showMeMoreMenu);
    }

    public MoreInformationMenu MoreInformationMenu { get; }

    //public VisibleByEnemyMenu VisibleByEnemyMenu { get; }

    //public TrueSightVisionMenu TrueSightVisionMenu { get; }

    //public TowerHelperMenu TowerHelperMenu { get; }

    //public IllusionShowMenu IllusionShowMenu { get; }

    //public LinkenShowMenu LinkenShowMenu { get; }

    public RangeMenu RangeMenu { get; }

    public RoshanMenu RoshanMenu { get; }

    public CheckRuneMenu CheckRuneMenu { get; }

    public CheckHandOfMidasMenu CheckHandOfMidasMenu { get; }
}