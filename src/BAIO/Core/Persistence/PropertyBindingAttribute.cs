namespace Ensage.SDK.Persistence
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PropertyBindingAttribute : Attribute
    {
        public PropertyBindingAttribute(string name = null)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}