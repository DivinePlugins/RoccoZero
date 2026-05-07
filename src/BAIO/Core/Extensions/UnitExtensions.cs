namespace BAIO.Core.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using BAIO.Core.UnitData;

using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Modifier.Modifiers.Exceptions;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Entity.Entities.Components;

internal static class UnitExtensions
{
    private static readonly Dictionary<string, Item> itemDictionary = new();

    private static readonly Dictionary<uint, double> TurnrateDictionary = new();

    private static readonly Dictionary<string, Ability> abilityDictionary = new();

    private static readonly Dictionary<string, bool> boolDictionary = new();

    private static Dictionary<string, Modifier> modifierDictionary = new();

    public static Item FindItem(this Unit unit, string name, bool cache = false)
    {
        if (!unit.IsVisible || !unit.HasInventory)
        {
            return null;
        }

        if (!cache)
        {
            return unit.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && x.Name == name);
        }

        var n = unit.Handle + name;
        if (!itemDictionary.TryGetValue(n, out var item) || item == null || !item.IsValid
            || Utils.SleepCheck("Common.FindItem." + name))
        {
            item = unit.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && x.Name == name);
            if (itemDictionary.ContainsKey(n))
            {
                itemDictionary[n] = item;
            }
            else
            {
                itemDictionary.TryAdd(n, item);
            }

            Utils.Sleep(1000, "Common.FindItem." + name);
        }

        if (item == null || !item.IsValid)
        {
            return null;
        }

        return item;
    }

    public static double GetTurnTime(this Unit unit, Entity entity)
    {
        return unit.GetTurnTime(entity.Position);
    }

    public static double GetTurnTime(this Unit unit, Vector3 position)
    {
        if (unit.NetworkName == "CDOTA_Unit_Hero_Wisp")
        {
            return 0;
        }

        var angle = unit.FindRelativeAngle(position);
        if (angle <= 0.5)
        {
            return 0;
        }

        return 0.03 / unit.GetTurnRate() * angle;
    }

    public static float FindRelativeAngle(this Unit unit, Vector3 pos)
    {
        var angle = Math.Abs(Math.Atan2(pos.Y - unit.Position.Y, pos.X - unit.Position.X) - unit.RotationRad);

        if (angle > Math.PI)
        {
            angle = Math.PI * 2 - angle;
        }

        return (float)angle;
    }

    public static double GetTurnRate(this Unit unit, bool currentTurnRate = true)
    {
        var handle = unit.Handle;
        double turnRate;

        if (!TurnrateDictionary.TryGetValue(handle, out turnRate))
        {
            try
            {
                turnRate = ((unit is Hero) ? Hero.GetKeyValueByName(unit.Name) : Unit.GetKeyValueByName(unit.Name))!.GetSubKey("MovementTurnRate")!.GetSingle();
            }
            catch
            {
                turnRate = 0.5;
            }

            TurnrateDictionary.TryAdd(handle, turnRate);
        }

        if (currentTurnRate)
        {
            if (unit.HasModifier("modifier_medusa_stone_gaze_slow"))
            {
                turnRate *= 0.65;
            }

            if (unit.HasModifier("modifier_batrider_sticky_napalm"))
            {
                turnRate *= 0.3;
            }
        }

        return turnRate;
    }

    public static Ability FindSpell(this Unit unit, string name, out Ability spell)
    {
        spell = unit.FindSpell(name);
        return spell;
    }

    public static Ability FindSpell(this Unit unit, string name, bool cache = false)
    {
        if (!cache)
        {
            return unit.Spellbook.Spells.FirstOrDefault(x => x.Name == name);
        }

        Ability ability;
        var n = unit.Handle + name;
        if (!abilityDictionary.TryGetValue(n, out ability) || ability == null || !ability.IsValid
            || Utils.SleepCheck("Common.FindSpell." + name))
        {
            ability = unit.Spellbook.Spells.FirstOrDefault(x => x.Name == name);
            if (abilityDictionary.ContainsKey(n))
            {
                abilityDictionary[n] = ability;
            }
            else
            {
                abilityDictionary.TryAdd(n, ability);
            }

            Utils.Sleep(1000, "Common.FindSpell." + name);
        }

        if (ability == null || !ability.IsValid)
        {
            return null;
        }

        return ability;
    }

    public static bool IsInvul(this Unit unit)
    {
        return IsUnitState(unit, UnitState.Invulnerable);
    }

    public static bool IsUnitState(this Unit unit, UnitState state)
    {
        if (unit == null || !unit.IsValid)
        {
            return false;
        }

        return unit.UnitState.HasFlag(state);
    }

    public static bool CanMove(this Unit unit)
    {
        var n = unit.Handle + "CanMove";
        if (!Utils.SleepCheck(n))
        {
            return boolDictionary[n];
        }

        var canMove = !unit.IsRooted() && !unit.IsStunned() && !unit.HasModifier("modifier_slark_pounce_leash")
                      && unit.IsAlive;
        if (!boolDictionary.ContainsKey(n))
        {
            boolDictionary.TryAdd(n, canMove);
        }
        else
        {
            boolDictionary[n] = canMove;
        }

        Utils.Sleep(150, n);
        return canMove;
    }

    public static double AttackRate(this Unit unit)
    {
        return UnitDatabase.GetAttackRate(unit);
    }

    public static Modifier FindModifier(this Unit unit, string modifierName)
    {
        if (Utils.SleepCheck("Ensage.Common.FindModifierReset"))
        {
            modifierDictionary = new();
            Utils.Sleep(20000, "Ensage.Common.FindModifierReset");
        }

        var name = unit.Handle + modifierName;
        Modifier modifier;
        var found = modifierDictionary.TryGetValue(name, out modifier);
        var isValid = true;
        if (found)
        {
            try
            {
                var test = modifier.RemainingTime;
            }
            catch (ModifierNotFoundException)
            {
                isValid = false;
            }
        }

        if (found && isValid && !Utils.SleepCheck("Ensage.Common.FindModifier" + name))
        {
            return modifier;
        }

        modifier = unit.Modifiers.FirstOrDefault(x => x.Name == modifierName);
        if (modifier == null)
        {
            return null;
        }

        if (modifierDictionary.ContainsKey(name))
        {
            modifierDictionary[name] = modifier;
        }
        else
        {
            modifierDictionary.TryAdd(name, modifier);
        }

        Utils.Sleep(100, "Ensage.Common.FindModifier" + name);
        return modifier;
    }

    public static bool CanCast(this Unit unit)
    {
        return !unit.IsSilenced() && !unit.IsStunned() && unit.IsAlive;
    }

    public static double AttackBackswing(this Unit unit)
    {
        return UnitDatabase.GetAttackBackswing(unit);
    }

    public static bool IsAttacking(this Unit unit)
    {
        var networkActivity = unit.NetworkActivity;
        return networkActivity == NetworkActivity.Attack || networkActivity == NetworkActivity.Crit
               || networkActivity == NetworkActivity.Attack2 || networkActivity == NetworkActivity.AttackEvent
               || networkActivity == NetworkActivity.AttackEventBash
               || networkActivity == NetworkActivity.EarthshakerTotemAttack;
    }
}