using Divine.Core.ComboFactory.Menus;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus
{
    internal sealed class RadiusMenu : BaseRadiusMenu
    {
        [Item("Arc Lightning")]
        public MenuSwitcher ArcLightningItem { get; set; }

        [Item("Lightning Bolt")]
        public MenuSwitcher LightningBoltItem { get; set; }

        [Item("Blink Dagger")]
        [Value(false)]
        public MenuSwitcher BlinkDaggerItem { get; set; }
    }
}
