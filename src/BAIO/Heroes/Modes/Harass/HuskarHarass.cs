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

    class HuskarHarass : HarassMode
    {
        private readonly Huskar Huskar;
        private Hero HarassTarget;

        public HuskarHarass(Huskar hero) : base(hero)
        {
            this.Huskar = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = this.GetTarget();

            if (target != null)
            {
                this.Orbwalker.OrbwalkTo(target);
                return;
            }

            if (Huskar.FireSpear.Enabled)
            {
                this.Huskar.FireSpear.Enabled = false;
            }

            HarassTarget =
                EntityManager.GetEntities<Hero>().Where(
                        x => x.IsValid && !x.IsMagicImmune() &&
                            x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
                            this.Owner.IsInAttackRange(x))
                    .OrderBy(x => x.Health).FirstOrDefault();

            if (HarassTarget != null && this.Owner.CanAttack())
            {
                var useSpears = Huskar.IsFireSpearGucci(HarassTarget);

                if (useSpears)
                {
                    await this.Huskar.UseFireSpear(HarassTarget, token);
                }

                this.Orbwalker.OrbwalkTo(HarassTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
