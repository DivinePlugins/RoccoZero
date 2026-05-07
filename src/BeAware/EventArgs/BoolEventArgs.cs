namespace BeAware;

using System;

internal class BoolEventArgs
{
    public event EventHandler<bool> ValueChanged;

    public bool IsEnable
    {
        get
        {
            return Value;
        }

        set
        {
            if (Value == value)
            {
                return;
            }

            ValueChanged?.Invoke(null, value);
            Value = value;
        }
    }

    private bool Value { get; set; }
}