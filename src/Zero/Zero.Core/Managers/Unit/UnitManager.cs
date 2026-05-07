namespace Divine.Core.Managers.Unit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Ability;
using Divine.Core.Managers.Unit.Delegates;
using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Game;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Update;
using Divine.Common.Log;

using DivineAbility = Entity.Entities.Abilities.Ability;
using DivineUnit = Entity.Entities.Units.Unit;

public sealed class UnitManager
{
    private static readonly Dictionary<uint, CUnit> units = new Dictionary<uint, CUnit>();

    private static readonly AbilityManager AbilityManager;

    static UnitManager()
    {
        foreach (var unit in EntityManager.GetEntities<DivineUnit>())
        {
            if (!unit.IsValid)
            {
                continue;
            }

            var classUnit = AddUnit(unit);

            foreach (var modifier in classUnit.Modifiers)
            {
                if (!modifierActions.TryGetValue(modifier.Name, out var action))
                {
                    continue;
                }

                action.Invoke(classUnit, true);
            }
        }

        Owner = units[EntityManager.LocalHero.Handle] as CHero;

        AbilityManager = new AbilityManager();

        EntityManager.EntityAdded += OnEntityAdded;
        EntityManager.EntityRemoved += OnEntityRemoved;

        Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;

        Entity.AnimationChanged += OnAnimationChanged;

        OrderManager.OrderAdding += OnOrderAdding;

        ModifierManager.ModifierAdded += OnModifierAdded;
        ModifierManager.ModifierRemoved += OnModifierRemoved;
    }

    private static UnitEventHandler unitAdded;

    public static event UnitEventHandler UnitAdded
    {
        add
        {
            foreach (var unit in Units)
            {
                try
                {
                    value.Invoke(unit);
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }
            }

            unitAdded += value;
        }

        remove
        {
            unitAdded -= value;
        }
    }

    public static event UnitEventHandler UnitRemoved;

    public static event UnitEventHandler AttackStart;

    public static event UnitEventHandler AttackEnd;

    private static void OnEntityAdded(EntityAddedEventArgs e)
    {
        var entity = e.Entity;
        if (entity is DivineAbility ability)
        {
            AbilityManager.AbilityAdded(ability);
            return;
        }

        var unit = entity as DivineUnit;
        if (unit == null)
        {
            return;
        }

        AddUnit(unit);
    }

    private static void OnEntityRemoved(EntityRemovedEventArgs e)
    {
        var entity = e.Entity;
        if (entity is DivineAbility ability)
        {
            AbilityManager.AbilityRemoved(ability);
            return;
        }

        var unit = entity as DivineUnit;
        if (unit == null)
        {
            return;
        }

        UnitRemove(unit.Handle);
    }

    private static CUnit AddUnit(DivineUnit unit)
    {
        var handle = unit.Handle;
        if (units.TryGetValue(handle, out var classUnit))
        {
            return classUnit;
        }

        classUnit = ClassUnit(unit);

        AddOwner(classUnit);

        units[handle] = classUnit;
        unitAdded?.Invoke(classUnit);

        return classUnit;
    }

    private static void AddOwner(CUnit classUnit)
    {
        var owner = classUnit.Base.Owner as DivineUnit;
        if (owner != null)
        {
            classUnit.Owner = AddUnit(owner);
        }
        else
        {
            classUnit.Owner = classUnit;
        }
    }

    private static void UnitRemove(uint handle)
    {
        if (!units.TryGetValue(handle, out var classUnit))
        {
            return;
        }

        classUnit.Dispose();

        units.Remove(handle);
        UnitRemoved?.Invoke(classUnit);
    }

    private static CUnit ClassUnit(DivineUnit unit)
    {
        var hero = unit as Hero;
        if (hero != null)
        {
            if (hero.IsIllusion)
            {
                return new CIllusion(hero);
            }

            return new CHero(hero);
        }

        var creep = unit as Creep;
        if (creep != null)
        {
            if (creep.IsNeutral)
            {
                return new CNeutral(creep);
            }

            return new CCreep(creep);
        }

        var courier = unit as Courier;
        if (courier != null)
        {
            return new CCourier(courier);
        }

        var building = unit as Building;
        if (building != null)
        {
            var tower = unit as Tower;
            if (tower != null)
            {
                return new CTower(tower);
            }

            return new CBuilding(building);
        }

        return new CUnit(unit);
    }

    private static readonly HashSet<NetworkActivity> AttackActivities = new HashSet<NetworkActivity>
    {
        NetworkActivity.Attack,
        NetworkActivity.Attack2,
        NetworkActivity.AttackEvent,
        NetworkActivity.AttackEventBash
    };

    private static readonly HashSet<ClassId> BrokenAttackUnits = new HashSet<ClassId>
    {
        ClassId.CDOTA_Unit_Hero_Visage,
        ClassId.CDOTA_Unit_VisageFamiliar
    };

    private static void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "m_NetworkActivity":
            {
                var newValue = e.NewValue.GetInt32();
                if (newValue == e.OldValue.GetInt32())
                {
                    break;
                }

                UpdateManager.BeginInvoke(() =>
                {
                    if (!units.TryGetValue(sender.Handle, out var unit) || BrokenAttackUnits.Contains(unit.ClassId) || unit is CTower)
                    {
                        return;
                    }

                    if (!unit.IsAttacking)
                    {
                        if (!AttackActivities.Contains((NetworkActivity)newValue))
                        {
                            return;
                        }

                        unit.LastAttackTime = GameManager.RawGameTime;

                        UpdateManager.BeginInvoke(() =>
                        {
                            unit.AttackTarget = Units.Where(x => x.IsAllEnemy(unit) && x.IsAlive && unit.IsInAttackRange(x, 50)).OrderBy(x => unit.GetRotationAngle(x, true)).FirstOrDefault(x => unit.GetRotationAngle(x, true) < 0.25f);
                            unit.IsAttacking = true;

                            AttackStart?.Invoke(unit);

                        });
                    }
                    else
                    {
                        if (!unit.CanMove(GameManager.RawGameTime + 0.05f))
                        {
                            unit.LastAttackTime = 0;
                        }

                        unit.AttackTarget = null;
                        unit.IsAttacking = false;

                        AttackEnd?.Invoke(unit);
                    }
                });

                break;
            }
            case "m_iTeamNum":
            {
                if (e.NewValue.GetInt32() == e.OldValue.GetInt32())
                {
                    break;
                }

                UpdateManager.BeginInvoke(() =>
                {
                    if (sender.Name != "npc_dota_watch_tower")
                    {
                        return;
                    }

                    UpdateManager.BeginInvoke(100, () =>
                    {
                        UnitRemove(sender.Handle);
                        AddUnit((DivineUnit)sender);
                    });
                });

                break;
            }
            case "m_iIsControllableByPlayer64":
            {
                if (e.NewValue.GetInt64() == e.OldValue.GetInt64())
                {
                    break;
                }

                UpdateManager.BeginInvoke(() =>
                {
                    var handle = sender.Handle;
                    if (handle == Owner.Handle)
                    {
                        return;
                    }

                    if (!units.ContainsKey(handle))
                    {
                        return;
                    }

                    UpdateManager.BeginInvoke(100, () =>
                    {
                        UnitRemove(handle);
                        AddUnit((DivineUnit)sender);
                    });
                });

                break;
            }
        }
    }

    private static void OnAnimationChanged(Entity sender, AnimationChangedEventArgs e)
    {
        if (sender is not Tower || !units.TryGetValue(sender.Handle, out var unit))
        {
            return;
        }

        if (e.Name.Contains("attack"))
        {
            unit.IsAttacking = true;
            AttackStart?.Invoke(unit);

            unit.IsAttacking = false;
            AttackEnd?.Invoke(unit);
        }
    }

    private static CUnit hurricanePikeTarget;

    private static void OnOrderAdding(OrderAddingEventArgs e)
    {
        var order = e.Order;
        if (order.IsQueued || !e.Process)
        {
            return;
        }

        switch (order.Type)
        {
            case OrderType.AttackTarget:
            {
                foreach (var entity in order.Units)
                {
                    if (!units.TryGetValue(entity.Handle, out var unit) || !BrokenAttackUnits.Contains(unit.ClassId))
                    {
                        continue;
                    }

                    if (unit.IsAttacking)
                    {
                        unit.LastAttackTime = 0;
                        unit.AttackTarget = null;
                        unit.IsAttacking = false;

                        AttackEnd?.Invoke(unit);
                    }

                    var attackTarget = GetUnitByHandle(order.Target.Handle);
                    if (attackTarget == null || unit.IsAlly(attackTarget))
                    {
                        continue;
                    }

                    unit.AttackTarget = attackTarget;
                    unit.LastAttackTime = GameManager.RawGameTime + unit.GetTurnTime(attackTarget.Position) + 0.05f;
                    unit.IsAttacking = true;

                    AttackStart?.Invoke(unit);
                }

                break;
            }
            case OrderType.MovePosition:
            case OrderType.MoveTarget:
            case OrderType.Hold:
            {
                foreach (var entity in order.Units)
                {
                    if (!units.TryGetValue(entity.Handle, out var unit) || !BrokenAttackUnits.Contains(unit.ClassId) || !unit.IsAttacking)
                    {
                        continue;
                    }

                    if (!unit.CanMove(GameManager.RawGameTime + 0.05f))
                    {
                        unit.LastAttackTime = 0;
                    }

                    unit.AttackTarget = null;
                    unit.IsAttacking = false;

                    AttackEnd?.Invoke(unit);
                }

                break;
            }
            case OrderType.CastToggleAutocast:
            {
                if (e.IsCustom)
                {
                    break;
                }

                var ability = order.Ability;
                var handle = ability.Owner.Handle;

                if (!units.TryGetValue(handle, out var unit))
                {
                    break;
                }

                UpdateManager.BeginInvoke(100, () =>
                {
                    unit.IsToggleAutoCast = ability.IsAutoCastEnabled;
                });

                break;
            }
            case OrderType.CastToggle:
            {
                if (order.Ability.Id != AbilityId.item_hurricane_pike)
                {
                    break;
                }

                foreach (var entity in order.Units)
                {
                    if (!units.TryGetValue(entity.Handle, out var unit))
                    {
                        continue;
                    }

                    hurricanePikeTarget = GetUnitByHandle(order.Target.Handle);
                }

                break;
            }
        }
    }

    private static readonly Dictionary<string, Action<CUnit, bool>> modifierActions = new Dictionary<string, Action<CUnit, bool>>
    {
        { "modifier_teleporting", (x, value) => x.IsTeleporting = value },
        { "modifier_fountain_aura_buff", (x, value) => x.IsFountainAura = value },
        { "modifier_treant_natures_guise_invis", (x, value) => x.CanUseAbilitiesInInvisibility = value },
        { "modifier_riki_permanent_invisibility", (x, value) => x.CanUseAbilitiesInInvisibility = value },
        { "modifier_ice_blast", (x, value) => x.CanBeHealed = !value },
        { "modifier_item_aegis", (x, value) => x.HasAegis = value },
        { "modifier_necrolyte_sadist_active", (x, value) => x.IsEthereal = value },
        { "modifier_pugna_decrepify", (x, value) => x.IsEthereal = value },
        { "modifier_item_ethereal_blade_ethereal", (x, value) => x.IsEthereal = value },
        { "modifier_ghost_state", (x, value) => x.IsEthereal = value },
        { "modifier_item_lotus_orb_active", (x, value) => x.IsLotusProtected = value },
        { "modifier_item_sphere_target", (x, value) => x.IsLinkensTargetProtected = value },
        { "modifier_item_blade_mail_reflect", (x, value) => x.IsReflectingDamage = value },
        { "modifier_item_ultimate_scepter", (x, value) => x.HasAghanimsScepter = value },
        { "modifier_item_ultimate_scepter_consumed", (x, value) => x.HasAghanimsScepter = value },
        { "modifier_item_ultimate_scepter_consumed_alchemist", (x, value) => x.HasAghanimsScepter = value },
        { "modifier_item_invisibility_edge_windwalk", (x, value) => x.IsShadowBlade = value },
        { "modifier_item_silver_edge_windwalk", (x, value) => x.IsSilverEdge = value },
        { "modifier_item_hurricane_pike_range", (x, value) => x.HurricanePikeTarget =  value ? hurricanePikeTarget : null},
        { "modifier_windrunner_windrun_invis", (x, value) => x.InvisibleFadeTime(300, value) }, //TODO
        { "modifier_clinkz_wind_walk", (x, value) => x.InvisibleFadeTime(700, value) }, //TODO
        { "modifier_bounty_hunter_wind_walk", (x, value) => x.InvisibleFadeTime((x.GetAbilityById(AbilityId.bounty_hunter_wind_walk).GetAbilitySpecialData("fade_time") * 1000) + 100, value) }, //TODO
        { "modifier_weaver_shukuchi", (x, value) => x.InvisibleFadeTime(350, value) }, //TODO
        { "modifier_wisp_tether_scepter", (x, value) => x.HasAghanimsScepter = value },
        { "modifier_slark_dark_pact", (x, value) => x.IsDarkPactProtected = value },
        { "modifier_bloodseeker_rupture", (x, value) => x.IsRuptured = value },
        { "modifier_spirit_breaker_charge_of_darkness", (x, value) => x.IsCharging = value },
        { "modifier_dragon_knight_dragon_form", (x, value) => x.IsRanged = value || x.Base.IsRanged },
        { "modifier_terrorblade_metamorphosis", (x, value) => x.IsRanged = value || x.Base.IsRanged },
        { "modifier_troll_warlord_berserkers_rage", (x, value) => x.IsRanged = !value || x.Base.IsRanged },
        { "modifier_lone_druid_true_form", (x, value) => x.IsRanged = !value || x.Base.IsRanged },

        { "modifier_medusa_stone_gaze_slow", (x, value) => x.TurnRate = value ? x.TurnRate * 0.65f : -1f },
        { "modifier_batrider_sticky_napalm", (x, value) => x.TurnRate = value ? x.TurnRate * 0.3f : -1f },
        //{ "modifier_slark_shadow_dance_visual", (x, value) => x}, TODO
        //{ "modifier_morphling_replicate", (x, value) => x }, TODO
        //{ "modifier_morphling_replicate_manager", (x, value) => x }, TODO
        //{ "modifier_alchemist_chemical_rage", (x, value) => x } TODO
    };

    private static void OnModifierAdded(ModifierAddedEventArgs e)
{
        if (!modifierActions.TryGetValue(e.Modifier.Name, out var action) || !units.TryGetValue(e.Modifier.Owner.Handle, out var unit))
        {
            return;
        }

        action.Invoke(unit, true);
    }

    private static void OnModifierRemoved(ModifierRemovedEventArgs e)
    {
        if (!modifierActions.TryGetValue(e.Modifier.Name, out var action) || !units.TryGetValue(e.Modifier.Owner.Handle, out var unit))
        {
            return;
        }

        action.Invoke(unit, false);
    }

    public static CUnit GetUnitByHandle(uint handle)
    {
        if (units.TryGetValue(handle, out var unit))
        {
            return unit;
        }

        return null;
    }

    public static CUnit GetUnitByEntity(Entity entity)
    {
        if (entity == null)
        {
            return null;
        }

        if (units.TryGetValue(entity.Handle, out var unit))
        {
            return unit;
        }

        return null;
    }

    public static CHero Owner { get; }

    public static IEnumerable<CUnit> Units
    {
        get
        {
            return units.Values.Where(x => x.IsValid);
        }
    }
}
