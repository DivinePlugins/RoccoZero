namespace BAIO.Heroes.Modes.Harass
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;

    public class ObsidianDestroyerHarass : BAIO.Modes.HarassMode
    {
        private readonly Base.ObsidianDestroyer ObsidianDestroyer;

        public ObsidianDestroyerHarass(Base.ObsidianDestroyer hero)
            : base(hero)
        {
            this.ObsidianDestroyer = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var unitTarget = this.GetTarget();
            if (unitTarget != null)
            {
                this.Orbwalker.OrbwalkTo(unitTarget);
                return;
            }

            var harrasTarget = EntityManager
                .GetEntities<Hero>().Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.IsEnemy(this.Owner) && this.Owner.IsInAttackRange(x))
                .OrderBy(x => x.Health)
                .FirstOrDefault();

            if (harrasTarget != null)
            {
                var useArrow = this.ObsidianDestroyer.ShouldUseArcaneOrb(harrasTarget);
                if (useArrow)
                {
                    await this.ObsidianDestroyer.UseArcaneOrb(harrasTarget, token);
                    return;
                }

                this.Orbwalker.OrbwalkTo(harrasTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}