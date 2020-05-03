using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class GameEvent // <StageEnum> where StageEnum : System.Enum
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

    private GameStage mPreviousStage;
    private EventResult mEventStatus = EventResult.CONTINUE;
    protected EventStage[] mCurrentOptionOutcomes;
    private EventStage mCurrentStage = EventStage.START;

    public virtual ScheduledEvent.EventSpecificSaveData GetSaveData()
    {
        return null;
    }

    public void DoEvent()
    {
        //mPreviousStage = GameData.singleton.currentStage;
        //GameData.singleton.currentStage = GameStage.GS_EVENT; // pause the simulation
        EventState.currentEvent = this; // set self as the callback for the UI. This also signals to other systems that they may need to pause.

        mEventStatus = EventStart(); // derived classes override this to do whatever, including populating the UI
        Debug.Log("EVENT START " + EventState.currentEventText + " - " + mEventStatus.ToString());

        if (mEventStatus == EventResult.SKIP)
        {
            // the event decided not to fire at all for whatever reason
            SetEventToNull();
        }
        if (mEventStatus != EventResult.PERSISTENT)
        {
            mEventStatus = OnStage(mCurrentStage);
            if (mEventStatus == EventResult.SKIP)
            {
                // the event decided not to fire at all for whatever reason
                SetEventToNull();
            }
        }
    }

    public void PlayerDecision(int choice)
    {
        // callback from the UI
        Debug.Log("Player made choice " + choice);

        if(mEventStatus == EventResult.DONE)
        {
            // return to the game (this happens *before* setting it so that events that end have a chance to display their final message
            //GameData.singleton.currentStage = mPreviousStage;
            EventEnd(choice);
            SetEventToNull();
            return;
        }

        // If not DONE, the state should change.
        mCurrentStage = mCurrentOptionOutcomes[choice];
        mEventStatus = OnStage(mCurrentStage);
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

    protected virtual EventResult EventStart() { return EventResult.CONTINUE; }
    protected virtual EventResult OnStage(EventStage currentStage) { return EventResult.DONE;}
    protected virtual void EventEnd(int choice) {}
    protected virtual bool ShouldPersistStill() { return false; }

    private void SetEventToNull()
    {
        // GameData.singleton.currentStage = mPreviousStage;
        // Set all of the current-event related fields back to defaults
        // This way the next event that happens won't accidentally get any values from the previous event
        EventState.currentEvent = null;
        EventState.currentEventImage = "";
        EventState.currentEventOptions = new string[] { };
    }
}
