namespace BeAware.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Divine.Media;

public static class SoundPlayer
{
    private static readonly Process CurrentProcess = Process.GetCurrentProcess();

    private static readonly Task WaitHandler;

    private static readonly Dictionary<string, SoundData> Sounds = [];

    static SoundPlayer()
    {
        WaitHandler = Task.Run(() =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            Sounds["check_rune_en.wav"] = new(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_en.wav"));
            Sounds["check_rune_ru.wav"] = new(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.check_rune_ru.wav"));
            Sounds["default.wav"] = new(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.default.wav"));
            Sounds["item_smoke_of_deceit.wav"] = new(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.item_smoke_of_deceit.wav"));
            Sounds["furion_teleportation.wav"] = new(assembly.GetManifestResourceStream("BeAware.Resources.Sounds.furion_teleportation_en.wav"));
        });
    }

    [DllImport("User32.dll", SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    public static bool Play(string fileName, int volume)
    {
        if (!Sounds.TryGetValue(fileName, out var data) || GetForegroundWindow() != CurrentProcess.MainWindowHandle)
        {
            return false;
        }

        Task.Run(async () =>
        {
            try
            {
                await WaitHandler;

                Divine.Media.SoundPlayer.Play(data, volume / 100f);

            }
            catch
            {
            }
        });

        return true;
    }
}