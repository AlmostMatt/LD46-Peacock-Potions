using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    private float mEventTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameState.currentStage == GameState.GameStage.GS_SIMULATION)
        {
            mEventTimer -= Time.deltaTime;
            if(mEventTimer <= 0)
            {
                GameEvent e = EventState.PopEvent();
                if(e != null)
                {
                    if(e.CanHappen())
                    {
                        e.DoEvent();
                    }
                }
                
                mEventTimer = Random.Range(2, 5);
            }
        }
    }
}
