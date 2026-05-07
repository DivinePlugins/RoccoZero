using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public abstract class BaseItemsMenu
    {
        [Item("Use:")]
        public abstract MenuItemToggler ItemsSelection { get; set; }
    }
}
