using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeacockActivityType
{
    PA_SING,
    PA_STORY,
    PA_HUG,

    PA_MAX
}

public static class PeacockActivityTypeExtensions
{
    public static string GetLabel(this PeacockActivityType activityType)
    {
        switch(activityType)
        {
            case PeacockActivityType.PA_SING:
                return "Sing";
            case PeacockActivityType.PA_STORY:
                return "Story";
            case PeacockActivityType.PA_HUG:
                return "Hug";
        }
        return "??";
    }
}