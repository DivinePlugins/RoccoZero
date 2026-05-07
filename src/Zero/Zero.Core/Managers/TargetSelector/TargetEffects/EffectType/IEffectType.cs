using Divine.Core.Entities;

namespace Divine.Core.Managers.TargetSelector.TargetEffects.EffectType
{
    public interface IEffectType
    {
        void Effect(CUnit target);

        void Remove();

        int Red { get; set; }

        int Green { get; set; }

        int Blue { get; set; }
    }
}
