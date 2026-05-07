using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus
{
    public abstract class BaseLinkenBreakerMenu
    {
        [Item("Linkens Sphere:")]
        public MenuText LinkensSphereString { get; set; }

        [Item("PriorityLinkensItem", "Priority:")]
        [Parameter("Priority", true)]
        [Priority(3)]
        public abstract MenuAbilityToggler PriorityLinkensItem { get; set; }

        [Item("EmptyString", " ")]
        public MenuText EmptyString { get; set; }

        [Item("Antimage Spell Shield:")]
        public MenuText AntimageSpellShieldString { get; set; }

        [Item("PrioritySpellShieldItem", "Priority:")]
        [Parameter("Priority", true)]
        [Priority(5)]
        public abstract MenuAbilityToggler PrioritySpellShieldItem { get; set; }

        [Item("EmptyString2", " ")]
        public MenuText EmptyString2 { get; set; }

        [Item("Use Only From Range")]
        [Tooltip("Use only from the Range and do not use another Ability")]
        [Value(false)]
        public MenuSwitcher UseOnlyFromRangeItem { get; set; }
    }
}
