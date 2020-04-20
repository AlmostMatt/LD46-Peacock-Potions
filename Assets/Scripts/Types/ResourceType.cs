using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    RT_GREEN_FEATHER,
    RT_RED_FEATHER,
    RT_GOLD_FEATHER,
    RT_BLUE_FEATHER,

    RT_MAX
}

public static class ResourceTypeExtensions
{
    public static string GetImage(this ResourceType resourceType)
    {
        return "feather";
    }

    public static Color GetColor(this ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.RT_BLUE_FEATHER:
                return new Color(124f / 255f, 175f / 255f, 198f / 255f);
            case ResourceType.RT_GREEN_FEATHER:
                return new Color(39f / 255f, 140f / 255f, 81f / 255f);
            case ResourceType.RT_RED_FEATHER:
                return new Color(231f / 255f, 78f / 255f, 145f / 255f);
            case ResourceType.RT_GOLD_FEATHER:
                return new Color(231 / 255f, 161f / 255f, 25f / 255f);
            default: return Color.white;
        }
    }
}