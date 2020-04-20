using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfStockEvent : GameEvent
{
    public static bool onCooldown = false;

    private ProductType mProductType;

    public OutOfStockEvent(ProductType productType)
    {
        mProductType = productType;
    }

    protected override EventResult EventStart()
    {
        EventState.currentEventText = string.Format("A woman approaches the counter. \"Do you have any {0} potions?\"", mProductType.GetName().ToLower());
        EventState.currentEventOptions = new string[]
        {
            "Tell her you don't have any now, but you should have some next season.",
            "Tell her you don't have any now, and you're not sure when you'll have more."
        };
        return EventResult.CONTINUE;
    }

    protected override EventResult OnPlayerDecision(int choice)
    {
        switch(choice)
        {
            case 0:
                EventState.currentEventText = "\"Oh okay, I'll try again next season. Thanks!\" the woman says as she leaves.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                EventState.PushEvent(new OutOfStockReturnEvent(mProductType), GameState.quarter + 1, 0.6f);
                break;
            case 1:
                EventState.currentEventText = "\"Oh okay. I'll have to look elsewhere,\" she says. She takes her leave.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                CustomerState.storePopularity *= 0.95f;
                onCooldown = false;
                break;

        }
        return EventResult.DONE;
    }

    private class OutOfStockReturnEvent : GameEvent
    {
        private ProductType mProductType;
        private enum Stage
        {
            OPENING,
            HAS_POTION,
            NO_POTION
        };
        private Stage mStage = Stage.OPENING;
        private bool mHasPotion = false;

        public OutOfStockReturnEvent(ProductType productType)
        {
            mProductType = productType;
        }

        protected override EventResult EventStart()
        {
            EventState.currentEventText = "A familiar woman approaches the counter.";
            EventState.currentEventOptions = EventState.CONTINUE_OPTION;
            mHasPotion = BusinessState.inventory[(int)mProductType] > 0;
            return EventResult.CONTINUE;
        }

        protected override EventResult OnPlayerDecision(int choice)
        {
            switch(mStage)
            {
                case Stage.OPENING:
                    string potionNameLower = mProductType.GetName().ToLower();
                    if(mHasPotion)
                    {
                        EventState.currentEventText = string.Format("\"Hi! I was in here last season looking for a {0} potion. Do you have any now?\"", potionNameLower);
                        EventState.currentEventOptions = new string[]
                        {
                            string.Format("Sell her a {0} potion", potionNameLower),
                            "Say no"
                        };
                        mStage = Stage.HAS_POTION;
                    }
                    else
                    {
                        EventState.currentEventText = string.Format("\"Hi, I was in here last season looking for a {0} potion, and you told me to return. I still don't see any, though.\"", potionNameLower);
                        EventState.currentEventOptions = new string[]
                        {
                            "Apologize"
                        };
                        mStage = Stage.NO_POTION;
                    }
                    return EventResult.CONTINUE;
                case Stage.HAS_POTION:
                    switch(choice)
                    {
                        case 0:
                            EventState.currentEventText = "She thanks you and pays for her potion.";
                            EventState.currentEventOptions = EventState.OK_OPTION;
                            CustomerState.storePopularity *= 1.1f;
                            BusinessSystem.SellProduct((int)mProductType);
                            break;
                        case 1:
                            EventState.currentEventText = "The woman is visibly annoyed. She leaves briskly.";
                            EventState.currentEventOptions = EventState.OK_OPTION;
                            CustomerState.storePopularity *= 0.85f;                            
                            break;
                    }
                    break;
                case Stage.NO_POTION:
                    EventState.currentEventText = "She looks disappointed as she leaves the store empty-handed.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    CustomerState.storePopularity *= 0.9f;
                    break;
            }
            onCooldown = false;
            return EventResult.DONE;
        }
    }
}
