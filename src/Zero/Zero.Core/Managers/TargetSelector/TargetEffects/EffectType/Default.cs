using Divine.Core.Entities;
using Divine.Particle;

namespace Divine.Core.Managers.TargetSelector.TargetEffects.EffectType
{
    internal class Default : BaseEffectType, IEffectType
    {
        public void Effect(CUnit target)
        {
            if (target == null)
            {
                Remove();
                return;
            }

            ParticleManager.CreateTargetLineParticle("Divine.TargetEffects", Owner, target.Position, Color);
        }
    }
}
