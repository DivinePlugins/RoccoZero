namespace Ensage.SDK.Menu;

using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ParameterAttribute : Attribute
{
    public ParameterAttribute(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public object Value { get; }
}