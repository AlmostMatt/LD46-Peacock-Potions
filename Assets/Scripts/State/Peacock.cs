using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    public float health = 75;
    public float happiness = 25;
    public float comfort = 50;

    public FoodType quarterlyFoodType = FoodType.FT_BASIC;
    public PeacockActivityType quarterlyActivity = PeacockActivityType.PA_STORY;
    public PeacockInteraction quarterlyInteraction;
    
    public class QuarterlyReport
    {
        public string foodDesc;
        public string activityDesc;
        public string extraDesc;
        public string generalDesc;
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
                quarterlyReport.foodDesc = "Fed it cheap food.";
                break;
            case FoodType.FT_BASIC:
                quarterlyReport.foodDesc = "Fed it good food.";
                break;
            case FoodType.FT_DELUXE:
                quarterlyReport.foodDesc = "Fed it deluxe food.";
                health += 20;
                break;
        }

        switch(quarterlyActivity)
        {
            case PeacockActivityType.PA_STORY:
                quarterlyReport.activityDesc = "Read it a story every night.";
                break;
            case PeacockActivityType.PA_SING:
                quarterlyReport.activityDesc = "Sang it a song every week.";
                break;
            case PeacockActivityType.PA_HUG:
                quarterlyReport.activityDesc = "Hugged it every day.";
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

        float baseNumFeathers = Mathf.Lerp(10, 100, (happiness / 100));
        int totalFeathers = Mathf.RoundToInt(baseNumFeathers * Random.Range(0.9f, 1.1f));
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            int numFeathers = Mathf.RoundToInt(totalFeathers * mProductionDistribution[i]);
            quarterlyReport.featherCounts.Add(new ResourceAndCount((ResourceType)resource, numFeathers));
            BusinessState.resources[resource] += numFeathers;
        }

        string generalDesc;
        List<string> goodStrings = new List<string>();
        List<string> badStrings = new List<string>();
        if(health < 50)
        {
            badStrings.Add("a little sick");
        }
        else
        {
            goodStrings.Add("healthy");
        }

        if(comfort < 50)
        {
            badStrings.Add("stressed");
        }
        else
        {
            goodStrings.Add("relaxed");
        }

        if(happiness < 50)
        {
            badStrings.Add("sad");
        }
        else
        {
            goodStrings.Add("happy");
        }

        if(goodStrings.Count == 3)
        {
            generalDesc = string.Format("It looks {0}, {1} and {2}.", goodStrings[0], goodStrings[1], goodStrings[2]);
        }
        else if(badStrings.Count == 3)
        {
            generalDesc = string.Format("It looks {0}, {1} and {2}.", badStrings[0], badStrings[1], badStrings[2]);
        }
        else if(goodStrings.Count > badStrings.Count)
        {
            generalDesc = string.Format("It looks {0} and {1}, but {2}.", goodStrings[0], goodStrings[1], badStrings[0]);
        }
        else
        {
            generalDesc = string.Format("It looks {0} and {1}, but {2}.", badStrings[0], badStrings[1], goodStrings[0]);
        }
        quarterlyReport.generalDesc = generalDesc;

    }
}
