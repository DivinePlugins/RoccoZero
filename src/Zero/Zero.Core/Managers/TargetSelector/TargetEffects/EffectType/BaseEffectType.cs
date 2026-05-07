namespace Divine.Core.Managers.TargetSelector.TargetEffects.EffectType;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Numerics;
using Divine.Particle;

internal class BaseEffectType
{
    protected Unit Owner { get; } = EntityManager.LocalHero;

    public void Remove()
    {
        ParticleManager.DestroyParticle("Divine.TargetEffects");
    }

    public int Red { get; set; }

    public int Green { get; set; }

    public int Blue { get; set; }

    protected Color Color
    {
        get
        {
            return new Color(Red, Green, Blue);
        }
    }
}
