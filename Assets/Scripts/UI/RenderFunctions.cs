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
        // Ingredients
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
            Debug.LogError("A product had more than 2 ingredients - cant display it!");
        }
        // 1 2
        // TODO: don't set num to create and price for next quarter if they have already been edited by the player
        obj.transform.Find("H/Text3").GetComponent<Text>().text = string.Format("0"); // num to create
        obj.transform.Find("H/Text4").GetComponent<Text>().text = string.Format("{0}", report.currentStock);
        obj.transform.Find("H/Text5").GetComponent<Text>().text = string.Format("${0}", report.salePrice); // price for next quarter
    }
}
