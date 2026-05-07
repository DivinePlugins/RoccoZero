namespace Ensage.SDK.Persistence
{
    using System;
    using System.Reflection;

    using Ensage.SDK.Extensions;

    public class PropertyBinding<T>
    {
        public PropertyBinding(PropertyInfo propertyInfo, object target)
        {
            this.PropertyInfo = propertyInfo;
            this.Reference = new WeakReference(target);

            this.Setter = propertyInfo.GetPropertySetter<T>(target);
            this.Getter = propertyInfo.GetPropertyGetter<T>(target);
        }

        public PropertyInfo PropertyInfo { get; }

        public WeakReference Reference { get; }

        private Func<object, T> Getter { get; }

        private Action<object, T> Setter { get; }

        public T GetValue()
        {
            return this.Getter(this.Reference.Target);
        }

        public void SetValue(T value)
        {
            this.Setter(this.Reference.Target, value);
        }

        public override string ToString()
        {
            return $"{this.PropertyInfo?.Name} @ {this.Reference.Target}";
        }
    }

    public class PropertyBinding : PropertyBinding<object>
    {
        public PropertyBinding(PropertyInfo propertyInfo, object target)
            : base(propertyInfo, target)
        {
        }
    }
}