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

    public static void RenderItemQuarterlySummary(string str, GameObject obj)
    {
        // TODO: customize the image
        // obj.transform.Find("H/Icon").GetComponent<Image>().sprite = null;
        obj.transform.Find("H/Text1").GetComponent<Text>().text = "Had 3";
        obj.transform.Find("H/Text2").GetComponent<Text>().text = "Created 3";
        obj.transform.Find("H/Text3").GetComponent<Text>().text = "Sold 3";
        obj.transform.Find("H/Text4").GetComponent<Text>().text = "Have 3";
    }
}
