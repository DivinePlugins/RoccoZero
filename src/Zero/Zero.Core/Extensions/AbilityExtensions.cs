using System.Linq;

using Divine.Core.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Common.Log;

namespace Divine.Core.Extensions
{
    public static class AbilityExtensions
    {
        public static float GetSpecialData(this CAbility ability, string name, uint level = 0)
        {
            return GetSpecialData(ability.Base, name, level);
        }

        public static float GetSpecialData(this Ability ability, string name, uint level = 0)
        {
            var data = ability.AbilitySpecialData.FirstOrDefault(x => x.Name == name);
            if (data == null)
            {
                LogManager.Error($"BrokenAbilitySpecialData => Ability: {ability.Name}, SpecialData: {name}");
                return 0;
            }

            if (data.Count == 1)
            {
                return data.Value;
            }

            if (level == 0)
            {
                level = ability.Level;
            }

            if (level == 0)
            {
                return 0;
            }

            return data.GetValue(level - 1);
        }
    }
}