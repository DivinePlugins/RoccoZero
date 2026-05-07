namespace Divine.Core.Extensions;

using Divine.Modifier.Modifiers;

public static class ModifierExtensions
{
    public static bool IsModifier(this Modifier modifier, float remainingTime)
    {
        if (modifier == null || !modifier.IsValid || modifier.RemainingTime <= remainingTime)
        {
            return true;
        }

        return false;
    }
}
