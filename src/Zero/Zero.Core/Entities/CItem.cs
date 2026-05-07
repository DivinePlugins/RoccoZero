namespace Divine.Core.Entities;

using Divine.Core.Managers.Ability;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Players;

public class CItem : CAbility
{
    public CItem(Item item)
        : base(item)
    {
        Base = item;

        Cost = item.Cost;
    }

    public new Item Base { get; }

    public override bool IsItem { get; } = true;

    public bool IsCombinable
    {
        get
        {
            return Base.IsCombinable;
        }
    }

    public bool IsPermanent
    {
        get
        {
            return Base.IsPermanent;
        }
    }

    public bool IsStackable
    {
        get
        {
            return Base.IsStackable;
        }
    }

    public bool IsRecipe
    {
        get
        {
            return Base.IsRecipe;
        }
    }

    public Shareability Shareability
    {
        get
        {
            return Base.Shareability;
        }
    }

    public bool IsDroppable
    {
        get
        {
            return Base.IsDroppable;
        }
    }

    public bool IsPurchasable
    {
        get
        {
            return Base.IsPurchasable;
        }
    }

    public bool IsSellable
    {
        get
        {
            return Base.IsSellable;
        }
    }

    public bool IsRequiringCharges
    {
        get
        {
            return Base.IsRequiringCharges;
        }
    }

    public bool IsDisplayingCharges
    {
        get
        {
            return Base.IsDisplayingCharges;
        }
    }

    public bool IsHidingCharges
    {
        get
        {
            return Base.IsHidingCharges;
        }
    }

    public bool IsKillable
    {
        get
        {
            return Base.IsKillable;
        }
    }

    public bool IsDisassemblable
    {
        get
        {
            return Base.IsDisassemblable;
        }
    }

    public bool IsAlertable
    {
        get
        {
            return Base.IsAlertable;
        }
    }

    public uint InitialCharges
    {
        get
        {
            return Base.InitialCharges;
        }
    }

    public bool IsCastedOnPickup
    {
        get
        {
            return Base.IsCastedOnPickup;
        }
    }

    public uint CurrentCharges
    {
        get
        {
            return Base.CurrentCharges;
        }
    }

    public uint SecondaryCharges
    {
        get
        {
            return Base.SecondaryCharges;
        }
    }

    public bool IsCombineLocked
    {
        get
        {
            return Base.IsCombineLocked;
        }
    }

    public float PurchaseTime
    {
        get
        {
            return Base.PurchaseTime;
        }
    }

    public float AssembledTime
    {
        get
        {
            return Base.AssembledTime;
        }
    }

    public bool IsPurchasedWhileDead
    {
        get
        {
            return Base.IsPurchasedWhileDead;
        }
    }

    public bool CanBeUsedOutOfInventory
    {
        get
        {
            return Base.CanBeUsedOutOfInventory;
        }
    }

    public bool IsEnabled
    {
        get
        {
            return Base.IsEnabled;
        }
    }

    public float EnableTime
    {
        get
        {
            return Base.EnableTime;
        }
    }

    public Entity OldOwner
    {
        get
        {
            return Base.OldOwner;
        }
    }

    public Player Purchaser
    {
        get
        {
            return Base.Purchaser;
        }
    }

    public int PurchaserId
    {
        get
        {
            return Base.PurchaserId;
        }
    }

    public string ItemRecipeName
    {
        get
        {
            return Base.ItemRecipeName;
        }
    }

    public uint Cost { get; }

    public string ModelName
    {
        get
        {
            return Base.ModelName;
        }
    }

    public string EffectName
    {
        get
        {
            return Base.EffectName;
        }
    }

    public void DisassembleItem()
    {
        Base.Disassemble();
    }

    public void DropFromStash()
    {
        Base.DropFromStash();
    }

    public void MoveItem(ItemSlot targetPosition)
    {
        Base.Move(targetPosition);
    }

    public void SellItem()
    {
        Base.Sell();
    }

    public void LockCombining()
    {
        Base.CombineLock();
    }

    public void UnlockCombining()
    {
        Base.CombineUnlock();
    }

    public static implicit operator Item(CItem item)
    {
        return item.Base;
    }

    public static explicit operator CItem(Entity entity)
    {
        return AbilityManager.GetItemByEntity(entity);
    }
}