using Divine.Core.Entities;
using Divine.Entity.Entities.Components;

namespace Divine.Core.Managers.Unit
{
    public class Neutral : IType
    {
        public bool GetControl(CUnit unit)
        {
            return unit.Team == Team.Neutral;
        }
    }
}
