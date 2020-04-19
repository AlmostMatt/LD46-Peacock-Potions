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

    // Render a ResourceAndCount (struct) to an IconAndCount (prefab)
    public static void RenderResourceAndCount(ResourceAndCount resAndCount, GameObject obj)
    {
        // TODO: set icon and icon color
        obj.GetComponentInChildren<Image>().color = resAndCount.type.GetColor();
        obj.GetComponentInChildren<Text>().text = resAndCount.count.ToString();
    }

    public static void RenderItemQuarterlySummary(BusinessState.PerItemReport report, GameObject obj)
    {
        // TODO: customize the image
        // obj.transform.Find("H/Icon").GetComponent<Image>().sprite = null;   productType
        // TODO: set some fields to empty string for zero values

        obj.transform.Find("H/Text1").GetComponent<Text>().text = string.Format("{0}", report.numLost);
        obj.transform.Find("H/Text2").GetComponent<Text>().text = string.Format("{0}x${1} = ${2}", report.numSold, report.salePrice, report.numSold*report.salePrice);
        obj.transform.Find("H/Text4").GetComponent<Text>().text = string.Format("{0}", report.currentStock);

        // Show the ingredients necessary to make a product
        ResourceAndCount[] price = report.productType.GetIngredients();
        RenderResourceAndCount(price[0], obj.transform.Find("H/Ingredients/IconAndCount").gameObject);
        GameObject iconAndCount2 = obj.transform.Find("H/Ingredients/IconAndCount (1)").gameObject;
        iconAndCount2.SetActive(price.Length >= 2);
        if (price.Length >= 2)
        {
            RenderResourceAndCount(price[1], iconAndCount2);
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
        createPotionGroup.SetCanIncrement(report.productType.PlayerHasIngredients());
        createPotionGroup.SetOnChangeCallback(NewNumPotionChangeCallback(report.productType));
        // Input group 2: sale price
        InputGroup setPriceGroup = obj.transform.Find("H/InputGroup (1)").GetComponent<InputGroup>();
        setPriceGroup.SetInitialValue(report.salePrice); // price from the previous quarter
    }

    /** 
     * An on-changed callback function for an input group related to creating potions of a given type

     * It's a bit of a hack that this is here. There may also be a way to make this function always exist.
     */
    public static System.Action<int,int> NewNumPotionChangeCallback(ProductType productType)
    {
        // Delta indicates how many potions were just created (or refunded)
        return (int delta, int newCount) =>
        {
            while (delta > 0)
            {
                productType.SpendPlayerIngredients();
                BusinessState.inventory[(int)productType] += 1;
                delta -= 1;
            }
            while (delta < 0)
            {
                productType.RefundPlayerIngredients();
                BusinessState.inventory[(int)productType] -= 1;
                delta += 1;
            }
        };
    }
}
