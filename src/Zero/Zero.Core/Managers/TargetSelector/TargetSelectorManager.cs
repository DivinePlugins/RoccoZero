using System.Collections.Generic;

using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.TargetSelector;
using Divine.Core.Entities;
using Divine.Core.Managers.TargetSelector.Selector;
using Divine.Core.Managers.TargetSelector.TargetEffects;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

namespace Divine.Core.Managers.TargetSelector
{
    public class TargetSelectorManager
    {
        public BaseComboMenu ComboMenu { get; } // TODO public

        private BaseTargetSelectorMenu TargetSelectorMenu { get; }

        internal TargetEffectsMenu TargetEffectsMenu { get; }

        public TargetEffectsManager TargetEffectsManager { get; }

        public CHero Target { get; private set; }

        private ISelector Selector { get; set; }

        private readonly Dictionary<string, ISelector> SelectorService = new Dictionary<string, ISelector>
        {
            { "Lowest Health", new LowestHealth() },
            { "Near Mouse", new NearMouse() }
        };

        public TargetSelectorManager(BaseMenuConfig menuConfig)
        {
            ComboMenu = menuConfig.ComboMenu;
            TargetSelectorMenu = menuConfig.TargetSelectorMenu;
            TargetEffectsMenu = TargetSelectorMenu.TargetEffectsMenu;

            Selector = SelectorService["Near Mouse"];

            TargetSelectorMenu.TargetSelectorItem.ValueChanged += TargetSelectorChanging;

            UpdateManager.CreateIngameUpdate(40, OnUpdate);

            TargetEffectsManager = new TargetEffectsManager(this);
        }

        public void Dispose()
        {
            TargetEffectsManager.Dispose();

            UpdateManager.DestroyIngameUpdate(OnUpdate);

            TargetSelectorMenu.TargetSelectorItem.ValueChanged -= TargetSelectorChanging;
        }

        private void TargetSelectorChanging(MenuSelector selector, SelectorChangedEventArgs e)
        {
            Selector = SelectorService[e.Value];
        }

        protected virtual void OnUpdate()
        {
            var target = Selector.GetTarget();
            if (TargetSelectorMenu.TargetLockItem)
            {
                if (!ComboMenu.ComboHotkeyItem || Target == null || !Target.IsValid || !Target.IsAlive)
                {
                    Target = target;
                }
            }
            else
            {
                Target = target;
            }
        }
    }
}