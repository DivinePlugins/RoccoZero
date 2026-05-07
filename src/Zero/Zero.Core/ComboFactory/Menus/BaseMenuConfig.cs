using Divine.Core.ComboFactory.Menus.AutoItems;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.Settings;
using Divine.Core.ComboFactory.Menus.TargetSelector;
using Divine.Core.Managers.Menu;
using Divine.Entity.Entities.Abilities.Components;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public abstract class BaseMenuConfig
    {
        protected BaseMenuConfig()
        {
            if (MoreMenu == null)
            {
                MoreMenu = new BaseMoreMenu();
            }

            if (FarmMenu == null)
            {
                FarmMenu = new BaseFarmMenu();
            }

            if (DisableMenu == null)
            {
                DisableMenu = new BaseDisableMenu();
            }

            if (BladeMailMenu == null)
            {
                BladeMailMenu = new BaseBladeMailMenu();
            }

            if (RadiusMenu == null)
            {
                RadiusMenu = new BaseRadiusMenu();
            }

            MenuFactory.RegisterMenu(this);
        }

        public void Dispose()
        {
            MenuFactory.DeregisterMenu(this);
        }

        [Menu("Combo")]
        [Priority(1)]
        public abstract BaseComboMenu ComboMenu { get; }

        [Menu("Kill Steal")]
        [Priority(2)]
        public abstract BaseKillStealMenu KillStealMenu { get; }

        [Menu("More")]
        [Priority(3)]
        public virtual BaseMoreMenu MoreMenu { get; }

        [Menu("Farm")]
        [Priority(4)]
        public virtual BaseFarmMenu FarmMenu { get; }

        //[Menu("Escape")]
        //[Priority(5)]
        //public BaseEscapeMenu EscapeMenu { get; } = new BaseEscapeMenu();

        [Menu("Disable"), AbilityImage(AbilityId.item_sheepstick)]
        [Priority(7)]
        public virtual BaseDisableMenu DisableMenu { get; } = new BaseDisableMenu();

        [Menu("Linken Breaker"), AbilityImage(AbilityId.item_sphere)]
        [Priority(8)]
        public abstract BaseLinkenBreakerMenu LinkenBreakerMenu { get; }

        [Menu("Auto Items")]
        [Priority(9)]
        public AutoItemsMenu AutoItemsMenu { get; } = new AutoItemsMenu();

        [Menu("Blade Mail"), AbilityImage(AbilityId.item_blade_mail)]
        [Priority(10)]
        public virtual BaseBladeMailMenu BladeMailMenu { get; }

        [Menu("Radius")]
        [Priority(11)]
        public virtual BaseRadiusMenu RadiusMenu { get; }

        [Menu("Target Selector")]
        [Priority(12)]
        public BaseTargetSelectorMenu TargetSelectorMenu { get; } = new BaseTargetSelectorMenu();

        [Menu("Settings")]
        [Priority(13)]
        public BaseSettingsMenu SettingsMenu { get; } = new BaseSettingsMenu();
    }
}