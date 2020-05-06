using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestmentEventChain
{
    private const int COST = 1000;

    public static void Init()
    {
        EventState.PushEvent(new InvestmentEventStart(), 3);
    }

    private class InvestmentEventStart : GameEvent
    {        
        protected override EventResult EventStart()
        {
            if(GameData.singleton.money < COST)
            {
                Debug.Log("postponing investment event to next quarter");
                EventState.PushEvent(new InvestmentEventStart(), GameData.singleton.quarter + 1);
                return EventResult.SKIP;
            }

            return EventResult.CONTINUE;
        }

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
                    EventState.currentEventText = string.Format("I've got an investment opportunity. Lend me {0}, and I'll pay you back double.", Utilities.FormatMoney(COST));
                    EventState.currentEventOptions = new string[]
                    {
                        "Accept the deal",
                        "Refuse"
                    };
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.ACCEPT, EventStage.REFUSE };
                    break;
                case EventStage.ACCEPT:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = string.Format("\"You won't regret this!\", he says. He takes your {0} and leaves.", Utilities.FormatMoney(COST));
                    EventState.PushEvent(new InvestmentReturnEvent(), GameData.singleton.quarter + 4);
                    BusinessState.MoneyChangeFromEvent(-COST);
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
                    EventState.currentEventText = string.Format("\"Didn't I tell you you wouldn't regret it?\" He hands you {0}, then goes on his way.", Utilities.FormatMoney(COST));
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    BusinessState.MoneyChangeFromEvent(COST * 2);
                    return EventResult.DONE;
            }
            return EventResult.CONTINUE;
        }
    }
}
