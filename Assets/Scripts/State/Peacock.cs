using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    private float mHealth = 75;
    public float health
    {
        get { return mHealth; }
        set { mHealth = Mathf.Clamp(value, 0, 100); }
    }
    public float happiness = 25;
    public float comfort = 35;

    public FoodType quarterlyFoodType = FoodType.FT_BASIC;
    public PeacockActivityType quarterlyActivity = PeacockActivityType.PA_SING;

    private int mQuarterlyTotalCost = FoodType.FT_BASIC.GetPrice();
    public int quarterlyTotalCost
    {
        get { return mQuarterlyTotalCost; }
    }
    private int mQuarterlyFoodCost = FoodType.FT_BASIC.GetPrice();
    public int quarterlyFoodCost
    {
        get { return mQuarterlyFoodCost; }
        set
        {
            mQuarterlyTotalCost -= mQuarterlyFoodCost;
            mQuarterlyFoodCost = value;
            mQuarterlyTotalCost += value;
        }
    }
    private bool[] mQuarterlyExtras = new bool[(int)PeacockExtraType.ET_MAX];
    private int mNumExtras = 0;
    public void SetQuarterlyExtra(int extraType, bool active)
    {
        if(mQuarterlyExtras[extraType] != active)
        {
            int price = ((PeacockExtraType)extraType).GetPrice();
            if(active)
            {
                mQuarterlyTotalCost += price;
                ++mNumExtras;
            }
            else
            {
                mQuarterlyTotalCost -= price;
                --mNumExtras;
            }
            mQuarterlyExtras[extraType] = active;
        }
    }
    public bool HasQuarterlyExtra(int extraType)
    {
        return mQuarterlyExtras[extraType];
    }

    public class QuarterlyReport
    {
        public string foodDesc = "Fed it good food.";
        public string activityDesc = "Read it a story every night.";
        public string extraDesc = "Gave it nothing extra.";
        public string generalDesc = "It looks healthy, relaxed, and happy.";
        public List<ResourceAndCount> featherCounts = new List<ResourceAndCount>();
    }
    public QuarterlyReport quarterlyReport = new QuarterlyReport();

    private static ResourceType[] producableResources = new ResourceType[]
    {
        ResourceType.RT_GREEN_FEATHER,
        ResourceType.RT_RED_FEATHER,
        ResourceType.RT_GOLD_FEATHER,
        ResourceType.RT_BLUE_FEATHER
    };
    private float[] mProductionDistribution = new float[producableResources.Length];

    public void StartQuarter()
    {
    }

    private void Normalize(float[] dist)
    {
        float totalDist = 0;
        for(int i = 0; i < dist.Length; ++i)
        {
            totalDist += dist[i];
        }        
        for(int i = 0; i < dist.Length; ++i)
        {
            dist[i] /= totalDist;
        }
    }

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
                quarterlyReport.foodDesc = "Fed it basic food.";
                break;
            case FoodType.FT_DELUXE:
                quarterlyReport.foodDesc = "Fed it deluxe food.";
                happiness += 5;
                break;
        }

        switch(quarterlyActivity)
        {
            case PeacockActivityType.PA_STORY:
                quarterlyReport.activityDesc = "Read it tales of other lands.";
                comfort += 2;
                break;
            case PeacockActivityType.PA_SING:
                quarterlyReport.activityDesc = "Sang it to sleep with seasonal songs.";
                comfort += 10;
                happiness += 1;
                break;
            case PeacockActivityType.PA_HUG:
                quarterlyReport.activityDesc = "Hugged it every day.";
                happiness += 15;
                break;
        }

        if(mQuarterlyExtras[(int)PeacockExtraType.ET_MEDICINE])
        {
            health += 30;
        }

        if(mQuarterlyExtras[(int)PeacockExtraType.ET_HORMONES])
        {
            health -= 20;
        }

        if(Random.Range(0f, 1f) < 0.1f && health > 20)
        {
            health -= 15;
            // sometimes it just gets sick
        }

        if(health < 50 && health > 25)
        {
            health += 1; // sometimes it gets better on its own
        }

        switch(GameState.season)
        {
            case GameState.Season.SPRING:
                mProductionDistribution = new float[] { 60, 20, 0, 20 };
                break;
            case GameState.Season.SUMMER:
                mProductionDistribution = new float[] { 20, 60, 20, 0 };
                break;
            case GameState.Season.FALL:
                mProductionDistribution = new float[] { 0, 20, 60, 20 };
                break;
            case GameState.Season.WINTER:                
                mProductionDistribution = new float[] { 20, 0, 20, 60 };
                break;
        }

        Normalize(mProductionDistribution);

        bool readStory = quarterlyActivity == PeacockActivityType.PA_STORY;
        if(readStory)
        {
            for(int i = 0; i < mProductionDistribution.Length; ++i)
            {
                mProductionDistribution[i] = Mathf.Lerp(mProductionDistribution[i], 0.5f, 0.3f);
            }

            Normalize(mProductionDistribution);
        }
        else if(quarterlyActivity == PeacockActivityType.PA_SING)
        {
            for(int i = 0; i < mProductionDistribution.Length; ++i)
            {
                if(i == (int)GameState.season)
                {
                    mProductionDistribution[i] *= 1.5f;
                }
                else
                {
                    mProductionDistribution[i] *= 0.5f;
                }
            }
            Normalize(mProductionDistribution);
        }

        bool hasBlanket = mQuarterlyExtras[(int)PeacockExtraType.ET_PILLOW];
        float[] mBlanketModifier = new float[] { 1, 1.5f, 1, 0.5f };
        if(hasBlanket)
        {
            for(int i = 0; i < mProductionDistribution.Length; ++i)
            {
                mProductionDistribution[i] *= mBlanketModifier[i];
            }
            Normalize(mProductionDistribution);

            switch(GameState.season)               
            {
                case GameState.Season.SUMMER:
                    comfort -= 20;
                    health -= 10;
                    break;
            }
        }
        else
        {
            switch(GameState.season)
            {
                case GameState.Season.WINTER:
                    comfort -= 10;
                    break;
            }
        }
        
        /*
        for(int i = 0; i < producableResources.Length; ++i)
        {

            ResourceType resource = producableResources[i];
            switch(resource)
            {
                case ResourceType.RT_BLUE_FEATHER:
                    mProductionDistribution[i] = GameState.season == GameState.Season.WINTER ? 70 : (GameState.season == GameState.Season.SUMMER ? 0 : 15};
                    // mProductionDistribution[i] = ((1 - (happiness / 100)) + (comfort / 100)) * 0.5f;
                    break;
                case ResourceType.RT_GREEN_FEATHER:
                    mProductionDistribution[i] = GameState.season == GameState.Season.SPRING ? 70 : 0;
                    //mProductionDistribution[i] = (1 - (health / 100));
                    break;
                case ResourceType.RT_GOLD_FEATHER:
                    mProductionDistribution[i] = GameState.season == GameState.Season.FALL ? 1 : 0;
                    // mProductionDistribution[i] = (happiness / 100);
                    break;
                case ResourceType.RT_RED_FEATHER:
                    mProductionDistribution[i] = GameState.season == GameState.Season.SUMMER ? 1 : 0;
                    // mProductionDistribution[i] = ((health / 100) + (1 - (comfort / 100))) * 0.5f;
                    break;
                
            }
        }
        */

        float baseNumFeathers = Mathf.Lerp(0, 20, 0.5f * ((happiness / 100) + (comfort / 100)));
        if(mQuarterlyExtras[(int)PeacockExtraType.ET_HORMONES])
        {
            baseNumFeathers *= 1.5f;
        }
        int totalFeathers = Mathf.RoundToInt(baseNumFeathers * Random.Range(0.9f, 1.1f));
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            int numFeathers = Mathf.RoundToInt(totalFeathers * mProductionDistribution[i]);
            quarterlyReport.featherCounts.Add(new ResourceAndCount((ResourceType)resource, numFeathers));
            BusinessState.resources[resource] += numFeathers;
        }

        string extraDesc = "Gave it ";
        int numExtrasSoFar = 0;
        if(mNumExtras == 0)
        {
            extraDesc += "nothing extra";
        }
        else
        {
            for(int i = 0; i < mQuarterlyExtras.Length; ++i)
            {
                if(mQuarterlyExtras[i])
                {
                    string name = ((PeacockExtraType)i).GetName();
                    if(mNumExtras == 1)
                    {
                        extraDesc += name;
                        break;
                    }
                    else if(mNumExtras == 2)
                    {
                        if(numExtrasSoFar == 0)
                        {
                            extraDesc += name + " and ";
                        }
                        else
                        {
                            extraDesc += name;
                            break;
                        }
                    }
                    else // mNumExtras == 3
                    {
                        switch(numExtrasSoFar)
                        {
                            case 0:
                            case 1:
                                extraDesc += name + ", ";
                                break;
                            case 2:
                                extraDesc += "and " + name;
                                break;
                        }
                    }
                    ++numExtrasSoFar;
                }
            }
        }
        extraDesc += ".";
        quarterlyReport.extraDesc = extraDesc;

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

        if(comfort <= 25)
        {
            badStrings.Add("very uncomfortable");
        }
        else if(comfort <= 40)
        {
            badStrings.Add("a bit distressed");
        }
        else if(comfort <= 60)
        {
            goodStrings.Add("comfortable");
        }
        else
        {
            goodStrings.Add("very relaxed");
        }

        if(happiness <= 25)
        {
            badStrings.Add("quite upset");
        }
        else if(happiness <= 40)
        {
            badStrings.Add("a little sad");
        }
        else if(happiness <= 60)
        {
            goodStrings.Add("content");
        }
        else
        {
            goodStrings.Add("very happy");
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
