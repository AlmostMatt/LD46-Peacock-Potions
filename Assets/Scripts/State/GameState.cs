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
        // summary screen + crafting potions
        GS_RESOURCE_ALLOCATION,
        // Interacting with and feeding the peacock
        GS_PEACOCK,
        // time is passing
        GS_SIMULATION,
        // time is paused - an event has occurred and a decision is being made
        GS_EVENT,
        // shop with overlay: Potions sold
        GS_OVERLAY_POTION_SALES,
        // shop with overlay: Financial summary
        GS_OVERLAY_FINANCIAL_SUMMARY,
        // shop with overlay: Feather collection
        GS_OVERLAY_FEATHER_COLLECTION,
        // the game has ended
        GS_GAME_OVER,

        // how many stages there are
        MAX_VALUE
    };

    // This can be used to change stage for the main game loop.
    // Game over, events, and the time-skip will not use this.
    public static void GameLoopGotoNextStage()
    {
        GameStage prevStage = currentStage;
        switch (currentStage)
        {
            case GameStage.GS_SIMULATION:
                /* End the Quarter */
                MainGameSystem.EndCurrentQuarter();
                currentStage = GameStage.GS_OVERLAY_POTION_SALES;
                break;
            case GameStage.GS_OVERLAY_POTION_SALES:
                currentStage = GameStage.GS_OVERLAY_FINANCIAL_SUMMARY;
                break;
            case GameStage.GS_OVERLAY_FINANCIAL_SUMMARY:
                /* Pay rent when the OK button is clicked */
                MainGameSystem.PayEndOfQuarterExpenses();
                currentStage = GameStage.GS_OVERLAY_FEATHER_COLLECTION;
                break;
            case GameStage.GS_OVERLAY_FEATHER_COLLECTION:
                currentStage = GameStage.GS_RESOURCE_ALLOCATION;
                break;
            case GameStage.GS_RESOURCE_ALLOCATION:
                currentStage = GameStage.GS_PEACOCK;
                GameObject.FindObjectsOfType<UIControllerSystem>()[0].PreparePeacockSummary();
                break;
            case GameStage.GS_PEACOCK:
                /* Pay peacock-related expenses*/
                MainGameSystem.PayPeacockExpenses();
                /* Start the next quarter */
                MainGameSystem.StartNextQuarter();
                currentStage = GameStage.GS_SIMULATION;
                break;
            default:
                // stay in the current stage
                Debug.LogError("GameLoopGotoNextStage was used from an invalid stage!");
                break;
        }
        Debug.Log("Stage change from " + prevStage.ToString() + " to " + currentStage.ToString());
    }

    public static GameStage currentStage = GameStage.GS_MAIN_MENU;
    public static int quarter = 0;
    public static float quarterTime = 0; // seconds elapsed in the current quarter
    public static int year
    {
        get { return 452 + (quarter / 4); }
    }

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

public static class SeasonExtensions
{
    public static string GetImage(this GameState.Season season)
    {
        switch (GameState.season) // surely this should be the season passed in, right?
        {
            case GameState.Season.SPRING:
                return "spring";
            case GameState.Season.SUMMER:
                return "summer";
            case GameState.Season.FALL:
                return "fall";
            case GameState.Season.WINTER:
                return "winter";
            default:
                return "summer";
        }
    }

    public static string GetName(this GameState.Season season)
    {
        switch(season)
        {
            case GameState.Season.SPRING:
                return "Spring";
            case GameState.Season.SUMMER:
                return "Summer";
            case GameState.Season.FALL:
                return "Fall";
            case GameState.Season.WINTER:
                return "Winter";
            default:
                return "Smarch";
        }
    }
}
