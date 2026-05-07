using Divine.Core.ComboFactory.Combos;
using Divine.Core.Entities;
using Divine.Core.Managers.TargetSelector;

namespace Divine.Zeus.Combos
{
    internal sealed class LinkenBreaker : BaseLinkenBreaker
    {
        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        public LinkenBreaker(Common common)
            : base(common.MenuConfig)
        {
            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;
        }

        protected override CAbility[] LinkenBreakerAbilities
        {
            get
            {
                return new CAbility[]
                {
                    Abilities.Eul,
                    Abilities.WindWaker,
                    Abilities.ForceStaff,
                    Abilities.Orchid,
                    Abilities.Bloodthorn,
                    Abilities.Nullifier,
                    Abilities.Atos,
                    Abilities.Hex,
                    Abilities.ArcLightning,
                    Abilities.LightningBolt
                };
            }
        }

        protected override CUnit CurrentTarget
        {
            get
            {
                return TargetSelector.Target;
            }
        }
    }
}
