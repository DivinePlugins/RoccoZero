using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Settings.Drawing
{
    public class DrawingMenu
    {
        [Menu("Text Panel")]
        public BaseTextPanelMenu TextPanelMenu { get; } = new BaseTextPanelMenu();

        [Menu("Damage Calculation")]
        public BaseDamageCalculationMenu DamageCalculationMenu { get; } = new BaseDamageCalculationMenu();
    }
}