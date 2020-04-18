using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    public float health = 50;
    public float happiness = 50;

    private static ResourceType[] producableResources = new ResourceType[]
    {
        ResourceType.RT_BLUE_FEATHER,        
        ResourceType.RT_GREEN_FEATHER,
        ResourceType.RT_RED_FEATHER,
        ResourceType.RT_GOLD_FEATHER
    };

    public void Produce()
    {
        // TODO: some mapping between animal state and what it produces
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            BusinessState.resources[resource] = Random.Range(50,100);
        }
    }
}
