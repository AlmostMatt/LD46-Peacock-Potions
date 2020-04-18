using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProductType
{
    PT_LOVE_POTION,
    PT_INVIS_POTION,
    PT_FIRE_POTION,
    PT_LUCK_POTION,
    PT_POISON_POTION,
        
    PT_MAX
}

public static class ProductTypeExtensions
{
    public static float[] SeasonalDemand(this ProductType product)
    {
        switch(product)
        {
            case ProductType.PT_FIRE_POTION:
                return new float[] { 0.3f, 0.1f, 0.5f, 0.7f };
            case ProductType.PT_INVIS_POTION:
                return new float[] { 0.2f, 0.2f, 0.2f, 0.2f };
            case ProductType.PT_LOVE_POTION:
                return new float[] { 0.4f, 0.2f, 0.2f, 0.3f };
            case ProductType.PT_LUCK_POTION:
                return new float[] { 0.2f, 0.2f, 0.2f, 0.2f };
            case ProductType.PT_POISON_POTION:
                return new float[] { 0.2f, 0.5f, 0.5f, 0.1f };
        }
        return new float[] { 0.1f, 0.1f, 0.1f, 0.1f };
    }
}