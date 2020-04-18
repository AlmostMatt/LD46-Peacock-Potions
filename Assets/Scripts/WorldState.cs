using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{
    public static int totalPopulation = 1000;
    public static float storePopularity = 0;
    
    public static float[] productDemand = new float[(int)ProductType.PT_MAX];
    public static float[] optimalPrices = new float[(int)ProductType.PT_MAX];

    // recomputed each quarter by MainGameSystem
    public static int[] customers = new int[(int)ProductType.PT_MAX];
    public static int totalQuarterlyCustomers = 0;
}
