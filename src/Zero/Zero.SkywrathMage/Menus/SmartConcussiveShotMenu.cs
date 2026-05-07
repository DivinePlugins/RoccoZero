using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartConcussiveShotMenu
    {
        [Item("Anti Fail")]
        public MenuSwitcher AntiFailItem { get; set; }

        [Item("Use Only Target")]
        [Tooltip("This only works with Combo")]
        public MenuSwitcher UseOnlyTargetItem { get; set; }

        [Item("Use In Radius")]
        [Tooltip("This only works with Combo")]
        [Value(1400, 800, 1600)]
        public MenuSlider UseInRadiusItem { get; set; }
    }
}
