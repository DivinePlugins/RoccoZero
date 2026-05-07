namespace Ensage.SDK.TargetSelector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Divine.Entity;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Update;

    public sealed class TargetSelectorManager : ITargetSelectorManager
    {
        private IEnumerable<Unit> Targets = Array.Empty<Unit>();

        public TargetSelectorManager()
        {
            UpdateManager.CreateIngameUpdate(100, OnUpdate);
        }

        private void OnUpdate()
        {
            var pos = GameManager.MousePosition;
            var team = EntityManager.LocalHero.Team;

            Targets = EntityManager.GetEntities<Hero>()
                .Where(e => e.IsVisible && e.IsAlive && (!e.IsIllusion || e.HasModifier("modifier_morphling_replicate")) && e.Team != team)
                .Where(e => e.Position.Distance(pos) < 2000)
                .OrderBy(e => e.Position.Distance(pos))
                .ToArray();
        }

        public IEnumerable<Unit> GetTargets()
        {
            return Targets;
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(OnUpdate);
        }
    }
}