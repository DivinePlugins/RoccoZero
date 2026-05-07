namespace Ensage.SDK.Menu
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class MenuAttribute : Attribute
    {
        public MenuAttribute(string displayName)
            : this(displayName, displayName)
        {
        }

        public MenuAttribute(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; }

        public string DisplayName { get; }
    }
}