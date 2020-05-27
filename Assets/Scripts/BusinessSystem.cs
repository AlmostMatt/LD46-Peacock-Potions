using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessSystem : MonoBehaviour
{
    private float mPaymentTimer = 0;
    private float mPaymentTime = 0;
    private const float QUARTER_TIME = 7f;
    public static float QuarterTotalTime
    {
        get { return DebugOverrides.QuarterDuration.GetValueOrDefault(QUARTER_TIME); }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
        if(EventState.currentEvent != null) return; // no updating while an event is happening

        if (GameData.singleton.currentStage == GameStage.GS_SIMULATION)
        {
            if(GameData.singleton.totalQuarterlyCustomers <= 0) // no more customers to handle
            {
                if(!EventState.hasMoreEventsRightNow && GameData.singleton.quarterTimeElapsed >= QuarterTotalTime)
                {                    
                    // transition to "end of quarter" stage
                    Debug.Log("All customers served this quarter.");
                    GameStageExtensions.GameLoopGotoNextStage();
                }
            }
            else
            {
                // yeah like it's slightly gross to do this here in this way...
                // running code on state changes really does want an event-driven style thing, I guess
                if (mPaymentTime == 0)
                {
                    mPaymentTime = QuarterTotalTime / GameData.singleton.totalQuarterlyCustomers;
                    mPaymentTimer = mPaymentTime;
                }

                mPaymentTimer -= Time.deltaTime;
                if (mPaymentTimer <= 0)
                {
                    int product = Random.Range(0, (int)PotionType.PT_MAX);
                    while (GameData.singleton.quarterlyCustomers[product] == 0)
                    {
                        product = (product + 1) % (int)PotionType.PT_MAX;
                    }

                    if(GameData.singleton.potionsOwned[product] > 0)
                    {
                        SellProduct(product);
                    }
                    else
                    {
                        GameData.singleton.unfulfilledDemand[product] += 1;
                        Debug.Log("A customer wanted " + (PotionType)product + " but we were out of stock");

                        // get enough of this, and we queue up an event where a customer asks for this type specifically
                        if(GameData.singleton.unfulfilledDemand[product] > 3 && GameData.singleton.outOfStockEventCooldown <= GameData.singleton.quarter)
                        {
                            EventState.PushEvent(new OutOfStockEvent((PotionType)product), GameData.singleton.quarter);
                            GameData.singleton.outOfStockEventCooldown = GameData.singleton.quarter + 2;
                        }
                    }

                    mPaymentTimer = Random.Range(mPaymentTime * 0.9f, mPaymentTime * 1.1f);
                    GameData.singleton.quarterlyCustomers[product] -= 1;
                    --GameData.singleton.totalQuarterlyCustomers;
                }
            }
        }
        else if(GameData.singleton.currentStage == GameStage.GS_END_OF_QUARTER)
        {
            if(GameData.singleton.money < GameData.singleton.rent && !GameData.singleton.missedRent)
            {
                if(GameData.singleton.lastMissedRentQuarter == -1)
                {
                    EventState.PushEvent(new MissedRentEvents.MissedRentEvent(), GameData.singleton.quarter, 0, GameStage.GS_END_OF_QUARTER); // going to miss rent? add event
                    GameData.singleton.lastMissedRentQuarter = GameData.singleton.quarter;
                }
                else if(GameData.singleton.lastMissedRentQuarter == GameData.singleton.quarter - 1)
                {
                    EventState.PushEvent(new MissedRentEvents.MissedRentAgainEvent(), GameData.singleton.quarter, 0, GameStage.GS_END_OF_QUARTER); // game over
                    GameData.singleton.missedRent = true;
                }
                else
                {
                    Debug.LogError("Missed rent in quarter " + GameData.singleton.lastMissedRentQuarter + " but it's now " + GameData.singleton.quarter + ", and somehow it wasn't cleared?");
                }
            }
            else if(GameData.singleton.lastMissedRentQuarter < GameData.singleton.quarter)
            {
                GameData.singleton.lastMissedRentQuarter = -1;
            }

            if(GameData.singleton.peacockHealth <= 0 && !GameData.singleton.peacockDied)
            {
                EventState.PushEvent(new PeacockSickEvent(), GameData.singleton.quarter, 0, GameStage.GS_END_OF_QUARTER);
                GameData.singleton.peacockDied = true;
            }

            if(GameData.singleton.quarter >= 16 && !GameData.singleton.reachedEndOfLife && !GameData.singleton.missedRent && !GameData.singleton.peacockDied)
            {
                EventState.PushEvent(new EndOfLifeEvent(), GameData.singleton.quarter, 0);
                GameData.singleton.reachedEndOfLife = true;
            }

            if(!EventState.hasMoreEventsRightNow) // ensure we play through all events before advancing to next quarter
            {
                mPaymentTime = 0;

                // Advance to the next quarter and update any other affected state
                // Go tho the end-of-quarter summary state (or game over state)
                if (GameData.singleton.reachedEndOfLife)
                {
                    MainGameSystem.GameOver();
                }
                else
                {
                    // This will advance to the summaries after the end of the quarter
                    GameStageExtensions.GameLoopGotoNextStage();
                }
                Debug.Log("game stage is now " + GameData.singleton.currentStage);
            }
        }
    }

    public static void SellProduct(int product)
    {
        GameData.singleton.money += GameData.singleton.potionPrices[product];
        GameData.singleton.potionsOwned[product] -= 1;
        GameData.singleton.quarterlySales[product] += 1;

        Debug.Log("Sold a " + (PotionType)product + "! money: " + GameData.singleton.money);

        // Hacky way to call code from another system
        GameObject.FindObjectsOfType<UIControllerSystem>()[0].ShowSale((PotionType) product);

        // Have a random chance to spawn an event
        // TODO: move event spawning into a new system
        if (Random.Range(0f, 1f) <= 0.01f && GameData.singleton.quarter > 4)
        {
            EventState.PushEvent(new ThiefEvent(), GameData.singleton.quarter);
        }
    }
}
