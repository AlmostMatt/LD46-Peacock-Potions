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

    public static void RenderItemQuarterlySummary(BusinessState.PerItemReport report, GameObject obj)
    {
        // TODO: customize the image
        // obj.transform.Find("H/Icon").GetComponent<Image>().sprite = null;
        obj.transform.Find("H/Text1").GetComponent<Text>().text = string.Format("Had {0}", report.previousStock);
        obj.transform.Find("H/Text2").GetComponent<Text>().text = string.Format("Created {0}", report.numProduced);
        obj.transform.Find("H/Text3").GetComponent<Text>().text = string.Format("Sold {0}", report.numSold);
        obj.transform.Find("H/Text4").GetComponent<Text>().text = string.Format("Lost {0}", report.numLost);
        obj.transform.Find("H/Text5").GetComponent<Text>().text = string.Format("Have {0}", report.currentStock);
    }
}
