namespace BeAware;

using System.Reflection;

using BeAware.MenuManager;

using Divine.Renderer;
using Divine.Service;

internal sealed class Bootstrap : Bootstrapper
{
    private MenuConfig MenuConfig;

    private Common common;

    protected override void OnActivateUnsafe()
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.StartsWith("BeAware.Resources.Textures"))
            {
                continue;
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            RendererManager.LoadImage(resourceName, stream);
        }
    }

    protected override void OnMainActivate()
    {
        MenuConfig = new MenuConfig();
    }

    protected override void OnMainDeactivate()
    {
        MenuConfig.Dispose();
    }

    protected override void OnActivate()
    {
        common = new Common(MenuConfig);
    }

    protected override void OnDeactivate()
    {
        common?.Dispose();
    }
}