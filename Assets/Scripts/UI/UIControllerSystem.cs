using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private RenderableGroup<string> mEventOptionRenderGroup;
    private RenderableGroup<string> mItemQuarterlySummary;

    // Use this for initialization
    void Start()
    {
        mEventOptionRenderGroup = new RenderableGroup<string>(
            SimulationEventContent.transform.Find("DecisionPanel/Options"),
            RenderFunctions.RenderToText);
        mItemQuarterlySummary = new RenderableGroup<string>(
            SummaryView.transform.Find("ItemSummaries"),
            RenderFunctions.RenderItemQuarterlySummary);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUiVisibility();
        UpdateUiContent();
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

    // Change text and other fields in UI content
    private void UpdateUiContent()
    {
        GameState.GameStage stage = GameState.currentStage;
        // Summary Screen
        if (SummaryView.activeInHierarchy)
        {
            SummaryView.transform.Find("SummaryText").GetComponent<Text>().text = string.Format("Money: {0}$", BusinessState.money);
            List<string> summaries = new List<string>();
            summaries.Add("potion1");
            summaries.Add("potion2");
            mItemQuarterlySummary.UpdateRenderables(summaries);
        }
        // Simulation / Event
        SimulationView.transform.Find("Info Overlay/TopRight/Text").GetComponent<Text>().text = string.Format("Money: {0}$", BusinessState.money);
        // Event
        if (EventState.currentEvent != null)
        {
            SimulationEventContent.transform.Find("DecisionPanel/Text").GetComponent<Text>().text = EventState.currentEventText;
            mEventOptionRenderGroup.UpdateRenderables(new List<string>(EventState.currentEventOptions));
        }
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

    public void MakeDecision(Button button)
    {
        // Made a choice
        EventState.currentEvent.PlayerDecision(button.transform.GetSiblingIndex());
    }
}
