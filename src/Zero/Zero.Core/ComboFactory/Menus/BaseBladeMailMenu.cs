using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseBladeMailMenu
    {
        [Item("Cancel Combo")]
        [Tooltip("Cancel Combo if there is enemy Blade Mail")]
        [Value(false)]
        public MenuSwitcher BladeMailItem { get; set; }
    }
}
