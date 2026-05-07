namespace Ensage.SDK.Menu
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ItemAttribute : Attribute
    {
        public ItemAttribute(string displayName)
            : this(displayName, displayName)
        {
        }

        public ItemAttribute(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; }

        public string DisplayName { get; }
    }
}