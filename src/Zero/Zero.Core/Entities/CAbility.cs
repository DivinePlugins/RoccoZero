namespace Divine.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Managers.Ability;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Runes;
using Divine.Entity.Entities.Trees;
using Divine.Entity.Entities.Units.Components;
using Divine.Game;
using Divine.Numerics;

public class CAbility : CEntity
{
    internal CAbility(Ability ability)
        : base(ability)
    {
        Base = ability;

        AbilityData = ability.AbilityData;
        TextureName = ability.TextureName;
        Id = ability.Id;
        AbilityType = ability.AbilityType;
        AbilityBehavior = ability.AbilityBehavior;
        DamageType = ability.DamageType;
    }

    public new Ability Base { get; }

    public virtual bool IsItem { get; }

    private float cooldownLastTime;

    public float GetCooldownInFog()
    {
        if (Owner.IsVisible)
        {
            cooldownLastTime = GameManager.RawGameTime;
            return Cooldown;
        }

        return Cooldown - (GameManager.RawGameTime - cooldownLastTime);
    }

    public virtual float ActivationDelay { get; } = 0;

    public virtual UnitState AppliesUnitState { get; } = 0;

    public virtual bool IsReady
    {
        get
        {
            if (Level == 0 || Cooldown > 0)
            {
                return false;
            }

            if (Owner.Mana < ManaCost)
            {
                return false;
            }

            return true;
        }
    }

    public float RemainingCooldown
    {
        get
        {
            var cooldown = Base.Cooldown;
            if (!Owner.IsVisible)
            {
                cooldown = Math.Max(cooldown - (GameManager.RawGameTime - Owner.LastVisibleTime), 0);
            }

            return cooldown;
        }
    }

    public virtual float TimeSinceCasted
    {
        get
        {
            var cooldownLength = Base.CooldownLength;
            if (cooldownLength > 0)
            {
                return cooldownLength - RemainingCooldown;
            }

            return float.MaxValue;
        }
    }

    public virtual bool CanBeCasted { get; }

    public virtual float Duration
    {
        get
        {
            var level = Level;
            if (level == 0)
            {
                return 0.0f;
            }

            return GetDuration(level - 1);
        }
    }

    protected virtual float BaseCastRange
    {
        get
        {
            return Base.BaseCastRange;
        }
    }

    protected virtual float RawDamage
    {
        get
        {
            var level = Level;
            if (level == 0)
            {
                return 0;
            }

            return GetDamage(level - 1);
        }
    }

    public virtual float GetDamage(params CUnit[] targets)
    {
        return 0;
    }

    public virtual float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
    {
        return 0;
    }

    public AbilityData AbilityData { get; }

    public static AbilityData GetAbilityDataById(AbilityId abilityId)
    {
        return Ability.GetAbilityDataById(abilityId);
    }

    public static AbilityData GetAbilityDataByName(string name)
    {
        return Ability.GetAbilityDataByName(name);
    }

    public bool HasAltCastState
    {
        get
        {
            return Base.HasAltCastState;
        }
    }

    public int EnemyLevel
    {
        get
        {
            return Base.EnemyLevel;
        }
    }

    public bool IsReplicated
    {
        get
        {
            return Base.IsReplicated;
        }
    }

    public bool IsHidden
    {
        get
        {
            return Base.IsHidden;
        }
    }

    public bool IsActivated
    {
        get
        {
            return Base.IsActivated;
        }
    }

    public bool IsStolen
    {
        get
        {
            return Base.IsStolen;
        }
    }

    public virtual uint Level
    {
        get
        {
            return Base.Level;
        }
    }

    public bool IsToggled
    {
        get
        {
            return Base.IsToggled;
        }
    }

    public bool IsCooldownFrozen
    {
        get
        {
            return Base.IsCooldownFrozen;
        }
    }

    public bool IsInAbilityPhase
    {

        get
        {
            return Base.IsInAbilityPhase;
        }
    }

    public float Cooldown
    {
        get
        {
            return Base.Cooldown;
        }
    }

    public virtual float CastRange
    {
        get
        {
            return BaseCastRange;
        }
    }

    public float CooldownLength
    {
        get
        {
            return Base.CooldownLength;
        }
    }

    public float ManaCost
    {
        get
        {
            return Base.ManaCost;
        }
    }

    public bool IsAutoCastEnabled
    {
        get
        {
            return Base.IsAutoCastEnabled;
        }
    }

    public float ChannelTime
    {
        get
        {
            return Base.ChannelTime;
        }
    }

    public bool IsInIndefiniteCooldown
    {

        get
        {
            return Base.IsInIndefiniteCooldown;
        }
    }

    public float OverrideCastPoint
    {
        get
        {
            return Base.OverrideCastPoint;
        }
    }

    public float LastCastClickTime
    {
        get
        {
            return Base.LastCastClickTime;
        }
    }

    public float ChannelStartTime
    {
        get
        {
            return Base.ChannelStartTime;
        }
    }

    public int MaximumLevel
    {
        get
        {
            return Base.MaximumLevel;
        }
    }

    public string TextureName { get; }

    public string SharedCooldownName
    {
        get
        {
            return Base.SharedCooldownName;
        }
    }

    public AbilityId Id { get; }

    public AbilityType AbilityType { get; }

    public virtual AbilityBehavior AbilityBehavior { get; }

    public TargetTeamType TargetTeamType
    {
        get
        {
            return Base.TargetTeamType;
        }
    }

    public TargetFlags TargetFlags
    {
        get
        {
            return Base.TargetFlags;
        }
    }

    public TargetType TargetType
    {
        get
        {
            return Base.TargetType;
        }
    }

    public virtual DamageType DamageType { get; }

    public virtual SpellPierceImmunityType SpellPierceImmunityType
    {
        get
        {
            return Base.SpellPierceImmunityType;
        }
    }

    public DispellableType DispellableType
    {
        get
        {
            return Base.DispellableType;
        }
    }

    public bool IsGrantedByScepter
    {
        get
        {
            return Base.IsGrantedByScepter;
        }
    }

    public int RequiredLevel
    {
        get
        {
            return Base.RequiredLevel;
        }
    }

    public ushort AbilityIndex
    {
        get
        {
            return AbilityData.AbilityIndex;
        }
    }

    public int LevelsBeetweenUpgrades
    {
        get
        {
            return Base.LevelsBeetweenUpgrades;
        }
    }

    public KeyValue KeyValue
    {
        get
        {
            return Base.KeyValue;
        }
    }

    public AbilityState AbilityState
    {
        get
        {
            return Base.AbilityState;
        }
    }

    public IEnumerable<AbilitySpecialData> AbilitySpecialData
    {
        get
        {
            return Base.AbilitySpecialData;
        }
    }

    public float GetAbilitySpecialData(string name, uint level = 0)
    {
        var data = AbilitySpecialData.First(x => x.Name == name);

        if (data.Count == 1)
        {
            return data.Value;
        }

        if (level == 0)
        {
            level = Level;
        }

        if (level == 0)
        {
            return 0;
        }

        return data.GetValue(level - 1);
    }

    public float GetAbilitySpecialDataWithTalent(string name, uint level = 0)
    {
        var data = AbilitySpecialData.First(x => x.Name == name);

        //var talent = Owner.GetAbilityById(data.SpecialBonusAbility);
        var talentValue = 0f;
        //if (talent?.Level > 0)
        //{
        //    talentValue = talent.AbilitySpecialData.First(x => x.Name == "value").Value;
        //}

        if (data.Count == 1)
        {
            return data.Value + talentValue;
        }

        if (level == 0)
        {
            level = Level;
        }

        if (level == 0)
        {
            return 0;
        }

        return data.GetValue(level - 1) + talentValue;
    }

    public virtual bool IsChanneling
    {
        get
        {
            return Base.IsChanneling;
        }
    }

    public AbilitySlot AbilitySlot
    {
        get
        {
            return Base.AbilitySlot;
        }
    }

    public float GetCastPoint(uint index)
    {
        return Base.AbilityData.GetCastPoint(index);
    }

    public float GetChannelTime(uint index)
    {
        return Base.AbilityData.GetChannelMaximumTime(index);
    }

    public float GetCooldown(uint index)
    {
        return Base.AbilityData.GetCooldownLength(index);
    }

    public int GetDamage(uint index)
    {
        return Base.AbilityData.GetDamage(index);
    }

    public int GetManaCost(uint index)
    {
        return Base.AbilityData.GetManaCost(index);
    }

    public int GetRange(uint index)
    {
        return Base.AbilityData.GetCastRange(index);
    }

    public float GetDuration(uint index)
    {
        return Base.AbilityData.GetDuration(index);
    }

    public virtual bool UseAbility(Tree target, bool queued, bool bypassQueue)
    {
        return Base.Cast(target, queued, bypassQueue);
    }

    public virtual bool UseAbility(Tree target, bool queued)
    {
        return Base.Cast(target, queued);
    }

    public virtual bool UseAbility(Tree target)
    {
        return Base.Cast(target);
    }

    public virtual bool UseAbility(Rune target, bool queued, bool bypassQueue)
    {
        return Base.Cast(target, queued, bypassQueue);
    }

    public virtual bool UseAbility(Rune target, bool queued)
    {
        return Base.Cast(target, queued);
    }

    public virtual bool UseAbility(Rune target)
    {
        return Base.Cast(target);
    }

    public virtual bool UseAbility(CUnit target, bool queued, bool bypassQueue)
    {
        return Base.Cast(target.Base, queued, bypassQueue);
    }

    public virtual bool UseAbility(CUnit target, bool queued)
    {
        return Base.Cast(target.Base, queued);
    }

    public virtual bool UseAbility(CUnit target)
    {
        return Base.Cast(target.Base);
    }

    public virtual bool UseAbility(Vector3 position, bool queued, bool bypassQueue)
    {
        return Base.Cast(position, queued, bypassQueue);
    }

    public virtual bool UseAbility(Vector3 position, bool queued)
    {
        return Base.Cast(position, queued);
    }

    public virtual bool UseAbility(Vector3 position)
    {
        return Base.Cast(position);
    }

    public virtual bool UseAbility(bool queued, bool bypassQueue)
    {
        return Base.Cast(queued, bypassQueue);
    }

    public virtual bool UseAbility(bool queued)
    {
        return Base.Cast(queued);
    }

    public virtual bool UseAbility()
    {
        return Base.Cast();
    }

    public bool ToggleAbility(bool queued, bool bypassQueue)
    {
        return Base.CastToggle(queued, bypassQueue);
    }

    public bool ToggleAbility(bool queued)
    {
        return Base.CastToggle(queued);
    }

    public bool ToggleAbility()
    {
        return Base.CastToggle();
    }

    public bool ToggleAutocastAbility()
    {
        return Base.CastToggleAutocast();
    }

    public bool UpgradeAbility()
    {
        return Base.Upgrade();
    }

    public bool Announce()
    {
        return Base.Announce();
    }

    public static implicit operator Ability(CAbility ability)
    {
        return ability.Base;
    }

    public static explicit operator CAbility(Entity entity)
    {
        return AbilityManager.GetAbilityByEntity(entity);
    }
}