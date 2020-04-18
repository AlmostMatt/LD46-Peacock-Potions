using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    public void DoEvent()
    {
        // pause the simulation
        GameState.currentStage = GameState.GameStage.GS_EVENT;

        // populate data for the UI to pull from
        EventState.currentEvent = this;
        EventState.currentEventText = "Look, an event!";
        EventState.currentEventOptions = new string[] { "Option 1", "Option 2" };
    }

    public void PlayerDecision(int choice)
    {
        // callback from the UI
        Debug.Log("Player made choice " + choice);

        // End the event and return to simulation
        EventState.currentEvent = null;
        GameState.currentStage = GameState.GameStage.GS_SIMULATION;
    }
}
