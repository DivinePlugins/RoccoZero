using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_echo_sabre)]
    public sealed class EchoSabre : PassiveItem, IHasTargetModifier
    {
        public EchoSabre(Item item)
            : base(item)
        {
        }

        internal override void Dispose()
        {
            base.Dispose();

            Owner.EchoSabre = null;
        }

        private CUnit owner;

        public override CUnit Owner
        {
            get
            {
                return owner;
            }

            internal set
            {
                owner = value;
                owner.EchoSabre = this;
            }
        }

        public string TargetModifierName { get; } = "modifier_echo_sabre_debuff";
    }
}