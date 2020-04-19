using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    // if we decide to change our currency to something more magical, (e.g. 500g instead of $500)
    // this gives us a common place to change that.
    public static string FormatMoney(int amount)
    {
        return string.Format("${0}", amount);
    }
}
