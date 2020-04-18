using UnityEngine;
using System.Collections;

/**
 * Manipulates UI objects based on the game state.
 * Also receives user input from the UI.
 */
public class UIControllerSystem : MonoBehaviour
{
    public GameObject SummaryView;
    public GameObject SimulationView;
    public GameObject SimulationDefaultContent;
    public GameObject SimulationEventContent;

    // Use this for initialization
    void Start()
    {
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
        // Summary / resource allocation
        SummaryView.SetActive(stage == GameState.GameStage.GS_RESOURCE_ALLOCATION);
        // UI shared by Simulation / Event
        SimulationView.SetActive(stage == GameState.GameStage.GS_EVENT || stage == GameState.GameStage.GS_SIMULATION);
        // Simulation
        SimulationDefaultContent.GetComponent<CanvasGroup>().alpha = (stage == GameState.GameStage.GS_SIMULATION ? 1.0f : 0.5f);
        // Event
        SimulationEventContent.SetActive(stage == GameState.GameStage.GS_EVENT);
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
        // TODO: pass in the ID of the decision
        EventState.currentEvent.PlayerDecision(1);
    }
}
