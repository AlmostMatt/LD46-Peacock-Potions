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
}
