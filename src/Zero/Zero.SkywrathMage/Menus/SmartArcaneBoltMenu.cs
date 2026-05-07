using Divine.Input;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class SmartArcaneBoltMenu
    {
        [Item("Toggle Hotkey")]
        [Value(Key.None)]
        public MenuToggleKey ToggleHotkeyItem { get; set; }

        [Item("Owner Min Health % To Auto Arcane Bolt:")]
        [Value(20, 0, 70)]
        public MenuSlider OwnerMinHealthItem { get; set; }

        [Item(" ")]
        public MenuText EmptyString { get; set; }

        [Item("Spam Hotkey")]
        [Value(Key.None)]
        public MenuHoldKey SpamHotkeyItem { get; set; }

        [Item("Spam Units")]
        public MenuSwitcher SpamUnitsItem { get; set; }

        [Item("Orbwalker")]
        [Value("Distance", "Default", "Free", "Only Attack", "No Move")]
        public MenuSelector OrbwalkerItem { get; set; }

        [Item("Min Distance In Orbwalk:")]
        [Value(600, 200, 600)]
        public MenuSlider MinDisInOrbwalkItem { get; set; }

        [Item("Full Free Mode")]
        [Value(false)]
        public MenuSwitcher FullFreeModeItem { get; set; }
    }
}
