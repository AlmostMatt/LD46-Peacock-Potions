using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    public float health = 75;
    public float happiness = 25;
    public float comfort = 50;

    public FoodType quarterlyFoodType = FoodType.FT_BASIC;
    public PeacockInteraction quarterlyInteraction;
    
    public class QuarterlyReport
    {
        public List<ResourceAndCount> featherCounts = new List<ResourceAndCount>();
    }
    public QuarterlyReport quarterlyReport = new QuarterlyReport();

    private static ResourceType[] producableResources = new ResourceType[]
    {
        ResourceType.RT_BLUE_FEATHER,        
        ResourceType.RT_GREEN_FEATHER,
        ResourceType.RT_RED_FEATHER,
        ResourceType.RT_GOLD_FEATHER
    };
    private float[] mProductionDistribution = new float[producableResources.Length];

    public void QuarterOver()
    {
        quarterlyReport = new QuarterlyReport();

        // update health and stuff based on what it got during the quarter
        switch(quarterlyFoodType)
        {
            case FoodType.FT_CRAP:
                health -= 20;
                break;
            case FoodType.FT_BASIC:
                break;
            case FoodType.FT_DELUXE:
                health += 20;
                break;
        }

        float totalDist = 0;
        for(int i = 0; i < producableResources.Length; ++i)
        {
            ResourceType resource = producableResources[i];
            switch(resource)
            {
                case ResourceType.RT_BLUE_FEATHER:
                    mProductionDistribution[i] = ((1 - (happiness / 100)) + (comfort / 100)) * 0.5f;
                    break;
                case ResourceType.RT_GREEN_FEATHER:
                    mProductionDistribution[i] = (1 - (health / 100));
                    break;
                case ResourceType.RT_GOLD_FEATHER:
                    mProductionDistribution[i] = (happiness / 100);
                    break;
                case ResourceType.RT_RED_FEATHER:
                    mProductionDistribution[i] = ((health / 100) + (1 - (comfort / 100))) * 0.5f;
                    break;
            }
            totalDist += mProductionDistribution[i];
        }
        // normalize distribution
        for(int i = 0; i < producableResources.Length; ++i)
        {
            mProductionDistribution[i] /= totalDist;
        }

        // TODO: some mapping between animal state and what it produces
        float baseNumFeathers = Mathf.Lerp(10, 100, (happiness / 100));
        int totalFeathers = Mathf.RoundToInt(baseNumFeathers * Random.Range(0.9f, 1.1f));
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            int numFeathers = Mathf.RoundToInt(totalFeathers * mProductionDistribution[i]);
            quarterlyReport.featherCounts.Add(new ResourceAndCount((ResourceType)resource, numFeathers));
            BusinessState.resources[resource] += numFeathers;
        }

        string foodRating = "";
        if(health <= 0)
        {
            foodRating = "The peacock is skin and bone. It seems very weak.";
        }
        else if(health <= 25)
        {
            foodRating = "The peacock looks thin.";
        }
        else if(health <= 75)
        {
            foodRating = "The peacock looks fine.";
        }
        else if(health <= 100)
        {
            foodRating = "The peacock looks chubby. It seems relaxed.";
        }
        else
        {
            foodRating = "The peacock is clearly overweight. Its breathing is laboured.";
        }
        Debug.Log(foodRating + " " + health);
    }
}
