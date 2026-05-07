using Divine.Core.ComboFactory.Menus;
using Divine.Entity.Entities.Abilities.Components;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Zeus.Menus
{
    internal sealed class MoreMenu : BaseMoreMenu
    {
        [Menu("Ability Breaker")]
        public AbilityBreakerMenu AbilityBreakerMenu { get; } = new AbilityBreakerMenu();
    }
}