using Divine.Core.ComboFactory.Menus.Settings.Drawing;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings
{
    public class BaseSettingsMenu
    {
        [Menu("Drawing")]
        public DrawingMenu DrawingMenu { get; } = new DrawingMenu();

        [Item("Enable Damage Calculation")]
        [Tooltip("Disabling Damage Calculation will give you some FPS! But you will lose KillSteal and Drawing Damage Calculation")]
        public MenuSwitcher EnableDamageCalculationItem { get; set; }

        [Item("Enable Drawing")]
        [Tooltip("Disabling Drawing will give you some FPS!")]
        public MenuSwitcher EnableDrawingItem { get; set; }
    }
}
