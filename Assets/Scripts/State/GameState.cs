using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // does this really need to be separate from BusinessState? enh whatever
    public enum GameStage
    {
        GS_MAIN_MENU,
        GS_RESOURCE_ALLOCATION,
        GS_SIMULATION,
        GS_EVENT,
        GS_GAME_OVER
    };

    public static GameStage currentStage = GameStage.GS_MAIN_MENU;
    public static int quarter = 0;
}
