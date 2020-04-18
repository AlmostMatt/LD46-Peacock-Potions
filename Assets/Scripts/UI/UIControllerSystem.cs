using UnityEngine;
using System.Collections;

/**
 * Manipulates UI objects based on the game state.
 * Also receives user input from the UI.
 */
public class UIControllerSystem : MonoBehaviour
{
    public GameObject SummaryView;
    public GameObject EventView;

    // Use this for initialization
    void Start()
    {
        SummaryView.SetActive(true);
        EventView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUiVisibility();
    }

    // Update the visibility of UI elements
    private void UpdateUiVisibility()
    {
        GameState.GameStage stage = GameState.currentStage;
        SummaryView.SetActive(stage == GameState.GameStage.GS_RESOURCE_ALLOCATION);
        EventView.SetActive(stage == GameState.GameStage.GS_EVENT || stage == GameState.GameStage.GS_SIMULATION);
    }

    public void SummaryScreenOK()
    {
        // State change - from summary to simulation (or event)
        GameState.currentStage = GameState.GameStage.GS_SIMULATION; //  GS_EVENT;

        // Randomize prices and inventory
        // TODO: read this information from the UI
        for (int i = 0; i < (int)ProductType.PT_MAX; ++i)
        {
            BusinessState.prices[i] = Random.Range(50, 100);
            Debug.Log("Selling " + (ProductType)i + " for " + BusinessState.prices[i]);

            BusinessState.inventory[i] = Random.Range(0, 30);
        }
        Debug.Log("game stage is now " + GameState.currentStage);
    }

    public void MakeDecision()
    {
        // Made a choice
        // State change - from simulation to summary (or game over)

        GameState.quarter += 1;
        if (GameState.quarter > 5)
        {
            // TEMP. later, something somewhere will check for proper game over (player death or business going under)
            GameState.currentStage = GameState.GameStage.GS_GAME_OVER;
        }
        else
        {
            GameState.currentStage = GameState.GameStage.GS_RESOURCE_ALLOCATION;
        }
        Debug.Log("game stage is now " + GameState.currentStage);
    }
}
