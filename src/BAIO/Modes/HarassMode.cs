namespace BAIO.Modes
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;

    using Ensage.SDK.Orbwalker.Modes;

    public class HarassMode : AttackOrbwalkingModeAsync
    {
        private readonly BaseHero BaseHero;

        public HarassMode(BaseHero hero)
            : base(hero.Context, hero.Config.General.HarassKey)
        {
            this.BaseHero = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var unitTarget = this.GetTarget();
            if (unitTarget != null)
            {
                this.Orbwalker.OrbwalkTo(unitTarget);
                return;
            }

            var harrasTarget = EntityManager.GetEntities<Hero>()
                .Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.IsEnemy(this.Owner) && this.Owner.IsInAttackRange(x))
                .OrderBy(x => x.Health)
                .FirstOrDefault();

            if (harrasTarget != null)
            {
                this.Orbwalker.OrbwalkTo(harrasTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
            await Task.Delay(10, token);
        }
    }
}