using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class MysticFlareMenu
    {
        [Item("Target Min Health % To Ult:")]
        [Value(0, 0, 70)]
        public MenuSlider MinHealthToUltItem { get; set; }

        [Item(" ")]
        public MenuText EmptyString { get; set; }

        [Item("Bad Ult")]
        [Tooltip("It is not recommended to enable this. If you do not have these items (RodofAtos, Hex, Ethereal) then this function is activated")]
        [Value(false)]
        public MenuSwitcher BadUltItem { get; set; }

        [Item("Bad Ult Movement Speed:")]
        [Tooltip("If the enemy has less Movement Speed from this value, then immediately ULT")]
        [Value(500, 240, 500)]
        public MenuSlider BadUltMovementSpeedItem { get; set; }
    }
}
