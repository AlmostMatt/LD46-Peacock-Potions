using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeatherType
{
    FT_GREEN_FEATHER,
    FT_RED_FEATHER,
    FT_GOLD_FEATHER,
    FT_BLUE_FEATHER,

    FT_MAX
}

public static class FeatherTypeExtensions
{
    public static string GetImage(this FeatherType FeatherType)
    {
        return "feather";
    }

    public static Color GetColor(this FeatherType FeatherType)
    {
        switch (FeatherType)
        {
            case FeatherType.FT_BLUE_FEATHER:
                return new Color(124f / 255f, 175f / 255f, 198f / 255f);
            case FeatherType.FT_GREEN_FEATHER:
                return new Color(39f / 255f, 140f / 255f, 81f / 255f);
            case FeatherType.FT_RED_FEATHER:
                return new Color(231f / 255f, 78f / 255f, 145f / 255f);
            case FeatherType.FT_GOLD_FEATHER:
                return new Color(231 / 255f, 161f / 255f, 25f / 255f);
            default: return Color.white;
        }
    }
}