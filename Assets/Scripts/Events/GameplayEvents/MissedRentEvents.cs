using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MissedRentEvents
{
    // the first time you miss
    public class MissedRentEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "Your landlord enters the shop and comes up to you.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    return EventResult.CONTINUE;
                case EventStage.S1:
                    EventState.currentEventText = "\"Looks like you can't pay rent this season.\"";
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                    return EventResult.CONTINUE;
                case EventStage.ACCEPT:
                    EventState.currentEventText = "\"I'll give you one more chance. Don't worry about this season's rent, but if you miss next season, I'll have to evict you.\""; // TODO: shorten, probably!
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new RestoreRentEvent(GameData.singleton.rent), GameData.singleton.quarter + 1, 0);
                    GameData.singleton.rent = 0; // forgive the rent for one season
                    return EventResult.DONE;
            }

            return EventResult.DONE;
        }
        
        private class RestoreRentEvent : GameEvent
        {
            private int mPrevRent;

            [System.Serializable]    
            public class RestoreRentEventSaveData : ScheduledEvent.EventSpecificSaveData
            {
                public RestoreRentEventSaveData(int prevRent)
                {
                    this.prevRent = prevRent;
                }
                public int prevRent;
            }

            public override ScheduledEvent.EventSpecificSaveData GetSaveData()
            {
                return new RestoreRentEventSaveData(mPrevRent);
            }
            
            // for reloading from a save file
            public RestoreRentEvent(RestoreRentEventSaveData saveData)
            {
                mPrevRent = saveData.prevRent;
                Debug.Log("reloaded restore rent event with amount " + mPrevRent);
            }

            public RestoreRentEvent(int prevRent)
            {
                mPrevRent = prevRent;
            }

            protected override EventResult EventStart()
            {
                Debug.Log("Rent restored to " + mPrevRent);
                GameData.singleton.rent = mPrevRent;
                return EventResult.SKIP;
            }
        }
    }

    // the second time you miss
    public class MissedRentAgainEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "Your landlord enters the shop.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventText = "\"Sorry, but I can't let you stay for free again. You have to leave.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventText = "You don't have enough money to pay the rent this season. The dream is dead.";
                    EventState.currentEventOptions = EventState.currentEventOptions = new string[] { "Aww :(" };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                    break;
                case EventStage.ACCEPT:
                    MainGameSystem.GameOver();
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }
}
