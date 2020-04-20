using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonEventChain
{
    private static bool letSonHelpBottle = false;

    public static void Init()
    {
        Debug.Log("pushing son event");
        EventState.PushEvent(new SonEventOne(), 0);
    }

    private class SonEventOne : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceSonHappy";
                    EventState.currentEventText = "Your son comes up to you and asks if he can help around the shop.";
                    EventState.currentEventOptions = new string[]
                        {
                    "Let him help bottle potions",
                    "Tell him to go outside play instead"
                        };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He happily grabs some empty bottles and goes over to the cauldron.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    letSonHelpBottle = true;
                    EventState.PushEvent(new SonDropBottlesEvent(), GameState.quarter); // TODO: randomize outcome? maybe he doesn't drop bottles
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He walks into the backyard, a little dejected.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    letSonHelpBottle = false;
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class SonDropBottlesEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "You hear a crash from across the store. Your son dropped some potions.";
                    EventState.currentEventOptions = new string[]
                    {
                        "Get him to clean it up",
                        "Tell him to go play outside"
                    };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.GO_CLEAN, EventStage.GO_OUTSIDE };
                    // Update inventory
                    int product = Random.Range(0, (int)ProductType.PT_MAX);
                    int numPotions = Mathf.Min(BusinessState.inventory[product], 3);
                    BusinessState.quarterlyReport.miscLosses[product] += numPotions;
                    BusinessState.inventory[product] -= numPotions;
                    break;
                case EventStage.GO_CLEAN:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He fetches a broom and cleans up. Then he decides to go outside.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new SonEventOne(), GameState.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
                case EventStage.GO_OUTSIDE:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He sniffles and goes outside.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new SonEventOne(), GameState.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }    
}
