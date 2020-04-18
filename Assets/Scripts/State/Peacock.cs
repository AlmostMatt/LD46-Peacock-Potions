using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    public float health = 50;
    public float happiness = 50;

    private float mFat = 50;
    private float mEatTimer = 1;

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
        int minFeathers = Mathf.RoundToInt(4 * (health / 100));
        int maxFeathers = Mathf.RoundToInt(6 * (health / 100));
        int totalFeathers = Random.Range(minFeathers,maxFeathers);
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            BusinessState.resources[resource] = totalFeathers;
        }

        string foodRating = "";
        if(mFat <= 0)
        {
            foodRating = "The peacock is skin and bone. It seems very weak.";
        }
        else if(mFat <= 25)
        {
            foodRating = "The peacock looks thin.";
        }
        else if(mFat <= 75)
        {
            foodRating = "The peacock looks fine.";
        }
        else if(mFat <= 100)
        {
            foodRating = "The peacock looks chubby. It seems relaxed.";
        }
        else
        {
            foodRating = "The peacock is clearly overweight. Its breathing is laboured.";
        }
        Debug.Log(foodRating + " " + mFat);
    }

    public void Update()
    {
        // over the course of the quarter, the peacock consumes food and changes its state based on various factors
        // keeping it alive should matter too!
        if(mFat > 0)
        {
            mFat -= Time.deltaTime;
        }

        mEatTimer -= Time.deltaTime;
        if(mEatTimer < 0)
        {
            // eat food
            if(BusinessState.peacockFood > 0)
            {
                BusinessState.peacockFood -= 1;
                mFat += 10;
            }
            else if(mFat < 0)
            {
                health -= Time.deltaTime;
            }
        }
    }
}
