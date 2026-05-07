namespace BeAware.Helpers;

using System;
using System.Runtime.CompilerServices;

using BeAware.MenuManager;

using Divine.Common.Log;

internal sealed class SoundHelper
{
    private readonly MenuConfig MenuConfig;

    public SoundHelper(Common common)
    {
        MenuConfig = common.MenuConfig;

        RuntimeHelpers.RunClassConstructor(typeof(SoundPlayer).TypeHandle);
    }

    public void Play(string name)
    {
        if (MenuConfig.FullyDisableSoundsItem)
        {
            return;
        }

        try
        {
            string file;

            if (name.Contains("check_rune"))
            {
                file = $"{name}_{MenuConfig.LanguageItem.Value.ToLower()}.wav";
            }
            else
            {
                file = $"{name}.wav";
            }

            var volume = (int)MenuConfig.VolumeItem;

            if (MenuConfig.DefaultSoundItem)
            {
                SoundPlayer.Play("default.wav", volume);
                return;
            }

            if (!SoundPlayer.Play(file, volume))
            {
                SoundPlayer.Play("default.wav", volume);
            }
        }
        catch (Exception e)
        {
            LogManager.Error(e);
        }
    }
}