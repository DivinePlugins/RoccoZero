namespace Divine.Core.Helpers;

using System.Linq;

using Divine.Core.Data;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Modifier.Modifiers;

public sealed class TargetModifiers
{
    public TargetModifiers(CUnit target, params string[] modifiers)
    {
        foreach (var modifier in target.Modifiers)
        {
            var name = modifier.Name;

            if (name == ModifierData.ModifierReflect)
            {
                ModifierReflect = modifier;
            }

            if (modifier.IsStunDebuff)
            {
                ModifierStun = modifier;
            }

            if (ModifierData.HexModifiers.Contains(name))
            {
                ModifierHex = modifier;
            }

            if (name == ModifierData.ModifierAtos)
            {
                ModifierAtos = modifier;
            }

            if (name == ModifierData.ModifierGleipnir)
            {
                ModifierGleipnir = modifier;
            }

            if (name == ModifierData.ModifierSilver)
            {
                IsSilverDebuff = true;
            }

            if (name == ModifierData.ModifierLegionDuel)
            {
                IsDuelAghanimsScepter = UnitManager<CHero>.Units.Any(x => x.IsVisible && x.IsAlive && x.HeroId == HeroId.npc_dota_hero_legion_commander && x.HasAghanimsScepter());
            }

            if (name == ModifierData.ModifierLinken)
            {
                IsLinken = true;
            }

            if (ModifierData.EtherealModifiers.Contains(name))
            {
                IsEthereal = true;
            }

            if (ModifierData.BlockModifiers.Contains(name))
            {
                IsBlockingModifers = true;
            }

            if (modifiers.Contains(name))
            {
                IsModifiers = true;
            }
        }
    }

    public Modifier ModifierReflect { get; }

    public Modifier ModifierStun { get; }

    public Modifier ModifierHex { get; }

    public Modifier ModifierAtos { get; }

    public Modifier ModifierGleipnir { get; }

    public bool IsSilverDebuff { get; }

    public bool IsDuelAghanimsScepter { get; }

    public bool IsLinken { get; }

    public bool IsEthereal { get; }

    public bool IsBlockingModifers { get; }

    public bool IsModifiers { get; }
}
