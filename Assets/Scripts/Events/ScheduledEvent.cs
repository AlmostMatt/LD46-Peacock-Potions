using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScheduledEvent
{
    public ScheduledEvent(GameEvent e, int quarter, float minDelay)
    {
        mEvent = e;
        mQuarter = quarter;
        mMinDelay = minDelay;
    }

    public GameEvent mEvent;
    public int mQuarter;
    public float mMinDelay;

    [Serializable]
    public class ScheduledEventSaveData
    {
        public ScheduledEventSaveData(ScheduledEvent se, GameStage gameStage, EventSpecificSaveData specificData)
        {
            this.gameStage = gameStage;
            eventType = se.mEvent.GetType();
            quarter = se.mQuarter;
            minDelay = se.mMinDelay;
            this.specificData = specificData;
        }

        public GameStage gameStage;
        public Type eventType;
        public int quarter;
        public float minDelay;
        public EventSpecificSaveData specificData;
    }

    [Serializable]
    public class EventSpecificSaveData
    {
    }
}
