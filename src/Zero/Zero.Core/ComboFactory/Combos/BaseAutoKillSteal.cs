using System.Linq;

using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseKillSteal : BaseTaskHandler
    {
        protected readonly BaseKillStealMenu KillStealMenu;

        private UpdateHandler UpdateHandler;

        public BaseKillSteal(BaseMenuConfig menuConfig)
        {
            KillStealMenu = menuConfig.KillStealMenu;

            KillStealMenu.EnableItem.ValueChanged += EnableChanged;
        }

        public override void Dispose()
        {
            KillStealMenu.EnableItem.ValueChanged -= EnableChanged;

            if (KillStealMenu.EnableItem)
            {
                base.Dispose();
                UpdateManager.DestroyIngameUpdate(Stop);
            }
        }

        private void EnableChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                UpdateHandler = UpdateManager.CreateIngameUpdate(0, false, Stop);
                RunAsync();
            }
            else
            {
                Cancel();
                UpdateManager.DestroyIngameUpdate(Stop);
            }
        }

        public bool IsKillSteal { get; private set; }

        private CHero Target { get; set; }

        private float Damage { get; set; }

        protected override CUnit CurrentTarget
        {
            get
            {
                IsKillSteal = false;
                var damageDate = BaseDamageCalculation.DamageDate.Where(x => x.IsKillSteal).OrderByDescending(x => x.GetHealth).FirstOrDefault();

                Target = null;
                if (damageDate != null)
                {
                    IsKillSteal = true;
                    Target = damageDate.GetHero;
                    Damage = damageDate.GetCanBeCastedDamage;
                }

                return Target;
            }
        }

        protected void StopCast()
        {
            if (!UpdateHandler.IsEnabled)
            {
                UpdateHandler.IsEnabled = true;
            }
        }

        private void Stop()
        {
            if (IsNullTarget(Target))
            {
                Target = null;
                UpdateHandler.IsEnabled = false;
                return;
            }

            var stop = UnitManager<CHero, Enemy>.Units.Any(x => x == Target && (x.IsBlockMagicDamage() || !x.IsAlive || x.Health - Damage > 0));
            if (stop && Owner.AnimationName.Contains("cast"))
            {
                Target = null;
                Owner.Stop();
                UpdateHandler.IsEnabled = false;
            }
        }

        protected bool Reincarnation(CUnit target)
        {
            var reincarnation = target.GetAbilityById(AbilityId.skeleton_king_reincarnation);
            return reincarnation != null && reincarnation.Cooldown == 0 && reincarnation.Level > 0;
        }
    }
}