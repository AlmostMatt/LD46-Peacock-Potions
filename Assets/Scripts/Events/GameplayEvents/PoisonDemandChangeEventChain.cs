using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDemandChangeEventChain
{
    public static void Init()
    {
        EventState.PushEvent(new PoisonDemandUpEvent(), 3);
    }

    private static float sDemandChange = 0.25f;

    public class PoisonDemandUpEvent : GameEvent
    {
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
                    GameData.singleton.productDemand[(int)PotionType.PT_POISON_POTION] += sDemandChange;
                    EventState.PushEvent(new PoisonDemandDownEvent(), GameData.singleton.quarter + 3);
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

    public class PoisonDemandDownEvent : GameEvent
    {
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
                    EventState.currentEventText = "\"How's that rat problem you were having?\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventText = "\"They haven't bothered me for a while, actually.\"";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.productDemand[(int)PotionType.PT_POISON_POTION] -= sDemandChange;
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

}

