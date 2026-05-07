namespace BodyBlocker;

using System.Collections.Generic;

using BodyBlocker.Modes.ControllablesBlocker;
using BodyBlocker.Modes.HeroBlocker;

using Divine.Menu;
using Divine.Service;

using Modes;

//[ExportPlugin("Body Blocker", StartupMode.Auto, "IdcNoob")]
internal class Bootstrap : Bootstrapper
{
    private readonly List<IBodyBlockMode> modes = [];

    protected override void OnMainActivate()
    {
        var menu = MenuManager.UtilityMenu.AddMenu("Body Blocker");

        modes.Add(new ControllablesBlocker(menu));
        modes.Add(new HeroBlocker(menu));
    }

    protected override void OnActivate()
    {
        foreach (var mode in modes)
        {
            mode.Activate();
        }
    }

    protected override void OnDeactivate()
    {
        foreach (var mode in modes)
        {
            mode.Deactivate();
        }
    }
}