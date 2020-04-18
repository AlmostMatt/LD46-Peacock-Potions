using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProductType
{
    PT_LOVE_POTION,
    PT_INVIS_POTION,
    PT_MAGICAL_QUILL,
    PT_MAGIC_EGG,
        
    PT_MAX
}

public static class ProductTypeExtensions
{
    public static ResourceAndCount[] GetIngredients(this ProductType productType)
    {
        switch (productType)
        {
            case ProductType.PT_LOVE_POTION:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_RED_FEATHER, 1)};
            case ProductType.PT_INVIS_POTION:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_RED_FEATHER, 1),
                    new ResourceAndCount(ResourceType.RT_BLUE_FEATHER, 1)};
            case ProductType.PT_MAGICAL_QUILL:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_BLUE_FEATHER, 1),
                    new ResourceAndCount(ResourceType.RT_GREEN_FEATHER, 1)};
            case ProductType.PT_MAGIC_EGG:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_GOLD_FEATHER, 1)};
        default: return new ResourceAndCount[] { };
        }
    }
}