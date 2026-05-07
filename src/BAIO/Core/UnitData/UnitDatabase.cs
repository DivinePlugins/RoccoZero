namespace BAIO.Core.UnitData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

using BAIO.Core;
using BAIO.Core.Extensions;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;

public static class UnitDatabase
{
    private static readonly Dictionary<uint, double> AttackPointDictionary = new Dictionary<uint, double>();

    private static readonly Dictionary<uint, double> AttackRateDictionary = new Dictionary<uint, double>();

    private static readonly Dictionary<uint, double> ProjSpeedDictionary = new Dictionary<uint, double>();

    static UnitDatabase()
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BAIO.Core.Resources.UnitDatabase.json");
        var reader = new StreamReader(stream);

        var json = JsonNode.Parse(reader.ReadToEnd(), null, new() { CommentHandling = JsonCommentHandling.Skip });
        Units = JsonSerializer.Deserialize<AttackAnimationData[]>(json[nameof(Units)]).ToList();
    }

    public static List<AttackAnimationData> Units { get; set; }

    public static double GetAttackBackswing(Hero unit)
    {
        var attackRate = GetAttackRate(unit);
        var attackPoint = GetAttackPoint(unit);
        return attackRate - attackPoint;
    }

    public static double GetAttackBackswing(Unit unit)
    {
        var attackRate = GetAttackRate(unit);
        var attackPoint = GetAttackPoint(unit);
        return attackRate - attackPoint;
    }

    public static double GetAttackPoint(Hero unit)
    {
        return GetAttackPoint(unit, GetAttackSpeed(unit));
    }

    public static double GetAttackPoint(Hero unit, float attackSpeedValue)
    {
        if (unit == null)
        {
            return 0;
        }

        try
        {
            var name = unit.Name;
            double attackAnimationPoint;
            if (!AttackPointDictionary.TryGetValue(unit.Handle, out attackAnimationPoint))
            {
                attackAnimationPoint = Hero.GetKeyValueByName(unit.Name).GetSubKey("AttackAnimationPoint")!.GetSingle();
                AttackPointDictionary.Add(unit.Handle, attackAnimationPoint);
            }

            return attackAnimationPoint / (1 + (attackSpeedValue - 100) / 100);
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return 0;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return 0;
        }
    }

    public static double GetAttackPoint(Unit unit)
    {
        return GetAttackPoint(unit, GetAttackSpeed(unit));
    }

    public static double GetAttackPoint(Unit unit, float attackSpeedValue)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            return GetAttackPoint(hero, attackSpeedValue);
        }

        if (unit == null)
        {
            return 0;
        }

        try
        {
            var name = unit.Name;
            double attackAnimationPoint;
            if (!AttackPointDictionary.TryGetValue(unit.Handle, out attackAnimationPoint))
            {
                attackAnimationPoint = Unit.GetKeyValueByName(unit.Name).GetSubKey("AttackAnimationPoint")!.GetSingle();
                AttackPointDictionary.Add(unit.Handle, attackAnimationPoint);
            }

            return attackAnimationPoint / (1 + (attackSpeedValue - 100) / 100);
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return 0;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return 0;
        }
    }

    public static double GetAttackRate(Hero unit)
    {
        return GetAttackRate(unit, GetAttackSpeed(unit));
    }

    public static double GetAttackRate(Hero unit, float attackSpeedValue)
    {
        try
        {
            double attackBaseTime;
            if (!AttackRateDictionary.TryGetValue(unit.Handle, out attackBaseTime))
            {
                attackBaseTime = Hero.GetKeyValueByName(unit.Name).GetSubKey("AttackRate")!.GetSingle();
                AttackRateDictionary.Add(unit.Handle, attackBaseTime);
            }

            Ability spell = null;
            if (
                !unit.HasModifiers(
                    new[]
                        {
                                "modifier_alchemist_chemical_rage", "modifier_terrorblade_metamorphosis",
                                "modifier_lone_druid_true_form", "modifier_troll_warlord_berserkers_rage"
                        },
                    false))
            {
                return attackBaseTime / (1 + (attackSpeedValue - 100) / 100);
            }

            switch (unit.HeroId)
            {
                case HeroId.npc_dota_hero_alchemist:
                spell = unit.Spellbook.Spells.First(x => x.Name == "alchemist_chemical_rage");
                break;
                case HeroId.npc_dota_hero_terrorblade:
                spell = unit.Spellbook.Spells.First(x => x.Name == "terrorblade_metamorphosis");
                break;
                case HeroId.npc_dota_hero_lone_druid:
                spell = unit.Spellbook.Spells.First(x => x.Name == "lone_druid_true_form");
                break;
                case HeroId.npc_dota_hero_troll_warlord:
                spell = unit.Spellbook.Spells.First(x => x.Name == "troll_warlord_berserkers_rage");
                break;
            }

            if (spell == null)
            {
                return attackBaseTime / (1 + (attackSpeedValue - 100) / 100);
            }

            attackBaseTime = spell.GetAbilityData("base_attack_time");

            return attackBaseTime / (1 + (attackSpeedValue - 100) / 100);
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return 0;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return 0;
        }
    }

    public static double GetAttackRate(Unit unit)
    {
        return GetAttackRate(unit, GetAttackSpeed(unit));
    }

    public static double GetAttackRate(Unit unit, float attackSpeedValue)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            return GetAttackRate(hero, attackSpeedValue);
        }

        try
        {
            double attackBaseTime;
            if (AttackRateDictionary.TryGetValue(unit.Handle, out attackBaseTime))
            {
                return attackBaseTime / (1 + (attackSpeedValue - 100) / 100);
            }

            attackBaseTime = Unit.GetKeyValueByName(unit.Name).GetSubKey("AttackRate")!.GetSingle();
            AttackRateDictionary.Add(unit.Handle, attackBaseTime);

            return attackBaseTime / (1 + (attackSpeedValue - 100) / 100);
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return 0;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return 0;
        }
    }

    public static float GetAttackSpeed(Hero unit)
    {
        var attackSpeed = Math.Min(unit.AttackSpeed, 600);

        if (unit.HasModifier("modifier_ursa_overpower"))
        {
            attackSpeed = 600;
        }

        return (float)attackSpeed;
    }

    public static float GetAttackSpeed(Unit unit)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            return GetAttackSpeed(hero);
        }

        var attackSpeed = Math.Min(unit.AttackSpeed, 600);
        return (float)attackSpeed;
    }

    public static AttackAnimationData GetByNetworkName(string networkName)
    {
        return Units.FirstOrDefault(unitData => unitData.UnitNetworkName.Equals(networkName));
    }
    public static AttackAnimationData GetByName(string unitName)
    {
        return Units.FirstOrDefault(unitData => unitData.UnitName.ToLower() == unitName);
    }
    public static double GetProjectileSpeed(Hero unit)
    {
        if (unit == null || !unit.IsRanged)
        {
            return double.MaxValue;
        }

        var name = unit.Name;
        try
        {
            double projSpeed;
            if (!ProjSpeedDictionary.TryGetValue(unit.Handle, out projSpeed))
            {
                projSpeed = Hero.GetKeyValueByName(unit.Name).GetSubKey("ProjectileSpeed")!.GetInt32();
            }

            return projSpeed;
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return double.MaxValue;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return double.MaxValue;
        }
    }

    public static double GetProjectileSpeed(Unit unit)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            return GetProjectileSpeed(hero);
        }

        if (unit == null || !unit.IsRanged)
        {
            return double.MaxValue;
        }

        var name = unit.Name;
        try
        {
            double projSpeed;
            if (!ProjSpeedDictionary.TryGetValue(unit.Handle, out projSpeed))
            {
                projSpeed = Hero.GetKeyValueByName(unit.Name).GetSubKey("ProjectileSpeed")!.GetInt32();
            }

            return projSpeed;
        }
        catch
        {
            if (!Utils.SleepCheck("Ensage.Common.DemoModeWarning"))
            {
                return double.MaxValue;
            }

            Utils.Sleep(10000, "Ensage.Common.DemoModeWarning");
            Console.WriteLine(@"[[Please do not use demo mode for testing assemblies]]");
            return double.MaxValue;
        }
    }
}