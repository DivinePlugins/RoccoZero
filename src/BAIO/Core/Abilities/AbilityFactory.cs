namespace Ensage.SDK.Abilities
{
    using System;

    using System.Linq;
    using System.Reflection;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Ensage.SDK.Service;

    public class AbilityFactory
    {
        public AbilityFactory(IServiceContext context)
        {
            this.Context = context;

            this.Types = Assembly.GetExecutingAssembly().GetExportedTypes().Where(e => !e.IsAbstract && typeof(BaseAbility).IsAssignableFrom(e)).ToArray();
        }

        private IServiceContext Context { get; }

        private Type[] Types { get; }

        public T GetAbility<T>(Ability ability)
            where T : BaseAbility
        {
            return (T)this.GetAbility(ability);
        }

        public BaseAbility GetAbility(Ability ability)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability));
            }

            var abilityTypeName = ability.Id.ToString();
            var type = this.Types.FirstOrDefault(e => e.Name == abilityTypeName);

            if (type == null)
            {
                throw new AbilityNotImplementedException($"Could not find {nameof(BaseAbility)} implementation for {abilityTypeName}");
            }

            return (BaseAbility)Activator.CreateInstance(type, ability);
        }

        public T GetAbility<T>()
            where T : BaseAbility
        {
            var abilityTypeName = typeof(T).Name;

            AbilityId id;
            if (!Enum.TryParse(abilityTypeName, out id))
            {
                throw new AbilityNotFoundException($"Could not find {abilityTypeName} in the AbilityId enum");
            }

            return this.GetAbility<T>(id);
        }

        public T GetAbility<T>(AbilityId id)
            where T : BaseAbility
        {
            return (T)this.GetAbility(id);
        }

        public BaseAbility GetAbility(AbilityId id)
        {
            Ability ability = id < AbilityId.default_attack
                              ? this.Context.Owner.Inventory.Items.FirstOrDefault(x => x.Id == id)
                              : this.Context.Owner.Spellbook.Spells.FirstOrDefault(x => x.Id == id);
            if (ability == null)
            {
                throw new AbilityNotFoundException($"Could not find {id} for {this.Context}");
            }

            return this.GetAbility(ability);
        }
    }
}