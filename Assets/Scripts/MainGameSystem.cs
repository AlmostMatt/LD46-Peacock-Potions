using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSystem : MonoBehaviour
{
    // Updated to the current quarter once demand has been generated for that quarter
    private int mQuarterlyDemandGenerated = -1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // auto-start the game
        // TODO: 
        if (GameState.currentStage == GameState.GameStage.GS_MAIN_MENU)
        {
            InitNewGame();
            GameState.currentStage = GameState.GameStage.GS_RESOURCE_ALLOCATION;
        }

        if (mQuarterlyDemandGenerated < GameState.quarter)
        {
            CalculateDemand();
            mQuarterlyDemandGenerated = GameState.quarter;
        }
    }

    private void InitNewGame()
    {
        // initialize events here for the moment...
        SonEventChain.Init();

        BusinessState.animals[(int)AnimalType.AT_TURTLE] = true; // player starts with a turtle
        BusinessState.money = 500; // some starting money
        InitWorldParams();
    }
    
    private void InitWorldParams()
    {
        // some initial values for demand calculation
        CustomerState.totalPopulation = 1000;
        CustomerState.storePopularity = 0.1f;

        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            CustomerState.productDemand[i] = Random.Range(0.1f, 0.9f); // TODO: ensure they sum to 1? maybe... not necessarily needed, but it would be good to ensure some minimum sum so that players at least get SOME customers
            CustomerState.optimalPrices[i] = Random.Range(50, 100);
        }
    }

    private void CalculateDemand()
    {
        // model demand for each product for the quarter, based on some hidden factors
        // each of these factors could be modified by events, the current time of year, the total time passed, etc.

        // number of customers for a given product:
        // N = (total population) * (demand for product) * (popularity of store) * (price curve)
        float[] prices = BusinessState.prices;

        CustomerState.totalQuarterlyCustomers = 0;
        float incomingCustomers = CustomerState.totalPopulation * CustomerState.storePopularity;
        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            float willingToPay = Mathf.Clamp(((CustomerState.optimalPrices[i] * 2) - prices[i]) / (CustomerState.optimalPrices[i] * 2), 0, 1);
            CustomerState.customers[i] = Mathf.RoundToInt(incomingCustomers * CustomerState.productDemand[i] * willingToPay);
            CustomerState.totalQuarterlyCustomers += CustomerState.customers[i];
            Debug.Log(CustomerState.customers[i] + " customers are willing to buy " + (ProductType)i + " for " + prices[i]);
        }
    }

}
