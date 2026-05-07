using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public class BaseDisableMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Use:")]
        [Priority(2)]
        public virtual MenuAbilityToggler AbilitiesSelection { get; set; }
    }
}
