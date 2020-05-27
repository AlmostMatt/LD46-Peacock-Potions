using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // end of quarter: updating peacock and checking for game over
    GS_END_OF_QUARTER,
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

// TODO: find a home for these functions
public class GameStageExtensions
{
    // does this really need to be separate from BusinessState? enh whatever

    // This can be used to change stage for the main game loop.
    // Game over, events, and the time-skip will not use this.
    public static void GameLoopGotoNextStage()
    {
        GameStage prevStage = GameData.singleton.currentStage;
        switch (GameData.singleton.currentStage)
        {
            case GameStage.GS_SIMULATION:
                /* End the Quarter */
                MainGameSystem.CurrentQuarterEnding();
                GameData.singleton.currentStage = GameStage.GS_END_OF_QUARTER;
                break;
            case GameStage.GS_END_OF_QUARTER:
                /* Advance to summary screens */
                MainGameSystem.EndCurrentQuarter();
                GameData.singleton.currentStage = GameStage.GS_OVERLAY_POTION_SALES;
                break;
            case GameStage.GS_OVERLAY_POTION_SALES:
                GameData.singleton.currentStage = GameStage.GS_OVERLAY_FINANCIAL_SUMMARY;
                break;
            case GameStage.GS_OVERLAY_FINANCIAL_SUMMARY:
                /* Pay rent when the OK button is clicked */
                MainGameSystem.PayEndOfQuarterExpenses();
                GameData.singleton.currentStage = GameStage.GS_OVERLAY_FEATHER_COLLECTION;
                break;
            case GameStage.GS_OVERLAY_FEATHER_COLLECTION:
                GameData.singleton.currentStage = GameStage.GS_RESOURCE_ALLOCATION;
                break;
            case GameStage.GS_RESOURCE_ALLOCATION:
                GameData.singleton.currentStage = GameStage.GS_PEACOCK;
                break;
            case GameStage.GS_PEACOCK:
                /* Pay peacock-related expenses*/
                MainGameSystem.PayPeacockExpenses();
                /* Start the next quarter */
                MainGameSystem.StartNextQuarter();
                break;
            default:
                // stay in the current stage
                Debug.LogError("GameLoopGotoNextStage was used from an invalid stage!");
                break;
        }
        GameData.SaveGame();
        Debug.Log("Stage change from " + prevStage.ToString() + " to " + GameData.singleton.currentStage.ToString());
    }
    
    public static bool epilogueDirty = false;
    public static List<string> epilogueLines = new List<string>();
}
