using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Zeus.Menus.Combo
{
    internal sealed class LightningBoltMenu
    {
        [Item("Methods:")]
        [Value("On Target & Prediction On Position", "On Target", "Prediction On Position")]
        public MenuSelector MethodsItem { get; set; }
    }
}