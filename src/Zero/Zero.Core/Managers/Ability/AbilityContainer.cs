using System;
using System.Collections.Generic;
using System.Reflection;

using Divine.Core.Entities;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;

namespace Divine.Core.Managers.Ability
{
    public static class AbilityContainer
    {
        private static readonly Dictionary<AbilityId, Type> AbilityIdTypePairs = new Dictionary<AbilityId, Type>();

        static AbilityContainer()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (!type.IsClass || type.IsAbstract || !typeof(CAbility).IsAssignableFrom(type))
                {
                    continue;
                }

                var spellAttribute = type.GetCustomAttribute<SpellAttribute>();
                if (spellAttribute != null)
                {
                    AbilityIdTypePairs[spellAttribute.AbilityId] = type;
                    continue;
                }

                foreach (var itemAttribute in type.GetCustomAttributes<ItemAttribute>())
                {
                    AbilityIdTypePairs[itemAttribute.AbilityId] = type;
                }
            }
        }

        public static bool TryGetType(AbilityId abilityId, out Type type)
        {
            return AbilityIdTypePairs.TryGetValue(abilityId, out type);
        }

        public static IEnumerable<Type> Types
        {
            get
            {
                return AbilityIdTypePairs.Values;
            }
        }
    }
}