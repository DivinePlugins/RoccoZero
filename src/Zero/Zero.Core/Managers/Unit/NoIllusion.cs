using Divine.Core.Entities;

namespace Divine.Core.Managers.Unit
{
    public class NoIllusion : IType
    {
        public bool GetControl(CUnit unit)
        {
            return !unit.IsIllusion;
        }
    }
}
