namespace BAIO.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

using BAIO.Core.AbilityInfo;
using BAIO.Core.UnitData;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;

internal static class AbilityExtensions
{
    private static readonly Dictionary<string, AbilityBehavior> abilityBehaviorDictionary = new();

    private static readonly Dictionary<string, AbilitySpecialData> dataDictionary = new();

    private static readonly Dictionary<string, float> radiusDictionary = new();

    private static readonly Dictionary<float, float> rangeDictionary = new();

    private static readonly Dictionary<string, float> castRangeDictionary = new();

    private static readonly Dictionary<string, double> delayDictionary = new();

    private static readonly Dictionary<string, double> castPointDictionary = new();

    private static readonly Dictionary<string, float> channelDictionary = new();

    private static readonly Dictionary<string, float> speedDictionary = new();

    private static readonly Dictionary<uint, bool> canHitDictionary = new();

    private static readonly MultiSleeper sleeper = new();

    public static bool IsAbilityBehavior(this Ability ability, AbilityBehavior flag, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return false;
        }

        var name = abilityName ?? ability.Name;
        if (abilityBehaviorDictionary.TryGetValue(name, out var data))
        {
            return data.HasFlag(flag);
        }

        data = ability.AbilityBehavior;
        abilityBehaviorDictionary.TryAdd(name, data);
        return data.HasFlag(flag);
    }

    public static AbilityInfo CommonProperties(this Ability ability)
    {
        return AbilityDatabase.Find(ability.Name);
    }

    public static Vector3 GetPrediction(
            this Ability ability,
            Unit target,
            double customDelay = 0,
            string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return new Vector3();
        }

        if (target == null || !target.IsValid)
        {
            return new Vector3();
        }

        var name = abilityName ?? ability.Name;
        var data = ability.CommonProperties();
        var owner = ability.Owner as Unit;
        var delay = ability.GetCastDelay(owner as Hero, target, true, abilityName: name, useChannel: true);
        if (data != null)
        {
            delay += data.AdditionalDelay;
        }

        var speed = ability.GetProjectileSpeed(name);
        var radius = ability.GetRadius(name);
        Vector3 xyz;
        if (speed > 0 && speed < 6000)
        {
            xyz = Prediction.SkillShotXYZ(
                owner,
                target,
                (float)((delay + owner.GetTurnTime(target.Position)) * 1000),
                speed,
                radius);
            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget, name))
            {
                xyz = Prediction.SkillShotXYZ(
                    owner,
                    target,
                    (float)((delay + (float)owner.GetTurnTime(xyz)) * 1000),
                    speed,
                    radius);
            }
        }
        else
        {
            xyz = Prediction.PredictedXYZ(target, (float)(delay * 1000));
        }

        return xyz;
    }

    public static float GetProjectileSpeed(this Ability ability, string abilityName = null)
    {
        return GetProjectileSpeed(ability, 0, abilityName);
    }

    public static float GetProjectileSpeed(this Ability ability, uint abilityLevel, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        var level = abilityLevel != 0 ? abilityLevel : ability.Level;
        var name = abilityName ?? ability.Name;

        if (speedDictionary.TryGetValue(name + " " + level, out var speed))
        {
            return speed;
        }

        var data = ability.CommonProperties();
        if (data == null)
        {
            speed = float.MaxValue;
            speedDictionary.TryAdd(name + " " + level, speed);
            return speed;
        }

        if (data.Speed == null)
        {
            return speed;
        }

        speed = ability.GetAbilityData(data.Speed, abilityName: name);
        speedDictionary.TryAdd(name + " " + level, speed);

        return speed;
    }

    public static float GetAbilityData(this Ability ability, string dataName, uint level = 0, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        var lvl = ability.Level;
        var name = abilityName ?? ability.Name;

        if (!dataDictionary.TryGetValue(name + "_" + dataName, out var data))
        {
            data = ability.AbilitySpecialData.FirstOrDefault(x => x.Name == dataName);
            dataDictionary.TryAdd(name + "_" + dataName, data);
        }

        if (level > 0)
        {
            lvl = level;
        }

        if (data == null)
        {
            return 0;
        }

        return data.Count > 1 ? data.GetValue(lvl - 1) : data.Value;
    }

    public static float TravelDistance(this Ability ability)
    {
        var data = ability.CommonProperties();
        if (data == null)
        {
            return ability.GetCastRange();
        }

        var distance = ability.GetAbilityData(data.Distance);
        return distance > 0 ? distance : ability.GetCastRange();
    }

    public static float GetRadius(this Ability ability, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        var name = abilityName ?? ability.Name;
        float radius;
        if (radiusDictionary.TryGetValue(name + " " + ability.Level, out radius))
        {
            return radius;
        }

        var data = ability.CommonProperties();
        if (data == null)
        {
            radius = 0;
            radiusDictionary.TryAdd(name + " " + ability.Level, radius);
            return radius;
        }

        if (data.Width != null)
        {
            radius = ability.GetAbilityData(data.Width, abilityName: name);
            radiusDictionary.TryAdd(name + " " + ability.Level, radius);
            return radius;
        }

        if (data.StringRadius != null)
        {
            radius = ability.GetAbilityData(data.StringRadius, abilityName: name);
            radiusDictionary.TryAdd(name + " " + ability.Level, radius);
            return radius;
        }

        if (data.Radius > 0)
        {
            radius = data.Radius;
            radiusDictionary.TryAdd(name + " " + ability.Level, radius);
            return radius;
        }

        if (!data.IsBuff)
        {
            return radius;
        }

        radius = (ability.Owner as Unit).GetAttackRange() + 150;
        radiusDictionary.TryAdd(name + " " + ability.Level, radius);
        return radius;
    }

    public static float GetAttackRange(this Unit unit)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            return hero.GetAttackRange();
        }

        if (rangeDictionary.TryGetValue(unit.Handle, out var range)
            && !Utils.SleepCheck("Common.GetAttackRange." + unit.Handle))
        {
            return range;
        }

        range = unit.AttackRange + unit.HullRadius;
        if (!rangeDictionary.ContainsKey(unit.Handle))
        {
            rangeDictionary.TryAdd(unit.Handle, range);
        }
        else
        {
            rangeDictionary[unit.Handle] = range;
        }

        Utils.Sleep(1500, "Common.GetAttackRange." + unit.Handle);

        return range;
    }

    public static float GetCastRange(this Ability ability, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        var name = abilityName ?? ability.Name;
        var owner = ability.Owner;
        var n = name + owner.Handle;
        if (castRangeDictionary.ContainsKey(n) && !Utils.SleepCheck("Common.GetCastRange." + n))
        {
            return castRangeDictionary[n];
        }

        if (name == "templar_assassin_meld")
        {
            return (ability.Owner as Unit).GetAttackRange() + 50;
        }

        var data = ability.CommonProperties();
        if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget, name))
        {
            var castRange = (float)ability.CastRange;
            var bonusRange = 0f;
            if (data != null && data.RealCastRange != null)
            {
                castRange = ability.GetAbilityData(data.RealCastRange, abilityName: name);
            }

            if (castRange <= 0)
            {
                castRange = 999999;
            }

            var hero = owner as Hero;
            if (hero != null && name == "dragon_knight_dragon_tail"
                && hero.HasModifier("modifier_dragon_knight_dragon_form"))
            {
                bonusRange = 250;
            }
            else if (hero != null && name == "beastmaster_primal_roar" && hero.HasAghanimsScepter())
            {
                bonusRange = 350;
            }

            var aetherLens = hero?.FindItem("item_aether_lens", true);
            if (aetherLens != null)
            {
                bonusRange += aetherLens.GetAbilityData("cast_range_bonus");
            }

            var talent = hero?.Spellbook.Spells.FirstOrDefault(x => x.Name.StartsWith("special_bonus_cast_range_"));
            if (talent?.Level > 0)
            {
                bonusRange += talent.GetAbilityData("value");
            }

            if (!castRangeDictionary.ContainsKey(n))
            {
                castRangeDictionary.TryAdd(n, castRange + bonusRange);
                Utils.Sleep(5000, "Common.GetCastRange." + n);
            }
            else
            {
                castRangeDictionary[n] = castRange + bonusRange;
                Utils.Sleep(5000, "Common.GetCastRange." + n);
            }

            return castRange + bonusRange;
        }

        float radius;
        if (data == null)
        {
            return ability.CastRange;
        }

        if (ability.Name == "earthshaker_enchant_totem" && (owner as Hero).HasAghanimsScepter())
        {
            radius = ability.GetAbilityData("scepter_distance") + 100;
        }
        else if (!data.FakeCastRange)
        {
            radius = ability.GetRadius(name);
        }
        else
        {
            radius = ability.GetAbilityData(data.RealCastRange, abilityName: name);
        }

        if (!castRangeDictionary.ContainsKey(n))
        {
            castRangeDictionary.TryAdd(n, radius);
            Utils.Sleep(5000, "Common.GetCastRange." + n);
        }
        else
        {
            castRangeDictionary[n] = radius;
            Utils.Sleep(5000, "Common.GetCastRange." + n);
        }

        return radius;
    }

    public static double GetCastDelay(
            this Ability ability,
            Hero source,
            Unit target,
            bool usePing = false,
            bool useCastPoint = true,
            string abilityName = null,
            bool useChannel = false)
    {
        return ability.GetCastDelay(source as Unit, target, usePing, useCastPoint, abilityName, useChannel);
    }

    public static double GetCastDelay(
            this Ability ability,
            Unit source,
            Unit target,
            bool usePing = false,
            bool useCastPoint = true,
            string abilityName = null,
            bool useChannel = false)
    {
        return ability.GetCastDelay(source, target, ability.Level, usePing, useCastPoint, abilityName, useChannel);
    }

    public static double GetCastDelay(
            this Ability ability,
            Unit source,
            Unit target,
            uint abilityLevel,
            bool usePing = false,
            bool useCastPoint = true,
            string abilityName = null,
            bool useChannel = false)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        if (target == null || !target.IsValid || source == null || !source.IsValid)
        {
            return 0;
        }

        var level = abilityLevel != 0 ? abilityLevel : ability.Level;
        var name = abilityName ?? ability.Name;
        double delay;
        if (useCastPoint)
        {
            if (!delayDictionary.TryGetValue(name + " " + level, out delay))
            {
                delay = Math.Max(ability.FindCastPoint(name), 0.07);
                delayDictionary.TryAdd(name + " " + level, delay);
            }

            if (name == "templar_assassin_meld")
            {
                delay += UnitDatabase.GetAttackPoint(source) + GameManager.Ping / 500 + 0.1 + source.GetTurnTime(target);
            }

            if (name == "item_diffusal_blade" || name == "item_diffusal_blade_2")
            {
                delay += 2;
            }
        }
        else
        {
            if (ability is Item)
            {
                delay = 0;
            }
            else
            {
                delay = 0.05;
            }
        }

        if (usePing)
        {
            delay += GameManager.Ping / 1000;
        }

        if (useChannel)
        {
            delay += ability.ChannelTime(level, name);
        }

        if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget, name))
        {
            return Math.Max(delay + (!target.Equals(source) ? source.GetTurnTime(target) : 0), 0);
        }

        return Math.Max(delay, 0);
    }

    public static double FindCastPoint(this Ability ability, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        if (ability is Item)
        {
            return 0;
        }

        if (ability.OverrideCastPoint != -1)
        {
            return 0.1;
        }

        var name = abilityName ?? ability.Name;
        double castPoint;
        if (castPointDictionary.TryGetValue(name + " " + ability.Level, out castPoint))
        {
            return castPoint;
        }

        castPoint = ability.AbilityData.GetCastPoint(ability.Level);
        castPointDictionary.TryAdd(name + " " + ability.Level, castPoint);
        return castPoint;
    }

    public static float ChannelTime(this Ability ability, string abilityName = null)
    {
        return ChannelTime(ability, 0, abilityName);
    }

    public static float ChannelTime(this Ability ability, uint abilityLevel, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return 0;
        }

        var level = abilityLevel != 0 ? abilityLevel : ability.Level;
        var name = abilityName ?? ability.Name;
        float channel;
        if (!channelDictionary.TryGetValue(name + level, out channel))
        {
            channel = ability.AbilityData.GetChannelMaximumTime(level - 1);
            channelDictionary.TryAdd(name + level, channel);
        }

        // Console.WriteLine(ability.GetChannelTime(ability.Level - 1) + "  " + delay + " " + name);
        return channel;
    }

    public static bool CanBeCasted(this Ability ability, float bonusMana = 0)
    {
        if (ability == null || !ability.IsValid)
        {
            return false;
        }

        var item = ability as Item;
        if (item != null)
        {
            return item.CanBeCasted(bonusMana);
        }

        try
        {
            var owner = ability.Owner as Hero;
            bool canBeCasted;
            if (owner == null)
            {
                canBeCasted = ability.Level > 0 && ability.Cooldown <= Math.Max(GameManager.Ping / 1000 - 0.1, 0);
                return canBeCasted;
            }

            if (owner.NetworkName != "CDOTA_Unit_Hero_Invoker")
            {
                canBeCasted = ability.Level > 0 && owner.Mana + bonusMana >= ability.ManaCost
                              && ability.Cooldown <= Math.Max(GameManager.Ping / 1000 - 0.1, 0);
                return canBeCasted;
            }

            var name = ability.Name;
            if (name != "invoker_invoke" && name != "invoker_quas" && name != "invoker_wex"
                && name != "invoker_exort" && ability.AbilitySlot != AbilitySlot.Slot4
                && ability.AbilitySlot != AbilitySlot.Slot5)
            {
                return false;
            }

            canBeCasted = ability.Level > 0 && owner.Mana + bonusMana >= ability.ManaCost
                          && ability.Cooldown <= Math.Max(GameManager.Ping / 1000 - 0.1, 0);
            return canBeCasted;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool CanBeCasted(this Ability ability, Unit target)
    {
        if (ability == null || !ability.IsValid)
        {
            return false;
        }

        if (target == null || !target.IsValid)
        {
            return false;
        }

        if (!target.IsValidTarget())
        {
            return false;
        }

        var canBeCasted = ability.CanBeCasted();
        if (!target.IsMagicImmune())
        {
            return canBeCasted;
        }

        var data = ability.CommonProperties();

        return data == null ? canBeCasted : data.MagicImmunityPierce;
    }

    public static bool CanHit(this Ability ability, Unit target, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return false;
        }

        if (target == null || !target.IsValid)
        {
            return false;
        }

        return CanHit(ability, target, ability.Owner.Position, abilityName);
    }

    public static bool CanHit(this Ability ability, Unit target, Vector3 sourcePosition, string abilityName = null)
    {
        if (ability == null || !ability.IsValid)
        {
            return false;
        }

        if (target == null || !target.IsValid)
        {
            return false;
        }

        var name = abilityName ?? ability.Name;
        if (ability.Owner.Equals(target))
        {
            return true;
        }

        var id = ability.Handle + target.Handle;
        if (sleeper.Sleeping(id))
        {
            return canHitDictionary[id];
        }

        var position = sourcePosition;
        if (ability.IsAbilityBehavior(AbilityBehavior.Point, name) || name == "lion_impale"
            || name == "earthshaker_enchant_totem" && (ability.Owner as Hero).HasAghanimsScepter())
        {
            var pred = ability.GetPrediction(target, abilityName: name);
            var lion = name == "lion_impale" ? ability.GetAbilityData("length_buffer") : 0;
            return position.Distance2D(pred)
                   <= ability.TravelDistance() + ability.GetRadius(name) + lion + target.HullRadius;
        }

        if (ability.IsAbilityBehavior(AbilityBehavior.NoTarget, name))
        {
            var pred = ability.GetPrediction(target, abilityName: name);
            var distanceXyz = position.Distance2D(pred);
            var radius = ability.GetRadius(name);
            var range = ability.GetCastRange(name);
            if (name.StartsWith("nevermore_shadowraze"))
            {
                range += radius / 2;
            }

            if (distanceXyz <= range && position.Distance2D(target.Position) <= range)
            {
                canHitDictionary[id] = true;
                sleeper.Sleep(50, id);
                return true;
            }

            canHitDictionary[id] = name == "pudge_rot" && target.HasModifier("modifier_pudge_meat_hook")
                                   && position.Distance2D(target.Position) < 1500;
            sleeper.Sleep(50, id);
            return canHitDictionary[id];
        }

        if (!ability.IsAbilityBehavior(AbilityBehavior.UnitTarget, name))
        {
            canHitDictionary[id] = false;
            sleeper.Sleep(50, id);
            return false;
        }

        if (target.IsInvul())
        {
            canHitDictionary[id] = false;
            sleeper.Sleep(50, id);
            return false;
        }

        if (position.Distance2D(target.Position) <= ability.GetCastRange(name) + 100)
        {
            canHitDictionary[id] = true;
            sleeper.Sleep(50, id);
            return true;
        }

        canHitDictionary[id] = name == "pudge_dismember" && target.HasModifier("modifier_pudge_meat_hook")
                               && position.Distance2D(target.Position) < 600;
        sleeper.Sleep(50, id);
        return canHitDictionary[id];
    }
}