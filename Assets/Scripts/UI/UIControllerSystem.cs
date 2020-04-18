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
    private RenderableGroup<BusinessState.PerItemReport> mItemQuarterlySummaryRenderGroup;
    private RenderableGroup<ResourceAndCount> mResourceInventoryRenderGroup;

    // Use this for initialization
    void Start()
    {
        mEventOptionRenderGroup = new RenderableGroup<string>(
            SimulationEventContent.transform.Find("DecisionPanel/Options"),
            RenderFunctions.RenderToText);
        mItemQuarterlySummaryRenderGroup = new RenderableGroup<BusinessState.PerItemReport>(
            SummaryView.transform.Find("ItemSummaries"),
            RenderFunctions.RenderItemQuarterlySummary);
        mResourceInventoryRenderGroup = new RenderableGroup<ResourceAndCount>(
            SummaryView.transform.Find("InventoryFeathers"),
            RenderFunctions.RenderResourceAndCount);
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
            // Inventory (cash)
            SummaryView.transform.Find("InventoryCash/Text").GetComponent<Text>().text = string.Format("Balance: ${0}", BusinessState.money);
            // Inventory (feathers)
            List<ResourceAndCount> resourceCounts = new List<ResourceAndCount>();
            for (int i=0; i<(int)ResourceType.RT_MAX; i++)
            {
                resourceCounts.Add(new ResourceAndCount((ResourceType)i, BusinessState.resources[i]));
            }
            mResourceInventoryRenderGroup.UpdateRenderables(resourceCounts);
            // Per-item reports
            mItemQuarterlySummaryRenderGroup.UpdateRenderables(BusinessState.GetPerItemReports());
        }
        // Simulation / Event
        SimulationView.transform.Find("Info Overlay/TopRight/Text").GetComponent<Text>().text = string.Format("Money: ${0}", BusinessState.money);
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

            // for now, resources are 1-1 with the products
            if(i < BusinessState.resources.Length)
            {
                BusinessState.inventory[i] = BusinessState.resources[i];
            }
        }

        BusinessState.quarterlyReport = new BusinessState.QuarterlyReport();

        Debug.Log("game stage is now " + GameState.currentStage);
    }

    public void MakeDecision(Button button)
    {
        // Made a choice
        EventState.currentEvent.PlayerDecision(button.transform.GetSiblingIndex());
    }
}
