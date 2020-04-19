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
                return 1000;
            case FoodType.FT_DELUXE:
                return 5000;
        }
        return 0;
    }
}