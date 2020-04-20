using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSystem : MonoBehaviour
{
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
            StartNextQuarter();
            // Start with simulation
            GameState.currentStage = GameState.GameStage.GS_SIMULATION;
        }
        else if(GameState.currentStage == GameState.GameStage.GS_SIMULATION)
        {
            GameState.quarterTime += Time.deltaTime;
        }        
    }

    private void InitNewGame()
    {
        // initialize events here for the moment..
        EventState.PushEvent(new IntroductionEvent(), 0, 0);
        SonEventChain.Init();
        InvestmentEventChain.Init();
        
        BusinessState.money = 1000;
        BusinessState.rent = 250;

        // starting resources
        for(int i = 0; i < BusinessState.resources.Length; ++i)
        {
            BusinessState.resources[i] = Random.Range(1, 10);
        }

        // starting inventory
        for(int i = 0; i < BusinessState.inventory.Length; ++i)
        {
            BusinessState.inventory[i] = 5;
            BusinessState.prices[i] = 50;
            BusinessState.quarterlyReport.salePrices[i] = 50;
        }

        InitWorldParams();
    }
    
    private void InitWorldParams()
    {
        // some initial values for demand calculation
        CustomerState.totalPopulation = 1000;
        CustomerState.storePopularity = 0.01f;

        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            CustomerState.productDemand[i] = Random.Range(0.3f, 0.7f); // TODO: ensure they sum to 1? maybe... not necessarily needed, but it would be good to ensure some minimum sum so that players at least get SOME customers
            CustomerState.optimalPrices[i] = Random.Range(30,70); // TODO: non-uniform distribution
        }
    }

    private static void CalculateDemand()
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
   
    /**
     * Starts a new quarter (not to be confused with UIController's EndQuarter)
     */
    public static void StartNextQuarter()
    {
        GameState.quarter += 1;
        GameState.quarterTime = 0;
        BusinessState.quarterlyReport = new BusinessState.QuarterlyReport();
        BusinessState.peacock.StartQuarter();
        CalculateDemand();
    }

    public static void EndCurrentQuarter()
    {
        // expenses. We could do this as an event at the end of the quarter, if we wanted. Though that could get a bit repetitive.                    
        int expenses = BusinessState.rent;
        BusinessState.quarterlyReport.livingExpenses = expenses;
        BusinessState.money -= expenses;
        BusinessState.peacock.QuarterOver();
    }
}
