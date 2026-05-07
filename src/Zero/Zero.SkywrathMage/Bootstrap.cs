namespace Divine.SkywrathMage
{
    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Service;
    using Divine.SkywrathMage.Menus;

    //[ExportPlugin(name: "Divine.SkywrathMage", author: "YEEEEEEE", version: "", priority: 450, units: HeroId.npc_dota_hero_skywrath_mage)]
    internal sealed class Bootstrap : Bootstrapper
    {
        private MenuConfig MenuConfig;

        private Common common;

        protected override void OnMainActivate()
        {
            MenuConfig = new MenuConfig();
        }

        protected override void OnMainDeactivate()
        {
            MenuConfig.Dispose();
        }

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero?.Id != HeroId.npc_dota_hero_skywrath_mage)
            {
                return;
            }

            common = new Common(MenuConfig);
        }

        protected override void OnDeactivate()
        {
            common?.Dispose();
        }
    }
}
