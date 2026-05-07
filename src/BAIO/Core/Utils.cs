namespace BAIO.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.GameConsole;
using Divine.Modifier.Modifiers;
using Divine.Numerics;

public class Utils
{
    public static Dictionary<string, double> Sleeps = new Dictionary<string, double>();

    /// <summary>
    ///     The disable modifiers.
    /// </summary>
    private static readonly string[] DisableModifiers =
        {
                "modifier_shadow_demon_disruption",
                "modifier_obsidian_destroyer_astral_imprisonment_prison",
                "modifier_eul_cyclone", "modifier_invoker_tornado",
                "modifier_bane_nightmare",
                "modifier_shadow_shaman_shackles",
                "modifier_crystal_maiden_frostbite",
                "modifier_ember_spirit_searing_chains",
                "modifier_axe_berserkers_call",
                "modifier_lone_druid_spirit_bear_entangle_effect",
                "modifier_meepo_earthbind",
                "modifier_naga_siren_ensnare",
                "modifier_storm_spirit_electric_vortex_pull",
                "modifier_treant_overgrowth", "modifier_cyclone",
                "modifier_sheepstick_debuff",
                "modifier_shadow_shaman_voodoo", "modifier_lion_voodoo",
                "modifier_sheepstick",
                "modifier_brewmaster_storm_cyclone",
                "modifier_puck_phase_shift",
                "modifier_dark_troll_warlord_ensnare",
                "modifier_invoker_deafening_blast_knockback",
                "modifier_pudge_meat_hook"
            };

    private static readonly Dictionary<uint, string> KeyCodeDictionary = new Dictionary<uint, string>
                                                                                 {
                                                                                     { 0, "Mouse3" },
                                                                                     { 1, "Mouse4" },
                                                                                     { 2, "Mouse5" },
                                                                                     { 8, "Backspace" }, { 9, "Tab" },
                                                                                     { 13, "Enter" }, { 16, "Shift" },
                                                                                     { 17, "Ctrl" }, { 18, "Alt" },
                                                                                     { 19, "Pause" }, { 20, "CapsLock" },
                                                                                     { 27, "Escape" }, { 32, "Space" },
                                                                                     { 33, "PageUp" },
                                                                                     { 34, "PageDown" }, { 35, "End" },
                                                                                     { 36, "Home" }, { 37, "LeftArrow" },
                                                                                     { 38, "UpArrow" },
                                                                                     { 39, "RightArrow" },
                                                                                     { 40, "DownArrow" },
                                                                                     { 45, "Insert" }, { 46, "Delete" },
                                                                                     { 48, "0" }, { 49, "1" },
                                                                                     { 50, "2" }, { 51, "3" },
                                                                                     { 52, "4" }, { 53, "5" },
                                                                                     { 54, "6" }, { 55, "7" },
                                                                                     { 56, "8" }, { 57, "9" },
                                                                                     { 91, "LeftWindow" },
                                                                                     { 92, "RightWindow" },
                                                                                     { 93, "Select" }, { 96, "Num0" },
                                                                                     { 97, "Num1" }, { 98, "Num2" },
                                                                                     { 99, "Num3" }, { 100, "Num4" },
                                                                                     { 101, "Num5" }, { 102, "Num6" },
                                                                                     { 103, "Num7" }, { 104, "Num8" },
                                                                                     { 105, "Num9" }, { 106, "*" },
                                                                                     { 107, "+" }, { 109, "-" },
                                                                                     { 110, "," }, { 111, "/" },
                                                                                     { 144, "NumLock" },
                                                                                     { 145, "ScrollLock" }, { 186, ";" },
                                                                                     { 187, "=" }, { 188, "," },
                                                                                     { 189, "-" }, { 190, "." },
                                                                                     { 191, "/" }, { 192, "`" },
                                                                                     { 219, "(" }, { 220, "\\" },
                                                                                     { 221, ")" }, { 222, "'" },
                                                                                     { uint.MaxValue, ""} // undefined key name
                                                                                 };


    private static string lastStunAbility;

    public enum WindowsMessages
    {
        /// <summary>
        ///     Left mouse button double-click
        /// </summary>
        WM_LBUTTONDBLCLCK = 0x203,

        /// <summary>
        ///     Right mouse button double click
        /// </summary>
        WM_RBUTTONDBLCLCK = 0x206,

        /// <summary>
        ///     Middle mouse button double click
        /// </summary>
        WM_MBUTTONDBLCLCK = 0x209,

        /// <summary>
        ///     Middle mouse button down
        /// </summary>
        WM_MBUTTONDOWN = 0x207,

        /// <summary>
        ///     Middle mouse button up
        /// </summary>
        WM_MBUTTONUP = 0x208,

        /// <summary>
        ///     Mouse being moved
        /// </summary>
        WM_MOUSEMOVE = 0x200,

        /// <summary>
        ///     Left mouse button down
        /// </summary>
        WM_LBUTTONDOWN = 0x201,

        /// <summary>
        ///     Left mouse button up
        /// </summary>
        WM_LBUTTONUP = 0x202,

        /// <summary>
        ///     Right mouse button down
        /// </summary>
        WM_RBUTTONDOWN = 0x204,

        /// <summary>
        ///     Right mouse button up
        /// </summary>
        WM_RBUTTONUP = 0x205,

        /// <summary>
        ///     Key down
        /// </summary>
        WM_KEYDOWN = 0x0100,

        /// <summary>
        ///     Key up
        /// </summary>
        WM_KEYUP = 0x0101,

        WM_XBUTTONDOWN = 0x20B,

        WM_XBUTTONUP = 0x20C
    }

    public static float TickCount
    {
        get
        {
            return GameManager.RawGameTime * 1000;
        }
    }

    public static bool ChainStun(Unit unit, double delay, string except, bool onlychain, string abilityName = "")
    {
        if (!SleepCheck("CHAINSTUN_SLEEP") && abilityName != lastStunAbility)
        {
            return false;
        }

        lastStunAbility = abilityName;
        var stunned = false;
        var remainingTime = DisableDuration(unit, except);
        var chain = false;
        if (remainingTime > 0)
        {
            stunned = true;
            chain = remainingTime <= delay;
        }

        return (!(stunned || unit.IsStunned()) || chain) && !onlychain || onlychain && chain;
    }

    /// <summary>
    ///     Switches given degrees to radians
    /// </summary>
    /// <param name="angle">
    ///     The angle.
    /// </param>
    /// <returns>
    ///     The <see cref="double" />.
    /// </returns>
    public static double DegreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0f;
    }

    /// <summary>
    ///     The disable duration.
    /// </summary>
    /// <param name="unit">
    ///     The unit.
    /// </param>
    /// <param name="except">
    ///     The except.
    /// </param>
    /// <returns>
    ///     The <see cref="float" />.
    /// </returns>
    public static float DisableDuration(Unit unit, string except = null)
    {
        Modifier disableModifier = null;
        var maxTime = 0f;
        foreach (var modifier in unit.Modifiers)
        {
            if (
                !((modifier.IsStunDebuff || DisableModifiers.Contains(modifier.Name))
                  && (except == null || modifier.Name != except)))
            {
                continue;
            }

            var remainingTime = modifier.RemainingTime;
            if (!(remainingTime > maxTime))
            {
                continue;
            }

            disableModifier = modifier;
            maxTime = remainingTime;
        }

        if (disableModifier == null)
        {
            return 0;
        }

        if (disableModifier.Name == "modifier_eul_cyclone" || disableModifier.Name == "modifier_invoker_tornado")
        {
            maxTime += 0.07f;
        }

        return maxTime;
    }

    /// <summary>
    ///     The fix virtual key.
    /// </summary>
    /// <param name="key">
    ///     The key.
    /// </param>
    /// <returns>
    ///     The <see cref="byte" />.
    /// </returns>
    public static byte FixVirtualKey(byte key)
    {
        switch (key)
        {
            case 160:
            return 0x10;
            case 161:
            return 0x10;
            case 162:
            return 0x11;
            case 163:
            return 0x11;
        }

        return key;
    }

    /// <summary>
    ///     Returns true if the point is under the rectangle
    /// </summary>
    /// <param name="point">
    ///     The point.
    /// </param>
    /// <param name="x">
    ///     The x.
    /// </param>
    /// <param name="y">
    ///     The y.
    /// </param>
    /// <param name="width">
    ///     The width.
    /// </param>
    /// <param name="height">
    ///     The height.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    public static bool IsUnderRectangle(Vector2 point, float x, float y, float width, float height)
    {
        return point.X > x && point.X < x + width && point.Y > y && point.Y < y + height;
    }

    /// <summary>
    ///     Converts given key code to text
    /// </summary>
    /// <param name="keyCode">
    ///     The v Key.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    public static string KeyToText(uint keyCode)
    {
        /*A-Z */
        if (keyCode >= 65 && keyCode <= 90)
        {
            return ((char)keyCode).ToString();
        }

        /*F1-F12*/
        if (keyCode >= 112 && keyCode <= 123)
        {
            return "F" + (keyCode - 111);
        }

        return KeyCodeDictionary.ContainsKey(keyCode) ? KeyCodeDictionary[keyCode] : keyCode.ToString();
    }

    /// <summary>
    ///     Returns the md5 hash from a string.
    /// </summary>
    /// <param name="s">
    ///     The s.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    public static string Md5Hash(string s)
    {
        var sb = new StringBuilder();
        HashAlgorithm algorithm = MD5.Create();
        var h = algorithm.ComputeHash(Encoding.UTF8.GetBytes(s));

        foreach (var b in h)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    /// <summary>
    ///     The move camera.
    /// </summary>
    /// <param name="position">
    ///     The position.
    /// </param>
    public static void MoveCamera(Vector3 position)
    {
        GameConsoleManager.ExecuteCommand("dota_camera_set_lookatpos " + position.X + " " + position.Y);
    }

    /// <summary>
    ///     Converts given radian to degrees
    /// </summary>
    /// <param name="angle">
    ///     The angle.
    /// </param>
    /// <returns>
    ///     The <see cref="double" />.
    /// </returns>
    public static double RadianToDegree(double angle)
    {
        return angle * 180 / Math.PI;
    }

    /// <summary>
    ///     Sleeps the sleeping engine with the given id for given milliseconds. If engine is already sleeping for more than
    ///     the
    ///     given time it will be ignored.
    /// </summary>
    /// <param name="duration">
    ///     The duration.
    /// </param>
    /// <param name="name">
    ///     The name.
    /// </param>
    public static void Sleep(double duration, string name)
    {
        double dur;
        var tick = TickCount;
        if (!Sleeps.TryGetValue(name, out dur) || dur < tick + duration)
        {
            Sleeps[name] = tick + duration;
        }
    }

    /// <summary>
    ///     Checks sleeping status of the sleep engine with given id
    /// </summary>
    /// <param name="id">
    ///     The id.
    /// </param>
    /// <returns>
    ///     Returns true in case id was not found or is not sleeping
    /// </returns>
    public static bool SleepCheck(string id)
    {
        double time;
        return !Sleeps.TryGetValue(id, out time) || TickCount > time;
    }

    /// <summary>
    ///     Checks sleeping status of the sleep engine with given id
    /// </summary>
    /// <param name="id">
    ///     The id.
    /// </param>
    /// <param name="remainingTime">
    ///     The remaining time in milliseconds. 0 in case not sleeping.
    /// </param>
    /// <returns>
    ///     Returns true in case id was not found or is not sleeping
    /// </returns>
    public static bool SleepCheck(string id, out double remainingTime)
    {
        double time;
        var found = Sleeps.TryGetValue(id, out time);
        if (!found)
        {
            remainingTime = 0;
            return true;
        }

        remainingTime = time - TickCount;
        return remainingTime > 0;
    }
}