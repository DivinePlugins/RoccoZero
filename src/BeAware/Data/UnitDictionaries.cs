namespace BeAware.Data;

using System.Collections.Generic;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

internal static class UnitDictionaries
{
    public static Dictionary<HeroId, AbilityId> Units { get; } = new Dictionary<HeroId, AbilityId>
    {
        { HeroId.npc_dota_hero_tinker, AbilityId.tinker_march_of_the_machines },
        { HeroId.npc_dota_hero_tusk, AbilityId.tusk_frozen_sigil },
        { HeroId.npc_dota_hero_monkey_king, AbilityId.monkey_king_primal_spring },
        { HeroId.npc_dota_hero_riki, AbilityId.riki_smoke_screen },
        { HeroId.npc_dota_hero_disruptor, AbilityId.disruptor_kinetic_field },
        { HeroId.npc_dota_hero_enigma, AbilityId.enigma_black_hole },
        { HeroId.npc_dota_hero_skywrath_mage, AbilityId.skywrath_mage_mystic_flare },
        { HeroId.npc_dota_hero_night_stalker, AbilityId.night_stalker_darkness },
        { HeroId.npc_dota_hero_gyrocopter, AbilityId.gyrocopter_homing_missile },
        { HeroId.npc_dota_hero_juggernaut, AbilityId.juggernaut_healing_ward },
        { HeroId.npc_dota_hero_slark, AbilityId.slark_shadow_dance },
        { HeroId.npc_dota_hero_templar_assassin, AbilityId.templar_assassin_psionic_trap },
        { HeroId.npc_dota_hero_pugna, AbilityId.pugna_nether_ward },
        { HeroId.npc_dota_hero_shadow_shaman, AbilityId.shadow_shaman_mass_serpent_ward },
        { HeroId.npc_dota_hero_storm_spirit, AbilityId.storm_spirit_static_remnant }
    };

    public static Dictionary<int, AbilityId> UnitsDayVision { get; } = new Dictionary<int, AbilityId>
    {
        //{ 200, AbilityId.tusk_ice_shards },
        { 325, AbilityId.skywrath_mage_arcane_bolt },
        //{ 450, AbilityId.phantom_assassin_stifling_dagger }
    };
}