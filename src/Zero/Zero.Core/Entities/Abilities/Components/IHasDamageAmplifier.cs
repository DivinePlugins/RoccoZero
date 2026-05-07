namespace Divine.Core.Entities.Abilities.Components;

using Divine.Entity.Entities.Abilities.Components;

public interface IHasDamageAmplifier
{
    DamageType AmplifierType { get; }

    float DamageAmplification { get; }
}