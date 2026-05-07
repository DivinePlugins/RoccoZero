using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class MoreMenu : BaseMoreMenu
    {
        [Menu("Auto Combo")]
        public AutoComboMenu AutoComboMenu { get; } = new AutoComboMenu();

        [Menu("Smart Arcane Bolt"), AbilityImage(AbilityId.skywrath_mage_arcane_bolt)]
        public SmartArcaneBoltMenu SmartArcaneBoltMenu { get; } = new SmartArcaneBoltMenu();

        [Menu("Smart Concussive Shot"), AbilityImage(AbilityId.skywrath_mage_concussive_shot)]
        public SmartConcussiveShotMenu SmartConcussiveShotMenu { get; } = new SmartConcussiveShotMenu();
    }
}