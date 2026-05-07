namespace Ensage.SDK.TargetSelector
{
using System;
    using System.Collections.Generic;

    using Divine.Entity.Entities.Units;

    public interface ITargetSelectorManager : IDisposable
    {
        IEnumerable<Unit> GetTargets();
    }
}