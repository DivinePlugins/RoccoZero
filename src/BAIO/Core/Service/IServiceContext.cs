namespace Ensage.SDK.Service;

using System;

using Divine.Entity.Entities.Units;

using Ensage.SDK.Abilities;
using Ensage.SDK.Inventory;
using Ensage.SDK.Orbwalker;

using Ensage.SDK.TargetSelector;

public interface IServiceContext : IDisposable
{
    Unit Owner { get; }

    //IAbilityDetector AbilityDetector { get; }

    AbilityFactory AbilityFactory { get; }

    IInventoryManager Inventory { get; }

    IOrbwalkerManager Orbwalker { get; }

    ITargetSelectorManager TargetSelector { get; }
}