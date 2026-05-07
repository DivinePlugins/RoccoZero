using Divine.Core.Helpers;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.SkywrathMage.Menus;

namespace Divine.SkywrathMage.Features
{
    internal sealed class ExecuteOrder
    {
        private readonly SmartConcussiveShotMenu SmartConcussiveShotMenu;

        private readonly Abilities Abilities;

        public ExecuteOrder(Common common)
        {
            SmartConcussiveShotMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartConcussiveShotMenu;

            Abilities = (Abilities)common.Abilities;

            OrderManager.OrderAdding += OnExecuteOrder;
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= OnExecuteOrder;
        }

        private void OnExecuteOrder(OrderAddingEventArgs e)
        {
            var order = e.Order;
            if (order.Type == OrderType.Cast)
            {
                var concussiveShot = Abilities.ConcussiveShot;
                if (order.Ability == concussiveShot.Base)
                {
                    var targetHit = concussiveShot.TargetHit;
                    if (targetHit == null)
                    {
                        if (SmartConcussiveShotMenu.AntiFailItem)
                        {
                            e.Process = false;
                        }

                        return;
                    }

                    var castDelay = concussiveShot.GetCastDelay();
                    var hitTime = concussiveShot.GetHitTime(targetHit) - (castDelay + 150);
                    MultiSleeper<string>.DelaySleep($"IsHitTime_{targetHit.Name}_{concussiveShot.Name}", castDelay + 50, hitTime);
                }
            }
        }
    }
}
