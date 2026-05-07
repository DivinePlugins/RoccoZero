namespace BAIO.Heroes.Modes.Harass
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Heroes.Base;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;

    class StormSpiritHarass : HarassMode
    {
        private readonly StormSpirit StormSpirit;
        private Hero HarassTarget;
        public StormSpiritHarass(StormSpirit hero) : base(hero)
        {
            this.StormSpirit = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = this.GetTarget();

            if (target != null)
            {
                this.Orbwalker.OrbwalkTo(target);
                return;
            }

            HarassTarget =
                EntityManager.GetEntities<Hero>().Where(
                        x => x.IsValid && !x.IsMagicImmune() &&
                            x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
                            this.Owner.IsInAttackRange(x))
                    .OrderBy(x => x.Health).FirstOrDefault();

            if (HarassTarget != null && this.Owner.CanAttack())
            {
                var ultiPos = this.StormSpirit.UltiPos(this.HarassTarget);
                if (!this.StormSpirit.InOverload && this.StormSpirit.Ulti.CanBeCasted)
                {
                    this.StormSpirit.Ulti.Cast(ultiPos);
                    await Task.Delay(this.StormSpirit.Ulti.GetCastDelay() + 500, token);
                }

                this.Orbwalker.OrbwalkTo(HarassTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
