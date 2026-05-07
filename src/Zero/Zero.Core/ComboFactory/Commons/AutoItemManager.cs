using Divine.Core.ComboFactory.AutoItems;
using Divine.Core.ComboFactory.Menus;

namespace Divine.Core.ComboFactory.Commons
{
    internal sealed class AutoItemManager
    {
        private AutoPhaseBoots autoPhaseBoots;

        public AutoItemManager(BaseMenuConfig menuConfig)
        {
            autoPhaseBoots = new AutoPhaseBoots(menuConfig.AutoItemsMenu.AutoPhaseBootsMenu);
        }

        public void Dispose()
        {
            autoPhaseBoots.Dispose();
        }
    }
}
