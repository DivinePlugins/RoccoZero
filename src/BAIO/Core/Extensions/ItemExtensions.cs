namespace BAIO.Core.Extensions;

using System;

using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units;
using Divine.Game;

internal static class ItemExtensions
{
    public static bool CanBeCasted(this Item item, float bonusMana = 0)
    {
        if (item == null || !item.IsValid)
        {
            return false;
        }

        try
        {
            var owner = item.Owner as Unit;
            bool canBeCasted;
            if (owner == null)
            {
                canBeCasted = item.Level > 0 && item.Cooldown <= Math.Max(GameManager.Ping / 1000 - 0.1, 0);
                if (item.IsRequiringCharges)
                {
                    canBeCasted = canBeCasted && item.CurrentCharges > 0;
                }

                return canBeCasted;
            }

            canBeCasted = item.Level > 0 && owner.Mana + bonusMana >= item.ManaCost
                          && item.Cooldown <= Math.Max(GameManager.Ping / 1000 - 0.1, 0);
            if (item.IsRequiringCharges)
            {
                canBeCasted = canBeCasted && item.CurrentCharges > 0;
            }

            return canBeCasted;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
