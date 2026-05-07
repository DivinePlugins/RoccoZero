using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class PassiveItem : CItem
    {
        protected PassiveItem(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return IsReady && !Owner.UnitState.HasFlag(UnitState.PassivesDisabled);
            }
        }
    }
}