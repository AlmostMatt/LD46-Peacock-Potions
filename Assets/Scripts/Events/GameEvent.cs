using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    private bool mEventComplete = false;

    public void DoEvent()
    {
        GameState.currentStage = GameState.GameStage.GS_EVENT; // pause the simulation
        EventState.currentEvent = this; // set self as the callback for the UI

        EventStart(); // derived classes override this to do whatever, including populating the UI
    }

    public void PlayerDecision(int choice)
    {
        // callback from the UI
        Debug.Log("Player made choice " + choice);

        if(mEventComplete)
        {
            // return to the simluation (this happens *before* setting it so that events that end have a chance to display their final message
            GameState.currentStage = GameState.GameStage.GS_SIMULATION;
            return;
        }

        mEventComplete = OnPlayerDecision(choice); // derived classes implement whatever they need here. return true to indicate the event is over.
    }

    protected abstract void EventStart();
    protected abstract bool OnPlayerDecision(int choice);
}
