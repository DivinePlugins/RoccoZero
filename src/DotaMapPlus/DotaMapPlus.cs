namespace DotaMapPlus;

using Divine.Service;

internal sealed class DotaMapPlus : Bootstrapper
{
    private Context Context;

    protected override void OnActivate()
    {
        Context = new Context();
    }

    protected override void OnDeactivate()
    {
        Context?.Dispose();
    }
}