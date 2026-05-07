using System;
using System.Collections.Generic;
using System.Linq;

using BAIO.Core;
using BAIO.Core.Extensions;

using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.GameConsole;
using Divine.Numerics;
using Divine.Renderer;

namespace BAIO
{
    public static class Extensions
    {
        private static Dictionary<uint, bool> canHitDictionary = new Dictionary<uint, bool>();

        private static MultiSleeper sleeper = new MultiSleeper();

        private static readonly HashSet<string> EtherealModifiers = new(StringComparer.OrdinalIgnoreCase)
        {
            "modifier_ghost_state",
            "modifier_item_ethereal_blade_ethereal",
            "modifier_pugna_decrepify",
            "modifier_necrolyte_sadist_active"
        };

        public static IEnumerable<TEntity> GetAlliesInRange<TEntity>(this Unit unit, float range)
            where TEntity : Unit, new()
        {
            var handle = unit.Handle;
            var team = unit.Team;
            var pos = unit.Position;
            var sqrRange = range * range;

            return EntityManager
                .GetEntities<TEntity>().Where(e => e.Handle != handle && e.IsVisible && e.IsAlive && e.Team == team && pos.DistanceSquared(e.Position) < sqrRange)
                .OrderBy(e => pos.DistanceSquared(e.Position));
        }

        public static Vector3 FacePosition(Unit unit)
        {
            var vector3 = unit.Position + unit.Vector3FromPolarAngle(0.0f, 1f);
            return new Vector3(vector3.X, vector3.Y, 0.0f);
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
            if (ability.IsAbilityBehavior(AbilityBehavior.Point, name) || name == "lion_impale")
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

                if (name.Contains("earthshaker_enchant_totem") && (ability.Owner as Hero).HasAghanimsScepter())
                {
                    range += 1100;
                }

                if (name.Contains("faceless_void_time_walk"))
                {
                    range += GetAbilityCastRange(ability.Owner as Unit, ability);
                }

                if (distanceXyz <= range && position.Distance2D(target.Position) <= range)
                {
                    canHitDictionary[id] = true;
                    sleeper.Sleep(50, id);
                    return true;
                }

                canHitDictionary[id] = name == "pudge_rot" && target.HasModifier("modifier_pudge_meat_hook") && position.Distance2D(target.Position) < 1500;
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

            if (position.Distance2D(target.Position) <= GetAbilityCastRange(ability.Owner as Unit, ability) + 100)
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

            return ability.CanHit(target, ability.Owner.Position, abilityName);
        }

        public static float GetAbilityCastRange(Unit owner, Ability ability)
        {
            var range = 0f;
            try
            {
                if (ability.Id == AbilityId.zuus_thundergods_wrath || ability.Id == AbilityId.zuus_cloud || ability.Id == AbilityId.furion_wrath_of_nature)
                {
                    range += float.MaxValue;
                }

                if (ability.Id == AbilityId.clinkz_searing_arrows)
                {
                    range += owner.GetAttackRange() + 150f;
                    return range;
                }

                if (ability.Id == AbilityId.earthshaker_enchant_totem)
                {
                    range += ability.GetAbilitySpecialData("distance_scepter");
                    return range;
                }

                if (ability.Id == AbilityId.faceless_void_time_walk)
                {
                    range += ability.GetAbilitySpecialDataWithTalent(owner, "range");
                    return range;
                }

                range += ability.GetCastRange() >= ability.AbilitySpecialData.First(x => x.Name.Contains("range")).GetValue(ability.Level - 1)
                    ? ability.GetCastRange() : ability.AbilitySpecialData.First(x => x.Name.Contains("range")).GetValue(ability.Level - 1);
                var aetherLens = owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    range += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }
                var talent = owner.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    range += talent.GetAbilitySpecialData("value");
                }
                if (ability.Id == AbilityId.viper_viper_strike && owner.HasAghanimsScepter())
                {
                    range += 400f;
                }
                return range;
            }
            catch (IndexOutOfRangeException)
            {
                range += ability.AbilitySpecialData.First(x => x.Name.Contains("range")).Value;
                var aetherLens = owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    range += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }
                var talent = owner.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    range += talent.GetAbilitySpecialData("value");
                }
                return range;
            }
            catch (InvalidOperationException)
            {
                range += ability.GetCastRange();
                return range;
            }
        }

        public static float GetAbilityRadius(Unit owner, Ability ability)
        {
            try
            {
                var radius = 0f;
                radius += ability.AbilitySpecialData.First(x => x.Name.Contains("radius")).Value;
                if (owner.GetAbilityById(ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                        .SpecialBonusAbility) != null && owner.GetAbilityById(ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                        .SpecialBonusAbility).Level > 0)
                {
                    radius += owner.GetAbilityById(
                        ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                            .SpecialBonusAbility).GetAbilitySpecialData("value");
                }

                if (ability.Name == "templar_assassin_self_trap")
                {
                    radius = 400f;
                }
                return radius;
            }
            catch (Exception)
            {
                return ability.GetRadius();
            }
        }

        public static bool PositionCamera(float x, float y)
        {
            var pos = new Vector3(x, y, 256);

            var screenposVector2 = RendererManager.WorldToScreen(pos, true);
            if (screenposVector2.IsZero)
            {
                GameConsoleManager.ExecuteCommand($"dota_camera_set_lookatpos {x} {y}");
                return true;
            }

            return true;
        }
        public static bool PositionCamera(Unit unit)
        {
            var x = unit.Position.X;
            var y = unit.Position.Y;
            var screenposVector2 = RendererManager.WorldToScreen(unit.Position, true);
            if (screenposVector2.IsZero)
            {
                GameConsoleManager.ExecuteCommand($"dota_camera_set_lookatpos {x} {y}");
                return true;
            }

            return true;
        }

        public static bool IsEthereal(this Unit unit)
        {
            return unit.HasModifiers(EtherealModifiers, false);
        }
    }
}