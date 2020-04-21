using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessSystem : MonoBehaviour
{
    private float mPaymentTimer = 0;
    private float mPaymentTime = 0;
    private float QUARTER_TIME = 10f; // 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If in any other state (including random event), simulation will be paused
        if (GameState.currentStage == GameState.GameStage.GS_SIMULATION && EventState.currentEvent == null)
        {
            if (CustomerState.totalQuarterlyCustomers <= 0) // won't make any more money from customers...
            {
                if(!EventState.hasMoreEventsRightNow) // won't make any more money from events...
                {
                    if(BusinessState.money < BusinessState.rent && !BusinessState.missedRent)
                    {
                        EventState.PushEvent(new MissedRentEvent(), GameState.quarter, 0); // going to miss rent? add event
                        BusinessState.missedRent = true;
                    }

                    if(!EventState.hasMoreEventsRightNow && GameState.quarterTime >= QUARTER_TIME) // ensure we play through all events before advancing to next quarter
                    {
                        mPaymentTime = 0;

                        Debug.Log("All customers served this quarter.");
                        // Advance to the next quarter and update any other affected state
                        // Go tho the end-of-quarter summary state (or game over state)
                        if (GameState.reachedEndOfLife)
                        {
                            // TODO: something somewhere will check for proper game over (player death or business going under)
                            MainGameSystem.GameOver();
                        }
                        else
                        {
                            // This will call MainGameSystem.EndCurrentQuarter
                            GameState.GameLoopGotoNextStage();
                        }
                        Debug.Log("game stage is now " + GameState.currentStage);
                        return;
                    }
                }
            }
            else
            {
                 // yeah like it's slightly gross to do this here in this way...
                // running code on state changes really does want an event-driven style thing, I guess
                if (mPaymentTime == 0)
                {
                    mPaymentTime = QUARTER_TIME / CustomerState.totalQuarterlyCustomers;
                    mPaymentTimer = mPaymentTime;
                }

                mPaymentTimer -= Time.deltaTime;
                if (mPaymentTimer <= 0)
                {
                    int product = Random.Range(0, (int)ProductType.PT_MAX);
                    while (CustomerState.customers[product] == 0)
                    {
                        product = (product + 1) % (int)ProductType.PT_MAX;
                    }

                    if(BusinessState.inventory[product] > 0)
                    {
                        SellProduct(product);
                    }
                    else
                    {
                        BusinessState.quarterlyReport.unfulfilledDemand[product] += 1;
                        Debug.Log("A customer wanted " + (ProductType)product + " but we were out of stock");

                        // get enough of this, and we queue up an event where a customer asks for this type specifically
                        if(BusinessState.quarterlyReport.unfulfilledDemand[product] > 3 && OutOfStockEvent.nextAllowedQuarter <= GameState.quarter)
                        {
                            EventState.PushEvent(new OutOfStockEvent((ProductType)product), GameState.quarter);
                            OutOfStockEvent.nextAllowedQuarter = GameState.quarter + 2;
                        }
                    }

                    mPaymentTimer = Random.Range(mPaymentTime * 0.9f, mPaymentTime * 1.1f);
                    CustomerState.customers[product] -= 1;
                    --CustomerState.totalQuarterlyCustomers;
                }
            }
        }
    }

    public static void SellProduct(int product)
    {
        BusinessState.money += BusinessState.prices[product];
        BusinessState.inventory[product] -= 1;
        Debug.Log("Sold a " + (ProductType)product + "! money: " + BusinessState.money);

        BusinessState.quarterlyReport.sales[product] += 1;

        // Hacky way to call code from another system
        GameObject.FindObjectsOfType<UIControllerSystem>()[0].ShowSale((ProductType) product);

        // Have a random chance to spawn an event
        // TODO: move event spawning into a new system
        if (Random.Range(0f, 1f) <= 0.01f && GameState.quarter > 4)
        {
            EventState.PushEvent(new ThiefEvent(), GameState.quarter);
        }
    }
}
