using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_smoke_of_deceit)]
    //[Particle("items2_fx/smoke_of_deceit.vpcf", ParticleFlags.Item | ParticleFlags.Position | ParticleFlags.Released)]
    public sealed class SmokeOfDeceit : ActiveItem, IAreaOfEffectAbility, IHasModifier
    {
        public SmokeOfDeceit(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_smoke_of_deceit";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("application_radius");
            }
        }

        public int StockCount
        {
            get
            {
                /*var itemStockInfo = Game.StockInfo.FirstOrDefault(x => x.AbilityId == Id && x.Team == Owner.Team);
                return itemStockInfo?.StockCount ?? 0;*/

                return 0;
            }
        }
    }
}