using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    private float mEventTimer = 0;

    private GameState.GameStage mPrevStage = GameState.GameStage.GS_MAIN_MENU;
    
    // Update is called once per frame
    void Update()
    {        
        if(mPrevStage != GameState.currentStage)
        {
            mEventTimer = 0;
            mPrevStage = GameState.currentStage;
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
                    mEventTimer = Random.Range(1.5f,2.5f);
                }
            }
        }
        else if(EventState.currentEvent.persistent)
        {
            EventState.currentEvent.UpdatePersistence();
        }
    }
}
