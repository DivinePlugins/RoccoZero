using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.AutoItems
{
    public sealed class AutoPhaseBootsMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Distance Check:")]
        [Value(700, 0, 2000)]
        public MenuSlider DistanceCheckItem { get; set; }
    }
}