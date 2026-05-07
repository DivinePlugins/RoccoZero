using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Extensions;
using Divine.Entity.Entities.Players;
using Divine.Game;
using Divine.Helpers;
using Divine.Numerics;
using Divine.Update;
using Divine.Common.Log;

namespace Divine.Core.Managers.Orbwalker
{
    public sealed class OrbwalkerManager
    {
        private readonly CUnit Unit;

        internal OrbwalkerManager(CUnit unit)
        {
            Unit = unit;
        }

        private bool CanOrbwalk()
        {
            return Unit.IsAlive && !Unit.IsChanneling() && !Unit.IsStunned() && !Unit.IsInvulnerable();
        }

        private static readonly Dictionary<uint, OrbwalkerManager> Orbwalks = new Dictionary<uint, OrbwalkerManager>();

        private static readonly Sleeper orbwalkSleeper = new Sleeper();

        private readonly Sleeper turnEndSleeper = new Sleeper();

        private float orderTime;

        private CUnit Target;

        private OrbSpell OrbSpell;

        private Vector3 Position;

        private bool IsFollow;

        private static bool IsRunning;

        public void OrbwalkTo(CUnit target, OrbSpell orbSpell, Vector3 position, bool isFollow)
        {
            if (orbwalkSleeper.Sleeping || turnEndSleeper.Sleeping || !CanOrbwalk())
            {
                return;
            }

            var handle = Unit.Handle;
            if (!Orbwalks.ContainsKey(handle))
            {
                Target = target;
                OrbSpell = orbSpell;
                Position = position;
                IsFollow = isFollow;

                Orbwalks[handle] = this;
            }

            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    orbwalkSleeper.Sleep(30);

                    var orbwalks = Orbwalks.Values.OrderBy(x => x.orderTime);
                    var orderOrbwalk = orbwalks.FirstOrDefault();

                    var unit = orderOrbwalk.Unit;
                    var orbwalkTarget = orderOrbwalk.Target;

                    var time = GameManager.RawGameTime;
                    if (orderOrbwalk.IsFollow)
                    {
                        Follow(orbwalkTarget, orbwalks, time);
                    }
                    else if (orbwalkTarget != null && unit.CanAttack(orbwalkTarget, time))
                    {
                        var orbwalkOrbSpell = orderOrbwalk.OrbSpell;
                        if (orbwalkOrbSpell != null)
                        {
                            Attack(unit, orbwalkTarget, orbwalkOrbSpell, orderOrbwalk, orbwalks, time);
                        }
                        else
                        {
                            Attack(unit, orbwalkTarget, orderOrbwalk, orbwalks, time);
                        }
                    }
                    else
                    {
                        Move(unit, orbwalkTarget, orderOrbwalk, orbwalks, time);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }

                Orbwalks.Clear();
                IsRunning = false;
            });
        }

        public void OrbwalkTo(CUnit target, OrbSpell orbSpell, Vector3 position)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }
            else if (orbSpell == null)
            {
                throw new OrbwalkerException("OrbSpell is null");
            }
            else if (position == Vector3.Zero)
            {
                throw new OrbwalkerException("Position is zero");
            }

            OrbwalkTo(target, orbSpell, position, false);
        }

        public void OrbwalkTo(CUnit target, OrbSpell orbSpell)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }
            else if (orbSpell == null)
            {
                throw new OrbwalkerException("OrbSpell is null");
            }

            OrbwalkTo(target, orbSpell, GameManager.MousePosition, false);
        }

        public void OrbwalkTo(CUnit target, Vector3 position)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }
            else if (position == Vector3.Zero)
            {
                throw new OrbwalkerException("Position is zero");
            }

            OrbwalkTo(target, null, position, false);
        }

        public void OrbwalkTo(CUnit target)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }

            OrbwalkTo(target, null, GameManager.MousePosition, false);
        }

        public void AttackTo(CUnit target, OrbSpell orbSpell)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }
            else if (orbSpell == null)
            {
                throw new OrbwalkerException("OrbSpell is null");
            }

            OrbwalkTo(target, orbSpell, Vector3.Zero, false);
        }

        public void AttackTo(CUnit target)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }

            OrbwalkTo(target, null, Vector3.Zero, false);
        }

        public void MoveTo(Vector3 position)
        {
            if (position == Vector3.Zero)
            {
                throw new OrbwalkerException("Position is zero");
            }

            OrbwalkTo(null, null, position, false);
        }

        public void MoveToMousePosition()
        {
            OrbwalkTo(null, null, GameManager.MousePosition, false);
        }

        public void FollowTo(CUnit target)
        {
            if (target == null)
            {
                throw new OrbwalkerException("Target is null");
            }

            OrbwalkTo(target, null, Vector3.Zero, true);
        }

        private void Attack(CUnit unit, CUnit target, OrbSpell orbSpell, OrbwalkerManager orderOrbwalk, IEnumerable<OrbwalkerManager> orbwalks, float time)
        {
            var targetPosition = target.Position;
            if (unit.Distance2D(targetPosition) <= unit.AttackRange(target))
            {
                var isHurricanePike = unit.HurricanePikeTarget?.Handle == target.Handle;
                var isReady = orbSpell.IsReady;
                if (isReady && !unit.IsToggleAutoCast)
                {
                    if (isHurricanePike)
                    {
                        orbSpell.Enabled = true;
                    }
                    else
                    {
                        orbSpell.Enabled = false;
                    }
                }

                if (isHurricanePike || !isReady || unit.IsMuted() || unit.IsSilenced() || target.IsAlly(unit))
                {
                    var attacks = orbwalks.Where(x =>
                                                 x.Target == target &&
                                                 x.Unit.Distance2D(targetPosition) <= x.Unit.AttackRange(target) &&
                                                 x.Unit.CanAttack(target, time));

                    Player.Attack(attacks.Select(x => x.Unit.Base), target.Base);

                    foreach (var attack in attacks)
                    {
                        attack.turnEndSleeper.Sleep(((GameManager.Ping / 2000f) + attack.Unit.GetTurnTime(target)) * 1000);
                        attack.orderTime = time;
                    }
                }
                else
                {
                    orbSpell.UseAbility(target);

                    orderOrbwalk.turnEndSleeper.Sleep(((GameManager.Ping / 2000f) + unit.GetTurnTime(target)) * 1000);
                    orderOrbwalk.orderTime = time;
                }
            }
            else if (unit.CanMove(time))
            {
                var moves = orbwalks.Where(x =>
                                           x.Target == target &&
                                           x.Unit.Distance2D(targetPosition) > x.Unit.AttackRange(target) &&
                                           x.Unit.CanMove(time) &&
                                           x.Unit.CanAttack(target, time));

                Player.Move(moves.Select(x => x.Unit.Base), targetPosition);

                foreach (var move in moves)
                {
                    move.orderTime = time;
                }
            }
            else
            {
                orderOrbwalk.orderTime = time;
            }
        }

        private void Attack(CUnit unit, CUnit target, OrbwalkerManager orderOrbwalk, IEnumerable<OrbwalkerManager> orbwalks, float time)
        {
            var targetPosition = target.Position;
            if (unit.Distance2D(targetPosition) <= unit.AttackRange(target))
            {
                var attacks = orbwalks.Where(x =>
                                             x.OrbSpell == null &&
                                             x.Target == target &&
                                             x.Unit.Distance2D(targetPosition) <= x.Unit.AttackRange(target) &&
                                             x.Unit.CanAttack(target, time));

                Player.Attack(attacks.Select(x => x.Unit.Base), target.Base);

                foreach (var attack in attacks)
                {
                    attack.turnEndSleeper.Sleep(((GameManager.Ping / 2000f) + attack.Unit.GetTurnTime(target)) * 1000);
                    attack.orderTime = time;
                }
            }
            else if (unit.CanMove(time))
            {
                var moves = orbwalks.Where(x =>
                                           x.Target == target &&
                                           x.Unit.Distance2D(targetPosition) > x.Unit.AttackRange(target) &&
                                           x.Unit.CanMove(time) &&
                                           x.Unit.CanAttack(target, time));

                Player.Move(moves.Select(x => x.Unit.Base), targetPosition);

                foreach (var move in moves)
                {
                    move.orderTime = time;
                }
            }
            else
            {
                orderOrbwalk.orderTime = time;
            }
        }

        private void Move(CUnit unit, CUnit target, OrbwalkerManager orderOrbwalk, IEnumerable<OrbwalkerManager> orbwalks, float time)
        {
            var movePosition = orderOrbwalk.Position;
            if (!movePosition.IsZero && unit.Distance2D(movePosition) > 30 && unit.CanMove(time))
            {
                var isValidTarget = target != null;
                var moves = orbwalks.Where(x =>
                                           x.Position == movePosition &&
                                           x.Unit.Distance2D(movePosition) > 30 &&
                                           x.Unit.CanMove(time) &&
                                           (!isValidTarget || !x.Unit.CanAttack(target, time)));

                Player.Move(moves.Select(x => x.Unit.Base), movePosition);

                foreach (var move in moves)
                {
                    move.orderTime = time;
                }
            }
            else
            {
                orderOrbwalk.orderTime = time;
            }
        }

        private void Follow(CUnit target, IEnumerable<OrbwalkerManager> orbwalks, float time)
        {
            var follows = orbwalks.Where(x => x.IsFollow && x.Target == target);
            Player.Follow(follows.Select(x => x.Unit.Base), target.Base);

            foreach (var follow in follows)
            {
                follow.orderTime = time;
            }
        }
    }
}