namespace Ensage.SDK.Menu;

using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ValueAttribute : Attribute
{
    public ValueAttribute(params object[] objects)
    {
        Objects = objects;
    }

    public object[] Objects { get; }
}