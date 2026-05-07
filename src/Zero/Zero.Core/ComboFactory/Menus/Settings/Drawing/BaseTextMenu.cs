using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings.Drawing
{
    public class BaseTextPanelMenu
    {
        [Item("Combo Panel")]
        public MenuSwitcher ComboPanelItem { get; set; }

        [Item("Move")]
        [Value(false)]
        public MenuSwitcher MoveItem { get; set; }

        [Item("X")]
        [Value(0, -5000, 5000)]
        public MenuSlider X { get; set; }

        [Item("Y")]
        [Value(0, -5000, 5000)]
        public MenuSlider Y { get; set; }
    }
}
