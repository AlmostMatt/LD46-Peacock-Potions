using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameSystem : MonoBehaviour
{
    // probably temporary; just a way for me to advance the game without any UI

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            AdvanceStage();
        }
    }

    private void InitNewGame()
    {
        BusinessState.animals[(int)AnimalType.AT_TURTLE] = true; // player starts with a turtle
        BusinessState.money = 500; // some starting money
    }

    private void AdvanceStage()
    {
        switch(GameState.currentStage)
        {
            case GameState.GameStage.GS_MAIN_MENU:
                InitNewGame();
                GameState.currentStage = GameState.GameStage.GS_RESOURCE_ALLOCATION;
                break;
            case GameState.GameStage.GS_RESOURCE_ALLOCATION:
                GameState.currentStage = GameState.GameStage.GS_SIMULATION;
                break;
            case GameState.GameStage.GS_SIMULATION:
                GameState.quarter += 1;
                if(GameState.quarter > 5)
                {
                    // TEMP. later, something somewhere will check for proper game over (player death or business going under)
                    GameState.currentStage = GameState.GameStage.GS_GAME_OVER;
                }
                else
                {
                    GameState.currentStage = GameState.GameStage.GS_RESOURCE_ALLOCATION;
                }
                break;
        }
        Debug.Log("game stage is now " + GameState.currentStage);
        
    }
}
