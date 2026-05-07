using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ItemsMenu : BaseItemsMenu
    {
        [Value(ItemId.item_sheepstick, true)]
        [Value(ItemId.item_orchid, true)]
        [Value(ItemId.item_bloodthorn, true)]
        [Value(ItemId.item_nullifier, true)]
        [Value(ItemId.item_rod_of_atos, true)]
        [Value(ItemId.item_gungir, true)]
        [Value(ItemId.item_ethereal_blade, true)]
        [Value(ItemId.item_veil_of_discord, true)]
        [Value(ItemId.item_dagon_5, true)]
        [Value(ItemId.item_shivas_guard, true)]
        [Value(ItemId.item_urn_of_shadows, true)]
        [Value(ItemId.item_spirit_vessel, true)]
        [Value(ItemId.item_blink, false)]
        public override MenuItemToggler ItemsSelection { get; set; }
    }
}
