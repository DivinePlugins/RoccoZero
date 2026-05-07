namespace BeAware.Data;

using Divine.Entity.Entities.Abilities.Components;

internal static class ParticleCorrection
{
    public static string[] EndParticles { get; } =
    {
        "furion_teleport_end.vpcf",
        "furion_teleport_end_fallback_mid.vpcf",
        "wisp_relocate_marker_endpoint.vpcf",
        "abyssal_underlord_darkrift_target.vpcf",
        "/teleport_end.vpcf"
    };

    public static string[] IgnoreParticles { get; } =
    {
        "ui_mouseactions",
        "base_attacks",
        "ensage_ui",
        "generic_gameplay",
        "siege_fx",
        "radiant_fx",
        "dire_fx"
    };

    public static AbilityId[] DagonId { get; } =
    {
        AbilityId.item_dagon,
        AbilityId.item_dagon_2,
        AbilityId.item_dagon_3,
        AbilityId.item_dagon_4,
        AbilityId.item_dagon_5
    };
}