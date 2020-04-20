using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    private float mEventTimer = 0;

    // Update is called once per frame
    void Update()
    {
        //if(GameState.currentStage == GameState.GameStage.GS_SIMULATION)
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
                    mEventTimer = Random.Range(1.5f,2.5f); // want to reset this at the beginning of a new stage
                }
            }
        }
    }
}
