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
        if(GameData.singleton.currentStage == GameState.GameStage.GS_MAIN_MENU)
        {
            // testing save/load
            if(DebugOverrides.ShouldLoadSaveData && GameData.LoadGame())
            {
                Debug.Log("loaded game. resuming at " + GameData.singleton.currentStage.ToString());
                if (GameData.singleton.eventSaveData != null)
                {
                    GameEventSystem.Load(GameData.singleton.eventSaveData);
                } else
                {
                    Debug.LogWarning("eventSaveData was null");
                }
            }
            else
            {
                InitNewGame();
                GameData.singleton.quarter = -1; // hack it to start at 0
                StartNextQuarter();
                            
                // Start with simulation
                GameData.singleton.currentStage = GameState.GameStage.GS_SIMULATION;
            }
            if (DebugOverrides.StartState.HasValue)
            {
                // TODO: figure out what functions need to be called here to make various stages somewhat functional
                // OR - just fast-forward and auto-OK until the desired state is reached
                GameData.singleton.currentStage = DebugOverrides.StartState.Value;
            }
        }
        else if(GameData.singleton.currentStage == GameState.GameStage.GS_SIMULATION && EventState.currentEvent == null)
        {
            GameData.singleton.quarterTimeElapsed += Time.deltaTime;
        }
    }

    private void InitNewGame()
    {
        // initialize events here for the moment..
        EventState.PushEvent(new TutorialEventChain.IntroductionEvent(), 0, 0);
        WifeEventChain.Init();
        InvestmentEventChain.Init();
        
        GameData.singleton.money = 1000;
        GameData.singleton.initialBalance = GameData.singleton.money;
        // Set final balance for the previous quarter so that next Q can use it as initial balance
        GameData.singleton.finalBalance = GameData.singleton.money;
        GameData.singleton.rent = 300;

        // starting resources
        for(int i = 0; i < GameData.singleton.resources.Length; ++i)
        {
            GameData.singleton.resources[i] = 10;
        }

        // starting inventory
        for(int i = 0; i < GameData.singleton.inventory.Length; ++i)
        {
            GameData.singleton.inventory[i] = 10;
            GameData.singleton.potionPrices[i] = 50;
            GameData.singleton.quarterlyReportSalePrices[i] = 50;
        }

        InitWorldParams();
    }
    
    private void InitWorldParams()
    {
        // some initial values for demand calculation
        GameData.singleton.totalPopulation = 50;
        GameData.singleton.storePopularity = 0.25f;

        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            GameData.singleton.productDemand[i] = Random.Range(0.3f, 0.6f); // TODO: ensure they sum to 1? maybe... not necessarily needed, but it would be good to ensure some minimum sum so that players at least get SOME customers
            GameData.singleton.optimalPrices[i] = Random.Range(5,150); // TODO: non-uniform distribution
        }
    }

    private static void CalculateDemand()
    {
        // model demand for each product for the quarter, based on some hidden factors
        // each of these factors could be modified by events, the current time of year, the total time passed, etc.

        // number of customers for a given product:
        // N = (total population) * (demand for product) * (popularity of store) * (price curve)

        GameData.singleton.totalQuarterlyCustomers = 0;
        float incomingCustomers = GameData.singleton.totalPopulation * GameData.singleton.storePopularity;
        for(int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            float willingToPay = Mathf.Clamp(((GameData.singleton.optimalPrices[i] * 2) - GameData.singleton.potionPrices[i]) / (GameData.singleton.optimalPrices[i] * 2), 0, 1);
            GameData.singleton.quarterlyCustomers[i] = Mathf.RoundToInt(incomingCustomers * GameData.singleton.productDemand[i] * willingToPay);
            GameData.singleton.totalQuarterlyCustomers += GameData.singleton.quarterlyCustomers[i];
            Debug.Log(GameData.singleton.quarterlyCustomers[i] + " customers are willing to buy " + (ProductType)i + " for " + GameData.singleton.potionPrices[i]);
        }
    }

     private static void ResetQuarterlyReport()
     {
        // all the other data is stuff we can leave,
        // but these ones get inc'd, so they need to have their values reset
        System.Array.Clear(GameData.singleton.quarterlySales, 0, GameData.singleton.quarterlySales.Length);
        System.Array.Clear(GameData.singleton.unfulfilledDemand, 0, GameData.singleton.unfulfilledDemand.Length);
        System.Array.Clear(GameData.singleton.miscLosses, 0, GameData.singleton.miscLosses.Length);        
        GameData.singleton.miscLosses = new int[(int)ProductType.PT_MAX];        
        GameData.singleton.eventIncome = 0;
        GameData.singleton.eventExpenses = 0;
     }

    /**
     * Starts a new quarter (beginning of simulation)
     * reset anything that needs to be reset
     */
    public static void StartNextQuarter()
    {
        ResetQuarterlyReport();

        if(GameData.singleton.quarter >= 4)
        {
            GameData.singleton.storePopularity += 0.02f;
        }

        GameData.singleton.quarter += 1;
        GameData.singleton.quarterTimeElapsed = 0;
        // Take a snapshot of the current prices for reports
        System.Array.Copy(GameData.singleton.potionPrices, GameData.singleton.quarterlyReportSalePrices, GameData.singleton.potionPrices.Length);

        CalculateDemand();

        GameData.singleton.currentStage = GameState.GameStage.GS_SIMULATION;

        // GameData.SaveGame();
    }

    /**
     * End of simulation (prepare end-of-Q reports)
     */
    public static void EndCurrentQuarter()
    {
        GameData.singleton.livingExpenses = GameData.singleton.rent;
        // Take a snapshot of the current inventory for end-of-q report
        System.Array.Copy(GameData.singleton.inventory, GameData.singleton.unsoldPotions, GameData.singleton.inventory.Length);
        Peacock.EndOfQuarter();
    }

    /**
     * Causes peacock-related payments to happen
     */
    public static void PayPeacockExpenses()
    {
        GameData.singleton.money -= GameData.singleton.peacockQuarterlyTotalCost;
    }

    /**
     * Causes rent payment to actually happen
     */
    public static void PayEndOfQuarterExpenses()
    {
        GameData.singleton.money -= GameData.singleton.rent;
        // just after rent payment is the timing that is used for end-of-q and start-of-q balance
        GameData.singleton.finalBalance = GameData.singleton.money;
    }

    public static void GameOver()
    {
        GameState.epilogueDirty = true;
        GameState.epilogueLines.Add("<b>Epilogue</b>");

        // Summarize family relationships
        if (GameData.singleton.wifeMarried)
        {
            if (GameData.singleton.wifeRelationship > 0f)
            {
                GameState.epilogueLines.Add("You married " + WifeEventChain.NAME + " and had a happy life together.");
            } else
            {
                GameState.epilogueLines.Add("You married " + WifeEventChain.NAME + ", and had happy beginnings but some disagreements later in life.");
            }
        } else
        {
            //GameState.epilogueLines.Add("Your wife died");
        }
        // wife relationship tiers [<0 >0]
        // son relationship tiers [<0 >0]

        bool happyMarriage = GameData.singleton.wifeMarried;

        // Did you die of old age?
        if (GameData.singleton.reachedEndOfLife)
        {
            // wealth > X

            GameState.epilogueLines.Add("You kept the business alive for " + GameData.singleton.elapsedYears + " years before retiring.");
            // Did anyone inherit?
            // TODO: is there a more explicit relationship between son and plan to inherit
            if (GameData.singleton.sonWasBorn && GameData.singleton.sonRelationship > 5f)
            {
                GameState.epilogueLines.Add("Your son inherited the business, so the business will live on for generations to come.");
            } else if (GameData.singleton.sonWasBorn)
            {
                GameState.epilogueLines.Add("Your son had no interest in the business, so it closed down after you retired.");
            }
            else
            {
                GameState.epilogueLines.Add("You had no children, so there was nobody to inherit the business after you retired.");
            }
        } else
        {
            // did the peacock die?
            if(GameData.singleton.peacockDied)
            {
                string causeOfFailure = ", but its death ended the business.";
                GameState.epilogueLines.Add("You kept the peacock alive for " + GameData.singleton.elapsedYears + " years"+ causeOfFailure);
            }
            else if(GameData.singleton.missedRent)
            {
                string causeOfFailure = "before going bankrupt.";
                GameState.epilogueLines.Add("You kept the business alive for " + GameData.singleton.elapsedYears + " years "+ causeOfFailure);
            }

        }

        GameData.singleton.currentStage = GameState.GameStage.GS_GAME_OVER;
    }
}
