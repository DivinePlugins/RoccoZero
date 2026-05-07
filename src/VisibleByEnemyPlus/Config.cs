namespace VisibleByEnemyPlus;

using System.Collections.Generic;
using System.Linq;

using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;

internal sealed class Config
{
    public MenuSwitcher HeroesItem { get; }

    public MenuSwitcher CouriersItem { get; }

    public MenuSwitcher WardsItem { get; }

    public MenuSwitcher MinesItem { get; }

    public MenuSwitcher TowersItem { get; }

    public MenuSwitcher OutpostsItem { get; }

    public MenuSwitcher WatchersItem { get; }

    public MenuSwitcher TormentorsItem { get; }

    public MenuSwitcher BuildingsItem { get; }

    public MenuSwitcher NeutralsItem { get; }

    public MenuSwitcher CreepsItem { get; }

    public MenuSwitcher OthersItem { get; }

    public MenuSwitcher PredictionItem { get; }

    public MenuSelector EffectTypeItem { get; }

    public MenuSlider RedItem { get; set; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider AlphaItem { get; }

    public Config()
    {
        var rootMenu = MenuManager.CreateRootMenu("VisibleByEnemyPlus").SetFontColor(Color.Aqua);

        //PredictionItem = rootMenu.CreateSwitcher("Prediction1", "Prediction");

        EffectTypeItem = rootMenu.CreateSelector("Effect Type", Effects.Keys.ToArray());

        RedItem = rootMenu.CreateSlider("Red", 255, 0, 255);
        GreenItem = rootMenu.CreateSlider("Green", 255, 0, 255);
        BlueItem = rootMenu.CreateSlider("Blue", 255, 0, 255);
        AlphaItem = rootMenu.CreateSlider("Alpha", 255, 0, 255);

        if (EffectTypeItem == "Default")
        {
            RedItem.SetFontColor(Color.Black);
            GreenItem.SetFontColor(Color.Black);
            BlueItem.SetFontColor(Color.Black);
            AlphaItem.SetFontColor(Color.Black);
        }
        else
        {
            RedItem.SetFontColor(new Color(RedItem, 0, 0, 255));
            GreenItem.SetFontColor(new Color(0, GreenItem, 0, 255));
            BlueItem.SetFontColor(new Color(0, 0, BlueItem, 255));
            AlphaItem.SetFontColor(new Color(185, 176, 163, AlphaItem));
        }

        HeroesItem = rootMenu.CreateSwitcher("Heroes");
        CouriersItem = rootMenu.CreateSwitcher("Couriers");
        WardsItem = rootMenu.CreateSwitcher("Wards");
        MinesItem = rootMenu.CreateSwitcher("Mines");
        TowersItem = rootMenu.CreateSwitcher("Towers");
        WatchersItem = rootMenu.CreateSwitcher("Watchers");
        OutpostsItem = rootMenu.CreateSwitcher("Outposts");
        TormentorsItem = rootMenu.CreateSwitcher("Tormentors");
        BuildingsItem = rootMenu.CreateSwitcher("Buildings22", "Buildings", false);
        NeutralsItem = rootMenu.CreateSwitcher("Neutrals");
        CreepsItem = rootMenu.CreateSwitcher("Creeps22", "Creeps", false);
        OthersItem = rootMenu.CreateSwitcher("Others22", "Others", false);
    }

    public Dictionary<string, string> Effects { get; } = new Dictionary<string, string>
    {
        { "Default", "particles/items_fx/aura_shivas.vpcf" },
        { "Default MOD", "materials/ensage_ui/particles/visiblebyenemy.vpcf" },
        { "VBE", "materials/ensage_ui/particles/vbe.vpcf" },
        { "Omniknight", "materials/ensage_ui/particles/visiblebyenemy_omniknight.vpcf" },
        { "Assault", "materials/ensage_ui/particles/visiblebyenemy_assault.vpcf" },
        { "Arrow", "materials/ensage_ui/particles/visiblebyenemy_arrow.vpcf" },
        { "Mark", "materials/ensage_ui/particles/visiblebyenemy_mark.vpcf" },
        { "Glyph", "materials/ensage_ui/particles/visiblebyenemy_glyph.vpcf" },
        { "Coin", "materials/ensage_ui/particles/visiblebyenemy_coin.vpcf" },
        { "Lightning", "materials/ensage_ui/particles/visiblebyenemy_lightning.vpcf" },
        { "Energy Orb", "materials/ensage_ui/particles/visiblebyenemy_energy_orb.vpcf" },
        { "Pentagon", "materials/ensage_ui/particles/visiblebyenemy_pentagon.vpcf" },
        { "Axis", "materials/ensage_ui/particles/visiblebyenemy_axis.vpcf" },
        { "Beam Jagged", "materials/ensage_ui/particles/visiblebyenemy_beam_jagged.vpcf" },
        { "Beam Rainbow", "materials/ensage_ui/particles/visiblebyenemy_beam_rainbow.vpcf" },
        { "Walnut Statue", "materials/ensage_ui/particles/visiblebyenemy_walnut_statue.vpcf" },
        { "Thin Thick", "materials/ensage_ui/particles/visiblebyenemy_thin_thick.vpcf" },
        { "Ring Wave", "materials/ensage_ui/particles/visiblebyenemy_ring_wave.vpcf" },
        { "Visible", "materials/ensage_ui/particles/visiblebyenemy_visible.vpcf" }
    };
}