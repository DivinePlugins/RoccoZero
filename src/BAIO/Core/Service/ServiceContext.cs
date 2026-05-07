namespace Ensage.SDK.Service;

using Divine.Entity;
using Divine.Entity.Entities.Units;

using Ensage.SDK.Abilities;
using Ensage.SDK.Inventory;
using Ensage.SDK.Orbwalker;
using Ensage.SDK.Service;
using Ensage.SDK.TargetSelector;

public class ServiceContext : IServiceContext
{
    public ServiceContext()
    {
        Owner = EntityManager.LocalHero;
        AbilityFactory = new AbilityFactory(this);
        Inventory = new InventoryManager(this);
        TargetSelector = new TargetSelectorManager();
        Orbwalker = new OrbwalkerManager(this);
    }

    public Unit Owner { get; }

    //public IAbilityDetector AbilityDetector { get; }

    public AbilityFactory AbilityFactory { get; }

    public IInventoryManager Inventory { get; }

    public IOrbwalkerManager Orbwalker { get; }

    public ITargetSelectorManager TargetSelector { get; }

    public void Dispose()
    {
        Orbwalker.Dispose();
        TargetSelector.Dispose();
        Inventory.Dispose();
    }
}