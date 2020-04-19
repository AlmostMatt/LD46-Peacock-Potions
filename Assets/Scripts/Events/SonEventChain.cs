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
        protected override void EventStart()
        {
            EventState.currentEventImage = "faceChild";
            EventState.currentEventText = "Your son comes up to you and asks if he can help around the shop.";
            EventState.currentEventOptions = new string[]
                {
                    "Let him help bottle potions",
                    "Tell him to go outside play instead"
                };
        }

        protected override bool OnPlayerDecision(int choice)
        {
            switch(choice)
            {
                case 0:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He happily grabs some empty bottles and goes over to the cauldron.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    letSonHelpBottle = true;
                    EventState.PushEvent(new SonDropBottlesEvent(), GameState.quarter); // TODO: randomize outcome? maybe he doesn't drop bottles
                    break;
                case 1:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He walks into the backyard, a little dejected.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    letSonHelpBottle = false;
                    break;
            }
            return true;
        }
    }

    private class SonDropBottlesEvent : GameEvent
    {
        protected override void EventStart()
        {
            EventState.currentEventImage = "faceChild";
            EventState.currentEventText = "You hear a crash from across the store. Your son dropped some potions.";
            EventState.currentEventOptions = new string[]
            {
                "Get him to clean it up",
                "Tell him to go play outside"
            };
            
            int product = Random.Range(0, (int)ProductType.PT_MAX);
            int numPotions = Mathf.Min(BusinessState.inventory[product], 3);
            BusinessState.quarterlyReport.miscLosses[product] += numPotions;
            BusinessState.inventory[product] -= numPotions;
        }

        protected override bool OnPlayerDecision(int choice)
        {
            switch(choice)
            {
                case 0:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He fetches a broom and cleans up. Then he decides to go outside.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    break;
                case 1:
                    EventState.currentEventImage = "faceChild";
                    EventState.currentEventText = "He sniffles and goes outside.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    break;
            }
            EventState.PushEvent(new SonEventOne(), GameState.quarter + 1); // schedule another event for next quarter
            return true;
        }
    }
    
}
