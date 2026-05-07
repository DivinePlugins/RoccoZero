using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public abstract class BaseSpellsMenu
    {
        [Item("Use:")]
        public abstract MenuAbilityToggler SpellsSelection { get; set; }
    }
}