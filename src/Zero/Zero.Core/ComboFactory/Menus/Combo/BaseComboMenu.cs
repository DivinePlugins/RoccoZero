using System.ComponentModel;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Input;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus.Combo
{
    public abstract class BaseComboMenu
    {
        protected BaseComboMenu()
        {
            if (BlinkDaggerMenu == null)
            {
                BlinkDaggerMenu = new BaseBlinkDaggerMenu();
            }
        }

        [Menu("Spells")]
        public abstract BaseSpellsMenu SpellsMenu { get; }

        [Menu("Items")]
        public abstract BaseItemsMenu ItemsMenu { get; }

        [Menu("Blink Dagger"), AbilityImage(AbilityId.item_blink)]
        public virtual BaseBlinkDaggerMenu BlinkDaggerMenu { get; }

        [Menu("Aeon Disk"), AbilityImage(AbilityId.item_aeon_disk)]
        public BaseAeonDiskMenu AeonDiskMenu { get; } = new BaseAeonDiskMenu();

        [Menu("With Mute")]
        public BaseWithMuteMenu WithMuteMenu { get; } = new BaseWithMuteMenu();

        [Item("Combo Hotkey")]
        [Value(Key.D)]
        public MenuHoldKey ComboHotkeyItem { get; set; }

        [Item("Orbwalker:")]
        [Value("Default", "Distance", "Free", "Only Attack", "No Move")]
        public virtual MenuSelector OrbwalkerItem { get; set; }

        [Item("Min Distance In Orbwalk:")]
        [Value(600, 200, 600)]
        public MenuSlider MinDisInOrbwalkItem { get; set; }

        [Item("Full Distance Mode")]
        [Value(false)]
        public MenuSwitcher FullDistanceModeItem { get; set; }

        [Item("Full Free Mode")]
        [Value(false)]
        public MenuSwitcher FullFreeModeItem { get; set; }
    }
}
