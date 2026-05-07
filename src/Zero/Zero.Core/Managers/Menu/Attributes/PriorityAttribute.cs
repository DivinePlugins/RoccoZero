namespace Ensage.SDK.Menu.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
