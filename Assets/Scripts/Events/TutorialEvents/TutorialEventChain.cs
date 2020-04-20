﻿using System.Collections;
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
                    EventState.currentEventImage = "facePlayer";
                    EventState.currentEventText = "You inherited the family potion business from your father, who passed away last fall.";
                    EventState.currentEventOptions = EventState.CONTINUE_OPTION;
                    mCurrentOptionOutcomes = new EventStage[] { EventStage.S2 };
                    return EventResult.CONTINUE;
                case EventStage.S2:
                    EventState.currentEventImage = "peacock";
                    EventState.currentEventText = "Gather feathers from your magical peacock, use them to brew potions, and keep the business alive!";
                    EventState.currentEventOptions = EventState.OK_OPTION;
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
            EventState.currentEventText = "You look over the end-of-season report. Not bad for your first spring!";
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
        protected override EventResult EventStart()
        {
            EventState.currentEventText = "You take stock of your inventory. Maybe you should brew potions to replace the ones you sold. You could change their prices, too.";
            EventState.currentEventOptions = EventState.OK_OPTION;
            EventState.PushEvent(new ExplainPeacockEvent(), 0, 0, GameState.GameStage.GS_PEACOCK);
            return EventResult.DONE;
        }
    }

    private class ExplainPeacockEvent : GameEvent
    {
        protected override EventResult EventStart()
        {
            EventState.currentEventText = "You go over to your peacock. How should you treat it this summer?";
            EventState.currentEventOptions = EventState.OK_OPTION;
            return EventResult.DONE;
        }
    }


}
