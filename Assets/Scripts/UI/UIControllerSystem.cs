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
    private RenderableGroup<ResourceAndCount> mInventoryResourceRenderGroup;
    private RenderableGroup<ProductAndCount> mInventoryProductRenderGroup;

    private RenderableGroup<ResourceAndCount> mPeacockFeatherRenderGroup;

    private RenderableGroup<ResourceAndCount> mOverlayFeatherCollectionFeatherRenderGroup;
    private RenderableGroup<BusinessState.PerItemReport> mOverlayPotionSalesRenderGroup;
    private RenderableGroup<string[]> mOverlayFinancialRenderGroup;

    // Use this for initialization
    void Start()
    {
        mItemQuarterlySummaryRenderGroup = new RenderableGroup<BusinessState.PerItemReport>(
            SummaryView.transform.Find("ItemSummaries"),
            RenderFunctions.RenderItemQuarterlySummary);

        mInventoryResourceRenderGroup = new RenderableGroup<ResourceAndCount>(
            InventoryView.transform.Find("InvGroups/InventoryFeathers"),
            RenderFunctions.RenderResourceAndCount);
        mInventoryProductRenderGroup = new RenderableGroup<ProductAndCount>(
            InventoryView.transform.Find("InvGroups/InventoryPotions"),
            RenderFunctions.RenderProductAndCount);

        mPeacockFeatherRenderGroup = new RenderableGroup<ResourceAndCount>(
            PeacockView.transform.Find("InventoryFeathers"),
            RenderFunctions.RenderResourceAndCount);
        // Overlays
        mOverlayFeatherCollectionFeatherRenderGroup = new RenderableGroup<ResourceAndCount>(
            OverlayViewFeathers.transform.Find("Content/InventoryFeathers"),
            RenderFunctions.RenderResourceAndCount);
        mOverlayPotionSalesRenderGroup = new RenderableGroup<BusinessState.PerItemReport>(
            OverlayViewPotions.transform.Find("PotionSaleRows"),
            RenderFunctions.RenderPotionSale);
        mOverlayFinancialRenderGroup = new RenderableGroup<string[]>(
            OverlayViewFinancial.transform.Find("Content"),
            RenderFunctions.RenderStringArrayToTextChildren);


        // don't start with all customers visible
        RandomizeInitialCustomers();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUiVisibility();
        UpdateUiContent();

        if (GameState.currentStage == GameState.GameStage.GS_SIMULATION && EventState.currentEvent == null)
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
    private void UpdateUiVisibility()
    {
        GameState.GameStage stage = GameState.currentStage;
        // Summary / resource allocation
        SummaryView.SetActive(stage == GameState.GameStage.GS_RESOURCE_ALLOCATION);
        // Peacock UI
        PeacockView.SetActive(stage == GameState.GameStage.GS_PEACOCK);
        // UI shared by Simulation / Event and visible behind some overlays
        SimulationView.SetActive(
            stage == GameState.GameStage.GS_EVENT
            || stage == GameState.GameStage.GS_SIMULATION
            || stage == GameState.GameStage.GS_OVERLAY_POTION_SALES
            || stage == GameState.GameStage.GS_OVERLAY_FINANCIAL_SUMMARY
            || stage == GameState.GameStage.GS_OVERLAY_FEATHER_COLLECTION
        );
        // Overlay UIs
        OverlayViewFeathers.SetActive(stage == GameState.GameStage.GS_OVERLAY_FEATHER_COLLECTION);
        OverlayViewPotions.SetActive(stage == GameState.GameStage.GS_OVERLAY_POTION_SALES);
        OverlayViewFinancial.SetActive(stage == GameState.GameStage.GS_OVERLAY_FINANCIAL_SUMMARY);

        InventoryView.SetActive(stage != GameState.GameStage.GS_GAME_OVER);
        // Simulation
        // Fade the background?
        //SimulationDefaultContent.GetComponent<CanvasGroup>().alpha = (stage == GameState.GameStage.GS_SIMULATION ? 1.0f : 0.5f);
        // Event
        bool hasEvent = EventState.currentEvent != null;        
        SimulationEventContent.SetActive(hasEvent || mPrevHasEvent); // hack a one-frame delay on hiding the event display, in case we want to show 2 in a row. Does mean there's a 1-frame window for the player to click the button when there's no event...
        mPrevHasEvent = hasEvent;

        // ending
        EpilogueView.SetActive(stage == GameState.GameStage.GS_GAME_OVER);
    }

    // Change text and other fields in UI content
    private void UpdateUiContent()
    {
        GameState.GameStage stage = GameState.currentStage;
        /**
         * Inventory
         */
        {
            // Inventory (cash)
            InventoryView.transform.Find("InvGroups/InventoryCash/IconAndCount/Count")
                .GetComponent<Text>().text = string.Format("{0}", BusinessState.money);
            // Inventory (feathers)
            List<ResourceAndCount> resourceCounts = new List<ResourceAndCount>();
            for (int i = 0; i < (int)ResourceType.RT_MAX; i++)
            {
                resourceCounts.Add(new ResourceAndCount((ResourceType)i, BusinessState.resources[i]));
            }
            mInventoryResourceRenderGroup.UpdateRenderables(resourceCounts);
            // Inventory (feathers)
            List<ProductAndCount> productCounts = new List<ProductAndCount>();
            for (int i = 0; i < (int)ProductType.PT_MAX; i++)
            {
                productCounts.Add(new ProductAndCount((ProductType)i, BusinessState.inventory[i]));
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
            mPeacockFeatherRenderGroup.UpdateRenderables(BusinessState.peacock.quarterlyReport.featherCounts);
        }
        /**
         * Simulation / Event
         */
        if (SimulationView.activeInHierarchy)
        {
            // Color and show/hide potions in the shop
            for (int i = 0; i < (int)ProductType.PT_MAX; i++)
            {
                var PotionGroup = GetPotionGroup((ProductType)i);
                for (int j = 0; j < PotionGroup.childCount; j++)
                {
                    PotionGroup.GetChild(j).gameObject.SetActive(j < BusinessState.inventory[i]);
                    PotionGroup.GetChild(j).GetComponent<Image>().color = ((ProductType)i).GetColor();
                    // TODO: if a potion stopped being visible it was just sold. Show the +money animation there
                }
            }
            SimulationDefaultContent.transform.Find("Shop/Season").GetComponent<Image>().sprite = null; // clear the sprite in case of weird sprite change bug
            SimulationDefaultContent.transform.Find("Shop/Season").GetComponent<Image>().sprite = SpriteManager.GetSprite(GameState.season.GetImage());
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
                BusinessState.peacock.quarterlyReport.featherCounts);
        } else if (OverlayViewPotions.activeInHierarchy)
        {
            mOverlayPotionSalesRenderGroup.UpdateRenderables(BusinessState.GetPerItemReports());
            OverlayViewPotions.transform.Find("PotionSaleTotal/Row/H/Total").GetComponent<Text>().text
                = string.Format("${0}",totalPotionRevenue);
        } else if (OverlayViewFinancial.activeInHierarchy)
        {
            int startOfQuarterBalance = BusinessState.quarterlyReport.initialBalance;
            int eventIncome = BusinessState.quarterlyReport.eventIncome;
            int peacockExpenses = BusinessState.peacock.quarterlyTotalCost;
            int eventExpense = BusinessState.quarterlyReport.eventExpenses;
            int profit = (totalPotionRevenue + eventIncome) - (peacockExpenses + eventExpense);
            int rent = BusinessState.quarterlyReport.livingExpenses;
            int newBalance = (int)BusinessState.money - rent;
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

        GameState.GameLoopGotoNextStage();
        Debug.Log("game stage is now " + GameState.currentStage);
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

    public void PeacockScreenOK()
    {
        GameState.GameLoopGotoNextStage();
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
                if (GameState.quarter < 2) {
                    go.GetComponent<CanvasGroup>().alpha = 0;
                    FancyUIAnimations.PushFadeIn(go);
                }
            }
        }

        // I assume there's a more proper way to do this, but I'm too lazy to figure it out
        PeacockView.transform.Find("Date").GetComponent<Text>().text = string.Format("{0}, Year {1}", GameState.season.GetName(), GameState.year);
        PeacockView.transform.Find("FoodReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.foodDesc;
        PeacockView.transform.Find("ActivityReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.activityDesc;
        PeacockView.transform.Find("ExtraReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.extraDesc;
        PeacockView.transform.Find("StatusReport").GetComponent<Text>().text = BusinessState.peacock.quarterlyReport.generalDesc;
        PeacockView.transform.Find("NextQuarterTitle").GetComponent<Text>().text = string.Format("Plan for the {0}: ", GameState.season.GetNextSeasonName());

        Transform foodPlan = PeacockView.transform.Find("FoodPlan");
        for(int i = 0; i < (int)FoodType.FT_MAX; ++i)
        {
            FoodType food = ((FoodType)i);
            Transform button = foodPlan.GetChild(i);
            button.GetChild(0).GetComponent<Text>().text = food.GetLabel();
            if((int)BusinessState.peacock.quarterlyFoodType == i)
            {
                 button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }

        Transform activityPlan = PeacockView.transform.Find("ActivityPlan");
        for(int i = 0; i < (int)PeacockActivityType.PA_MAX; ++i)
        {
            PeacockActivityType activity = ((PeacockActivityType)i);
            Transform button = activityPlan.GetChild(i);
            button.GetChild(0).GetComponent<Text>().text = activity.GetLabel();
            if((int)BusinessState.peacock.quarterlyActivity == i)
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
            if(BusinessState.peacock.HasQuarterlyExtra(i))
            {
                 button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }

        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(BusinessState.peacock.quarterlyTotalCost);
    }

    public void PeacockScreenFood(int foodType)
    {
        Debug.Log("food select " + (FoodType)foodType);
        Transform selections = PeacockView.transform.Find("FoodPlan");
        for (int i = 0; i < selections.childCount; ++i)
        {
            Transform button = selections.GetChild(i);
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(1f, 1f, 1f, (i == foodType) ? 1f : 0.1f);
            }
        }
        FoodType food = ((FoodType)foodType);
        int price = food.GetPrice();
        //selections.Find("FoodCost").GetComponent<Text>().text = Utilities.FormatMoney(price);
        BusinessState.peacock.quarterlyFoodCost = price;
        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(BusinessState.peacock.quarterlyTotalCost);
        BusinessState.peacock.quarterlyFoodType = (FoodType)foodType;
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
                img.color = new Color(1f, 1f, 1f, (i == activityType) ? 1f : 0.1f);
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
        bool prevSelected = img.color.a == 1f;
        img.color = new Color(1f, 1f, 1f, prevSelected ? 0.1f : 1f);
        BusinessState.peacock.SetQuarterlyExtra(extraType, !prevSelected);

        // button.Find("Cost").GetComponent<Text>().text = Utilities.FormatMoney(selected ? ((PeacockExtraType)extraType).GetPrice() : 0);

        PeacockView.transform.Find("CostTitle/Cost").GetComponent<Text>().text = Utilities.FormatMoney(BusinessState.peacock.quarterlyTotalCost);
    }

    // --------- END PEACOCK SCREEN ------------ //

    public void OverlayScreenOK()
    {
        GameState.GameLoopGotoNextStage();
    }

    private Transform GetPotionGroup(ProductType productType)
    {
        return SimulationDefaultContent.transform.Find("Potions").GetChild((int)productType);
    }

    // TODO: add UI or audio feedback for a sale happening.
    // This is called from BusinessSystem
    public void ShowSale(ProductType productType)
    {
        int amount = (int)BusinessState.prices[(int)productType];
        SpecialEffects.ShowNumberChange(GetPotionGroup(productType), amount, Color.yellow);
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
        if(GameState.epilogueDirty)
        {
            string text = "";
            for(int i = 0; i < GameState.epilogueLines.Count; ++i)
            {
                text += GameState.epilogueLines[i] + "\n";
            }
            EpilogueView.transform.Find("Text").GetComponent<Text>().text = text;
            GameState.epilogueDirty = false;
        }
    }
}
