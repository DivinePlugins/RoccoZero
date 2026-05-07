namespace Ensage.SDK.Orbwalker
{
    using System;

    using Ensage.SDK.Orbwalker.Modes;

    public interface IOrbwalkerManager : IOrbwalker, IDisposable
    {
        void RegisterMode(IOrbwalkingMode mode);

        void UnregisterMode(IOrbwalkingMode mode);
    }
}