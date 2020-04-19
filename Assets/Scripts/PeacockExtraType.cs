using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeacockExtraType
{
    ET_MEDICINE,
    ET_PILLOW,
    ET_HORMONES,

    ET_MAX
}

public static class PeacockExtraTypeExtensions
{
    public static int GetPrice(this PeacockExtraType food)
    {
        switch(food)
        {
            case PeacockExtraType.ET_MEDICINE:
                return 1500;
            case PeacockExtraType.ET_PILLOW:
                return 500;
            case PeacockExtraType.ET_HORMONES:
                return 1000;
        }
        return 0;
    }
}