using Divine.Core.ComboFactory.Menus;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class RadiusMenu : BaseRadiusMenu
    {
        [Item("Arcane Bolt")]
        public MenuSwitcher ArcaneBoltItem { get; set; }

        [Item("Concussive Shot")]
        public MenuSwitcher ConcussiveShotItem { get; set; }

        [Item("Ancient Seal")]
        public MenuSwitcher AncientSealItem { get; set; }

        [Item("Mystic Flare")]
        public MenuSwitcher MysticFlareItem { get; set; }

        [Item("Target Hit Concussive Shot")]
        public MenuSwitcher TargetHitConcussiveShotItem { get; set; }

        [Item("Blink Dagger")]
        [Value(false)]
        public MenuSwitcher BlinkDaggerItem { get; set; }
    }
}
