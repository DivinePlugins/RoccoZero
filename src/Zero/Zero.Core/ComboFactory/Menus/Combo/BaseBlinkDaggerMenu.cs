using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public class BaseBlinkDaggerMenu
    {
        [Item("Blink Activation Distance Mouse:")]
        [Value(1000, 0, 1200)]
        public virtual MenuSlider BlinkActivationItem { get; set; }

        [Item("Blink Distance From Enemy:")]
        [Value(300, 0, 500)]
        public MenuSlider BlinkDistanceEnemyItem { get; set; }
    }
}