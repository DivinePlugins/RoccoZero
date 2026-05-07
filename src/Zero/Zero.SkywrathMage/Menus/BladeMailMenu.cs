using Divine.Core.ComboFactory.Menus;
using Divine.Menu.Items;

using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class BladeMailMenu : BaseBladeMailMenu
    {
        [Item("Eul Control")]
        [Tooltip("Use Eul if there is BladeMail with ULT")]
        public MenuSwitcher EulControlItem { get; set; }
    }
}