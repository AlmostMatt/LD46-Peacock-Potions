using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfStockEvent : GameEvent
{
    private ProductType mProductType;

    public OutOfStockEvent(ProductType productType)
    {
        mProductType = productType;
    }

    protected override EventResult EventStart()
    {
        EventState.currentEventText = string.Format("A woman approaches the counter. \"Do you have any {0} potions?\"", mProductType.GetName().ToLower());
        EventState.currentEventOptions = new string[]
        {
            "NOPE GET OUT LOL"
        };
        return EventResult.DONE;
    }

    protected override EventResult OnPlayerDecision(int choice)
    {
        return EventResult.DONE;
    }
}
