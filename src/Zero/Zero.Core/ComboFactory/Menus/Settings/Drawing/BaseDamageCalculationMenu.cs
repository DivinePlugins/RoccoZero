using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings.Drawing
{
    public class BaseDamageCalculationMenu
    {
        [Item("Hp Bar")]
        [DefaultValue(true)]
        public MenuSwitcher HpBarItem { get; set; }

        [Item("Value")]
        [DefaultValue(false)]
        public MenuSwitcher ValueItem { get; set; }
    }
}