using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    protected enum EventResult
    {
        CONTINUE,
        DONE,
        PERSISTENT,
        SKIP
    }
    public bool persistent
    {
        get { return mEventStatus == EventResult.PERSISTENT; }
    }

    private GameState.GameStage mPreviousStage;
    private EventResult mEventStatus = EventResult.CONTINUE;

    public void DoEvent()
    {
        //mPreviousStage = GameState.currentStage;
        //GameState.currentStage = GameState.GameStage.GS_EVENT; // pause the simulation
        EventState.currentEvent = this; // set self as the callback for the UI. This also signals to other systems that they may need to pause.

        mEventStatus = EventStart(); // derived classes override this to do whatever, including populating the UI

        if(mEventStatus == EventResult.SKIP)
        {
            // the event decided not to fire at all for whatever reason
            SetEventToNull();
        }
    }

    public void PlayerDecision(int choice)
    {
        // callback from the UI
        Debug.Log("Player made choice " + choice);

        if(mEventStatus == EventResult.DONE)
        {
            // return to the game (this happens *before* setting it so that events that end have a chance to display their final message
            //GameState.currentStage = mPreviousStage;
            EventEnd(choice);
            SetEventToNull();
            return;
        }

        mEventStatus = OnPlayerDecision(choice); // derived classes implement whatever they need here. return true to indicate the event is over.
    }

    public void UpdatePersistence()
    {
        if(mEventStatus == EventResult.PERSISTENT)
        {
            if(!ShouldPersistStill())
            {
                EventEnd(-1);
                SetEventToNull();
            }
        }
    }

    protected abstract EventResult EventStart();
    protected virtual EventResult OnPlayerDecision(int choice) { return EventResult.DONE; }
    protected virtual void EventEnd(int choice) {}
    protected virtual bool ShouldPersistStill() { return false; }

    private void SetEventToNull()
    {
        // GameState.currentStage = mPreviousStage;
        // Set all of the current-event related fields back to defaults
        // This way the next event that happens won't accidentally get any values from the previous event
        EventState.currentEvent = null;
        EventState.currentEventImage = "";
        EventState.currentEventOptions = new string[] { };
    }
}
