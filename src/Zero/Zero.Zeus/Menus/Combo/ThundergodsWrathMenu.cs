using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus.Combo
{
    internal sealed class ThundergodsWrathMenu
    {
        [Item("Target Min Health % To Ult:")]
        [Value(20, 10, 100)]
        public MenuSlider MinHealthToUltItem { get; set; }

        [Item("Target Min Range To Ult:")]
        [Value(1200, 300, 2000)]
        public MenuSlider MinRangeToUltItem { get; set; }
    }
}