using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PotionType
{
    PT_LOVE_POTION,
    PT_INVIS_POTION,
    PT_FIRE_POTION,
    PT_LUCK_POTION,
    PT_POISON_POTION,
        
    PT_MAX
}

public static class PotionTypeExtensions
{
    public static string GetImage(this PotionType PotionType)
    {
        return "potion";
    }

    public static string GetName(this PotionType PotionType)
    {
        switch (PotionType)
        {
            case PotionType.PT_LOVE_POTION:
                return "Love";
            case PotionType.PT_INVIS_POTION:
                return "Invisibility";
            case PotionType.PT_FIRE_POTION:
                return "Fire";
            case PotionType.PT_LUCK_POTION:
                return "Luck";
            case PotionType.PT_POISON_POTION:
                return "Poison";
            default: return PotionType.ToString();
        }
    }
    public static Color GetColor(this PotionType PotionType)
    {
        switch (PotionType)
        {
            case PotionType.PT_LOVE_POTION:
                return Color.magenta;
            case PotionType.PT_INVIS_POTION:
                return Color.white;
            case PotionType.PT_FIRE_POTION:
                return Color.red;
            case PotionType.PT_LUCK_POTION:
                return Color.yellow;
            case PotionType.PT_POISON_POTION:
                return Color.green;
            default: return Color.white;
        }
    }

    public static float[] SeasonalDemand(this PotionType product)
    {
        switch (product)
        {
            case PotionType.PT_FIRE_POTION:
                return new float[] { 0.3f, 0.1f, 0.5f, 0.7f };
            case PotionType.PT_INVIS_POTION:
                return new float[] { 0.2f, 0.2f, 0.2f, 0.2f };
            case PotionType.PT_LOVE_POTION:
                return new float[] { 0.4f, 0.2f, 0.2f, 0.3f };
            case PotionType.PT_LUCK_POTION:
                return new float[] { 0.2f, 0.2f, 0.2f, 0.2f };
            case PotionType.PT_POISON_POTION:
                return new float[] { 0.2f, 0.5f, 0.5f, 0.1f };
        }
        return new float[] { 0.1f, 0.1f, 0.1f, 0.1f };
    }

    public static FeatherAndCount[] GetIngredients(this PotionType PotionType)
    {
        switch (PotionType)
        {
            case PotionType.PT_LOVE_POTION:
                return new FeatherAndCount[] {
                    new FeatherAndCount(FeatherType.FT_RED_FEATHER, 2)};
            case PotionType.PT_INVIS_POTION:
                return new FeatherAndCount[] {
                    new FeatherAndCount(FeatherType.FT_GREEN_FEATHER, 1),
                    new FeatherAndCount(FeatherType.FT_BLUE_FEATHER, 1)};
            case PotionType.PT_FIRE_POTION:
                return new FeatherAndCount[] {
                    new FeatherAndCount(FeatherType.FT_GOLD_FEATHER, 1),
                    new FeatherAndCount(FeatherType.FT_GREEN_FEATHER, 1)};
            case PotionType.PT_LUCK_POTION:
                return new FeatherAndCount[] {
                    new FeatherAndCount(FeatherType.FT_RED_FEATHER, 1),
                    new FeatherAndCount(FeatherType.FT_BLUE_FEATHER, 1)};
            case PotionType.PT_POISON_POTION:
                return new FeatherAndCount[] {
                    new FeatherAndCount(FeatherType.FT_GOLD_FEATHER, 1)};
            default: return new FeatherAndCount[] { };
        }
    }

    // Compares the ingredients to make a product with the BusinessState's resources
    public static bool PlayerHasIngredients(this PotionType PotionType)
    {
        FeatherAndCount[] price = PotionType.GetIngredients();
        if (price.Length == 0)
        {
            // Don't let the player buy anything that is free (to avoid infinite loops)
            return false;
        }
        for (int r = 0; r < price.Length; r++)
        {
            if (GameData.singleton.resources[(int)price[r].type] < price[r].count)
            {
                return false;
            }
        }
        return true;
    }

    // Decreases BusinessState's resources by the price
    public static void SpendPlayerIngredients(this PotionType PotionType)
    {
        FeatherAndCount[] price = PotionType.GetIngredients();
        // Decrease funds
        for (int r = 0; r < price.Length; r++)
        {
            GameData.singleton.resources[(int)price[r].type] -= price[r].count;
        }
    }

    // Increases BusinessState's resources by the price
    public static void RefundPlayerIngredients(this PotionType PotionType)
    {
        FeatherAndCount[] price = PotionType.GetIngredients();
        // Increase funds
        for (int r = 0; r < price.Length; r++)
        {
            GameData.singleton.resources[(int)price[r].type] += price[r].count;
        }
    }
}