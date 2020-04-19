using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // does this really need to be separate from BusinessState? enh whatever
    public enum GameStage
    {
        // before the game starts
        GS_MAIN_MENU,
        // summary screen + allocating resources
        GS_RESOURCE_ALLOCATION,
        // Interacting with and feeding the peacock
        GS_PEACOCK,
        // time is passing
        GS_SIMULATION,
        // time is paused - an event has occurred and a decision is being made
        GS_EVENT,
        // the game has ended
        GS_GAME_OVER
    };

    public static GameStage currentStage = GameStage.GS_MAIN_MENU;
    public static int quarter = 0;

    public enum Season
    {
        SPRING,
        SUMMER,
        FALL,
        WINTER
    }
    public static Season season
    {
        get { return (Season)(quarter % 4); }
    }
}
