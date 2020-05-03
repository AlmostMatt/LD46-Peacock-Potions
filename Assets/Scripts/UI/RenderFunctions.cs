using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RenderFunctions
{
    // Renders a string to text (or to a button)
    public static void RenderToText(string str, GameObject obj)
    {
        obj.GetComponentInChildren<Text>().text = str;
    }

    // Renders a list of strings to an object that has that many Text children
    public static void RenderStringArrayToTextChildren(string[] strings, GameObject obj)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            obj.transform.GetChild(i).GetComponent<Text>().text = strings[i];
        }
    }

    public static void RenderFeatherAndCountIgnoringUnlocks(FeatherAndCount feathAndCount, GameObject obj)
    {
        RenderFeatherAndCount(feathAndCount, obj);
        obj.SetActive(true);
    }

    // Render a FeatherAndCount (struct) to an IconAndCount (prefab)
    public static void RenderFeatherAndCount(FeatherAndCount feathAndCount, GameObject obj)
    {
        obj.SetActive(GameData.singleton.feathersUnlocked[(int)feathAndCount.type]);
        obj.GetComponentInChildren<Image>().sprite = null; // clear the sprite in case of weird sprite change bug
        obj.GetComponentInChildren<Image>().sprite = SpriteManager.GetSprite(feathAndCount.type.GetImage());
        obj.GetComponentInChildren<Image>().color = feathAndCount.type.GetColor();
        obj.GetComponentInChildren<Text>().text = feathAndCount.count.ToString();
    }

    // Render a PotionAndCount (struct) to an IconAndCount (prefab)
    public static void RenderPotionAndCount(PotionAndCount potionAndCount, GameObject obj)
    {
        obj.SetActive(GameData.singleton.potionsUnlocked[(int)potionAndCount.type]);
        obj.GetComponentInChildren<Image>().sprite = null; // clear the sprite in case of weird sprite change bug
        obj.GetComponentInChildren<Image>().sprite = SpriteManager.GetSprite(potionAndCount.type.GetImage());
        obj.GetComponentInChildren<Image>().color = potionAndCount.type.GetColor();
        obj.GetComponentInChildren<Text>().text = potionAndCount.count.ToString();
    }

    /**
     * Renders a row in the potion-sale screen
     */
     public static void RenderPotionSale(BusinessState.PerItemReport report, GameObject obj)
    {
        obj.SetActive(GameData.singleton.potionsUnlocked[(int)report.PotionType]);

        obj.transform.Find("H/Name").GetComponent<Text>().text = string.Format("{0}\nPotion", report.PotionType.GetName());
        obj.transform.Find("H/Icon").GetComponent<Image>().color = report.PotionType.GetColor();

        obj.transform.Find("H/Sales").GetComponent<Text>().text = string.Format("{0}x", report.numSold);
        obj.transform.Find("H/Profit").GetComponent<Text>().text = string.Format("${0}", report.numSold * report.salePrice);
        // TODO: Exclude reports that have 0 sales (so that they are not even rendered)
        obj.transform.Find("H/Profit").GetComponent<CanvasGroup>().alpha = (report.numSold == 0 ? 0f : 1f);
    }

    /**
     * Renders a row in the potion-crafting screen (shows sales and crafting options)
     */
    public static void RenderItemQuarterlySummary(BusinessState.PerItemReport report, GameObject obj)
    {
        obj.SetActive(GameData.singleton.potionsUnlocked[(int)report.PotionType]);

        obj.transform.Find("H/Name").GetComponent<Text>().text = string.Format("{0}\nPotion",report.PotionType.GetName());
        // TODO: customize the image
        // obj.transform.Find("H/Icon").GetComponent<Image>().sprite = null;   PotionType
        obj.transform.Find("H/Icon").GetComponent<Image>().color = report.PotionType.GetColor();

        // Losses (from events)
        obj.transform.Find("H/Losses").GetComponent<Text>().text = string.Format("{0}", report.numLost);
        obj.transform.Find("H/Losses").GetComponent<CanvasGroup>().alpha = (report.numLost == 0 ? 0f : 1f);
        // Sales
        obj.transform.Find("H/Price").GetComponent<Text>().text = string.Format("${0}", report.salePrice);
        obj.transform.Find("H/Sales").GetComponent<Text>().text = string.Format("{0}", report.numSold);
        obj.transform.Find("H/Profit").GetComponent<Text>().text = string.Format("${0}", report.numSold * report.salePrice);
        obj.transform.Find("H/Price").GetComponent<CanvasGroup>().alpha = (report.numSold == 0 ? 0f : 1f);
        obj.transform.Find("H/Sales").GetComponent<CanvasGroup>().alpha = (report.numSold == 0 ? 0f : 1f);
        obj.transform.Find("H/Profit").GetComponent<CanvasGroup>().alpha = (report.numSold == 0 ? 0f : 1f);
        // Inventory
        obj.transform.Find("H/Inventory").GetComponent<Text>().text = string.Format("{0}", report.endOfQStock);

        // Show the ingredients necessary to make a product
        FeatherAndCount[] price = report.PotionType.GetIngredients();
        RenderFeatherAndCountIgnoringUnlocks(price[0], obj.transform.Find("H/Ingredients/IconAndCount").gameObject);
        GameObject iconAndCount2 = obj.transform.Find("H/Ingredients/IconAndCount (1)").gameObject;
        iconAndCount2.SetActive(price.Length >= 2);
        if (price.Length >= 2)
        {
            RenderFeatherAndCountIgnoringUnlocks(price[1], iconAndCount2);
        }
        if (price.Length > 2)
        {
            Debug.LogError("A product had more than 2 ingredients - can't display it!");
        }

        // Set fields related to the input groups
        // Input group 1: num to create
        InputGroup createPotionGroup = obj.transform.Find("H/InputGroup").GetComponent<InputGroup>();
        createPotionGroup.SetInitialValue(0);
        createPotionGroup.SetCanDecrement(createPotionGroup.GetValue() > 0);
        createPotionGroup.SetCanIncrement(report.PotionType.PlayerHasIngredients());
        createPotionGroup.SetOnChangeCallback(NewNumPotionChangeCallback(report.PotionType));
        // Input group 2: sale price
        InputGroup setPriceGroup = obj.transform.Find("H/InputGroup (1)").GetComponent<InputGroup>();
        setPriceGroup.increments = 5;
        setPriceGroup.SetInitialValue(report.salePrice); // price from the previous quarter
        setPriceGroup.SetCanDecrement(setPriceGroup.GetValue() > setPriceGroup.increments);
        setPriceGroup.SetCanIncrement(setPriceGroup.GetValue() + setPriceGroup.increments <= 150); // TODO: generalize this max price
        setPriceGroup.SetOnChangeCallback(NewSetPriceCallback(report.PotionType));
        // Only show price if current inventory is non-zero
        setPriceGroup.transform.GetComponent<CanvasGroup>().alpha = (GameData.singleton.potionsOwned[(int)report.PotionType] == 0 ? 0f : 1f);
    }

    /** 
     * An on-changed callback function for an input group related to creating potions of a given type

     * It's a bit of a hack that this is here. There may also be a way to make this function always exist.
     */
    public static System.Action<int,int> NewNumPotionChangeCallback(PotionType PotionType)
    {
        // Delta indicates how many potions were just created (or refunded)
        return (int delta, int newCount) =>
        {
            while (delta > 0)
            {
                PotionType.SpendPlayerIngredients();
                GameData.singleton.potionsOwned[(int)PotionType] += 1;
                delta -= 1;
            }
            while (delta < 0)
            {
                PotionType.RefundPlayerIngredients();
                GameData.singleton.potionsOwned[(int)PotionType] -= 1;
                delta += 1;
            }
        };
    }

    public static System.Action<int, int> NewSetPriceCallback(PotionType PotionType)
    {
        return (int delta, int newValue) =>
        {
            GameData.singleton.potionPrices[(int)PotionType] = newValue;
            Debug.Log("Selling " + PotionType + " for " + GameData.singleton.potionPrices[(int)PotionType]);
        };
    }
}
