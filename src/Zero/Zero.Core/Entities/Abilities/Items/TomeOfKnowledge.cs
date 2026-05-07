using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_tome_of_knowledge)]
    public sealed class TomeOfKnowledge : ActiveItem
    {
        public TomeOfKnowledge(Item item)
            : base(item)
        {
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