using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDemandChangeEvent : GameEvent
{
    private float mDemandChange = 0.25f;

    protected override EventResult OnStage(EventStage currentStage)
    {
        switch(currentStage)
        {
            case EventStage.START:
                EventState.currentEventText = "You overhear a conversation between customers...";
                EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                break;
            case EventStage.S1:
                EventState.currentEventText = string.Format("\"Is it just me, or have there been more rats around lately?\"");
                EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                break;
            case EventStage.S2:
                EventState.currentEventText = string.Format("\"Yes! Just last week I found 3 of them in my basement!\"");
                EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                break;
            case EventStage.S3:
                EventState.currentEventText = string.Format("\"I hope they don't get too out of hand, the vermin.\"");
                EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                mCurrentOptionOutcomes = new EventStage[] { EventStage.S4 };                
                break;
            case EventStage.S4:
                EventState.currentEventText = string.Format("The customers return to their shopping.");
                EventState.currentEventOptions = EventState.OK_OPTION;
                GameData.singleton.productDemand[(int)PotionType.PT_POISON_POTION] += mDemandChange;
                return EventResult.DONE;
        }

        return EventResult.CONTINUE;
    }
}
