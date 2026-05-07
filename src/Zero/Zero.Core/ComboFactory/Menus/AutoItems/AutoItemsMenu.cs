using Divine.Entity.Entities.Abilities.Components;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus.AutoItems
{
    public sealed class AutoItemsMenu
    {
        [Menu("Auto Phase Boots"), AbilityImage(AbilityId.item_phase_boots)]
        public AutoPhaseBootsMenu AutoPhaseBootsMenu { get; } = new AutoPhaseBootsMenu();
    }
}