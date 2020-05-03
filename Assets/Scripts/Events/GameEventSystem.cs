using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class GameEventSystem : MonoBehaviour
{
    private float mEventTimer = 0;

    private GameStage mPrevStage = GameStage.GS_MAIN_MENU;

    // Update is called once per frame
    void Update()
    {
        if(mPrevStage != GameData.singleton.currentStage)
        {
            mEventTimer = 0;
            mPrevStage = GameData.singleton.currentStage;
        }

        if(EventState.currentEvent == null)
        {
            if(mEventTimer > 0)
            {
                mEventTimer -= Time.deltaTime;
            }

            if(mEventTimer <= 0)
            {
                GameEvent e = EventState.PopEvent();
                if(e != null)
                {
                    e.DoEvent();
                    mEventTimer = UnityEngine.Random.Range(1.5f, 2.5f);
                }
            }
        }
        else if(EventState.currentEvent.persistent)
        {
            EventState.currentEvent.UpdatePersistence();
        }
    }

    public static void Load(List<ScheduledEvent.ScheduledEventSaveData> saveData)
    {
        Debug.Log("loading event data");
        foreach(ScheduledEvent.ScheduledEventSaveData data in saveData)
        {
            Debug.Log("data is of type " + data.GetType());
            Debug.Log("event is type " + data.eventType);
            if(data.specificData != null)
            {
                ConstructorInfo constructor = data.eventType.GetConstructor(new Type[] { data.specificData.GetType() });
                if(constructor != null)
                {
                    GameEvent e = (GameEvent)constructor.Invoke(new object[] { data.specificData });
                    EventState.PushEvent(e, data.quarter, data.minDelay, data.gameStage);
                }
                else
                {
                    Debug.LogError("failed to find constructor for event of type " + data.eventType + " with param " + data.specificData.GetType());
                }
            }
            else
            {
                ConstructorInfo constructor = data.eventType.GetConstructor(Type.EmptyTypes);
                if(constructor != null)
                {
                    GameEvent e = (GameEvent)constructor.Invoke(null);
                    EventState.PushEvent(e, data.quarter, data.minDelay, data.gameStage);
                }
                else
                {
                    Debug.LogError("failed to find default constructor for event of type " + data.eventType);
                }
            }
        }
    }

}