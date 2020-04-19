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
    public static string[] CONTINUE_OPTION = new string[] { "Continue..." };

    private class ScheduledEvent
    {
        public ScheduledEvent(GameEvent e, int quarter)
        {
            mEvent = e;
            mGameTime = quarter;
        }

        public GameEvent mEvent;
        public int mGameTime;
    }

    private static List<ScheduledEvent> eventQueue = new List<ScheduledEvent>();
    public static void PushEvent(GameEvent e, int gameTime)
    {
        // TODO: give more control over scheduling events? e.g. we should be able to schedule an event for "next year" or whatever, and it inserts it sorted or something
        ScheduledEvent se = new ScheduledEvent(e, gameTime);
        int insertIdx = 0;
        while(insertIdx < eventQueue.Count && eventQueue[insertIdx].mGameTime <= se.mGameTime)
        {
            ++insertIdx;
        }
        eventQueue.Insert(insertIdx, se);
    }

    public static GameEvent PopEvent()
    {
        if(eventQueue.Count == 0) return null;

        ScheduledEvent e = eventQueue[0];

        if(e.mGameTime <= GameState.quarter)
        {
            eventQueue.RemoveAt(0);
            return e.mEvent;
        }

        return null;
    }

    // could track history here too, maybe. or somewhere else, idk

}
