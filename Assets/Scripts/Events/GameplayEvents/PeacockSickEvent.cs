using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeacockSickEvent : GameEvent
{
    protected override EventResult OnStage(EventStage currentStage)
    {
        switch(currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = "The family peacock has died! The business can't go on without it.";
                EventState.currentEventOptions = new string[] { "Nooo! :("};
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                return EventResult.CONTINUE;
            case EventStage.ACCEPT:
                MainGameSystem.GameOver();
                return EventResult.DONE;
        }

        return EventResult.DONE;
    }
}
