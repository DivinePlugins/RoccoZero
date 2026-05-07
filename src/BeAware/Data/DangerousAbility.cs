namespace BeAware.Data;

using Divine.Entity.Entities.Abilities.Components;

internal static class DangerousAbility
{
    public static AbilityId[] DangerousSpells { get; } =
    {
        AbilityId.ancient_apparition_ice_blast,
        AbilityId.mirana_invis,
        AbilityId.sandking_epicenter,
        AbilityId.furion_teleportation,
        AbilityId.furion_wrath_of_nature,
        AbilityId.alchemist_unstable_concoction,
        AbilityId.bounty_hunter_wind_walk,
        AbilityId.clinkz_wind_walk,
        AbilityId.nyx_assassin_vendetta,
        AbilityId.wisp_relocate,
        AbilityId.morphling_replicate,
        AbilityId.ursa_enrage,
        AbilityId.abyssal_underlord_dark_rift,
        AbilityId.mirana_arrow,
        AbilityId.monkey_king_primal_spring,
        AbilityId.invoker_sun_strike
    };

    public static AbilityId[] DangerousItems { get; } =
    {
        AbilityId.item_smoke_of_deceit,
        AbilityId.item_glimmer_cape
    };
}