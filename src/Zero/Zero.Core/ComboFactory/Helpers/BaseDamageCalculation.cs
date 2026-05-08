namespace Divine.Core.ComboFactory.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Settings;
using Divine.Core.Data;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities;
using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier.Modifiers;
using Divine.Update;

public abstract class BaseDamageCalculation
{
    private BaseSettingsMenu SettingsMenu { get; }

    protected CHero Owner { get; } = UnitManager.Owner;

    protected BaseDamageCalculation(BaseMenuConfig menuConfig)
    {
        SettingsMenu = menuConfig.SettingsMenu;

        SettingsMenu.EnableDamageCalculationItem.ValueChanged += EnableChanged;
    }

    public void Dispose()
    {
        SettingsMenu.EnableDamageCalculationItem.ValueChanged -= EnableChanged;
    }

    private void EnableChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            UpdateManager.CreateIngameUpdate(20, OnUpdate);
        }
        else
        {
            UpdateManager.DestroyIngameUpdate(OnUpdate);
        }
    }

    protected List<CAbility> DamageAbilities { get; } = new List<CAbility>();

    private void OnUpdate()
    {
        try
        {
            DamageAbilities.Clear();
            AbilityCheck();
            DamageDate.Clear();

            foreach (var hero in UnitManager<CHero, Enemy, NoIllusion>.Units)
            {
                var canHitdamage = 0.0f;
                var canBeCastedDamage = 0.0f;

                if (hero.IsVisible)
                {
                    var damageReduction = 0.0f;
                    var damageBlock = 0.0f;
                    var magicalDamageBlock = 0.0f;

                    foreach (var modifier in hero.Modifiers)
                    {
                        damageReduction += DamageReduction(modifier, hero);
                        damageBlock += DamageBlock(modifier, hero);
                        magicalDamageBlock += MagicalDamageBlock(modifier, hero);
                    }

                    var health = (float)hero.Health;

                    for (var i = 0; i < 2; i++)
                    {
                        var canHitActive = i == 1;
                        var physicalDamageReduction = 1f + damageReduction;
                        var magicalDamageReduction = 1f + damageReduction;
                        var pureDamageReduction = 1f + damageReduction;

                        var physicalDamage = damageBlock;
                        var magicalDamage = damageBlock + magicalDamageBlock;
                        var pureDamage = damageBlock;

                        foreach (var ability in DamageAbilities)
                        {
                            if (!ability.IsValid)
                            {
                                continue;
                            }

                            if (AbilityControl(hero, ability))
                            {
                                continue;
                            }

                            var isHitTime = IsHitTime(hero, ability);
                            if (!CanBeCasted(ability) && !isHitTime)
                            {
                                continue;
                            }

                            if (canHitActive && ability is IActiveAbility activeAbility && !activeAbility.CanHit(hero) && !isHitTime)
                            {
                                continue;
                            }

                            if (ability is IHasDamageAmplifier amplifier)
                            {
                                var modifier = (ability as IHasTargetModifier);
                                if (modifier == null || !hero.HasModifier(modifier.TargetModifierName))
                                {
                                    if (amplifier.AmplifierType.HasFlag(DamageType.Physical))
                                    {
                                        physicalDamageReduction *= 1 + amplifier.DamageAmplification;
                                    }

                                    if (amplifier.AmplifierType.HasFlag(DamageType.Magical))
                                    {
                                        magicalDamageReduction *= 1 + amplifier.DamageAmplification;
                                    }

                                    if (amplifier.AmplifierType.HasFlag(DamageType.Pure))
                                    {
                                        pureDamageReduction *= 1 + amplifier.DamageAmplification;
                                    }
                                }
                            }

                            physicalDamage += PhysicalDamageHealth(ability, hero, physicalDamageReduction, health, physicalDamage);
                            magicalDamage += MagicalDamageHealth(ability, hero, physicalDamageReduction, health, magicalDamage);
                            pureDamage += PureDamageHealth(ability, hero, physicalDamageReduction, health, pureDamage);

                            var physicalCurrentHealth = Math.Max(0, health - physicalDamage);
                            var magicalCurrentHealth = Math.Max(0, health - magicalDamage);
                            var pureCurrentHealth = Math.Max(0, health - pureDamage);

                            if (ability.DamageType == DamageType.Physical)
                            {
                                physicalDamage += ability.GetDamage(hero, physicalDamageReduction - 1, physicalCurrentHealth);
                            }

                            if (ability.DamageType == DamageType.Magical)
                            {
                                magicalDamage += GetMagicalDamage(ability, hero, magicalDamageReduction, magicalCurrentHealth, canHitActive);
                            }

                            if (ability.DamageType == DamageType.Pure)
                            {
                                pureDamage += ability.GetDamage(hero, pureDamageReduction - 1, pureCurrentHealth);
                            }
                        }

                        if (canHitActive)
                        {
                            canHitdamage = physicalDamage + magicalDamage + pureDamage;
                        }
                        else
                        {
                            canBeCastedDamage = physicalDamage + magicalDamage + pureDamage;
                        }
                    }
                }

                if (hero.IsInvulnerable() || hero.HasAnyModifiers(ModifierData.BlockModifiers))
                {
                    canHitdamage = 0.0f;
                    canBeCastedDamage = 0.0f;
                }

                if (hero.IsAlive)
                {
                    DamageAdd(hero, canHitdamage, canBeCastedDamage);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    protected abstract void AbilityCheck();

    protected virtual bool AbilityControl(CHero hero, CAbility ability)
    {
        return false;
    }

    protected virtual bool IsHitTime(CHero hero, CAbility ability)
    {
        return false;
    }

    protected virtual float PhysicalDamageHealth(CAbility ability, CHero hero, float physicalDamageReduction, float health, float physicalDamage)
    {
        return 0;
    }

    protected virtual float MagicalDamageHealth(CAbility ability, CHero hero, float magicalDamageReduction, float health, float magicalDamage)
    {
        return 0;
    }

    protected virtual float PureDamageHealth(CAbility ability, CHero hero, float pureDamageReduction, float health, float pureDamage)
    {
        return 0;
    }

    protected virtual float GetMagicalDamage(CAbility ability, CHero hero, float magicalDamageReduction, float magicalCurrentHealth, bool canHitActive)
    {
        return ability.GetDamage(hero, magicalDamageReduction - 1, magicalCurrentHealth);
    }

    // TODO REWORK
    protected bool CanBeCasted(CAbility ability)
    {
        if (!ability.IsReady)
        {
            return false;
        }

        var owner = ability.Owner;
        var isItem = ability is CItem;
        if (owner.IsStunned() || isItem && owner.IsMuted() || !isItem && owner.IsSilenced())
        {
            return false;
        }

        return true;
    }

    private float DamageReduction(Modifier modifier, CHero hero)
    {
        var value = 0.0f;

        // Modifier Veil Of Discord
        if (modifier.Name == "modifier_item_veil_of_discord_debuff")
        {
            value -= modifier.Ability.GetSpecialData("spell_amp") / -100f;
        }

        // Modifier Centaur Stampede
        if (modifier.Name == "modifier_centaur_stampede")
        {
            var centaur = UnitManager<CHero, Enemy>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_centaur && x.HasAghanimsScepter());
            if (centaur != null)
            {
                value -= centaur.GetAbilityById(AbilityId.centaur_stampede).GetSpecialData("damage_reduction") / 100f;
            }
        }

        // Modifier Kunkka Ghostship
        if (modifier.Name == "modifier_kunkka_ghost_ship_damage_absorb")
        {
            var kunkka = UnitManager<CHero, Enemy>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_kunkka);
            value -= kunkka.GetAbilityById(AbilityId.kunkka_ghostship).GetSpecialData("ghostship_absorb") / 100f;
        }

        // Modifier Wisp Overcharge
        if (modifier.Name == "modifier_wisp_overcharge")
        {
            var wisp = UnitManager<CHero, Enemy>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_wisp);
            value += wisp.GetAbilityById(AbilityId.wisp_overcharge).GetSpecialData("bonus_damage_pct") / 100f;
        }

        // Modifier Bloodseeker Bloodrage
        if (modifier.Name == "modifier_bloodseeker_bloodrage" || Owner.HasModifier("modifier_bloodseeker_bloodrage"))
        {
            var bloodseeker = UnitManager<CHero>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_bloodseeker);
            value += bloodseeker.GetAbilityById(AbilityId.bloodseeker_bloodrage).GetSpecialData("damage_increase_pct") / 100f;
        }

        // Modifier Medusa Mana Shield
        if (modifier.Name == "modifier_medusa_mana_shield")
        {
            if (hero.Mana >= 50)
            {
                value -= hero.GetAbilityById(AbilityId.medusa_mana_shield).GetSpecialData("absorption_tooltip") / 100f;
            }
        }

        // Modifier Ursa Enrage
        if (modifier.Name == "modifier_ursa_enrage")
        {
            value -= hero.GetAbilityById(AbilityId.ursa_enrage).GetSpecialData("damage_reduction") / 100f;
        }

        // Modifier Chen Penitence
        if (modifier.Name == "modifier_chen_penitence")
        {
            var chen = UnitManager<CHero, Ally>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_chen);
            value += chen.GetAbilityById(AbilityId.chen_penitence).GetSpecialData("bonus_damage_taken") / 100f;
        }

        // Modifier Pangolier Shield Crash
        if (modifier.Name == "modifier_pangolier_shield_crash_buff")
        {
            value -= modifier.StackCount / 100f;
        }

        // Modifier Bristleback Bristleback
        if (modifier.Name == "modifier_bristleback_bristleback")
        {
            var bristleback = hero.GetAbilityById(AbilityId.bristleback_bristleback);

            if (hero.GetRotationAngle(Owner.Position) > 1.90f)
            {
                value -= bristleback.GetSpecialData("back_damage_reduction") / 100f;
            }
            else if (hero.GetRotationAngle(Owner.Position) > 1.20f)
            {
                value -= bristleback.GetSpecialData("side_damage_reduction") / 100f;
            }
        }

        return value;
    }

    private float MagicalDamageBlock(Modifier modifier, CHero hero)
    {
        var value = 0.0f;

        // Modifier Hood Of Defiance Barrier
        if (modifier.Name == "modifier_item_hood_of_defiance_barrier")
        {
            var item = hero.GetItemById(AbilityId.item_hood_of_defiance);
            if (item != null)
            {
                value -= item.GetSpecialData("barrier_block");
            }
        }

        // Modifier Pipe Barrier
        if (modifier.Name == "modifier_item_pipe_barrier")
        {
            var pipehero = UnitManager<CHero, Enemy>.Units.FirstOrDefault(x => x.Items.Any(v => v.Id == AbilityId.item_pipe));
            if (pipehero != null)
            {
                value -= pipehero.GetItemById(AbilityId.item_pipe).GetSpecialData("barrier_block");
            }
        }

        // Modifier Infused Raindrop
        if (modifier.Name == "modifier_item_infused_raindrop")
        {
            var item = hero.GetItemById(AbilityId.item_infused_raindrop);
            if (item != null && item.Cooldown <= 0)
            {
                value -= item.GetSpecialData("magic_damage_block");
            }
        }

        // Modifier Ember Spirit Flame Guard
        if (modifier.Name == "modifier_ember_spirit_flame_guard")
        {
            var ability = hero.GetAbilityById(AbilityId.ember_spirit_flame_guard);
            if (ability != null)
            {
                value -= ability.GetSpecialData("absorb_amount");

                var talent = ability.Owner.GetAbilityById(AbilityId.special_bonus_unique_ember_spirit_1);
                if (talent != null && talent.Level > 0)
                {
                    value -= talent.GetSpecialData("value");
                }
            }
        }

        return value;
    }

    private float DamageBlock(Modifier modifier, CHero hero)
    {
        var value = 0.0f;

        // Modifier Abaddon Aphotic Shield
        if (modifier.Name == "modifier_abaddon_aphotic_shield")
        {
            var abaddon = UnitManager<CHero, Enemy>.Units.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_abaddon);
            value -= abaddon.GetAbilityById(AbilityId.abaddon_aphotic_shield).GetSpecialData("damage_absorb");

            var talent = abaddon.GetAbilityById(AbilityId.special_bonus_unique_abaddon);
            if (talent != null && talent.Level > 0)
            {
                value -= talent.GetSpecialData("value");
            }
        }

        return value;
    }

    public static List<Damage> DamageDate { get; } = new List<Damage>();

    private void DamageAdd(CHero hero, float canHitdamage, float canBeCastedDamage)
    {
        DamageDate.Add(new Damage(hero, canHitdamage, canBeCastedDamage, hero.Health));
    }
}

public class Damage
{
    public Damage(CHero hero, float canHitdamage, float canBeCastedDamage, uint health)
    {
        GetHero = hero;
        GetCanHitdamage = canHitdamage;
        GetCanBeCastedDamage = canBeCastedDamage;
        GetHealth = health;

        IsKillSteal = (health - canHitdamage) / GetHero.MaximumHealth <= 0f;
    }

    public CHero GetHero { get; }

    public float GetCanHitdamage { get; }

    public float GetCanBeCastedDamage { get; }

    public uint GetHealth { get; }

    public bool IsKillSteal { get; }
}
