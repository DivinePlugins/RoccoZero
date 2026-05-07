namespace Ensage.SDK.Orbwalker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Order;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders.Components;
    using Divine.Update;
    using Divine.Zero.Log;

    using Ensage.SDK.Inventory;
    using Ensage.SDK.Service;

    public sealed class Orbwalker : IOrbwalker
    {
        private readonly HashSet<NetworkActivity> attackActivities = new HashSet<NetworkActivity>
        {
            NetworkActivity.Attack,
            NetworkActivity.Attack2,
            NetworkActivity.AttackEvent
        };

        private readonly HashSet<NetworkActivity> attackCancelActivities = new HashSet<NetworkActivity>
        {
            NetworkActivity.Idle,
            NetworkActivity.IdleRare,
            NetworkActivity.Move
        };

        public Orbwalker(IServiceContext context)
        {
            this.Context = context;
            this.Owner = context.Owner;
        }

        public IServiceContext Context { get; }

        public bool IsActive { get; private set; }

        public Vector3 OrbwalkingPoint { get; set; } = Vector3.Zero;

        private InventoryItem EchoSabre { get; set; }

        private float LastAttackOrderIssuedTime { get; set; }

        private float LastAttackTime { get; set; }

        private float LastMoveOrderIssuedTime { get; set; }

        private Unit Owner { get; }

        private float PingTime
        {
            get
            {
                return GameManager.Ping / 2000f;
            }
        }

        private float TurnEndTime { get; set; }

        public void Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            this.IsActive = true;

            this.Context.Inventory.CollectionChanged += this.OnItemsChanged;

            var hero = this.Owner as Hero;
            if (hero?.HeroId == HeroId.npc_dota_hero_visage)
            {
                //HACK visage workaround
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
            else
            {
                if (hero?.HeroId == HeroId.npc_dota_hero_viper)
                {
                    //HACK viper Q workaround
                    this.attackActivities.Add(NetworkActivity.CastAbilityQ);
                }

                Entity.NetworkPropertyChanged += this.OnNetworkPropertyChanged;
            }
        }

        public bool Attack(Unit unit, float time)
        {
            if ((time - this.LastAttackOrderIssuedTime) < 0.005f)
            {
                return false;
            }

            this.TurnEndTime = this.GetTurnTime(unit, time);

            if (this.Owner.Attack(unit))
            {
                this.LastAttackOrderIssuedTime = time;
                return true;
            }

            return false;
        }

        public bool Attack(Unit unit)
        {
            return this.Attack(unit, GameManager.RawGameTime);
        }

        public bool CanAttack(Unit target, float time)
        {
            return this.Owner.CanAttack() && (this.GetTurnTime(target, time) - this.LastAttackTime) > (1f / this.Owner.AttacksPerSecond);
        }

        public bool CanAttack(Unit target)
        {
            return this.CanAttack(target, GameManager.RawGameTime);
        }

        public bool CanMove(float time)
        {
            return (((time - 0.1f) + this.PingTime) - this.LastAttackTime) > this.Owner.AttackPoint();
        }

        public bool CanMove()
        {
            return this.CanMove(GameManager.RawGameTime);
        }

        public void Deactivate()
        {
            if (!this.IsActive)
            {
                return;
            }

            this.IsActive = false;

            LogManager.Debug($"Deactivate Orbwalker: {this.Owner.GetDisplayName()}");

            Entity.NetworkPropertyChanged -= this.OnNetworkPropertyChanged;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            this.attackActivities.Remove(NetworkActivity.CastAbilityQ);
            this.Context.Inventory.CollectionChanged -= this.OnItemsChanged;
        }

        public float GetTurnTime(Entity unit, float time)
        {
            return time + this.PingTime + this.Owner.TurnTime(unit.Position) + 0.1f;
        }

        public float GetTurnTime(Entity unit)
        {
            return this.GetTurnTime(unit, GameManager.RawGameTime);
        }

        public bool Move(Vector3 position, float time)
        {
            if (this.Owner.Position.Distance(position) < 60f)
            {
                return false;
            }

            if ((time - this.LastMoveOrderIssuedTime) < 0.06f)
            {
                return false;
            }

            if (this.Owner.Move(position))
            {
                this.LastMoveOrderIssuedTime = time;
                return true;
            }

            return false;
        }

        public bool Move(Vector3 position)
        {
            return this.Move(position, GameManager.RawGameTime);
        }

        public bool OrbwalkTo(Unit target)
        {
            var time = GameManager.RawGameTime;

            // turning
            if (this.TurnEndTime > time)
            {
                return false;
            }

            // owner disabled
            if (this.Owner.IsChanneling() || !this.Owner.IsAlive || this.Owner.IsStunned())
            {
                return false;
            }

            var validTarget = target != null;

            // move
            if ((!validTarget || !this.CanAttack(target)) && this.CanMove(time))
            {
                if (this.OrbwalkingPoint != Vector3.Zero)
                {
                    return this.Move(this.OrbwalkingPoint, time);
                }

                return this.Move(GameManager.MousePosition, time);
            }

            // attack
            if (validTarget && this.CanAttack(target))
            {
                return this.Attack(target, time);
            }

            return false;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            var order = e.Order;
            if (order.Type != OrderType.AttackTarget || order.IsQueued || !e.Process || !order.Units.Contains(this.Owner))
            {
                return;
            }

            var target = order.Target as Unit;
            if (target == null || !target.IsValid)
            {
                return;
            }

            if (this.CanMove())
            {
                this.LastAttackTime = this.GetTurnTime(target) - this.PingTime;
            }
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in args.NewItems.OfType<InventoryItem>())
                {
                    if (item.Id == AbilityId.item_echo_sabre)
                    {
                        this.EchoSabre = item;
                    }
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in args.OldItems.OfType<InventoryItem>())
                {
                    if (item.Id == AbilityId.item_echo_sabre)
                    {
                        this.EchoSabre = null;
                    }
                }
            }
        }

        private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("m_networkactivity", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var newNetworkActivity = (NetworkActivity)e.NewValue.GetUInt32();

            UpdateManager.BeginInvoke(() =>
            {
                if (sender != this.Owner)
                {
                    return;
                }

                if (this.attackActivities.Contains(newNetworkActivity))
                {
                    if (this.EchoSabre?.IsValid == true && Math.Abs(this.EchoSabre.Item.Cooldown) < 0.15)
                    {
                        return;
                    }

                    this.LastAttackTime = GameManager.RawGameTime - this.PingTime;
                }
                else if (this.attackCancelActivities.Contains(newNetworkActivity))
                {
                    if (!this.CanMove(GameManager.RawGameTime + 0.05f))
                    {
                        this.LastAttackTime = 0;
                    }
                }
            });
        }
    }
}