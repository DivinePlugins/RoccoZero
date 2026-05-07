using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus
{
    internal sealed class LinkenBreakerMenu : BaseLinkenBreakerMenu
    {
        [Value(AbilityId.zuus_arc_lightning, true)]
        [Value(AbilityId.item_force_staff, true)]
        [Value(AbilityId.item_cyclone, true)]
        [Value(AbilityId.item_wind_waker, true)]
        [Value(AbilityId.item_orchid, true)]
        [Value(AbilityId.item_bloodthorn, true)]
        [Value(AbilityId.item_nullifier, true)]
        [Value(AbilityId.item_rod_of_atos, true)]
        [Value(AbilityId.item_sheepstick, true)]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        public override MenuAbilityToggler PriorityLinkensItem { get; set; }

        [Value(AbilityId.zuus_arc_lightning, true)]
        [Value(AbilityId.item_force_staff, true)]
        [Value(AbilityId.item_cyclone, true)]
        [Value(AbilityId.item_wind_waker, true)]
        [Value(AbilityId.item_orchid, false)]
        [Value(AbilityId.item_bloodthorn, false)]
        [Value(AbilityId.item_nullifier, false)]
        [Value(AbilityId.item_rod_of_atos, true)]
        [Value(AbilityId.item_sheepstick, false)]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        public override MenuAbilityToggler PrioritySpellShieldItem { get; set; }
    }
}
