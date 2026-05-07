using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus.Combo
{
    internal sealed class SpellsMenu : BaseSpellsMenu
    {
        [Value(AbilityId.zuus_arc_lightning, true)]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        [Value(AbilityId.zuus_cloud, true)]
        [Value(AbilityId.zuus_thundergods_wrath, false)]
        public override MenuAbilityToggler SpellsSelection { get; set; }
    }
}