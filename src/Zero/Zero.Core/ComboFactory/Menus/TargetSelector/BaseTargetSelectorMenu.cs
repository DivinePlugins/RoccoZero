using System.ComponentModel;

using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.Core.ComboFactory.Menus.TargetSelector
{
    public class BaseTargetSelectorMenu
    {
        [Menu("Target Effects")]
        public TargetEffectsMenu TargetEffectsMenu { get; } = new TargetEffectsMenu();

        [Item("Select:")]
        [Value("Near Mouse", "Lowest Health")]
        public MenuSelector TargetSelectorItem { get; set; }

        [Item("Target Lock")]
        public MenuSwitcher TargetLockItem { get; set; }
    }
}