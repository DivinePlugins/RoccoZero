using Divine.Core.Entities;

namespace Divine.Core.Managers.Unit
{
    public class Enemy : IType
    {
        public bool GetControl(CUnit unit)
        {
            return unit.IsEnemy();
        }
    }
}