﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefEvent : GameEvent
{
    private enum Stage
    {
        OPENING,
        REVENGE
    }
    private Stage mStage = Stage.OPENING;

    protected override void EventStart()
    {
        // populate data for the UI to pull from
        EventState.currentEventText = "You caught a thief trying to steal some potions. What do you want to do?";
        EventState.currentEventOptions = new string[]
            {
                "Let the thief go with a warning",
                "Make an example of the thief"
            };
    }

    protected override bool OnPlayerDecision(int choice)
    {
        switch(mStage)
        {
            case Stage.OPENING:
                {
                    switch(choice)
                    {
                        case 0:
                            EventState.currentEventText = "The thief thanks you and promises never to steal again.";
                            EventState.currentEventOptions = EventState.OK_OPTION;
                            return true;
                        case 1:
                            EventState.currentEventText = "Everyone stares as you punch the thief in the nose.";
                            EventState.currentEventOptions = new string[] { "Back off", "Stick to your guns" };
                            mStage = Stage.REVENGE;
                            break;
                    }
                    break;
                }
            case Stage.REVENGE:
                {
                    switch(choice)
                    {
                        case 0:
                            EventState.currentEventText = "The thief scrambles out of the store and people return to shopping.";                            
                            break;
                        case 1:
                            EventState.currentEventText = "People crowd around the thief to protect him. Some people leave the shop.";
                            break;
                    }
                    EventState.currentEventOptions = EventState.OK_OPTION;
                    return true;
                }
        }

        return false;
    }
}
