using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionEvent : GameEvent
{
    protected override void EventStart()
    {
        EventState.currentEventText = "You inherited the family potion business from your father, who passed away last fall.";
        EventState.currentEventOptions = EventState.CONTINUE_OPTION;
    }

    protected override bool OnPlayerDecision(int choice)
    {
        EventState.currentEventText = "Gather feathers from your magical peacock, use them to brew potions, and keep the business alive!";
        EventState.currentEventOptions = EventState.OK_OPTION;
        return true;
    }

}
