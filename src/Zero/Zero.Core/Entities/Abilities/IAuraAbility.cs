namespace Divine.Core.Entities.Abilities
{
    public interface IAuraAbility
    {
        string AuraModifierName { get; }

        float AuraRadius { get; }
    }
}