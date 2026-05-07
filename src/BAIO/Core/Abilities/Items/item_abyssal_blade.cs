namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Units.Components;

    using Ensage.SDK.Abilities.Components;

    public class item_abyssal_blade : RangedAbility, IHasTargetModifierTexture
    {
        public item_abyssal_blade(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public string[] TargetModifierTextureName { get; } = { "item_abyssal_blade" };
    }
}