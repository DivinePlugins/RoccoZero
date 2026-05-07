namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_aeon_disk : PassiveAbility, IHasModifier
    {
        public item_aeon_disk(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_aeon_disk_buff";
    }
}