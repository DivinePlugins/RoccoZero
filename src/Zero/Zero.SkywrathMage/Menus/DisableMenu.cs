using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class DisableMenu : BaseDisableMenu
    {
        [Value(AbilityId.item_sheepstick, true)]
        [Value(AbilityId.item_orchid, true)]
        [Value(AbilityId.item_bloodthorn, true)]
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        public override MenuAbilityToggler AbilitiesSelection { get; set; }
    }
}