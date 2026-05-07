using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Spells.SkywrathMage;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Components;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Prediction;
using Divine.SkywrathMage.Menus;

namespace Divine.SkywrathMage
{
    internal static class Utils
    {
        public static Vector3 MysticFlarePrediction(this MysticFlare mysticFlare, CUnit target)
        {
            var dubleMysticFlare = false;
            var owner = mysticFlare.Owner;
            if (mysticFlare.Owner.HasAghanimsScepter())
            {
                dubleMysticFlare = UnitManager<CHero, Enemy, NoIllusion>.Units.Count(x => x.IsVisible && x.IsAlive && x.Distance2D(target) <= 700) <= 1;
            }

            var input = new PredictionInput
            {
                Owner = owner.Base,
                Delay = mysticFlare.CastPoint + mysticFlare.ActivationDelay,
                Range = mysticFlare.CastRange,
                Radius = mysticFlare.Radius,
            };

            var output = PredictionManager.GetPrediction(input.WithTarget(target.Base));
            var castPosition = output.CastPosition;

            Vector3 position;
            if (target.NetworkActivity != NetworkActivity.Run || target.IsStunned() || target.IsRooted())
            {
                position = castPosition;
            }
            else
            {
                position = castPosition + (100 * target.Direction2D().ToVector3());
            }

            return dubleMysticFlare ? castPosition + (175 * target.Direction2D().ToVector3()) : position;
        }

        public static bool AutoCombo(CUnit target)
        {
            var borrowedTime = target.GetAbilityById(AbilityId.abaddon_borrowed_time);
            if (borrowedTime != null && borrowedTime.Owner.Health <= 2000 && borrowedTime.Cooldown <= 0 && borrowedTime.Level > 0)
            {
                return false;
            }

            if (target.HasAnyModifiers("modifier_dazzle_shallow_grave", "modifier_spirit_breaker_charge_of_darkness") || UnitManager.Owner.HasModifier("modifier_pugna_nether_ward_aura"))
            {
                return false;
            }

            if (target.Spellbook.Spells.Any(x => AutoComboAbilities.Contains(x.Id) && x.IsInAbilityPhase))
            {
                return true;
            }

            var targetModifiers = new TargetModifiers(target, AutoComboModifiers);
            var modifierStun = targetModifiers.ModifierStun;
            return target.MovementSpeed < 240 || (modifierStun != null && modifierStun.Duration > 1) || targetModifiers.IsModifiers;
        }

        private static AbilityId[] AutoComboAbilities { get; } =
        {
            AbilityId.rattletrap_power_cogs,
            AbilityId.enigma_black_hole,
            AbilityId.bane_fiends_grip,
            AbilityId.witch_doctor_death_ward
        };

        private static string[] AutoComboModifiers { get; } =
        {
            "modifier_skywrath_mystic_flare_aura_effect",
            "modifier_rod_of_atos_debuff",
            "modifier_crystal_maiden_frostbite",
            "modifier_crystal_maiden_freezing_field",
            "modifier_naga_siren_ensnare",
            "modifier_meepo_earthbind",
            "modifier_lone_druid_spirit_bear_entangle_effect",
            "modifier_legion_commander_duel",
            "modifier_kunkka_torrent",
            "modifier_enigma_black_hole_pull",
            "modifier_ember_spirit_searing_chains",
            "modifier_dark_troll_warlord_ensnare",
            "modifier_rattletrap_cog_marker",
            "modifier_axe_berserkers_call",
            "modifier_faceless_void_chronosphere_freeze",
            "modifier_winter_wyvern_cold_embrace"
        };

        public static bool ConcussiveShotTarget(SmartConcussiveShotMenu smartConcussiveShotMenu, CUnit target, CHero targetHit)
        {
            if (!smartConcussiveShotMenu.UseOnlyTargetItem)
            {
                return true;
            }

            if (targetHit == null)
            {
                return false;
            }

            if (target.Handle == targetHit.Handle)
            {
                return true;
            }

            if (target.Distance2D(targetHit) < 200)
            {
                return true;
            }

            return false;
        }
    }
}
