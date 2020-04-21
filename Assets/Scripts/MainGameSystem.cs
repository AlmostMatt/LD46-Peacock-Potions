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
            GameState.quarter = -1; // hack it to start at 0
            StartNextQuarter();
            // Start with simulation
            GameState.currentStage = GameState.GameStage.GS_SIMULATION;
        }
        else if(GameState.currentStage == GameState.GameStage.GS_SIMULATION && EventState.currentEvent == null)
        {
            GameState.quarterTime += Time.deltaTime;
        }
    }

    private void InitNewGame()
    {
        // initialize events here for the moment..
        EventState.PushEvent(new TutorialEventChain.IntroductionEvent(), 0, 0);
        // SonEventChain.Init();
        WifeEventChain.Init();
        InvestmentEventChain.Init();
        
        BusinessState.money = 1000;
        BusinessState.quarterlyReport.initialBalance = (int)BusinessState.money;
        // Set final balance for the previous quarter so that next Q can use it as initial balance
        BusinessState.quarterlyReport.finalBalance = (int)BusinessState.money;
        BusinessState.rent = 250;

        // starting resources
        for(int i = 0; i < BusinessState.resources.Length; ++i)
        {
            BusinessState.resources[i] = Random.Range(1, 10);
        }

        // starting inventory
        for(int i = 0; i < BusinessState.inventory.Length; ++i)
        {
            BusinessState.inventory[i] = 10;
            BusinessState.prices[i] = 50;
            BusinessState.quarterlyReport.salePrices[i] = 50;
        }

        InitWorldParams();
    }
    
    private void InitWorldParams()
    {
        // some initial values for demand calculation
        CustomerState.totalPopulation = 70;
        CustomerState.storePopularity = 0.25f;

        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            CustomerState.productDemand[i] = Random.Range(0.3f, 0.7f); // TODO: ensure they sum to 1? maybe... not necessarily needed, but it would be good to ensure some minimum sum so that players at least get SOME customers
            CustomerState.optimalPrices[i] = Random.Range(5,150); // TODO: non-uniform distribution
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
     * Starts a new quarter (beginning of simulation)
     * reset anything that needs to be reset
     */
    public static void StartNextQuarter()
    {
        // Set the report's initial balance to the previous report's final balance
        int newInitialBalance = BusinessState.quarterlyReport.finalBalance;
        BusinessState.quarterlyReport = new BusinessState.QuarterlyReport();
        BusinessState.quarterlyReport.initialBalance = newInitialBalance;

        GameState.quarter += 1;
        GameState.quarterTime = 0;
        // Take a snapshot of the current prices for reports
        System.Array.Copy(BusinessState.prices, BusinessState.quarterlyReport.salePrices,BusinessState.prices.Length);

        BusinessState.peacock.StartQuarter();
        CalculateDemand();
    }

    /**
     * End of simulation (prepare end-of-Q reports)
     */
    public static void EndCurrentQuarter()
    {
        // expenses. We could do this as an event at the end of the quarter, if we wanted. Though that could get a bit repetitive.
        BusinessState.quarterlyReport.livingExpenses = BusinessState.rent;
        // Take a snapshot of the current inventory for end-of-q report
        System.Array.Copy(BusinessState.inventory, BusinessState.quarterlyReport.unsoldPotions, BusinessState.inventory.Length);
        BusinessState.peacock.QuarterOver();
    }

    /**
     * Causess peacock-related payments to happen
     */
    public static void PayPeacockExpenses()
    {
        BusinessState.money -= BusinessState.peacock.quarterlyTotalCost;
    }

    /**
     * Causess rent payment to actually happen
     */
    public static void PayEndOfQuarterExpenses()
    {
        BusinessState.money -= BusinessState.rent;
        // just after rent payment is the timing that is used for end-of-q and start-of-q balance
        BusinessState.quarterlyReport.finalBalance = (int)BusinessState.money;
    }

    public static void GameOver()
    {
        GameState.epilogueDirty = true;
        GameState.epilogueLines.Add("<b>Epilogue</b>");
        GameState.epilogueLines.Add("You died");
        GameState.epilogueLines.Add("Your wife died");

        // Summarize family relationships
        // wife relationship tiers [<0 >0]
        // son relationship tiers [<0 >0]

        bool happyMarriage = RelationshipState.wifeMarried;


        // Did you die of old age?
        if (GameState.reachedEndOfLife)
        {
            GameState.epilogueLines.Add("You kept the business alive for " + GameState.elapsedYears + " years before retiring.");
            // Did anyone inherit?
            // TODO: is there a more explicit relationship between son and plan to inherit
            if (RelationshipState.sonWasBorn && RelationshipState.sonRelationship > 0f)
            {
                GameState.epilogueLines.Add("Your son inherited the business, and the business will live on for generations to come.");
            } else if (RelationshipState.sonWasBorn)
            {
                GameState.epilogueLines.Add("Your son had no interest in the business, so it closed down after you retired.");
            }
            else
            {
                GameState.epilogueLines.Add("You had no children, so there was nobody to inherit the business after you retired.");
            }
        } else
        {
            // If so, what was the last expense? (event vs rent)
            // Did Peacock die?
            // Otherwise the business failed for financial reasons
            string causeOfFailure = "before going bankrupt.";
            GameState.epilogueLines.Add("You kept the business alive for " + GameState.elapsedYears + " years "+ causeOfFailure);
        }

        GameState.currentStage = GameState.GameStage.GS_GAME_OVER;
    }
}
