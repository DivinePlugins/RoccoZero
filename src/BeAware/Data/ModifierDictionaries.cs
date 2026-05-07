namespace BeAware.Data;

using System.Collections.Generic;

using Divine.Entity.Entities.Units.Heroes.Components;

internal static class ModifierDictionaries
{
    public static Dictionary<string, HeroId> AllyModifiers { get; } = new Dictionary<string, HeroId>
    {
        { "modifier_nevermore_presence", HeroId.npc_dota_hero_nevermore },
        { "modifier_sniper_assassinate", HeroId.npc_dota_hero_sniper },
        { "modifier_bounty_hunter_track", HeroId.npc_dota_hero_bounty_hunter },
        { "modifier_bloodseeker_thirst_vision", HeroId.npc_dota_hero_bloodseeker }
    };

    public static Dictionary<string, int> Modifiers { get; } = new Dictionary<string, int>
    {
        { "modifier_rune_haste", 10000 },
        { "modifier_rune_regen", 10000 },
        { "modifier_rune_arcane", 10000 },
        { "modifier_rune_doubledamage", 10000 },
        { "modifier_rune_invis", 3000 },
        { "modifier_item_invisibility_edge_windwalk", 3000 },
        { "modifier_item_shadow_amulet_fade", 3000 },
        { "modifier_item_silver_edge_windwalk", 3000 },
        { "modifier_item_gem_of_true_sight", 15000 },
        { "modifier_item_divine_rapier", 15000 }
    };
}