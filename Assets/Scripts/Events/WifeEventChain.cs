using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifeEventChain
{
    public static void Init()
    {
        Debug.Log("pushing son event");
        EventState.PushEvent(new WifeEventOne(), 0);
    }
    public const string NAME = "Susie";

    private class WifeEventOne : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "Hi there! Is that an Ancadian Peacock?";
                    EventState.currentEventOptions = new string[]
                        {"Umm, yeah!","I don’t actually know..."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1, EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "It’s quite majestic. Oh, my name is " + NAME + " by the way.";
                    EventState.currentEventOptions = new string[]
                        {"Magestic? I suppose it is.","It’s a fancy fantastical pheasant!"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2, EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "I've never seen one of this breed before. Do you mind if I study it for a little while?";
                    EventState.currentEventOptions = new string[]
                        {"Of course! It’s quite friendly."};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "She is staring intently at the peacock and seems to have forgotten about you for now.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new WifeEventTwo(), GameState.quarter + 1); // schedule another event for next quarter
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }

    private class WifeEventTwo : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "Hello again!";
                    EventState.currentEventOptions = new string[]
                        {"Welcome back!"};
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    break;
                case EventStage.S1:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "Thanks for letting me observe your peacock last time. I was writing a paper on the species, and seeing it was really helpful.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "Would you like to go out for tea after work? ";
                    EventState.currentEventOptions = new string[] {"Sure!", "No thanks, I'm too busy." };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceWifeHappy";
                    EventState.currentEventText = "Awesome, I know a great place!\n<i>She runs off in excitement</i>";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceWifeSad";
                    EventState.currentEventText = "Aw, okay.\nOn an unrelated note, do you sell love potions? ";
                    if (BusinessState.inventory[(int)ProductType.PT_LOVE_POTION] > 0) {
                        EventState.currentEventOptions = new string[] { "Yes" };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.HAS_POTION };
                    } else
                    {
                        EventState.currentEventOptions = new string[] { "Yes, but we don't have any right now" };
                        mCurrentOptionOutcomes = new EventStage[] { EventStage.NO_POTION };
                    }
                    break;
                case EventStage.HAS_POTION:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "I'll buy one love potion then!\n<i>She pays the price and heads out.</i>";
                    BusinessSystem.SellProduct((int)ProductType.PT_LOVE_POTION);
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    // TODO: push Event3
                    return EventResult.DONE;
                case EventStage.NO_POTION:
                    EventState.currentEventImage = "faceWifeNeutral";
                    EventState.currentEventText = "Alright. Well, I'll be around.";
                    BusinessSystem.SellProduct((int)ProductType.PT_LOVE_POTION);
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    // TODO: push Event3
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }
}
