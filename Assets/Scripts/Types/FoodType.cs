using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType
{
    FT_CRAP,
    FT_BASIC,
    FT_DELUXE,

    FT_MAX
}

public static class FoodTypeExtensions
{
    public static int GetPrice(this FoodType food)
    {
        switch(food)
        {
            case FoodType.FT_CRAP:
                return 0;
            case FoodType.FT_BASIC:
                return 100;
            case FoodType.FT_DELUXE:
                return 400;
        }
        return 0;
    }

    public static string GetProperName(this FoodType food)
    {
        switch(food)
        {
            case FoodType.FT_CRAP:
                return "Cheap";
            case FoodType.FT_BASIC:
                return "Basic";
            case FoodType.FT_DELUXE:
                return "Deluxe";
        }
        return "Mystery";
    }

    public static string GetLabel(this FoodType food)
    {
        string name = food.GetProperName();
        string price = Utilities.FormatMoney(food.GetPrice());
        return name + " " + price;
    }
}