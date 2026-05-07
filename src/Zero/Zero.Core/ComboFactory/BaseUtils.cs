using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Orbwalker;
using Divine.Core.Managers.Unit;
using Divine.Game;

namespace Divine.Core.ComboFactory
{
    public class BaseUtils
    {
        public BaseUtils()
        {
            Owner = UnitManager.Owner;
            Orbwalker = Owner.Orbwalker;
        }

        protected CHero Owner { get; }

        protected virtual OrbwalkerManager Orbwalker { get; }

        protected virtual bool IsStopped
        {
            get
            {
                return GameManager.IsPaused || !Owner.IsValid || !Owner.IsAlive || Owner.IsStunned();
            }
        }

        protected virtual CUnit CurrentTarget { get; set; }

        protected bool IsNullTarget(CUnit target)
        {
            if (target == null)
            {
                return true;
            }

            return false;
        }
    }
}
