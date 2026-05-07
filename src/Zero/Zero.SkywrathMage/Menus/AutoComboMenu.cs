using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoComboMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Disable When Combo")]
        public MenuSwitcher DisableWhenComboItem { get; set; }

        [Item("Owner Min Health % To Auto Combo:")]
        [Value(0, 0, 70)]
        public MenuSlider OwnerMinHealthItem { get; set; }

        [Item("Spells:")]
        [Value(AbilityId.skywrath_mage_arcane_bolt, true)]
        [Value(AbilityId.skywrath_mage_concussive_shot, true)]
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        [Value(AbilityId.skywrath_mage_mystic_flare, true)]
        public MenuAbilityToggler SpellsSelection { get; set; }

        [Item("Items:")]
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
        public MenuItemToggler ItemsSelection { get; set; }

        [Item("Target Min Health % To Ult:")]
        [Value(0, 0, 70)]
        public MenuSlider MinHealthToUltItem { get; set; }
    }
}