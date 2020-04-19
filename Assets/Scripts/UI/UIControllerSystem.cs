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
    public GameObject PeacockView;
    public GameObject SimulationView;

    public GameObject SimulationDefaultContent;
    public GameObject SimulationEventContent;

    private RenderableGroup<string> mEventOptionRenderGroup;
    private RenderableGroup<BusinessState.PerItemReport> mItemQuarterlySummaryRenderGroup;
    private RenderableGroup<ResourceAndCount> mResourceInventoryRenderGroup;
    private RenderableGroup<ResourceAndCount> mPeacockFeatherRenderGroup;

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
        mPeacockFeatherRenderGroup = new RenderableGroup<ResourceAndCount>(
            PeacockView.transform.Find("InventoryFeathers"),
            RenderFunctions.RenderResourceAndCount);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUiVisibility();
        UpdateUiContent();

        if(GameState.currentStage == GameState.GameStage.GS_SIMULATION)
        {
            UpdateCustomers();
        }

        FancyUIAnimations.Update();
    }

    // TODO: move this somewhere else, probably. clean it up, too.
    private int[] customerFade;
    private bool[] customerFadeTransitioning;
    private float[] customerFadeTimers;
    private void UpdateCustomers()
    {
        // naively fade customers in and out. TODO: make it linked to number/frequency of customers
        Transform customers = SimulationDefaultContent.transform.Find("Customers");
        if(customerFade == null) { customerFade = new int[customers.childCount]; }
        if(customerFadeTransitioning == null) { customerFadeTransitioning = new bool[customers.childCount]; }
        if(customerFadeTimers == null) { customerFadeTimers = new float[customers.childCount]; }
        for(int customerIdx = 0; customerIdx < customers.childCount; ++customerIdx)
        {
            if(!customerFadeTransitioning[customerIdx])
            {
                customerFadeTimers[customerIdx] -= Time.deltaTime;
                if(customerFadeTimers[customerIdx] <= 0)
                {
                    customerFade[customerIdx] = 1 - customerFade[customerIdx];
                    customerFadeTransitioning[customerIdx] = true;
                }
            }
            else
            {
                Transform customer = customers.GetChild(customerIdx);
                Image img = customer.GetComponent<Image>();
                float alpha = img.color.a;
                float newAlpha = Mathf.Clamp(alpha + Time.deltaTime * (customerFade[customerIdx] == 1 ? 5: -5), 0, 1);
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
                if(newAlpha == 0 || newAlpha == 1)
                {
                    customerFadeTransitioning[customerIdx] = false;
                    customerFadeTimers[customerIdx] = Random.Range(2f, 9f);
                }
            }
        }
    }

    // Update the visibility of UI elements
    private void UpdateUiVisibility()
    {
        GameState.GameStage stage = GameState.currentStage;
        // Summary / resource allocation
        SummaryView.SetActive(stage == GameState.GameStage.GS_RESOURCE_ALLOCATION);
        // Peacock UI
        PeacockView.SetActive(stage == GameState.GameStage.GS_PEACOCK);
        // UI shared by Simulation / Event
        SimulationView.SetActive(stage == GameState.GameStage.GS_EVENT || stage == GameState.GameStage.GS_SIMULATION);
        // Simulation
        //SimulationDefaultContent.GetComponent<CanvasGroup>().alpha = (stage == GameState.GameStage.GS_SIMULATION ? 1.0f : 0.5f);
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
        else if(PeacockView.activeInHierarchy)
        {
            mPeacockFeatherRenderGroup.UpdateRenderables(BusinessState.peacock.quarterlyReport.featherCounts);
        }
        // Simulation / Event
        SimulationView.transform.Find("Info Overlay/TopRight/Text").GetComponent<Text>().text = string.Format("Money: ${0}", BusinessState.money);
        if (SimulationDefaultContent.activeInHierarchy)
        {
            // Color and show/hide potions in the shop
            for (int i=0; i< (int)ProductType.PT_MAX; i++)
            {
                var PotionGroup = SimulationDefaultContent.transform.Find("Potions").GetChild(i);
                for (int j=0; j< PotionGroup.childCount; j++)
                {
                    PotionGroup.GetChild(j).gameObject.SetActive(j < BusinessState.inventory[i]);
                    PotionGroup.GetChild(j).GetComponent<Image>().color = ((ProductType)i).GetColor();
                    // TODO: if a potion stopped being visible it was just sold. Show the +money animation there
                }
            }
        }
        // Event
        if (EventState.currentEvent != null)
        {
            SimulationEventContent.transform.Find("Focus image").GetComponent<Image>().sprite = SpriteManager.GetSprite(EventState.currentEventImage);
            SimulationEventContent.transform.Find("DecisionPanel/Text").GetComponent<Text>().text = EventState.currentEventText;
            mEventOptionRenderGroup.UpdateRenderables(new List<string>(EventState.currentEventOptions));
        }
    }

    public void SummaryScreenOK()
    {
        // Reset all InputGroups - so that they will accept initial values at the start of the next summary.
        InputGroup[] inputGroups = SummaryView.GetComponentsInChildren<InputGroup>();
        foreach (InputGroup inputGroup in inputGroups)
        {
            inputGroup.ClearValue();
        }

        // State change - from summary to peacock management
        ShowPeacockSummary();

        BusinessState.quarterlyReport = new BusinessState.QuarterlyReport();
        // TODO: populate production based on the inputGroup values
        // Take a snapshot of the current prices for reference at the end of the quarter
        BusinessState.quarterlyReport.salePrices = BusinessState.prices;

        Debug.Log("game stage is now " + GameState.currentStage);
    }

    public void MakeDecision(Button button)
    {
        // Made a choice
        EventState.currentEvent.PlayerDecision(button.transform.GetSiblingIndex());
    }

    
    // --------- PEACOCK SCREEN ------------ //

    public void PeacockScreenOK()
    {
        // State change - from peacock management to simulation
        GameState.currentStage = GameState.GameStage.GS_SIMULATION;
        BusinessState.peacock.StartQuarter();
    }

    private void ShowPeacockSummary()
    {
        for(int i = 0; i < PeacockView.transform.childCount; ++i)
        {
            GameObject go = PeacockView.transform.GetChild(i).gameObject;
            go.GetComponent<CanvasGroup>().alpha = 0;
            FancyUIAnimations.PushAnimation(FancyUIAnimations.AnimationType.FADE_IN, go);
            GameState.currentStage = GameState.GameStage.GS_PEACOCK;
        }
        
        // I assume there's a more proper way to do this, but I'm too lazy to figure it out
        PeacockView.transform.Find("FoodReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.foodDesc;
        PeacockView.transform.Find("ActivityReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.activityDesc;
        PeacockView.transform.Find("ExtraReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.extraDesc;
        PeacockView.transform.Find("StatusReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.generalDesc;
    }


    public void PeacockScreenFood(int foodType)
    {
        Debug.Log("food select " + (FoodType)foodType);
        Transform selections = PeacockView.transform.Find("FoodPlan");
        for(int i = 0; i < selections.childCount; ++i)
        {
            Transform button = selections.GetChild(i);
            Image img = button.GetComponent<Image>();
            if(img != null)
            {
                img.color = new Color(1f, 1f, 1f, (i == foodType) ? 1f : 0f);
            }
        }
        FoodType food = ((FoodType)foodType);
        int price = food.GetPrice();
        selections.Find("FoodCost").GetComponent<Text>().text = Utilities.FormatMoney(price);
        BusinessState.peacock.quarterlyFoodCost = price;
        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(BusinessState.peacock.quarterlyTotalCost);
        BusinessState.peacock.quarterlyFoodType = (FoodType)foodType;
    }

    public void PeacockScreenActivity(int activityType)
    {
        Debug.Log("activity select " + (PeacockActivityType)activityType);
        Transform selections = PeacockView.transform.Find("ActivityPlan");
        for(int i = 0; i < selections.childCount; ++i)
        {
            Transform button = selections.GetChild(i);
            Image img = button.GetComponent<Image>();
            if(img != null)
            {
                img.color = new Color(1f, 1f, 1f, (i == activityType) ? 1f : 0f);
            }
        }
        
        BusinessState.peacock.quarterlyActivity = (PeacockActivityType)activityType;
    }

    public void PeacockScreenExtra(int extraType)
    {
        Debug.Log("extra peacock " + extraType);
        Transform extras = PeacockView.transform.Find("ExtraPlan");
        Transform button = extras.GetChild(extraType);
        Image img = button.GetComponent<Image>();        
        img.color = new Color(1f, 1f, 1f, 1 - img.color.a);
        bool selected = img.color.a == 1f;
        BusinessState.peacock.SetQuarterlyExtra(extraType, selected);

        button.Find("Cost").GetComponent<Text>().text = Utilities.FormatMoney(selected ? ((PeacockExtraType)extraType).GetPrice() : 0);

        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(BusinessState.peacock.quarterlyTotalCost);
    }

    // --------- END PEACOCK SCREEN ------------ //
}
