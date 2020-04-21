using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfStockEvent : GameEvent
{
    public static int nextAllowedQuarter = 0;

    private ProductType mProductType;

    public OutOfStockEvent(ProductType productType)
    {
        mProductType = productType;
    }

    protected override EventResult OnStage(EventStage currentStage)
    {
        switch (currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = string.Format("A woman approaches the counter. \"Do you have any {0} potions?\"", mProductType.GetName().ToLower());
                EventState.currentEventOptions = new string[]
                {
                    "Tell her you should have some next season.",
                    "Tell her you're not sure when you'll have more."
                };
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                return EventResult.CONTINUE;
            case EventStage.ACCEPT:
                EventState.currentEventText = "\"Oh okay, I'll try again next season. Thanks!\" the woman says as she leaves.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                EventState.PushEvent(new OutOfStockReturnEvent(mProductType), GameState.quarter + 1, 0.6f);
                return EventResult.DONE;
            case EventStage.REFUSE:
                EventState.currentEventText = "\"Oh okay. I'll have to look elsewhere,\" she says. She takes her leave.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                CustomerState.storePopularity *= 0.95f;
                nextAllowedQuarter = GameState.quarter + 2;
                return EventResult.DONE;
        }
        return EventResult.CONTINUE;
    }

    private class OutOfStockReturnEvent : GameEvent
    {
        private ProductType mProductType;
        private bool mHasPotion = false;

        public OutOfStockReturnEvent(ProductType productType)
        {
            mProductType = productType;
        }

        protected override EventResult OnStage(EventStage currentStage)
        {
            string potionNameLower = mProductType.GetName().ToLower();
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "A familiar woman approaches the counter.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.DECIDE };
                    return EventResult.CONTINUE;
                case EventStage.DECIDE:
                    EventState.currentEventText = "A familiar woman approaches the counter.";
                    if (mHasPotion)
                    {
                        EventState.currentEventText = string.Format("\"Hi! I was in here last season looking for a {0} potion. Do you have any now?\"", potionNameLower);
                        EventState.currentEventOptions = new string[]
                        {
                            string.Format("Sell her a {0} potion", potionNameLower),
                            "Say no"
                        };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    }
                    else
                    {
                        EventState.currentEventText = string.Format("\"Hi, I was in here last season looking for a {0} potion, and you told me to return. I still don't see any, though.\"", potionNameLower);
                        EventState.currentEventOptions = new string[]
                        {
                            "Apologize"
                        };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.UNABLE };
                    }
                    return EventResult.CONTINUE;
                case EventStage.ACCEPT:
                    EventState.currentEventText = "She thanks you and pays for her potion.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    CustomerState.storePopularity *= 1.1f;
                    BusinessSystem.SellProduct((int)mProductType);
                    break;
                case EventStage.REFUSE:
                    EventState.currentEventText = "The woman is visibly annoyed. She leaves briskly.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    CustomerState.storePopularity *= 0.85f; EventState.currentEventText = "\"Oh okay, I'll try again next season. Thanks!\" the woman says as she leaves.";
                    break;
                case EventStage.UNABLE:
                    EventState.currentEventText = "She looks disappointed as she leaves the store empty-handed.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    CustomerState.storePopularity *= 0.9f;
                    break;
            }
            return EventResult.DONE;
        }

        protected override EventResult EventStart()
        {
            mHasPotion = BusinessState.inventory[(int)mProductType] > 0;
            return EventResult.CONTINUE;
        }
    }
}
