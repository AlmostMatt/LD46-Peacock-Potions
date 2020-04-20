using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventChain
{
    public class IntroductionEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch (currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "facePlayerSad";
                    EventState.currentEventText = "It’s been a month since your father died and passed the family potions shop on to you.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    return EventResult.CONTINUE;
                case EventStage.S1:
                    EventState.currentEventImage = "facePlayerNeutral";
                    EventState.currentEventText = "Passed on for generations, it offers potions made from peacock feathers.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    return EventResult.CONTINUE;   
                case EventStage.S2:
                    EventState.currentEventImage = "facePlayerHappy";
                    EventState.currentEventText = "Now it's your turn to keep the business alive.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    return EventResult.CONTINUE;
                case EventStage.S3:
                    EventState.currentEventImage = "";
                    EventState.currentEventText = "Spring starts, and customers come and go, buying the potions your father made before he passed.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S4 };
                    EventState.PushEvent(new ExplainSalesEvent(), 0, 0, GameState.GameStage.GS_OVERLAY_POTION_SALES);
                    return EventResult.DONE;
            }
            return EventResult.DONE;
        }
    }

    private class ExplainSalesEvent : GameEvent
    {
        protected override EventResult EventStart()
        {
            GameObject.FindObjectsOfType<UIControllerSystem>()[0].MoveEventOverlayForTutorial();
            EventState.currentEventImage = "";
            EventState.currentEventText = "Spring comes to a close, and you look over the end-of-season report. Your father's potions sold well.";
            EventState.currentEventOptions = null;
            return EventResult.PERSISTENT;
        }

        protected override bool ShouldPersistStill()
        {
            return (GameState.currentStage == GameState.GameStage.GS_OVERLAY_POTION_SALES);
        }

        protected override void EventEnd(int choice)                    
        {
            EventState.PushEvent(new ExplainExpensesEvent(), 0, 0, GameState.GameStage.GS_OVERLAY_FINANCIAL_SUMMARY);
        }
    }

    private class ExplainExpensesEvent : GameEvent
    {
        protected override EventResult EventStart()
        {
            EventState.currentEventText = "You made enough money to cover your seasonal expenses, thankfully.";
            EventState.currentEventOptions = null;
            return EventResult.PERSISTENT;
        }

        protected override bool ShouldPersistStill()
        {
            return (GameState.currentStage == GameState.GameStage.GS_OVERLAY_FINANCIAL_SUMMARY);
        }

        protected override void EventEnd(int choice)
        {
            EventState.PushEvent(new ExplainFeathersEvent(), 0, 0, GameState.GameStage.GS_OVERLAY_FEATHER_COLLECTION);
        }
    }

    private class ExplainFeathersEvent : GameEvent
    {
        protected override EventResult EventStart()
        {
            EventState.currentEventText = "You gather up feathers shed by your family peacock. It's time to turn them into potions for next season!";
            EventState.currentEventOptions = null;
            return EventResult.PERSISTENT;
        }

        protected override bool ShouldPersistStill()
        {
            return (GameState.currentStage == GameState.GameStage.GS_OVERLAY_FEATHER_COLLECTION);
        }


        protected override void EventEnd(int choice)
        {
            EventState.PushEvent(new ExplainBrewingEvent(), 0, 0, GameState.GameStage.GS_RESOURCE_ALLOCATION);
            GameObject.FindObjectsOfType<UIControllerSystem>()[0].RestoreNormalEventOverlayPosition();
        }
    }

    private class ExplainBrewingEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:
                    EventState.currentEventImage = "facePlayerNeutral";
                    EventState.currentEventText = "You take stock of your potions. What did Dad say about this..?";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    return EventResult.CONTINUE;
                case EventStage.S1:
                    EventState.currentEventImage = "faceGrandfather";
                    EventState.currentEventText = "\"Make new potions out of feathers! Personally, I try to replace the potions I sold last season.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    return EventResult.CONTINUE;
                case EventStage.S2:
                    EventState.currentEventText = "\"You can change the prices too. If you didn't sell much of a potion, try making it cheaper.\"";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    return EventResult.CONTINUE;
                case EventStage.S3:
                    EventState.currentEventText = "\"Some potions just aren't as popular as others... and people's tastes can change, too. It's important to adapt!\"";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    EventState.PushEvent(new ExplainPeacockEvent(), 0, 0, GameState.GameStage.GS_PEACOCK);
                    return EventResult.DONE;
            }
            return EventResult.DONE;
        }
    }

    private class ExplainPeacockEvent : GameEvent
    {
        protected override EventResult OnStage(EventStage currentStage)
        {
            switch(currentStage)
            {
                case EventStage.START:  
                    EventState.currentEventImage = "facePlayerNeutral";
                    EventState.currentEventText = "Finished with potion-brewing, you check on the family peacock.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S1 };
                    return EventResult.CONTINUE;
                case EventStage.S1:
                    EventState.currentEventImage = "faceGrandfather";
                    EventState.currentEventText = "\"The peacock is the key to our business! Pay attention to its needs to keep it producing feathers.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    return EventResult.CONTINUE;
                case EventStage.S2:
                    EventState.currentEventText = "\"Decide what to do for it next season. It's a bit sad right now; I think it misses me.\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S3 };
                    return EventResult.CONTINUE;
                case EventStage.S3:
                    EventState.currentEventText = "\"The feathers you get will vary. I must admit, the peacock remains mysterious, even to me!\"";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S4 };
                    return EventResult.CONTINUE;
                case EventStage.S4:
                    EventState.currentEventText = "\"But remember, without it, you have no business! Keep it happy! Keep it healthy! Keep it alive!\"";
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return EventResult.DONE;
            }
            return EventResult.DONE;
        }
    }


}
