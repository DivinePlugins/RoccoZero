using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Zeus.Menus
{
    internal sealed class KillStealMenu : BaseKillStealMenu
    {
        [Priority(8)]
        [Value(AbilityId.item_veil_of_discord, true)]
        [Value(AbilityId.item_ethereal_blade, true)]
        [Value(AbilityId.item_dagon_5, true)]
        [Value(AbilityId.item_shivas_guard, true)]
        [Value(AbilityId.zuus_arc_lightning, true)]
        [Value(AbilityId.zuus_lightning_bolt, true)]
        [Value(AbilityId.zuus_cloud, true)]
        [Value(AbilityId.zuus_thundergods_wrath, true)]
        public override MenuAbilityToggler AbilitiesSelection { get; set; }

        [Item("EmptyString", " ")]
        [Priority(7)]
        public MenuText EmptyString { get; set; }

        [Item("Move Camera On Enemy")]
        [Priority(7)]
        public MenuSwitcher MoveCameraItem { get; set; }

        [Item("Camera Delay:")]
        [Value(2000, 800, 2000)]
        [Priority(7)]
        public MenuSlider CameraDelayItem { get; set; }

        [Item("EmptyString2", " ")]
        [Priority(7)]
        public MenuText EmptyString2 { get; set; }

    }
}