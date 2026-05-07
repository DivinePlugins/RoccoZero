using System;

using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Managers.TargetSelector;
using Divine.Update;

namespace Divine.Core.ComboFactory.Commons
{
    public abstract class BaseCommon : IDisposable
    {
        public abstract BaseAbilities Abilities { get; }

        public BaseMenuConfig MenuConfig { get; }

        public virtual TargetSelectorManager TargetSelector { get; }

        public abstract BaseDamageCalculation DamageCalculation { get; }

        public abstract BaseLinkenBreaker LinkenBreaker { get; }

        public abstract BaseKillSteal KillSteal { get; }

        public abstract BaseCombo Combo { get; }

        //private BaseFarm Farm { get; }

        public virtual BaseRenderer Renderer { get; set; }

        private readonly AutoItemManager AutoItem;

        public BaseCommon(BaseMenuConfig menuConfig)
        {
            MenuConfig = menuConfig;

            if (TargetSelector == null)
            {
                TargetSelector = new TargetSelectorManager(MenuConfig);
            }

            AutoItem = new AutoItemManager(MenuConfig);

            UpdateManager.BeginInvoke(() =>
            {
                if (Renderer == null)
                {
                    Renderer = new BaseRenderer(this);
                }
            });
        }

        public virtual void Dispose()
        {
            Renderer.Dispose();

            AutoItem.Dispose();

            Combo.Dispose();
            KillSteal.Dispose();

            DamageCalculation.Dispose();

            TargetSelector.Dispose();

            MenuConfig.Dispose();
            Abilities.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}