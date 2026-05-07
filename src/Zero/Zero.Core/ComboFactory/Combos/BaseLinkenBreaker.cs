using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities;
using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Common.Log;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseLinkenBreaker : BaseTaskHandler
    {
        private BaseLinkenBreakerMenu LinkenBreakerMenu { get; }

        public BaseLinkenBreaker(BaseMenuConfig menuConfig)
        {
            LinkenBreakerMenu = menuConfig.LinkenBreakerMenu;
        }

        public void RunAsync()
        {
            RunAsync(false);
        }

        protected abstract CAbility[] LinkenBreakerAbilities { get; }

        private AbilityId[] BreakerChanger(bool isSpellShieldProtected)
        {
            if (isSpellShieldProtected)
            {
                var prioritySpellShieldItem = LinkenBreakerMenu.PrioritySpellShieldItem;
                return prioritySpellShieldItem.Values.Where(x => x.Value).Select(x => x.Key).ToArray();
            }

            var priorityLinkensItem = LinkenBreakerMenu.PriorityLinkensItem;
            return priorityLinkensItem.Values.Where(x => x.Value).Select(x => x.Key).ToArray();
        }

        protected static HashSet<AbilityId> HitTimeAbilities { get; } = new HashSet<AbilityId>
        {
            AbilityId.item_nullifier,
            AbilityId.item_rod_of_atos
        };

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    return;
                }

                foreach (var order in BreakerChanger(target.IsAntimageSpellShield()))
                {
                    foreach (var ability in LinkenBreakerAbilities)
                    {
                        if (ability == null || ability.Id != order || !ability.CanBeCasted)
                        {
                            continue;
                        }

                        if (ability is IActiveAbility activeAbility && activeAbility.CanHit(target))
                        {
                            ability.UseAbility(target);
                            await Task.Delay(HitTimeAbilities.Contains(ability.Id) ? activeAbility.GetHitTime(target) + 100 : activeAbility.GetCastDelay(target), token);
                            break;
                        }
                        else if (LinkenBreakerMenu.UseOnlyFromRangeItem)
                        {
                            break;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }
        }
    }
}
