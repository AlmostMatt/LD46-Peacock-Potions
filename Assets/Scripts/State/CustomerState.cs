using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerState
{
    public static int totalPopulation = 1000;
    public static float storePopularity = 0;
    
    private static float[] _productDemand = new float[(int)ProductType.PT_MAX];
    public static float[] productDemand
    {
        get { return _productDemand; }
    }
    /*
     * // BALANCE: I actually don't like the seasonal stuff after all. The moving targets make it too confusing for a new player to get a grip on good prices.
    public static float[] productDemand
    {
        get
        {
            for(int i = 0; i < _productDemand.Length; ++i)
            {
                
                _productDemand[i] = ((ProductType)i).SeasonalDemand()[(int)GameState.season];
            }
            return _productDemand;
        }
    }
    */

    
    public static float[] optimalPrices = new float[(int)ProductType.PT_MAX];

    // recomputed each quarter by MainGameSystem
    public static int[] customers = new int[(int)ProductType.PT_MAX];
    public static int totalQuarterlyCustomers = 0;
}
