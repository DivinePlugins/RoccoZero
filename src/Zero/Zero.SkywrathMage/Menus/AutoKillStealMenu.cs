using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class AutoKillStealMenu : BaseKillStealMenu
    {
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        [Value(AbilityId.item_veil_of_discord, true)]
        [Value(AbilityId.item_ethereal_blade, true)]
        [Value(AbilityId.item_dagon_5, true)]
        [Value(AbilityId.item_shivas_guard, true)]
        [Value(AbilityId.skywrath_mage_concussive_shot, true)]
        [Value(AbilityId.skywrath_mage_arcane_bolt, true)]
        [Value(AbilityId.skywrath_mage_mystic_flare, true)]
        public override MenuAbilityToggler AbilitiesSelection { get; set; }
    }
}