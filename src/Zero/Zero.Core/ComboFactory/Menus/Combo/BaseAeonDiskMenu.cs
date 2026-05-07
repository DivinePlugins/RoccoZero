using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public class BaseAeonDiskMenu
    {
        [Item("Cancel Important Spells and Items")]
        [Tooltip("If Combo Breaker is ready then it will not use Important Spells and Items")]
        public MenuSwitcher EnableItem { get; set; }
    }
}