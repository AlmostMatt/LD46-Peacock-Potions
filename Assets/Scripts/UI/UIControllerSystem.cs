using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/**
 * Manipulates UI objects based on the game state.
 * Also receives user input from the UI.
 */
public class UIControllerSystem : MonoBehaviour
{
    // The main views whose visibility will be toggled
    public GameObject SummaryView;
    public GameObject PeacockView;
    public GameObject SimulationView;
    public GameObject InventoryView;
    public GameObject OverlayViewFeathers;
    public GameObject OverlayViewPotions;
    public GameObject OverlayViewFinancial;
    public GameObject EpilogueView;

    // Convenient references to find smaller pieces of the above
    public GameObject SimulationDefaultContent;
    public GameObject SimulationEventContent;

    private RenderableGroup<BusinessState.PerItemReport> mItemQuarterlySummaryRenderGroup;
    private RenderableGroup<FeatherAndCount> mInventoryResourceRenderGroup;
    private RenderableGroup<PotionAndCount> mInventoryProductRenderGroup;

    private RenderableGroup<FeatherAndCount> mPeacockFeatherRenderGroup;

    private RenderableGroup<FeatherAndCount> mOverlayFeatherCollectionFeatherRenderGroup;
    private RenderableGroup<BusinessState.PerItemReport> mOverlayPotionSalesRenderGroup;
    private RenderableGroup<string[]> mOverlayFinancialRenderGroup;

    // Use this for initialization
    void Start()
    {
        mItemQuarterlySummaryRenderGroup = new RenderableGroup<BusinessState.PerItemReport>(
            SummaryView.transform.Find("ItemSummaries"),
            RenderFunctions.RenderItemQuarterlySummary);

        mInventoryResourceRenderGroup = new RenderableGroup<FeatherAndCount>(
            InventoryView.transform.Find("InvGroups/InventoryFeathers"),
            RenderFunctions.RenderFeatherAndCount);
        mInventoryProductRenderGroup = new RenderableGroup<PotionAndCount>(
            InventoryView.transform.Find("InvGroups/InventoryPotions"),
            RenderFunctions.RenderPotionAndCount);

        mPeacockFeatherRenderGroup = new RenderableGroup<FeatherAndCount>(
            PeacockView.transform.Find("InventoryFeathers"),
            RenderFunctions.RenderFeatherAndCount);
        // Overlays
        mOverlayFeatherCollectionFeatherRenderGroup = new RenderableGroup<FeatherAndCount>(
            OverlayViewFeathers.transform.Find("Content/InventoryFeathers"),
            RenderFunctions.RenderFeatherAndCount);
        mOverlayPotionSalesRenderGroup = new RenderableGroup<BusinessState.PerItemReport>(
            OverlayViewPotions.transform.Find("PotionSaleRows"),
            RenderFunctions.RenderPotionSale);
        mOverlayFinancialRenderGroup = new RenderableGroup<string[]>(
            OverlayViewFinancial.transform.Find("Content"),
            RenderFunctions.RenderStringArrayToTextChildren);

        SetupButtonCallbacks();
        // don't start with all customers visible
        RandomizeInitialCustomers();
    }

    // It's a pain to link these via unity UI because there are references from one prefab to another
    private void SetupButtonCallbacks()
    {
        // Assume that the OK / DONE button is the last button for these views
        SummaryView.GetComponentsInChildren<Button>(true).Last().onClick.AddListener(SummaryScreenOK);
        PeacockView.GetComponentsInChildren<Button>(true).Last().onClick.AddListener(PeacockScreenOK);
        // Assume that the OK / DONE button is the only button for these views
        OverlayViewFeathers.GetComponentInChildren<Button>(true).onClick.AddListener(OverlayScreenOK);
        OverlayViewFinancial.GetComponentInChildren<Button>(true).onClick.AddListener(OverlayScreenOK);
        OverlayViewPotions.GetComponentInChildren<Button>(true).onClick.AddListener(OverlayScreenOK);
        // Set the same callback for all of the buttons in the Event view
        foreach (Button b in SimulationEventContent.GetComponentsInChildren<Button>(true))
        {
            b.onClick.AddListener(() => MakeDecision(b));
        }

        // TODO: dynammically setup other buttons on peacock and summary screen
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUiVisibility();
        UpdateUiContent();

        if (GameData.singleton.currentStage == GameStage.GS_SIMULATION && EventState.currentEvent == null)
        {
            UpdateCustomers();
        }

        FancyUIAnimations.Update();
    }

    // TODO: move this somewhere else, probably. clean it up, too.
    private int[] customerFade;
    private bool[] customerFadeTransitioning;
    private float[] customerFadeTimers;
    private void RandomizeInitialCustomers()
    {
        Transform customers = SimulationDefaultContent.transform.Find("Customers");
        if (customerFade == null) { customerFade = new int[customers.childCount]; }
        if (customerFadeTransitioning == null) { customerFadeTransitioning = new bool[customers.childCount]; }
        if (customerFadeTimers == null) { customerFadeTimers = new float[customers.childCount]; }
        for (int customerIdx = 0; customerIdx < customers.childCount; ++customerIdx)
        {            
            Transform customer = customers.GetChild(customerIdx);
            Image img = customer.GetComponent<Image>();
            if(Random.Range(0f, 1f) < 0.3f)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
                customerFade[customerIdx] = 1;
            }
            else
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                customerFade[customerIdx] = 0;
            }
                
            customerFadeTimers[customerIdx] = Random.Range(2f, 9f);
        }
    }

    private void UpdateCustomers()
    {
        // naively fade customers in and out. TODO: make it linked to number/frequency of customers
        Transform customers = SimulationDefaultContent.transform.Find("Customers");
        for (int customerIdx = 0; customerIdx < customers.childCount; ++customerIdx)
        {
            if (!customerFadeTransitioning[customerIdx])
            {
                customerFadeTimers[customerIdx] -= Time.deltaTime;
                if (customerFadeTimers[customerIdx] <= 0)
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
                float newAlpha = Mathf.Clamp(alpha + Time.deltaTime * (customerFade[customerIdx] == 1 ? 5 : -5), 0, 1);
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
                if (newAlpha == 0 || newAlpha == 1)
                {
                    customerFadeTransitioning[customerIdx] = false;
                    customerFadeTimers[customerIdx] = Random.Range(2f, 9f);
                }
            }
        }
    }

    // Update the visibility of UI elements
    private bool mPrevHasEvent = false;
    private bool mPrevPeacockActive = false;
    private void UpdateUiVisibility()
    {
        GameStage stage = GameData.singleton.currentStage;
        // Summary / resource allocation
        SummaryView.SetActive(stage == GameStage.GS_RESOURCE_ALLOCATION);
        // Peacock UI
        bool peacockViewActive = stage == GameStage.GS_PEACOCK;
        PeacockView.SetActive(peacockViewActive);
        if(peacockViewActive && !mPrevPeacockActive)
        {
            PreparePeacockSummary();
        }
        mPrevPeacockActive = peacockViewActive;
            
        // UI shared by Simulation / Event and visible behind some overlays
        SimulationView.SetActive(
            stage == GameStage.GS_EVENT
            || stage == GameStage.GS_SIMULATION
            || stage == GameStage.GS_OVERLAY_POTION_SALES
            || stage == GameStage.GS_OVERLAY_FINANCIAL_SUMMARY
            || stage == GameStage.GS_OVERLAY_FEATHER_COLLECTION
        );
        // Overlay UIs
        OverlayViewFeathers.SetActive(stage == GameStage.GS_OVERLAY_FEATHER_COLLECTION);
        OverlayViewPotions.SetActive(stage == GameStage.GS_OVERLAY_POTION_SALES);
        OverlayViewFinancial.SetActive(stage == GameStage.GS_OVERLAY_FINANCIAL_SUMMARY);

        InventoryView.SetActive(stage != GameStage.GS_GAME_OVER);
        // Simulation
        // Fade the background?
        //SimulationDefaultContent.GetComponent<CanvasGroup>().alpha = (stage == GameStage.GS_SIMULATION ? 1.0f : 0.5f);
        // Event
        bool hasEvent = EventState.currentEvent != null;        
        SimulationEventContent.SetActive(hasEvent || mPrevHasEvent); // hack a one-frame delay on hiding the event display, in case we want to show 2 in a row. Does mean there's a 1-frame window for the player to click the button when there's no event...
        mPrevHasEvent = hasEvent;

        // ending
        EpilogueView.SetActive(stage == GameStage.GS_GAME_OVER);
    }

    // Change text and other fields in UI content
    private void UpdateUiContent()
    {
        GameStage stage = GameData.singleton.currentStage;
        /**
         * Inventory
         */
        {
            // Inventory (cash)
            InventoryView.transform.Find("InvGroups/InventoryCash/IconAndCount/Count")
                .GetComponent<Text>().text = string.Format("{0}", GameData.singleton.money);
            // Inventory (feathers)
            List<FeatherAndCount> resourceCounts = new List<FeatherAndCount>();
            for (int i = 0; i < (int)FeatherType.FT_MAX; i++)
            {
                resourceCounts.Add(new FeatherAndCount((FeatherType)i, GameData.singleton.feathersOwned[i]));
            }
            mInventoryResourceRenderGroup.UpdateRenderables(resourceCounts);
            // Inventory (feathers)
            List<PotionAndCount> productCounts = new List<PotionAndCount>();
            for (int i = 0; i < (int)PotionType.PT_MAX; i++)
            {
                productCounts.Add(new PotionAndCount((PotionType)i, GameData.singleton.potionsOwned[i]));
            }
            mInventoryProductRenderGroup.UpdateRenderables(productCounts);
        }
        /**
         * Summary
         */
        if (SummaryView.activeInHierarchy)
        {
            // Per-item reports
            mItemQuarterlySummaryRenderGroup.UpdateRenderables(BusinessState.GetPerItemReports());
        }
        /**
         * Peacock view
         */
        if (PeacockView.activeInHierarchy)
        {
            mPeacockFeatherRenderGroup.UpdateRenderables(GameData.singleton.peacockReportFeatherCounts);
        }
        /**
         * Simulation / Event
         */
        if (SimulationView.activeInHierarchy)
        {
            // Color and show/hide potions in the shop
            for (int i = 0; i < (int)PotionType.PT_MAX; i++)
            {
                var PotionGroup = GetPotionGroup((PotionType)i);
                for (int j = 0; j < PotionGroup.childCount; j++)
                {
                    PotionGroup.GetChild(j).gameObject.SetActive(j < GameData.singleton.potionsOwned[i]);
                    PotionGroup.GetChild(j).GetComponent<Image>().color = ((PotionType)i).GetColor();
                    // TODO: if a potion stopped being visible it was just sold. Show the +money animation there
                }
            }
            SimulationDefaultContent.transform.Find("Shop/Season").GetComponent<Image>().sprite = null; // clear the sprite in case of weird sprite change bug
            SimulationDefaultContent.transform.Find("Shop/Season").GetComponent<Image>().sprite = SpriteManager.GetSprite(GameData.singleton.season.GetImage());

            // Date
            SimulationView.transform.Find("Overlay/TopLeft/Date/Month/Text").GetComponent<Text>().text = GameData.singleton.month;
            SimulationView.transform.Find("Overlay/TopLeft/Date/Day/Text").GetComponent<Text>().text = GameData.singleton.dayOfMonth.ToString() ;
        }
        /**
         * Overlay views
         */
        int totalPotionRevenue = 0;
        foreach (BusinessState.PerItemReport report in BusinessState.GetPerItemReports())
        {
            totalPotionRevenue += (report.numSold * report.salePrice);
        }
        if (OverlayViewFeathers.activeInHierarchy)
        {
            //OverlayViewPotions.transform.Find("Content/Content").GetComponent<Text>().text = "";
            mOverlayFeatherCollectionFeatherRenderGroup.UpdateRenderables(
                GameData.singleton.peacockReportFeatherCounts);
        } else if (OverlayViewPotions.activeInHierarchy)
        {
            mOverlayPotionSalesRenderGroup.UpdateRenderables(BusinessState.GetPerItemReports());
            OverlayViewPotions.transform.Find("PotionSaleTotal/Row/H/Total").GetComponent<Text>().text
                = string.Format("${0}",totalPotionRevenue);
        } else if (OverlayViewFinancial.activeInHierarchy)
        {
            int startOfQuarterBalance = GameData.singleton.initialBalance;
            int eventIncome = GameData.singleton.eventIncome;
            int peacockExpenses = GameData.singleton.peacockQuarterlyTotalCost;
            int eventExpense = GameData.singleton.eventExpenses;
            int profit = (totalPotionRevenue + eventIncome) - (peacockExpenses + eventExpense);
            int rent = GameData.singleton.livingExpenses;
            int newBalance = (int)GameData.singleton.money - rent;
            // totalRevenue = sum of perItemReport sales
            // include some event revenue and expenses
            // revenue rent etc
            List<string[]> financialReportRows = new List<string[]>();
            financialReportRows.Add(new string[] { "Beginning Balance", "", string.Format("{0}", startOfQuarterBalance) });
            financialReportRows.Add(new string[] { "Potion Sales", string.Format("+{0}", totalPotionRevenue), "" });
            financialReportRows.Add(new string[] { "Peacock Care", string.Format("-{0}", peacockExpenses), "" });
            // Only show event-related if non-zero
            financialReportRows.Add(new string[] { "Events", string.Format("{0:+#;-#;+0}", eventIncome - eventExpense), "" });
            // <LINE> at index 4
            financialReportRows.Add(new string[] { "Profit", "", string.Format("{0:+#;-#;+0}", profit) });
            // TODO: make expenses that happen right now stand out
            financialReportRows.Add(new string[] { "Pay Rent", "", string.Format("<b>-{0}</b>", rent) });
            // <LINE> at index 7
            financialReportRows.Add(new string[] { "New Balance", "", string.Format("{0}", newBalance) });
            mOverlayFinancialRenderGroup.UpdateRenderables(financialReportRows, new int[] {4,7});
        }
        /**
         * Event
         */
        if (EventState.currentEvent != null)
        {
            // Set the image in one of two possible spots
            string focusImage = EventState.currentEventImage;
            bool isFace = focusImage.Contains("face");
            bool isOtherImage = (focusImage != "" && !isFace);
            Sprite focusSprite = SpriteManager.GetSprite(EventState.currentEventImage);
            SimulationEventContent.transform.Find("Face").GetComponent<Image>().sprite = null; // clear sprite to reset size information
            SimulationEventContent.transform.Find("Face").GetComponent<Image>().sprite = focusSprite;
            SimulationEventContent.transform.Find("Face").gameObject.SetActive(isFace && focusSprite != null);
            SimulationEventContent.transform.Find("NonFace/Image").GetComponent<Image>().sprite = null; // clear the sprite in case of weird sprite change bug
            SimulationEventContent.transform.Find("NonFace/Image").GetComponent<Image>().sprite = focusSprite;
            SimulationEventContent.transform.Find("NonFace").gameObject.SetActive(isOtherImage && focusSprite != null);
            // Set the text and options
            // Set option text on the event buttons
            GameObject b1 = SimulationEventContent.transform.Find("DecisionPanel/Options/ButtonLeft").gameObject;
            GameObject b2 = SimulationEventContent.transform.Find("DecisionPanel/Options/ButtonRight").gameObject;
            GameObject b3 = SimulationEventContent.transform.Find("DecisionPanel/Options/ButtonWide").gameObject;
            b1.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
            b2.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
            b3.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
            if (EventState.currentEventOptions != null) {
                SimulationEventContent.transform.Find("DecisionPanel/TextTop").GetComponent<Text>().text = EventState.currentEventText;
                SimulationEventContent.transform.Find("DecisionPanel/TextFull").GetComponent<Text>().text = "";
                b1.SetActive(EventState.currentEventOptions.Length > 1);
                b2.SetActive(EventState.currentEventOptions.Length > 1);
                b3.SetActive(EventState.currentEventOptions.Length == 1);
                if (EventState.currentEventOptions.Length == 1)
                {
                    b3.GetComponentInChildren<Text>().text = EventState.currentEventOptions[0];
                } else
                {
                    b1.GetComponentInChildren<Text>().text = EventState.currentEventOptions[0];
                    b2.GetComponentInChildren<Text>().text = EventState.currentEventOptions[1];
                }
            } else
            {
                SimulationEventContent.transform.Find("DecisionPanel/TextTop").GetComponent<Text>().text = "";
                SimulationEventContent.transform.Find("DecisionPanel/TextFull").GetComponent<Text>().text = EventState.currentEventText;
                b1.SetActive(false);
                b2.SetActive(false);
                b3.SetActive(false);
            }
        }

        if(EpilogueView.activeInHierarchy)
        {
            UpdateEpilogueText();
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
        // TODO: populate production based on the inputGroup values

        GameStageExtensions.GameLoopGotoNextStage();
        Debug.Log("game stage is now " + GameData.singleton.currentStage);
    }

    public void MakeDecision(Button button)
    {
        // Made a choice
        if(EventState.currentEvent != null) // ahhh see comment about 1-frame hack in UpdateUiVisibility
        {
            // Buttons are ordered [left right wide] so [left and right] are choice 0
            int index = button.transform.GetSiblingIndex() % 2;
            EventState.currentEvent.PlayerDecision(index);
        }
    }

    // --------- PEACOCK SCREEN ------------ //

    private float PEACOCK_SCREEN_UNSELECTED_ALPHA = 0.1f;
    public void PeacockScreenOK()
    {
        GameStageExtensions.GameLoopGotoNextStage();
    }

    /**
     * Call this at the end of the quarter to setup the peacock view
     */
    public void PreparePeacockSummary()
    {
        for(int i = 0; i < PeacockView.transform.childCount; ++i)
        {
            GameObject go = PeacockView.transform.GetChild(i).gameObject;
            if(go.GetComponent<CanvasGroup>() != null)
            {
                if (GameData.singleton.quarter < 2) {
                    go.GetComponent<CanvasGroup>().alpha = 0;
                    FancyUIAnimations.PushFadeIn(go);
                }
            }
        }

        // I assume there's a more proper way to do this, but I'm too lazy to figure it out
        PeacockView.transform.Find("Date").GetComponent<Text>().text = string.Format("{0}, Year {1}", GameData.singleton.season.GetName(), GameData.singleton.year);
        PeacockView.transform.Find("FoodReport").GetComponent<Text>().text = GameData.singleton.peacockReportFoodDesc;
        PeacockView.transform.Find("ActivityReport").GetComponent<Text>().text = GameData.singleton.peacockReportActivityDesc;
        PeacockView.transform.Find("ExtraReport").GetComponent<Text>().text = GameData.singleton.peacockReportExtraDesc;
        PeacockView.transform.Find("StatusReport").GetComponent<Text>().text = GameData.singleton.peacockReportGeneralDesc;
        PeacockView.transform.Find("NextQuarterTitle").GetComponent<Text>().text = string.Format("Plan for the {0}: ", GameData.singleton.season.GetNextSeasonName());
        
        Transform foodPlan = PeacockView.transform.Find("FoodPlan");
        for(int i = (int)FoodType.FT_MAX - 1; i >= 0; --i)
        {
            FoodType food = ((FoodType)i); 
            Transform button = foodPlan.GetChild(i);
            button.GetChild(0).GetComponent<Text>().text = food.GetLabel();
            if((int)GameData.singleton.peacockQuarterlyFoodType == i)
            {
                // if we can no longer afford the food we previously bought, force a downgrade
                int price = food.GetPrice();
                if(price > GameData.singleton.money)
                {
                    GameData.singleton.peacockQuarterlyFoodType = (FoodType)(i - 1); // assumption: the first available food type is free
                    button.GetComponent<Image>().color = new Color(1f, 1f, 1f, PEACOCK_SCREEN_UNSELECTED_ALPHA);
                    GameData.singleton.peacockQuarterlyFoodCost = GameData.singleton.peacockQuarterlyFoodType.GetPrice();
                }
                else
                {
                    button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }

        Transform activityPlan = PeacockView.transform.Find("ActivityPlan");
        for(int i = 0; i < (int)PeacockActivityType.PA_MAX; ++i)
        {
            PeacockActivityType activity = ((PeacockActivityType)i);
            Transform button = activityPlan.GetChild(i);
            button.GetChild(0).GetComponent<Text>().text = activity.GetLabel();
            if((int)GameData.singleton.peacockQuarterlyActivity == i)
            {
                 button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        
        Transform extraPlan = PeacockView.transform.Find("ExtraPlan");
        for(int i = 0; i < (int)PeacockExtraType.ET_MAX; ++i)
        {
            PeacockExtraType extra = ((PeacockExtraType)i);
            Transform button = extraPlan.GetChild(i);
            button.GetChild(0).GetComponent<Text>().text = extra.GetLabel();
            button.GetComponent<Image>().color = new Color(1f, 1f, 1f, Peacock.HasQuarterlyExtra(i) ? 1f : PEACOCK_SCREEN_UNSELECTED_ALPHA);
        }

        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(GameData.singleton.peacockQuarterlyTotalCost);
    }

    public void PeacockScreenFood(int foodType)
    {
        Transform selections = PeacockView.transform.Find("FoodPlan");
        bool prevSelected = selections.GetChild(foodType).GetComponent<Image>().color.a == 1f;

        FoodType food = ((FoodType)foodType);
        Debug.Log("food select " + food);
        int price = food.GetPrice();
        if(!prevSelected && GameData.singleton.money - GameData.singleton.peacockQuarterlyTotalCost + GameData.singleton.peacockQuarterlyFoodCost < price)
        {
            Debug.Log("not enough money");
            return;
        }

        for (int i = 0; i < selections.childCount; ++i)
        {
            Transform button = selections.GetChild(i);
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(1f, 1f, 1f, (i == foodType) ? 1f : PEACOCK_SCREEN_UNSELECTED_ALPHA);
            }
        }

        GameData.singleton.peacockQuarterlyFoodCost = price;
        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(GameData.singleton.peacockQuarterlyTotalCost);
        GameData.singleton.peacockQuarterlyFoodType = (FoodType)foodType;
    }

    public void PeacockScreenActivity(int activityType)
    {
        Debug.Log("activity select " + (PeacockActivityType)activityType);
        Transform selections = PeacockView.transform.Find("ActivityPlan");
        for (int i = 0; i < selections.childCount; ++i)
        {
            Transform button = selections.GetChild(i);
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(1f, 1f, 1f, (i == activityType) ? 1f : PEACOCK_SCREEN_UNSELECTED_ALPHA);
            }
        }

        GameData.singleton.peacockQuarterlyActivity = (PeacockActivityType)activityType;
    }

    public void PeacockScreenExtra(int extraType)
    {
        Debug.Log("extra peacock " + extraType);
        Transform extras = PeacockView.transform.Find("ExtraPlan");
        Transform button = extras.GetChild(extraType);
        Image img = button.GetComponent<Image>();
        bool prevSelected = img.color.a == 1f;

        PeacockExtraType extra = (PeacockExtraType)extraType;
        int price = extra.GetPrice();
        if(!prevSelected && GameData.singleton.money - GameData.singleton.peacockQuarterlyTotalCost < price)
        {
            Debug.Log("not enough money");
            return;
        }

        img.color = new Color(1f, 1f, 1f, prevSelected ? 0.1f : 1f);
        Peacock.SetQuarterlyExtra(extraType, !prevSelected);
        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(GameData.singleton.peacockQuarterlyTotalCost);
    }

    // --------- END PEACOCK SCREEN ------------ //

    public void OverlayScreenOK()
    {
        GameStageExtensions.GameLoopGotoNextStage();
    }

    private Transform GetPotionGroup(PotionType PotionType)
    {
        return SimulationDefaultContent.transform.Find("Potions").GetChild((int)PotionType);
    }

    // TODO: add UI or audio feedback for a sale happening.
    // This is called from BusinessSystem
    public void ShowSale(PotionType PotionType)
    {
        int amount = GameData.singleton.potionPrices[(int)PotionType];
        SpecialEffects.ShowNumberChange(GetPotionGroup(PotionType), amount, Color.yellow);
    }

    public void RestoreNormalSummaryPosition()
    {
        GameObject overlays = transform.Find("Overlays").gameObject;
        FancyUIAnimations.PushTranslation(overlays, new Vector2(485, overlays.GetComponent<RectTransform>().anchoredPosition.y), 0.5f);
    }

    private Vector2 mPreviousEventOverlayPos;
    private Vector2 mPreviousEventOverlayFacePos;
    public void MoveEventOverlayForTutorial()
    {
        mPreviousEventOverlayPos = SimulationEventContent.GetComponent<RectTransform>().anchoredPosition;
        RectTransform faceRect = SimulationEventContent.transform.Find("Face").GetComponent<RectTransform>();
        mPreviousEventOverlayFacePos = faceRect.anchoredPosition;
        SimulationEventContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-145, 0);
        faceRect.anchoredPosition = new Vector2(-450, 0);
    }

    public void RestoreNormalEventOverlayPosition()
    {
        SimulationEventContent.GetComponent<RectTransform>().anchoredPosition = mPreviousEventOverlayPos;
        SimulationEventContent.transform.Find("Face").GetComponent<RectTransform>().anchoredPosition = mPreviousEventOverlayFacePos;
    }

    public void UpdateEpilogueText()
    {
        if(GameStageExtensions.epilogueDirty)
        {
            string text = "";
            for(int i = 0; i < GameStageExtensions.epilogueLines.Count; ++i)
            {
                text += GameStageExtensions.epilogueLines[i] + "\n";
            }
            EpilogueView.transform.Find("Text").GetComponent<Text>().text = text;
            GameStageExtensions.epilogueDirty = false;
        }
    }
}
