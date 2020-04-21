using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissedRentEvent : GameEvent
{
    protected override EventResult OnStage(EventStage currentStage)
    {
        switch(currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = "You don't have enough money to pay the rent this season. The dream is dead.";
                EventState.currentEventOptions = new string[] { "Aww :("};
                mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT };
                return EventResult.CONTINUE;
            case EventStage.ACCEPT:
                MainGameSystem.GameOver();
                return EventResult.DONE;
        }

        return EventResult.DONE;
    }
}
