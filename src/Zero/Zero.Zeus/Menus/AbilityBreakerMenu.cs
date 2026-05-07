using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus
{
    internal sealed class AbilityBreakerMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Spells:")]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        [Value(AbilityId.zuus_cloud, true)]
        public MenuAbilityToggler AbilitiesSelection { get; set; }

        [Item("Full Range")]
        public MenuSwitcher FullRangeItem { get; set; }

        [Item("Range:")]
        [Value(5000, 1000, 8000)]
        public MenuSlider RangeItem { get; set; }

        [Item("EmptyString", " ")]
        public MenuText EmptyString { get; set; }

        [Item("Teleport:")]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        [Value(AbilityId.zuus_cloud, true)]
        public MenuAbilityToggler TeleportAbilitiesSelection { get; set; }

        [Item("Only Visible")]
        [Value(false)]
        public MenuSwitcher TeleportVisibleItem { get; set; }

        [Item("Full Range")]
        [Value(false)]
        public MenuSwitcher TeleportFullRangeItem { get; set; }

        [Item("Range:")]
        [Value(5000, 1000, 8000)]
        public MenuSlider TeleportRangeItem { get; set; }
    }
}
