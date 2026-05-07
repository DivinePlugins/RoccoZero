using Divine.Core.Entities;

namespace Divine.Core.Managers.Unit
{
    public class Controllable : IType
    {
        public bool GetControl(CUnit unit)
        {
            return unit.IsControllable;
        }
    }
}
