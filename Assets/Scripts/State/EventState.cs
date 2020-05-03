using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventState
{
    // populate this data and the event UI will pull from it
    public static GameEvent currentEvent;
    public static string currentEventImage = "";
    public static string currentEventText = "DEFAULT TEXT!";
    public static string[] currentEventOptions = new string[]{ "uninitialized option 1", "uninitialized option 2" };
    public static string[] OK_OPTION = new string[] { "Ok" }; // many events might want this, so this is here for common use
    public static string[] CONTINUE_OPTION = new string[] { "Continue" };
    
    private static List<ScheduledEvent>[] eventQueues = new List<ScheduledEvent>[(int)GameStage.MAX_VALUE];

    static EventState()
    {
        for(int i = 0; i < eventQueues.Length; ++i)
        {
            eventQueues[i] = new List<ScheduledEvent>();
        }
    }

    public static void Clear()
    {
        currentEvent = null;
        currentEventImage = "";
        currentEventText = "DEFAULT TEXT!";
        currentEventOptions = new string[]{ "uninitialized option 1", "uninitialized option 2" };
        eventQueues = new List<ScheduledEvent>[(int)GameStage.MAX_VALUE];
        for(int i = 0; i < eventQueues.Length; ++i)
        {
            eventQueues[i] = new List<ScheduledEvent>();
        }
    }

    public static void PushEvent(GameEvent e, int quarter, float minDelay = 1f, GameStage gameStage = GameStage.GS_SIMULATION)
    {
        List<ScheduledEvent> eventQueue = eventQueues[(int)gameStage];
        ScheduledEvent se = new ScheduledEvent(e, quarter, minDelay);
        int insertIdx = 0;
        while(insertIdx < eventQueue.Count && eventQueue[insertIdx].mQuarter < se.mQuarter)
        {
            ++insertIdx;
        }
        while(insertIdx < eventQueue.Count && eventQueue[insertIdx].mQuarter == se.mQuarter && eventQueue[insertIdx].mMinDelay <= se.mMinDelay) // this does mean events could starve out ones with higher min delay...
        {
            ++insertIdx;
        }
        eventQueue.Insert(insertIdx, se);
    }

    public static GameEvent PopEvent()
    {
        List<ScheduledEvent> eventQueue = eventQueues[(int)GameData.singleton.currentStage];
        if(eventQueue.Count == 0) return null;

        ScheduledEvent e = eventQueue[0];

        if(e.mMinDelay > GameData.singleton.quarterTimeElapsed) return null; // TODO: generalize "quarterTime" to "stageTime" I guess
        
        if(e.mQuarter <= GameData.singleton.quarter)
        {
            eventQueue.RemoveAt(0);
            return e.mEvent;
        }

        return null;
    }

    public static bool hasMoreEventsRightNow
    {
        get
        {
            List<ScheduledEvent> eventQueue = eventQueues[(int)GameData.singleton.currentStage];
            if(eventQueue.Count == 0) return false;

            if(eventQueue[0].mQuarter > GameData.singleton.quarter) return false;

            return true;
        }
    }

    public static void SaveData()
    {
        GameData.singleton.eventSaveData.Clear();
        for(int i = 0; i < eventQueues.Length; ++i)
        {
            foreach(ScheduledEvent se in eventQueues[i])
            {
                Debug.Log("saving " + se.mEvent.ToString() + ", type " + se.mEvent.GetType());
                ;
                ScheduledEvent.ScheduledEventSaveData d = new ScheduledEvent.ScheduledEventSaveData(
                    se,
                    (GameStage)i,
                    se.mEvent.GetSaveData()
                );
                
                GameData.singleton.eventSaveData.Add(d);
            }
        }
    }
}
