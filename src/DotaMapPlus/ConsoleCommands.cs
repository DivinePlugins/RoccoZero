namespace DotaMapPlus;

using Divine.GameConsole;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Service;

internal sealed class ConsoleCommands
{
    private readonly MenuSwitcher FogItem;

    private readonly MenuSwitcher FilteringItem;

    //private readonly MenuSwitcher ParticleHackItem;

    public ConsoleCommands(RootMenu rootMenu)
    {
        var consoleCommandsMenu = rootMenu.CreateMenu("Console Commands");
        FogItem = consoleCommandsMenu.CreateSwitcher("Fog Disable");
        FilteringItem = consoleCommandsMenu.CreateSwitcher("Filtering Disable2", "Filtering Disable", false);
        //ParticleHackItem = consoleCommandsMenu.CreateSwitcher("Particle Hack Enable");

        if (DivineService.UserId != "2951089c-bef8-487b-a0cc-2bf92c28268b")
        {
            FilteringItem.IsHidden = true;
        }

        FogItem.ValueChanged += FogItemChanged;
        FilteringItem.ValueChanged += FilteringItemChanged;
        //ParticleHackItem.ValueChanged += ParticleHackItemChanged;
    }

    public void Dispose()
    {
        GameConsoleManager.SetValue("fog_enable", 1);
        GameConsoleManager.SetValue("fow_client_nofiltering", 0);
        //GameConsoleManager.SetValue("dota_use_particle_fow", 1);

        FogItem.ValueChanged -= FogItemChanged;
        FilteringItem.ValueChanged -= FilteringItemChanged;
        //ParticleHackItem.ValueChanged -= ParticleHackItemChanged;
    }

    private void FogItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        GameConsoleManager.SetValue("fog_enable", !e.Value);
    }

    private void FilteringItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        GameConsoleManager.SetValue("fow_client_nofiltering", e.Value);
    }

    //private void ParticleHackItemChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    //{
    //    GameConsoleManager.SetValue("dota_use_particle_fow", !e.Value);
    //}
}