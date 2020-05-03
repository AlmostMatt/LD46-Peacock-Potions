using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfStockEvent : GameEvent
{
    [System.Serializable]    
    public class OutOfStockEventSaveData : ScheduledEvent.EventSpecificSaveData
    {
        public OutOfStockEventSaveData(PotionType PotionType)
        {
            this.PotionType = PotionType;
        }
        public PotionType PotionType;
    }

    private PotionType mPotionType;

    public OutOfStockEvent(PotionType PotionType)
    {
        mPotionType = PotionType;
    }

    // for reloading from a save file
    public OutOfStockEvent(OutOfStockEventSaveData saveData)
    {
        mPotionType = saveData.PotionType;
        Debug.Log("reloaded out of stock event for type " + mPotionType);
    }

    public override ScheduledEvent.EventSpecificSaveData GetSaveData()
    {
        Debug.Log("saving out of stock event for type " + mPotionType);
        return new OutOfStockEventSaveData(mPotionType);
    }

    protected override EventResult OnStage(EventStage currentStage)
    {
        switch (currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = string.Format("A woman approaches the counter. \"Do you have any {0} potions?\"", mPotionType.GetName().ToLower());
                EventState.currentEventOptions = new string[]
                {
                    "We've run out, but you should  try again next season.",
                    "We've run out, and I don't know when we'll have more."
                };
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                return EventResult.CONTINUE;
            case EventStage.ACCEPT:
                EventState.currentEventText = "\"Oh okay, I'll try again next season. Thanks!\" the woman says as she leaves.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                EventState.PushEvent(new OutOfStockReturnEvent(mPotionType), GameData.singleton.quarter + 1, 0.6f);
                return EventResult.DONE;
            case EventStage.REFUSE:
                EventState.currentEventText = "\"Oh okay. I'll have to look elsewhere,\" she says. She takes her leave.";
                EventState.currentEventOptions = EventState.OK_OPTION;
                GameData.singleton.storePopularity *= 0.95f;
                GameData.singleton.outOfStockEventCooldown = GameData.singleton.quarter + 2;
                return EventResult.DONE;
        }
        return EventResult.CONTINUE;
    }

    private class OutOfStockReturnEvent : GameEvent
    {
        private PotionType mPotionType;
        private bool mHasPotion = false;

        public OutOfStockReturnEvent(PotionType PotionType)
        {
            mPotionType = PotionType;
        }

        public OutOfStockReturnEvent(OutOfStockEventSaveData saveData)
        {
            mPotionType = saveData.PotionType;
            Debug.Log("reloaded out of stock return event for type " + mPotionType);
        }

        public override ScheduledEvent.EventSpecificSaveData GetSaveData()
        {
            Debug.Log("saving out of stock return event for type " + mPotionType);
            return new OutOfStockEventSaveData(mPotionType);
        }

        protected override EventResult OnStage(EventStage currentStage)
        {
            string potionNameLower = mPotionType.GetName().ToLower();
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
                    GameData.singleton.storePopularity *= 1.1f;
                    BusinessSystem.SellProduct((int)mPotionType);
                    break;
                case EventStage.REFUSE:
                    EventState.currentEventText = "The woman is visibly annoyed. She leaves briskly.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.storePopularity *= 0.85f; EventState.currentEventText = "\"Oh okay, I'll try again next season. Thanks!\" the woman says as she leaves.";
                    break;
                case EventStage.UNABLE:
                    EventState.currentEventText = "She looks disappointed as she leaves the store empty-handed.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.storePopularity *= 0.9f;
                    break;
            }
            return EventResult.DONE;
        }

        protected override EventResult EventStart()
        {
            mHasPotion = GameData.singleton.potionsOwned[(int)mPotionType] > 0;
            return EventResult.CONTINUE;
        }
    }
}
