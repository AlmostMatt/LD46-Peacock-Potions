using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestmentEventChain
{
    
    public static void Init()
    {
        EventState.PushEvent(new InvestmentEventStart(), 3);
    }

    private class InvestmentEventStart : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "Someone approaches you at the counter. \"I have a proposition for you.\"";
                    EventState.currentEventOptions = new string[] { "Go on..." };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.DECIDE };
                    break;
                case EventStage.DECIDE:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "I've got an investment opportunity. Lend me $1000, and I'll pay you back double.";
                    EventState.currentEventOptions = new string[]
                    {
                        "Accept the deal",
                        "Refuse"
                    };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "\"You won't regret this!\", he says. He takes your $1000 and leaves.";
                    EventState.PushEvent(new InvestmentReturnEvent(), GameState.quarter + 4);
                    BusinessState.MoneyChangeFromEvent(-1000);
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return EventResult.DONE;
                case EventStage.REFUSE:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "\"Suit yourself...\", he says. He leaves without another word.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

    private class InvestmentReturnEvent : GameEvent
    {


        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "A man approaches you at the counter. He looks familiar. \"I'm back from my business venture!\"";
                    EventState.currentEventOptions = new string[] { "Continue" };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    break;
                case EventStage.S2:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "\"Didn't I tell you you wouldn't regret it?\" He hands you $2000, then goes on his way.";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    BusinessState.MoneyChangeFromEvent(+2000);
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }
}
