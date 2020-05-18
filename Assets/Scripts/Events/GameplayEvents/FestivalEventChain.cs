using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FestivalEventChain
{
    private float mDemandChange = 0.25f;

    public static void Init()
    {
        EventState.PushEvent(new FestivalIntroEvent(), 7); // winter of 2nd year
    }

    private class FestivalIntroEvent : GameEvent
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
                    EventState.currentEventText = string.Format("\"I can't wait for next year's summer festival!\"");
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventText = string.Format("\"Yes! I love trying my luck at all the little games they set up.\"");
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    break;
                case EventStage.S3:
                    EventState.currentEventText = string.Format("\"Not to mention the big lottery at the end.\"");
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S4 };
                    break;
                case EventStage.S4:
                    EventState.currentEventText = string.Format("\"Fingers crossed for that!\"");
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S5 };
                    break;
                case EventStage.S5:
                    EventState.currentEventText = string.Format("The customers return to their shopping.");
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.productDemand[(int)PotionType.PT_LUCK_POTION] += 0.1f; // increase demand now, so that it takes effect in the spring
                    EventState.PushEvent(new FestivalReminderEvent(), GameData.singleton.quarter + 1);
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }
    

    private class FestivalReminderEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "The summer festival is next season!";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S5 };
                    break;
                case EventStage.S5:
                    EventState.currentEventText = "Maybe I should go.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.productDemand[(int)PotionType.PT_LUCK_POTION] += 0.2f; // increase demand even more for the festival itself
                    EventState.PushEvent(new FestivalOverEvent(), GameData.singleton.quarter + 2);
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

    private class FestivalOverEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventText = "That festival was fun.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    GameData.singleton.productDemand[(int)PotionType.PT_LUCK_POTION] -= 0.3f;
                    break;
            }
            return EventResult.DONE;
        }
    }
}
