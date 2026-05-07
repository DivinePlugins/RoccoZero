namespace Divine.Core.Data
{
    public static class ModifierData
    {
        public static string ModifierReflect { get; } = "modifier_item_blade_mail_reflect";

        public static string ModifierAtos { get; } = "modifier_rod_of_atos_debuff";

        public static string ModifierGleipnir { get; } = "modifier_gungnir_debuff";

        public static string ModifierSilver { get; } = "modifier_silver_edge_debuff";

        public static string ModifierLegionDuel { get; } = "modifier_legion_commander_duel";

        public static string ModifierLinken { get; } = "modifier_item_sphere_target";

        public static string[] HexModifiers { get; } =
        {
            "modifier_sheepstick_debuff",
            "modifier_shadow_shaman_voodoo",
            "modifier_lion_voodoo"
        };

        public static string[] EtherealModifiers { get; } =
        {
            "modifier_ghost_state",
            "modifier_item_ethereal_blade_ethereal",
            "modifier_pugna_decrepify",
            "modifier_necrolyte_sadist_active"
        };

        public static string[] BlockModifiers { get; } =
        {
            "modifier_abaddon_borrowed_time",
            "modifier_item_aeon_disk_buff",
            "modifier_winter_wyvern_winters_curse_aura",
            "modifier_winter_wyvern_winters_curse",
            //"modifier_templar_assassin_refraction_absorb",
            "modifier_oracle_fates_edict"
        };
    }
}
