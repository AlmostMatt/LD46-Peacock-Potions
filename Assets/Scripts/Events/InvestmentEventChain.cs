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
        private enum Stage
        {
            OPENING,
            DECIDE
        }
        private Stage mStage = Stage.OPENING;

        protected override EventResult EventStart()
        {
            EventState.currentEventImage = "faceOther";
            EventState.currentEventText = "Someone approaches you at the counter. \"I have a proposition for you.\"";
            EventState.currentEventOptions = new string[] { "Go on..." };
            return EventResult.CONTINUE;
        }

        protected override EventResult OnPlayerDecision(int choice)
        {
            switch(mStage)
            {
                case Stage.OPENING:
                    EventState.currentEventImage = "faceOther";
                    EventState.currentEventText = "I've got an investment opportunity. Lend me $1000, and I'll pay you back double.";
                    EventState.currentEventOptions = new string[]
                    {
                        "Accept the deal",
                        "Refuse"
                    };
                    mStage = Stage.DECIDE;
                    break;
                case Stage.DECIDE:
                    {
                        switch(choice)
                        {
                            case 0:
                                EventState.currentEventImage = "faceOther";
                                EventState.currentEventText = "\"You won't regret this!\", he says. He takes your $1000 and leaves.";
                                EventState.PushEvent(new InvestmentReturnEvent(), GameState.quarter + 4);
                                BusinessState.MoneyChangeFromEvent(-1000);
                                break;
                            case 1:
                                EventState.currentEventImage = "faceOther";
                                EventState.currentEventText = "\"Suit yourself...\", he says. He leaves without another word.";
                                break;
                        }
                    }
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return EventResult.DONE;
            }

            return EventResult.CONTINUE;
        }
    }

    private class InvestmentReturnEvent : GameEvent
    {
        private enum Stage
        {
            OPENING
        }
        private Stage mStage = Stage.OPENING;

        protected override EventResult EventStart()
        {
            EventState.currentEventImage = "faceOther";
            EventState.currentEventText = "A man approaches you at the counter. He looks familiar. \"I'm back from my business venture!\"";
            EventState.currentEventOptions = new string[] { "Continue" };
            return EventResult.CONTINUE;
        }

        protected override EventResult OnPlayerDecision(int choice)
        {
            EventState.currentEventImage = "faceOther";
            EventState.currentEventText = "\"Didn't I tell you you wouldn't regret it?\" He hands you $2000, then goes on his way.";
            EventState.currentEventOptions = EventState.OK_OPTION;
            BusinessState.MoneyChangeFromEvent(+2000);
            return EventResult.DONE;
        }
    }

}
