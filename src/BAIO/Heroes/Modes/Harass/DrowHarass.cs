namespace BAIO.Heroes.Modes.Harass
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;

    public class DrowHarass : BAIO.Modes.HarassMode
    {
        private readonly Base.DrowRanger DrowRanger;

        public DrowHarass(Base.DrowRanger hero)
            : base(hero)
        {
            this.DrowRanger = hero;
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
                var useArrow = this.DrowRanger.ShouldUseFrostArrow(harrasTarget);
                if (useArrow)
                {
                    await this.DrowRanger.UseFrostArrows(harrasTarget, token);
                    return;
                }

                this.Orbwalker.OrbwalkTo(harrasTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
