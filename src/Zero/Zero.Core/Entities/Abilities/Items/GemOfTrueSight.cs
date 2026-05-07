using System.Linq;

using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_gem)]
    public sealed class GemOfTrueSight : PassiveItem, IHasModifier, IAreaOfEffectAbility
    {
        public GemOfTrueSight(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.ProvidesVision;

        public string ModifierName { get; } = "modifier_item_gem_of_true_sight";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        public int StockCount
        {
            get
            {
                //var itemStockInfo = GameManager.ItemStockInfo.FirstOrDefault(x => x.AbilityId == Id && x.Team == Owner.Team);
                return 0;//itemStockInfo?.StockCount ?? 0;
            }
        }
    }
}