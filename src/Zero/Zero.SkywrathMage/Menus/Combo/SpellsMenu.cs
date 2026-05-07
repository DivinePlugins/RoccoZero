using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class SpellsMenu : BaseSpellsMenu
    {
        [Value(AbilityId.skywrath_mage_arcane_bolt, true)]
        [Value(AbilityId.skywrath_mage_concussive_shot, true)]
        [Value(AbilityId.skywrath_mage_ancient_seal, true)]
        [Value(AbilityId.skywrath_mage_mystic_flare, true)]
        public override MenuAbilityToggler SpellsSelection { get; set; }
    }
}