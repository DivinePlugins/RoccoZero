namespace Divine.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Entities.Abilities.Items;
using Divine.Core.Entities.Utilities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Orbwalker;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.PhysicalItems;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Runes;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Game;
using Divine.Helpers;
using Divine.Modifier.Modifiers;
using Divine.Numerics;

public class CUnit : CEntity
{
    internal CUnit(Unit unit)
        : base(unit)
    {
        Base = unit;

        IsHero = unit is Hero;
        EnemyTeam = GetEnemyTeam();

        IsPhantom = unit.IsPhantom;
        UnitType = unit.UnitType;
        IsAncient = unit.IsAncient;
        IsNeutral = unit.IsNeutral;
        IsControllable = unit.IsControllable;
        BaseAttackTime = unit.BaseAttackTime;
        HealthBarOffset = unit.HealthBarOffset;
        IsIllusion = unit.IsIllusion;
        HullRadius = unit.HullRadius;
        HasInventory = unit.HasInventory;
        AttackCapability = unit.AttackCapability;
        MoveCapability = unit.MoveCapability;
        IsMelee = unit.IsMelee;
        IsRanged = unit.IsRanged;
    }

    internal override void Dispose()
    {
        CachedInfo?.Dispose();
    }

    public new Unit Base { get; }

    public bool IsHero { get; }

    private Team GetEnemyTeam()
    {
        switch (Team)
        {
            case Team.Radiant:
                {
                    return Team.Dire;
                }

            case Team.Dire:
                {
                    return Team.Radiant;
                }
        }

        return Team.None;
    }

    public Team EnemyTeam { get; }

    public bool IsAlly()
    {
        return Team == UnitManager.Owner.Team;
    }

    public bool IsAlly(CUnit target)
    {
        return Team == target.Team;
    }

    public bool IsEnemy()
    {
        return Team == UnitManager.Owner.EnemyTeam;
    }

    public bool IsEnemy(CUnit target)
    {
        return Team == target.EnemyTeam;
    }

    public bool IsAllEnemy()
    {
        return IsEnemy() || Team == Team.Neutral;
    }

    public bool IsAllEnemy(CUnit target)
    {
        return IsEnemy(target) || Team == Team.Neutral;
    }

    private List<CAbility> spells = new List<CAbility>();

    private IEnumerable<CAbility> ultimates = new List<CAbility>();

    private List<CItem> items = new List<CItem>();

    public uint NetWorthItems { get; private set; }

    internal IEnumerable<CAbility> GetSpells()
    {
        return spells;
    }

    internal IEnumerable<CItem> GetItems()
    {
        return items;
    }

    internal IEnumerable<CAbility> GetAbilities()
    {
        return spells.Concat(items);
    }

    internal void RefreshSpells(List<CAbility> refreshSpells, out IEnumerable<CAbility> oldISpells)
    {
        oldISpells = spells;
        spells = refreshSpells;

        ultimates = refreshSpells.Where(x => x.AbilityType == AbilityType.Ultimate && !x.IsHidden);
    }

    internal void RefreshItems(List<CItem> refreshItems, out IEnumerable<CItem> oldItems)
    {
        oldItems = items;
        items = refreshItems;

        NetWorthItems = 0;

        foreach (var item in refreshItems)
        {
            NetWorthItems += item.Cost;
        }
    }

    public IEnumerable<CAbility> Spells
    {
        get
        {
            return spells.Where(x => x.IsValid);
        }
    }

    public IEnumerable<CAbility> Ultimates
    {
        get
        {
            return ultimates.Where(x => x.IsValid);
        }
    }

    public IEnumerable<CItem> Items
    {
        get
        {
            return items.Where(x => x.IsValid);
        }
    }

    public IEnumerable<CAbility> Abilities
    {
        get
        {
            return Spells.Concat(Items);
        }
    }

    public TownPortalScroll TownPortalScroll { get; internal set; }

    public Vector2 HealthBarPosition
    {
        get
        {
            return CachedInfo.HealthBarPosition;
        }
    }

    public Vector3 Position
    {
        get
        {
            return Base.Position;
        }
    }

    public bool IsVisible
    {
        get
        {
            return Base.IsVisible;
        }
    }

    public bool IsVisibleByTime(float time)
    {
        return GameManager.RawGameTime - LastVisibleTime < time;
    }

    public float LastVisibleTime
    {
        get
        {
            return CachedInfo.LastVisibleTime;
        }
    }

    public Vector2 ScreenPosition
    {
        get
        {
            return CachedInfo.ScreenPosition;
        }
    }

    private CachedInfo cachedInfo;

    public CachedInfo CachedInfo
    {
        get
        {
            if (cachedInfo == null)
            {
                cachedInfo = new CachedInfo(this);
            }

            return cachedInfo;
        }

        private protected set
        {
            cachedInfo = value;
        }
    }

    public CAbility GetAbilityById(AbilityId abilityId)
    {
        return spells.Find(x => x.Id == abilityId);
    }

    public T GetAbility<T>()
        where T : CAbility
    {
        return (T)Spells.FirstOrDefault(x => x is T);
    }

    public Vector3 InFront(float range, bool rotationDifference = true)
    {
        var rotationRad = RotationRad + (rotationDifference ? MathUtil.DegreesToRadians(RotationDifference) : 0f);
        return Position + new Vector3((float)Math.Cos(rotationRad), (float)Math.Sin(rotationRad), 0f) * range;
    }

    internal CUnit HurricanePikeTarget { get; set; }

    private float attackAnimationPoint = -1f;

    private float AttackAnimationPoint
    {
        get
        {
            if (attackAnimationPoint < 0)
            {
                try
                {
                    attackAnimationPoint = ((Base is Hero) ? Hero.GetKeyValueByName(Base.Name) : Unit.GetKeyValueByName(Base.Name)).GetSubKey("AttackAnimationPoint").GetSingle();
                }
                catch
                {
                    attackAnimationPoint = 0;
                }
            }

            return attackAnimationPoint;
        }
    }

    public float GetAttackSpeed(CUnit target)
    {
        var speed = 0f;
        if (HurricanePikeTarget?.Handle == target.Handle)
        {
            speed += 100;
        }

        return AttackSpeedValue + speed;
    }

    public float AttackPoint
    {
        get
        {
            return AttackAnimationPoint / (1 + ((AttackSpeedValue - 100) / 100));
        }
    }

    public float GetAttackPoint(CUnit target)
    {
        return AttackAnimationPoint / (1 + ((GetAttackSpeed(target) - 100) / 100));
    }

    public float GetSecondsPerAttack(CUnit target)
    {
        return BaseAttackTime / GetAttackSpeed(target) * 100;
    }

    public float GetRotationAngle(CUnit target, bool rotationDifference = false)
    {
        return GetRotationAngle(target.Position, rotationDifference);
    }

    public float GetRotationAngle(Vector3 position, bool rotationDifference = false)
    {
        var angle = Math.Abs(Math.Atan2(position.Y - Position.Y, position.X - Position.X) - (rotationDifference ? MathUtil.DegreesToRadians(RotationDifference + NetworkRotation) : NetworkRotationRad));
        if (angle > Math.PI)
        {
            angle = Math.Abs((Math.PI * 2) - angle);
        }

        return (float)angle;
    }

    public float GetTurnTime(CUnit target)
    {
        return GetTurnTime(target.Position);
    }

    public float GetTurnTime(Vector3 position)
    {
        return GetTurnTime(GetRotationAngle(position));
    }

    public float GetTurnTime(float angle)
    {
        if (angle <= 0.2f)
        {
            return 0;
        }

        return 0.03f / TurnRate * angle;
    }

    private float turnRate = -1f;

    public float TurnRate
    {
        get
        {
            if (turnRate < 0)
            {
                try
                {
                    turnRate = ((Base is Hero) ? Hero.GetKeyValueByName(Base.Name) : Unit.GetKeyValueByName(Base.Name)).GetSubKey("MovementTurnRate").GetSingle();
                }
                catch
                {
                    turnRate = 0.5f;
                }
            }

            return turnRate;
        }

        internal set
        {
            turnRate = value;
        }
    }

    internal bool IsToggleAutoCast { get; set; }

    public virtual CUnit AttackTarget { get; internal set; }

    public bool IsAttacking { get; internal set; }

    internal float LastAttackTime { get; set; }

    internal EchoSabre EchoSabre { get; set; }

    public bool CanEchoStrike()
    {
        return EchoSabre != null && Math.Abs(EchoSabre.Cooldown) < 0.15f;
    }

    internal bool CanAttack(CUnit target, float time)
    {
        return time + GetTurnTime(target) - LastAttackTime > GetSecondsPerAttack(target) && !this.IsDisarmed() && !target.IsInvulnerable() && !target.IsAttackImmune() && target.IsVisible && !CanEchoStrike();
    }

    public bool CanAttack(CUnit target)
    {
        return CanAttack(target, GameManager.RawGameTime) && IsAlive && !this.IsChanneling() && !this.IsStunned() && !this.IsInvulnerable();
    }

    public bool CanAttack(CUnit target, CAbility ability)
    {
        return CanAttack(target) && ability.IsReady && !this.IsMuted() && !this.IsSilenced() && !target.IsAlly(this);
    }

    internal bool CanMove(float time)
    {
        return time - 0.1f + (GameManager.Ping / 2000f) - LastAttackTime > AttackPoint;
    }

    public bool CanMove()
    {
        return CanMove(GameManager.RawGameTime) && !this.IsChanneling() && !this.IsStunned() && !this.IsInvulnerable();
    }

    private OrbwalkerManager orbwalker;

    public OrbwalkerManager Orbwalker
    {
        get
        {
            if (orbwalker == null)
            {
                orbwalker = new OrbwalkerManager(this);
            }

            return orbwalker;
        }
    }

    public bool IsTeleporting { get; internal set; }

    public bool IsFountainAura { get; internal set; }

    public bool CanUseAbilitiesInInvisibility { get; internal set; }

    public bool CanBeHealed { get; internal set; }

    public bool HasAegis { get; internal set; }

    public bool IsEthereal { get; internal set; }

    public bool IsLotusProtected { get; internal set; }

    internal bool IsLinkensTargetProtected { get; set; }

    internal LinkensSphere LinkensSphere { get; set; }

    public virtual bool IsLinkensProtected
    {
        get
        {
            var linkensSphere = LinkensSphere;
            if (linkensSphere == null)
            {
                return false;
            }

            return linkensSphere.IsReady;
        }
    }

    public bool IsReflectingDamage { get; internal set; }

    public bool HasAghanimsScepter { get; internal set; }

    private bool isShadowBlade;

    public bool IsShadowBlade
    {
        get
        {
            return isShadowBlade;
        }

        internal set
        {
            isShadowBlade = value;

            if (!value)
            {
                return;
            }

            InvisibleFadeTime(400);
        }
    }

    private bool isSilverEdge;

    public bool IsSilverEdge
    {
        get
        {
            return isSilverEdge;
        }

        internal set
        {
            isSilverEdge = value;

            if (!value)
            {
                return;
            }

            InvisibleFadeTime(400);
        }
    }

    internal void InvisibleFadeTime(float milliseconds, bool isAdded = true)
    {
        if (!isAdded)
        {
            return;
        }

        if (invisibleSleeper == null)
        {
            invisibleSleeper = new ZSleeper();
        }

        invisibleSleeper.Sleep(milliseconds);
    }

    private ZSleeper invisibleSleeper;

    public bool IsUnsafeInvisible
    {
        get
        {
            if (invisibleSleeper == null)
            {
                return false;
            }

            return invisibleSleeper.Sleeping;
        }
    }

    public bool IsDarkPactProtected { get; internal set; }

    public bool IsRuptured { get; internal set; }

    public bool IsCharging { get; internal set; }

    public bool IsPhantom { get; }

    public int UnitType { get; }

    public uint Level
    {
        get
        {
            return Base.Level;
        }
    }

    public bool IsAncient { get; }

    public bool HasStolenScepter
    {
        get
        {
            return Base.HasStolenScepter;
        }
    }

    public bool IsNeutral { get; }

    public bool HasSharedAbilities
    {
        get
        {
            return Base.HasSharedAbilities;
        }
    }

    public bool IsSummoned
    {
        get
        {
            return Base.IsSummoned;
        }
    }

    public bool IsDominatable
    {
        get
        {
            return Base.IsDominatable;
        }
    }

    public bool HasUpgradeableAbilities
    {
        get
        {
            return Base.HasUpgradeableAbilities;
        }
    }

    public float HealthRegeneration
    {
        get
        {
            return Base.HealthRegeneration;
        }
    }

    public bool IsControllable { get; }

    public uint AttackRange
    {
        get
        {
            return Base.AttackRange;
        }
    }

    public float MovementSpeed
    {
        get
        {
            return Base.MovementSpeed;
        }
    }

    public float BaseMovementSpeed
    {
        get
        {
            return Base.BaseMovementSpeed;
        }
    }

    public float BaseAttackTime { get; }

    public int HealthBarOffset { get; }

    public float Mana
    {
        get
        {
            return Base.Mana;
        }
    }

    public float MaximumMana
    {
        get
        {
            return Base.MaximumMana;
        }
    }

    public float ManaRegeneration
    {
        get
        {
            return Base.ManaRegeneration;
        }
    }

    public uint BKBChargesUsed
    {
        get
        {
            return Base.BKBChargesUsed;
        }
    }

    public virtual bool IsIllusion { get; }

    public float InvisiblityLevel
    {
        get
        {
            return Base.InvisiblityLevel;
        }
    }

    public float HullRadius { get; }

    public float CollisionPadding
    {
        get
        {
            return Base.CollisionPadding;
        }
    }

    public float RingRadius
    {
        get
        {
            return Base.RingRadius;
        }
    }

    public float ProjectileCollisionSize
    {
        get
        {
            return Base.ProjectileCollisionSize;
        }
    }

    public float TauntCooldown
    {
        get
        {
            return Base.TauntCooldown;
        }
    }

    public ShopType ActiveShop
    {
        get
        {
            return Base.ActiveShop;
        }
    }

    public uint DayVision
    {
        get
        {
            return Base.DayVision;
        }
    }

    public uint NightVision
    {
        get
        {
            return Base.NightVision;
        }
    }

    public int MinimumDamage
    {
        get
        {
            return Base.MinimumDamage;
        }
    }

    public int MaximumDamage
    {
        get
        {
            return Base.MaximumDamage;
        }
    }

    public int BonusDamage
    {
        get
        {
            return Base.BonusDamage;
        }
    }

    public UnitState UnitState
    {
        get
        {
            return Base.UnitState;
        }
    }

    public UnitState DebuffState
    {
        get
        {
            return Base.DebuffState;
        }
    }

    public bool HasInventory { get; }

    public AttackCapability AttackCapability { get; }

    public MoveCapability MoveCapability { get; }

    public virtual string MinimapIcon
    {
        get
        {
            return Base.MinimapIcon;
        }
    }

    public virtual float MinimapIconSize
    {
        get
        {
            return Base.MinimapIconSize;
        }
    }

    public float DeathTime
    {
        get
        {
            return Base.DeathTime;
        }
    }

    public bool HasBaseStatsChanged
    {
        get
        {
            return Base.HasBaseStatsChanged;
        }
    }

    public float MagicDamageResist
    {
        get
        {
            return Base.MagicalDamageResistance;
        }
    }

    public bool IsWaitingToSpawn
    {
        get
        {
            return Base.IsWaitingToSpawn;
        }
    }

    public ulong TotalDamageTaken
    {
        get
        {
            return Base.TotalDamageTaken;
        }
    }

    public bool HasArcana
    {
        get
        {
            return Base.HasArcana;
        }
    }

    public NetworkActivity NetworkActivity
    {
        get
        {
            return Base.NetworkActivity;
        }
    }

    public bool IsMoving
    {
        get
        {
            return Base.IsMoving;
        }
    }

    public ShopFlags AvailableShops
    {
        get
        {
            return Base.AvailableShops;
        }
    }

    public float RotationDifference
    {
        get
        {
            return Base.RotationDifference;
        }
    }

    public AttackDamageType AttackDamageType
    {
        get
        {
            return Base.AttackDamageType;
        }
    }

    public ArmorType ArmorType
    {
        get
        {
            return Base.ArmorType;
        }
    }

    public int DamageAverage
    {
        get
        {
            return Base.DamageAverage;
        }
    }

    public float DamageResist
    {
        get
        {
            return Base.PhysicalDamageResistance;
        }
    }

    public Inventory Inventory
    {
        get
        {
            return Base.Inventory;
        }
    }

    public bool IsMelee { get; }

    public bool IsRanged { get; internal set; }

    public bool IsSpawned
    {

        get
        {
            return Base.IsSpawned;
        }
    }

    public bool IsVisibleToEnemies
    {
        get
        {
            return Base.IsVisibleToEnemies;
        }
    }

    public IEnumerable<Modifier> Modifiers
    {
        get
        {
            return Base.Modifiers;
        }
    }

    public Spellbook Spellbook
    {
        get
        {
            return Base.Spellbook;
        }
    }

    public float AttackSpeedValue
    {
        get
        {
            return Base.AttackSpeed;
        }
    }

    public float AttacksPerSecond
    {
        get
        {
            return Base.AttacksPerSecond;
        }
    }

    public float SecondsPerAttack
    {
        get
        {
            return Base.SecondsPerAttack;
        }
    }

    public float Armor
    {
        get
        {
            return Base.Armor;
        }
    }

    public float BaseArmor
    {
        get
        {
            return Base.BaseArmor;
        }
    }

    public bool IsControllableByPlayer(Player player)
    {
        return Base.IsControllableByPlayer(player);
    }

    public bool Move(Vector3 position, bool queued, bool bypassQueue)
    {
        return Base.Move(position, queued, bypassQueue);
    }

    public bool Move(Vector3 position, bool queued)
    {
        return Base.Move(position, queued);
    }

    public bool Move(Vector3 position)
    {
        return Base.Move(position);
    }

    public bool MoveToDirection(Vector3 position, bool queued)
    {
        return Base.MoveToDirection(position, queued);
    }

    public bool MoveToDirection(Vector3 position)
    {
        return Base.MoveToDirection(position);
    }

    public bool Patrol(Vector3 position, bool queued)
    {
        return Base.Patrol(position, queued);
    }

    public bool Patrol(Vector3 position)
    {
        return Base.Patrol(position);
    }

    public bool Attack(PhysicalItem target, bool queued)
    {
        return Base.Attack(target, queued);
    }

    public bool Attack(PhysicalItem target)
    {
        return Base.Attack(target);
    }

    public bool Attack(CUnit target, bool queued)
    {
        return Base.Attack(target.Base, queued);
    }

    public bool Attack(CUnit target)
    {
        return Base.Attack(target.Base);
    }

    public bool Attack(Vector3 position, bool queued)
    {
        return Base.Attack(position, queued);
    }

    public bool Attack(Vector3 position)
    {
        return Base.Attack(position);
    }

    public bool Hold(bool queued)
    {
        return Base.Hold(queued);
    }

    public bool Hold()
    {
        return Base.Hold();
    }

    public bool Stop(bool queued, bool bypassQueue)
    {
        return Base.Stop(queued, bypassQueue);
    }

    public bool Stop(bool queued)
    {
        return Base.Stop(queued);
    }

    public bool Stop()
    {
        return Base.Stop();
    }

    public bool Follow(CUnit target, bool queued)
    {
        return Base.Follow(target.Base, queued);
    }

    public bool Follow(CUnit target)
    {
        return Base.Follow(target.Base);
    }

    public bool PickUpRune(Rune target, bool queued)
    {
        return Base.PickUp(target, queued);
    }

    public bool PickUpRune(Rune target)
    {
        return Base.PickUp(target);
    }

    public bool PickUpItem(PhysicalItem target, bool queued)
    {
        return Base.PickUp(target, queued);
    }

    public bool PickUpItem(PhysicalItem target)
    {
        return Base.PickUp(target);
    }

    public bool DropItem(Item item, Vector3 position, bool queued)
    {
        return Base.Drop(item, position, queued);
    }

    public bool DropItem(Item item, Vector3 position)
    {
        return Base.Drop(item, position);
    }

    public bool GiveItem(Item item, Unit target, bool queued)
    {
        return Base.Give(item, target, queued);
    }

    public bool GiveItem(Item item, Unit target)
    {
        return Base.Give(item, target);
    }

    public static implicit operator Unit(CUnit unit)
    {
        return unit.Base;
    }

    public static explicit operator CUnit(Entity entity)
    {
        return UnitManager.GetUnitByEntity(entity);
    }
}
