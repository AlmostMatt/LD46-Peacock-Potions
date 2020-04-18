using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    protected string mEventText;
    protected string[] mOptions;

    protected GameEvent(string eventText, string[] options)
    {
        mEventText = eventText;
        mOptions = options;
    }

    public void DoEvent()
    {
        GameState.currentStage = GameState.GameStage.GS_EVENT; // pause the simulation
        EventState.currentEvent = this; // set self as the callback for the UI

        // populate data for the UI to pull from        
        EventState.currentEventText = mEventText;
        EventState.currentEventOptions = mOptions;
    }

    public void PlayerDecision(int choice)
    {
        // callback from the UI
        Debug.Log("Player made choice " + choice);

        if(OnPlayerDecision(choice)) // derived classes implement whatever they need here. return true to indicate the event is over.
        {        
            // return to the simluation
            GameState.currentStage = GameState.GameStage.GS_RESOURCE_ALLOCATION;
        }
    }

    protected abstract bool OnPlayerDecision(int choice);
}
