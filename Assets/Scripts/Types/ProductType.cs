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
    public static string GetImage(this ProductType productType)
    {
        return "potion";
    }

    public static string GetName(this ProductType productType)
    {
        switch (productType)
        {
            case ProductType.PT_LOVE_POTION:
                return "Love";
            case ProductType.PT_INVIS_POTION:
                return "Invisibility";
            case ProductType.PT_FIRE_POTION:
                return "Fire";
            case ProductType.PT_LUCK_POTION:
                return "Luck";
            case ProductType.PT_POISON_POTION:
                return "Poison";
            default: return productType.ToString();
        }
    }
    public static Color GetColor(this ProductType productType)
    {
        switch (productType)
        {
            case ProductType.PT_LOVE_POTION:
                return Color.magenta;
            case ProductType.PT_INVIS_POTION:
                return Color.white;
            case ProductType.PT_FIRE_POTION:
                return Color.red;
            case ProductType.PT_LUCK_POTION:
                return Color.cyan;
            case ProductType.PT_POISON_POTION:
                return Color.black;
            default: return Color.white;
        }
    }

    public static float[] SeasonalDemand(this ProductType product)
    {
        switch (product)
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
            case ProductType.PT_FIRE_POTION:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_BLUE_FEATHER, 1),
                    new ResourceAndCount(ResourceType.RT_GREEN_FEATHER, 1)};
            case ProductType.PT_LUCK_POTION:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_RED_FEATHER, 1),
                    new ResourceAndCount(ResourceType.RT_GREEN_FEATHER, 1)};
            case ProductType.PT_POISON_POTION:
                return new ResourceAndCount[] {
                    new ResourceAndCount(ResourceType.RT_GOLD_FEATHER, 1)};
            default: return new ResourceAndCount[] { };
        }
    }

    // Compares the ingredients to make a product with the BusinessState's resources
    public static bool PlayerHasIngredients(this ProductType productType)
    {
        ResourceAndCount[] price = productType.GetIngredients();
        if (price.Length == 0)
        {
            // Don't let the player buy anything that is free (to avoid infinite loops)
            return false;
        }
        for (int r = 0; r < price.Length; r++)
        {
            if (BusinessState.resources[(int)price[r].type] < price[r].count)
            {
                return false;
            }
        }
        return true;
    }

    // Decreases BusinessState's resources by the price
    public static void SpendPlayerIngredients(this ProductType productType)
    {
        ResourceAndCount[] price = productType.GetIngredients();
        // Decrease funds
        for (int r = 0; r < price.Length; r++)
        {
            BusinessState.resources[(int)price[r].type] -= price[r].count;
        }
    }

    // Increases BusinessState's resources by the price
    public static void RefundPlayerIngredients(this ProductType productType)
    {
        ResourceAndCount[] price = productType.GetIngredients();
        // Increase funds
        for (int r = 0; r < price.Length; r++)
        {
            BusinessState.resources[(int)price[r].type] += price[r].count;
        }
    }
}