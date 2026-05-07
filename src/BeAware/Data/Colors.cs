namespace BeAware.Data;

using System.Collections.Generic;

using Divine.Numerics;

internal static class Colors
{
    public static List<Vector3> HeroColors { get; } = new List<Vector3>
    {
        new Vector3(0.2f, 0.4588236f, 1), // 51 117 255
        new Vector3(0.4f, 1, 0.7490196f), // 102 255 191
        new Vector3(0.7490196f, 0, 0.7490196f), //191 0 191
        new Vector3(0.9529412f, 0.9411765f, 0.04313726f), // 243 240 11
        new Vector3(1, 0.4196079f, 0), // 255 107 0
        new Vector3(0.9960785f, 0.5254902f, 0.7607844f), // 254 134 194
        new Vector3(0.6313726f, 0.7058824f, 0.2784314f), // 161 180 71
        new Vector3(0.3960785f, 0.8509805f, 0.9686275f), // 101 217 247
        new Vector3(0, 0.5137255f, 0.1294118f), // 0 131 33
        new Vector3(0.6431373f, 0.4117647f, 0) // 164 105 0
    };
}