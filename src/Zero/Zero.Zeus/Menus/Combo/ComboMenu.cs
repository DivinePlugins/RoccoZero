using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Zeus.Menus.Combo
{
    internal sealed class ComboMenu : BaseComboMenu
    {
        public override BaseSpellsMenu SpellsMenu { get; } = new SpellsMenu();

        public override BaseItemsMenu ItemsMenu { get; } = new ItemsMenu();

        [Menu("Lightning Bolt"), AbilityImage(AbilityId.zuus_lightning_bolt)]
        public LightningBoltMenu LightningBoltMenu { get; } = new LightningBoltMenu();

        [Menu("Thundergods Wrath"), AbilityImage(AbilityId.zuus_thundergods_wrath)]
        public ThundergodsWrathMenu ThundergodsWrathMenu { get; } = new ThundergodsWrathMenu();

        [Item("Orbwalker:")]
        [Value("Free", "Default", "Distance", "Only Attack", "No Move")]
        [Priority(10)]
        public override MenuSelector OrbwalkerItem { get; set; }
    }
}