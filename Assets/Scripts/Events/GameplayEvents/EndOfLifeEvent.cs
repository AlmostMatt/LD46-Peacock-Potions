using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLifeEvent : GameEvent
{
    protected override EventResult OnStage(EventStage currentStage)
    {
        switch(currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = "You worked the potion shop for a long time now. It's time to retire.";
                EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                return EventResult.CONTINUE;
            case EventStage.ACCEPT:
                MainGameSystem.GameOver();
                return EventResult.DONE;
        }

        return EventResult.DONE;
    }
}
