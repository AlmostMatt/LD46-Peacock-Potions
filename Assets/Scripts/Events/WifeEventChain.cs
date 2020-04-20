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

    private class WifeEventOne : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            string name = "Susie";
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
                    EventState.currentEventText = "It’s quite majestic. Oh, my name is " + name + " by the way.";
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
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }    
}
