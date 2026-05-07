using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.TargetSelector
{
    public class TargetEffectsMenu
    {
        [Item("Enable")]
        public MenuSwitcher EnableItem { get; set; }

        [Item("Red1", "Red:")]
        [Value(255, 0, 255)]
        public MenuSlider RedItem { get; set; }

        [Item("Green1", "Green:")]
        [Value(0, 0, 255)]
        public MenuSlider GreenItem { get; set; }

        [Item("Blue1", "Blue:")]
        [Value(0, 0, 255)]
        public MenuSlider BlueItem { get; set; }

        [Item(" ")]
        public MenuText EmptyString { get; set; }

        [Item("Free Enable")]
        public MenuSwitcher FreeEnableItem { get; set; }

        [Item("Red2", "Red:")]
        [Value(0, 0, 255)]
        public MenuSlider FreeRedItem { get; set; }

        [Item("Green2", "Green:")]
        [Value(255, 0, 255)]
        public MenuSlider FreeGreenItem { get; set; }

        [Item("Blue2", "Blue:")]
        [Value(255, 0, 255)]
        public MenuSlider FreeBlueItem { get; set; }

        [Item("Effect Type:")]
        [Value("Default", "Without Circle")]
        public MenuSelector EffectTypeItem { get; set; }
    }
}
