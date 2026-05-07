using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class LinkenBreakerMenu : BaseLinkenBreakerMenu
    {
        [Value(AbilityId.item_force_staff, true)]
        [Value(AbilityId.item_cyclone, true)]
        [Value(AbilityId.item_wind_waker, true)]
        [Value(AbilityId.item_orchid, true)]
        [Value(AbilityId.item_bloodthorn, true)]
        [Value(AbilityId.item_nullifier, true)]
        [Value(AbilityId.item_rod_of_atos, true)]
        [Value(AbilityId.item_sheepstick, true)]
        [Value(AbilityId.skywrath_mage_arcane_bolt, false)]
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        public override MenuAbilityToggler PriorityLinkensItem { get; set; }

        [Value(AbilityId.item_force_staff, true)]
        [Value(AbilityId.item_cyclone, true)]
        [Value(AbilityId.item_wind_waker, true)]
        [Value(AbilityId.item_orchid, false)]
        [Value(AbilityId.item_bloodthorn, false)]
        [Value(AbilityId.item_rod_of_atos, true)]
        [Value(AbilityId.item_nullifier, false)]
        [Value(AbilityId.item_sheepstick, false)]
        [Value(AbilityId.skywrath_mage_arcane_bolt, true)]
        [Value(AbilityId.skywrath_mage_ancient_seal, false)]
        public override MenuAbilityToggler PrioritySpellShieldItem { get; set; }
    }
}
