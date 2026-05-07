using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Game;

namespace Divine.Core.Managers.TargetSelector.Selector
{
    internal sealed class LowestHealth : ISelector
    {
        public CHero GetTarget()
        {
            return UnitManager<CHero, Enemy, NoIllusion>.Units.Where(x => x.IsVisible && x.IsAlive && x.Distance2D(GameManager.MousePosition) <= 1200)
                                                              .OrderBy(x => x.Health)
                                                              .FirstOrDefault();
        }
    }
}