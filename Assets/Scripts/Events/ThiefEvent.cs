using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * the first value should be START and the last should be END
 * those will have special meaning to the event flow
 */
public class ThiefEvent : GameEvent
{
    protected override EventResult OnStage(EventStage currentStage)
    {
        switch (currentStage)
        {
            case EventStage.START:
                {
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "You caught a thief trying to steal some potions. What do you want to do?";
                    EventState.currentEventOptions = new string[]
                        {
                            "Let the thief go with a warning",
                            "Make an example of the thief"
                        };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.LET_GO, EventStage.MAKE_EXAMPLE };
                    return EventResult.CONTINUE;
                }
            case EventStage.LET_GO:
                {
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "The thief thanks you and promises never to steal again.";
                    break;
                }
            case EventStage.MAKE_EXAMPLE:
                {
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "Everyone stares as you punch the thief in the nose.";
                    EventState.currentEventOptions = new string[] { "Back off", "Stick to your guns" };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.BACK_OFF, EventStage.STICK_TO };
                    return EventResult.CONTINUE;
                }
            case EventStage.BACK_OFF:
                {
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "The thief scrambles out of the store and people return to shopping.";
                    break;
                }
            case EventStage.STICK_TO:
                {
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "People crowd around the thief to protect him. Some people leave the shop.";
                    break;
                }
        }
        EventState.currentEventOptions = EventState.OK_OPTION;
        return EventResult.DONE;
    }
}
