using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Game;

namespace Divine.Core.Managers.TargetSelector.Selector
{
    internal sealed class NearMouse : ISelector
    {
        public CHero GetTarget()
        {
            var pos = GameManager.MousePosition;
            return UnitManager<CHero, Enemy, NoIllusion>.Units.Where(x => x.IsVisible && x.IsAlive && x.Distance2D(pos) <= 800)
                                                              .OrderBy(x => x.Distance2D(pos))
                                                              .FirstOrDefault();
        }
    }
}