namespace Ensage.SDK.Inventory
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Zero.Log;

    using Ensage.SDK.Abilities;
    using Ensage.SDK.Persistence;

    public class ItemBindingHandler
    {
        private BaseAbility value;

        public ItemBindingHandler(AbilityId id, Type type)
        {
            this.Id = id;
            this.Type = type;
        }

        public AbilityId Id { get; }

        public Type Type { get; }

        public BaseAbility Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.UpdateBindings(value);
            }
        }

        private List<PropertyBinding> Bindings { get; } = new List<PropertyBinding>();

        public PropertyBinding Add(PropertyInfo info, object target)
        {
            //LogManager.Debug($"Attach [{this.Id}] {target}.{info.Name}");
            var binding = new PropertyBinding(info, target);

            this.Bindings.Add(binding);

            return binding;
        }

        public void Remove(object target)
        {
            this.Bindings.RemoveAll(e => !e.Reference.IsAlive || e.Reference.Target == target);
        }

        public void ResetBindings()
        {
            this.UpdateBindings(null);
        }

        public void UpdateBindings(object newValue)
        {
            foreach (var binding in this.Bindings)
            {
                try
                {
                    binding.SetValue(newValue);
                }
                catch (Exception e)
                {
                    LogManager.Warn(e);
                }
            }
        }
    }
}