namespace Divine.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Data;
using Divine.Core.Entities;
using Divine.Core.Helpers;
using Divine.Core.Managers;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Modifier.Modifiers;
using Divine.Numerics;

public static class UnitExtensions
{
    public static bool IsBlockMagicDamage(this CUnit target, TargetModifiers targetModifiers = null)
    {
        if (target.IsMagicImmune() || target.IsInvulnerable())
        {
            return true;
        }

        if (targetModifiers != null)
        {
            return targetModifiers.IsBlockingModifers || targetModifiers.IsDuelAghanimsScepter;
        }

        return false;
    }

    public static float GetAttackDamage(this CUnit source, CUnit target, bool useMinimumDamage = false, float damageAmplifier = 0.0f)
    {
        float damage = (!useMinimumDamage ? source.DamageAverage : source.MinimumDamage) + source.BonusDamage;
        var mult = 1f;
        var damageType = source.AttackDamageType;
        var armorType = target.ArmorType;

        if (damageType == AttackDamageType.Hero && armorType == ArmorType.Structure)
        {
            mult *= .5f;
        }
        else if (damageType == AttackDamageType.Basic && armorType == ArmorType.Hero)
        {
            mult *= .75f;
        }
        else if (damageType == AttackDamageType.Basic && armorType == ArmorType.Structure)
        {
            mult *= .7f;
        }
        else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Hero)
        {
            mult *= .5f;
        }
        else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Basic)
        {
            mult *= 1.5f;
        }
        else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Structure)
        {
            mult *= .35f;
        }
        else if (damageType == AttackDamageType.Siege && armorType == ArmorType.Hero)
        {
            mult *= .85f;
        }
        else if (damageType == AttackDamageType.Siege && armorType == ArmorType.Structure)
        {
            mult *= 2.50f;
        }

        if (target.IsNeutral || target is CCreep && source.IsEnemy(target))
        {
            var isMelee = source.IsMelee;

            var quellingBlade = source.GetItemById(AbilityId.item_quelling_blade);
            if (quellingBlade != null)
            {
                damage += quellingBlade.GetAbilitySpecialData(isMelee ? "damage_bonus" : "damage_bonus_ranged");
            }

            // apply percentage bonus damage from battle fury to base dmg
            var battleFury = source.GetItemById(AbilityId.item_bfury);
            if (battleFury != null)
            {
                mult *= battleFury.GetAbilitySpecialData(isMelee ? "quelling_bonus" : "quelling_bonus_ranged") / 100.0f; // 160 | 125
            }
        }

        var armor = target.Armor;

        mult *= 1 - (0.052f * armor / (0.9f + (0.048f * Math.Abs(armor))));
        mult *= 1.0f + damageAmplifier;
        return damage * mult;
    }

    public static bool IsChanneling(this CUnit unit)
    {
        if (unit.HasInventory && unit.Inventory.Items.Any(x => x.IsChanneling))
        {
            return true;
        }

        return unit.Spellbook.Spells.Any(s => s.IsChanneling);
    }

    public static bool IsMagicImmune(this CUnit unit)
    {
        return (unit.UnitState & UnitState.MagicImmune) == UnitState.MagicImmune;
    }

    public static bool IsMuted(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Muted) == UnitState.Muted;
    }

    public static bool IsDisarmed(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Disarmed) == UnitState.Disarmed;
    }

    public static bool IsInvisible(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Invisible) == UnitState.Invisible;
    }

    public static bool ComboBreaker(this CUnit target)
    {
        var comboBreaker = target.GetItemById(AbilityId.item_aeon_disk);
        if (comboBreaker != null && comboBreaker.Cooldown <= 0)
        {
            return true;
        }

        return false;
    }

    public static bool IsLinkensSphere(this CUnit unit, TargetModifiers targetModifiers)
    {
        var linkens = unit.GetItemById(AbilityId.item_sphere);
        return linkens?.Cooldown <= 0 || targetModifiers.IsLinken;
    }

    public static bool IsAntimageSpellShield(this CUnit unit)
    {
        var spellShield = unit.GetAbilityById(AbilityId.antimage_spell_shield);
        return spellShield != null && spellShield.Level > 0 && spellShield.Cooldown <= 0 && unit.HasAghanimsScepter();
    }

    public static bool IsShieldAbilities(this CUnit unit, TargetModifiers targetModifiers)
    {
        if (unit.IsLinkensSphere(targetModifiers))
        {
            return true;
        }

        if (unit.IsAntimageSpellShield())
        {
            return true;
        }

        return false;
    }

    public static bool IsAttackImmune(this CUnit unit)
    {
        return (unit.UnitState & UnitState.AttackImmune) == UnitState.AttackImmune;
    }

    public static bool HasAnyModifiers(this CUnit unit, params string[] modifierNames)
    {
        return unit.Modifiers.Any(x => modifierNames.Contains(x.Name));
    }

    public static bool HasModifier(this CUnit unit, string modifierName)
    {
        return unit.Modifiers.Any(modifier => modifier.Name == modifierName);
    }

    public static bool HasModifiers(this CUnit unit, IEnumerable<string> modifierNames, bool hasAll = true)
    {
        return hasAll ? modifierNames.All(x => unit.Modifiers.Any(y => y.Name == x)) : unit.Modifiers.Any(x => modifierNames.Contains(x.Name));
    }

    public static float HealthPercent(this CUnit unit)
    {
        return (float)unit.Health / unit.MaximumHealth;
    }

    public static float AttackRange(this CUnit unit, CUnit target = null) //TODO
    {
        var result = unit.AttackRange + unit.HullRadius;

        if (target != null)
        {
            if (unit.HurricanePikeTarget?.Handle == target.Handle)
            {
                return float.MaxValue;
            }

            result += target.HullRadius;
        }

        if (unit is CCreep)
        {
            result += 15f;
        }

        var hero = unit as CHero;
        if (hero != null)
        {
            if (hero.IsRanged)
            {
                // test for talents with bonus range
                foreach (var ability in hero.Spellbook.Spells.Where(x => x.Level > 0 && x.Name.StartsWith("special_bonus_attack_range_")))
                {
                    result += ability.GetAbilitySpecialData("value");
                }

                // test for items with bonus range
                var bonusRangeItem = hero.GetItemById(AbilityId.item_dragon_lance) ?? hero.GetItemById(AbilityId.item_hurricane_pike);
                if (bonusRangeItem != null)
                {
                    result += bonusRangeItem.GetAbilitySpecialData("base_attack_range");
                }
            }

            switch (hero.HeroId)
            {
                case HeroId.npc_dota_hero_sniper:
                    var sniperTakeAim = hero.GetAbilityById(AbilityId.sniper_take_aim);
                    if (sniperTakeAim?.Level > 0)
                    {
                        result += sniperTakeAim.GetAbilitySpecialData("bonus_attack_range");
                    }

                    break;

                case HeroId.npc_dota_hero_templar_assassin:
                    var psiBlades = hero.GetAbilityById(AbilityId.templar_assassin_psi_blades);
                    if (psiBlades?.Level > 0)
                    {
                        result += psiBlades.GetAbilitySpecialData("bonus_attack_range");
                    }

                    break;

                case HeroId.npc_dota_hero_enchantress:
                    var impetus = hero.GetAbilityById(AbilityId.enchantress_impetus);
                    if (impetus?.Level > 0 && hero.HasAghanimsScepter())
                    {
                        result += impetus.GetAbilitySpecialData("bonus_attack_range_scepter");
                    }

                    break;

                case HeroId.npc_dota_hero_terrorblade:
                    var metamorphosis = hero.GetAbilityById(AbilityId.terrorblade_metamorphosis);
                    if (metamorphosis != null && hero.HasModifier("modifier_terrorblade_metamorphosis"))
                    {
                        var talent = hero.GetAbilityById(AbilityId.special_bonus_unique_terrorblade_3);
                        if (talent?.Level > 0)
                        {
                            result += talent.GetAbilitySpecialData("value");
                        }
                        result += metamorphosis.GetAbilitySpecialData("bonus_range");
                    }

                    break;

                case HeroId.npc_dota_hero_dragon_knight:
                    var dragonForm = hero.GetAbilityById(AbilityId.dragon_knight_elder_dragon_form);
                    if (dragonForm != null && hero.HasModifier("modifier_dragon_knight_dragon_form"))
                    {
                        result += dragonForm.GetAbilitySpecialData("bonus_attack_range");
                    }

                    break;

                case HeroId.npc_dota_hero_winter_wyvern:
                    var arcticBurn = hero.GetAbilityById(AbilityId.winter_wyvern_arctic_burn);
                    if (arcticBurn != null && hero.HasModifier("modifier_winter_wyvern_arctic_burn_flight"))
                    {
                        result += arcticBurn.GetAbilitySpecialData("attack_range_bonus");
                    }

                    break;

                case HeroId.npc_dota_hero_troll_warlord:
                    var trollMeleeForm = hero.GetAbilityById(AbilityId.troll_warlord_berserkers_rage);
                    if (trollMeleeForm != null && hero.HasModifier("modifier_troll_warlord_berserkers_rage"))
                    {
                        result -= trollMeleeForm.GetAbilitySpecialData("bonus_range");
                    }

                    break;

                case HeroId.npc_dota_hero_lone_druid:
                    var druidMeleeForm = hero.GetAbilityById(AbilityId.lone_druid_true_form);
                    if (druidMeleeForm != null && hero.HasModifier("modifier_lone_druid_true_form"))
                    {
                        // no special data
                        result -= 400;
                    }

                    break;
            }
        }

        return result;
    }

    public static bool IsInAttackRange(this CUnit source, CUnit target, float bonusAttackRange = 0.0f)
    {
        return source.IsInRange(target, source.AttackRange(target) + bonusAttackRange, true);
    }

    public static bool IsInRange(this CUnit source, CUnit target, float range, bool centerToCenter = false)
    {
        return source.Position.IsInRange(target, centerToCenter ? range : Math.Max(0, range + source.HullRadius + target.HullRadius));
    }

    public static Modifier GetModifierByName(this CUnit unit, string name)
    {
        return unit.Modifiers.FirstOrDefault(x => x.Name == name);
    }

    public static Modifier GetModifierByTextureName(this CUnit unit, string name)
    {
        return unit.Modifiers.FirstOrDefault(x => x.TextureName == name);
    }

    public static CAbility GetAbilityById(this CUnit unit, AbilityId abilityId)
    {
        return unit.Spells.FirstOrDefault(x => x.Id == abilityId);
    }

    public static CItem GetItemById(this CUnit unit, AbilityId abilityId)
    {
        if (!unit.HasInventory)
        {
            return null;
        }

        return unit.Items.FirstOrDefault(x => x.Id == abilityId);
    }

    public static bool HasAghanimsScepter(this CUnit unit)
    {
        return unit.HasAnyModifiers("modifier_item_ultimate_scepter", "modifier_item_ultimate_scepter_consumed", "modifier_wisp_tether_scepter");
    }

    public static bool CanAttack(this CUnit unit)
    {
        return unit.AttackCapability != AttackCapability.None && !unit.IsDisarmed();
    }

    public static bool CanAttack(this CUnit attacker, CUnit target)
    {
        if (target == null || !target.IsValid || !target.IsAlive || !target.IsVisible || !target.IsSpawned || target.IsInvulnerable())
        {
            return false;
        }

        if (attacker.Team == target.Team)
        {
            if (target is CCreep)
            {
                return target.HealthPercent() < 0.5;
            }

            if (target is CHero)
            {
                return target.HealthPercent() < 0.25;
            }

            if (target is CBuilding)
            {
                return target.HealthPercent() < 0.10;
            }
        }

        return true;
    }

    public static float GetSpellAmplification(this CUnit source)
    {
        var spellAmp = 0.0f;

        var kaya = false;
        var yashaAndKaya = false;
        var kayaAndSange = false;

        foreach (var item in source.Items)
        {
            switch (item.Id)
            {
                case AbilityId.item_kaya:
                    {
                        if (kaya)
                        {
                            break;
                        }

                        kaya = true;
                        spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                    }
                    break;

                case AbilityId.item_yasha_and_kaya:
                    {
                        if (yashaAndKaya)
                        {
                            break;
                        }

                        yashaAndKaya = true;
                        spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                    }
                    break;

                case AbilityId.item_kaya_and_sange:
                    {
                        if (kayaAndSange)
                        {
                            break;
                        }

                        kayaAndSange = true;
                        spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                    }
                    break;
            }
        }

        var talent = source.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_spell_amplify_"));
        if (talent != null)
        {
            spellAmp += talent.AbilitySpecialData.First(x => x.Name == "value").Value / 100.0f;
        }

        return spellAmp;
    }

    public static Vector3 Escape(this CUnit unit)
    {
        var position = unit.Position;
        var clusterPosition = Vector3.Zero;

        var i = 0;
        foreach (var hero in UnitManager<CHero, Enemy>.Units.ToArray())
        {
            if (!hero.IsAlive || !hero.IsVisible)
            {
                continue;
            }

            var dangerRange = hero.AttackRange(unit);
            dangerRange = hero.IsMelee ? dangerRange * 2.5f : dangerRange * 1.6f;

            var heroPosition = hero.Position;
            if (heroPosition.Distance(position) > dangerRange * 0.8f)
            {
                continue;
            }

            i++;
            clusterPosition += heroPosition.Extend(position, dangerRange);
        }

        if (clusterPosition.IsZero)
        {
            return Vector3.Zero;
        }

        return clusterPosition /= i;
    }

    public static float GetAutoAttackArrivalTime(this CUnit source, CUnit target, bool takeRotationTimeIntoAccount = true)
    {
        var result = GetProjectileArrivalTime(source, target, source.AttackPoint, source.IsMelee ? float.MaxValue : source.ProjectileSpeed(), takeRotationTimeIntoAccount);

        if (!(source is CTower))
        {
            result -= 0.05f; // :broscience:
        }

        return result;
    }

    public static float GetProjectileArrivalTime(this CUnit source, CUnit target, float delay, float missileSpeed, bool takeRotationTimeIntoAccount = true)
    {
        var result = 0f;

        // rotation time
        result += takeRotationTimeIntoAccount ? source.GetTurnTime(target.Position) : 0f;

        // delay
        result += delay;

        // time that takes to the missile to reach the target
        if (missileSpeed != float.MaxValue)
        {
            result += source.Distance2D(target) / missileSpeed;
        }

        return result;
    }

    public static Vector3 Direction(this CUnit unit, float length = 1f)
    {
        var rotation = unit.NetworkRotationRad;
        return new Vector3((float)Math.Cos(rotation) * length, (float)Math.Sin(rotation) * length, unit.Position.Z);
    }

    public static Vector2 Direction2D(this CUnit unit, float length = 1f)
    {
        var rotation = unit.NetworkRotationRad;
        return new Vector2((float)Math.Cos(rotation) * length, (float)Math.Sin(rotation) * length);
    }

    public static bool IsInvulnerable(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Invulnerable) == UnitState.Invulnerable;
    }

    public static bool IsStunned(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Stunned) == UnitState.Stunned;
    }

    public static bool IsSilenced(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Silenced) == UnitState.Silenced;
    }

    public static bool IsRooted(this CUnit unit)
    {
        return (unit.UnitState & UnitState.Rooted) == UnitState.Rooted;
    }

    public static float ProjectileSpeed(this CUnit unit)
    {
        return unit.Base.ProjectileSpeed();
    }
}