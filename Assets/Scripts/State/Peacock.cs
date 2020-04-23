using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peacock
{
    public static void SetQuarterlyExtra(int extraType, bool active)
    {
        if(GameData.singleton.peacockQuarterlyExtras[extraType] != active)
        {
            int price = ((PeacockExtraType)extraType).GetPrice();
            if(active)
            {
                GameData.singleton.peacockQuarterlyTotalCost += price;                
            }
            else
            {
                GameData.singleton.peacockQuarterlyTotalCost -= price;
            }
            GameData.singleton.peacockQuarterlyExtras[extraType] = active;
        }
    }
    public static bool HasQuarterlyExtra(int extraType)
    {
        return GameData.singleton.peacockQuarterlyExtras[extraType];
    }

    public static ResourceType[] producableResources = new ResourceType[]
    {
        ResourceType.RT_GREEN_FEATHER,
        ResourceType.RT_RED_FEATHER,
        ResourceType.RT_GOLD_FEATHER,
        ResourceType.RT_BLUE_FEATHER
    };

    private static void Normalize(float[] dist)
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

    public static void EndOfQuarter()
    {
        // update health and stuff based on what it got during the quarter
        switch(GameData.singleton.peacockQuarterlyFoodType)
        {
            case FoodType.FT_CRAP:
                GameData.singleton.peacockHealth -= 20;
                GameData.singleton.peacockReportFoodDesc = "Fed it cheap food.";
                break;
            case FoodType.FT_BASIC:
                GameData.singleton.peacockReportFoodDesc = "Fed it basic food.";
                break;
            case FoodType.FT_DELUXE:
                GameData.singleton.peacockReportFoodDesc = "Fed it deluxe food.";
                GameData.singleton.peacockHappiness += 5;
                break;
        }

        switch(GameData.singleton.peacockQuarterlyActivity)
        {
            case PeacockActivityType.PA_STORY:
                GameData.singleton.peacockReportActivityDesc = "Read it tales of other lands.";
                GameData.singleton.peacockComfort += 2;
                break;
            case PeacockActivityType.PA_SING:
                GameData.singleton.peacockReportActivityDesc = "Sang it to sleep with seasonal songs.";
                GameData.singleton.peacockComfort += 10;
                GameData.singleton.peacockHappiness += 1;
                break;
            case PeacockActivityType.PA_HUG:
                GameData.singleton.peacockReportActivityDesc = "Hugged it every day.";
                GameData.singleton.peacockHappiness += 15;
                break;
        }

        if(GameData.singleton.peacockQuarterlyExtras[(int)PeacockExtraType.ET_MEDICINE])
        {
            if(GameData.singleton.peacockHealth >= 50)
            {
                GameData.singleton.peacockHealth -= 30; // don't give medicine to a GameData.singleton.peacockHealthly peacock!
            }
            else if(GameData.singleton.peacockHealth < 50)
            {
                GameData.singleton.peacockHealth = 50;
            }
        }

        if(GameData.singleton.peacockQuarterlyExtras[(int)PeacockExtraType.ET_HORMONES])
        {
            GameData.singleton.peacockHealth -= 20;
        }

        if(Random.Range(0f, 1f) < 0.1f && GameData.singleton.peacockHealth > 20)
        {
            GameData.singleton.peacockHealth -= 15;
            // sometimes it just gets sick
        }

        if(GameData.singleton.peacockHealth < 50 && GameData.singleton.peacockHealth > 25)
        {
            GameData.singleton.peacockHealth += 1; // sometimes it gets better on its own
        }

        float[] productionDistribution = null;
        switch(GameData.singleton.season)
        {
            case GameState.Season.SPRING:
                productionDistribution = new float[] { 60, 20, 0, 20 };
                break;
            case GameState.Season.SUMMER:
                productionDistribution = new float[] { 20, 60, 20, 0 };
                break;
            case GameState.Season.FALL:
                productionDistribution = new float[] { 0, 20, 60, 20 };
                break;
            case GameState.Season.WINTER:                
                productionDistribution = new float[] { 20, 0, 20, 60 };
                break;
        }

        Normalize(productionDistribution);

        bool readStory = GameData.singleton.peacockQuarterlyActivity == PeacockActivityType.PA_STORY;
        if(readStory)
        {
            for(int i = 0; i < productionDistribution.Length; ++i)
            {
                productionDistribution[i] = Mathf.Lerp(productionDistribution[i], 0.5f, 0.3f);
            }

            Normalize(productionDistribution);
        }
        else if(GameData.singleton.peacockQuarterlyActivity == PeacockActivityType.PA_SING)
        {
            for(int i = 0; i < productionDistribution.Length; ++i)
            {
                if(i == (int)GameData.singleton.season)
                {
                    productionDistribution[i] *= 1.5f;
                }
                else
                {
                    productionDistribution[i] *= 0.5f;
                }
            }
            Normalize(productionDistribution);
        }

        bool hasBlanket = GameData.singleton.peacockQuarterlyExtras[(int)PeacockExtraType.ET_PILLOW];
        float[] mBlanketModifier = new float[] { 1, 1.5f, 1, 0.5f };
        if(hasBlanket)
        {
            for(int i = 0; i < productionDistribution.Length; ++i)
            {
                productionDistribution[i] *= mBlanketModifier[i];
            }
            Normalize(productionDistribution);

            switch(GameData.singleton.season)               
            {
                case GameState.Season.SUMMER:
                    GameData.singleton.peacockComfort -= 20;
                    GameData.singleton.peacockHealth -= 10;
                    break;
            }
        }
        else
        {
            switch(GameData.singleton.season)
            {
                case GameState.Season.WINTER:
                    GameData.singleton.peacockComfort -= 10;
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
                    productionDistribution[i] = GameData.singleton.season == GameState.Season.WINTER ? 70 : (GameData.singleton.season == GameState.Season.SUMMER ? 0 : 15};
                    // productionDistribution[i] = ((1 - (GameData.singleton.peacockHappiness / 100)) + (GameData.singleton.peacockComfort / 100)) * 0.5f;
                    break;
                case ResourceType.RT_GREEN_FEATHER:
                    productionDistribution[i] = GameData.singleton.season == GameState.Season.SPRING ? 70 : 0;
                    //productionDistribution[i] = (1 - (GameData.singleton.peacockHealth / 100));
                    break;
                case ResourceType.RT_GOLD_FEATHER:
                    productionDistribution[i] = GameData.singleton.season == GameState.Season.FALL ? 1 : 0;
                    // productionDistribution[i] = (GameData.singleton.peacockHappiness / 100);
                    break;
                case ResourceType.RT_RED_FEATHER:
                    productionDistribution[i] = GameData.singleton.season == GameState.Season.SUMMER ? 1 : 0;
                    // productionDistribution[i] = ((GameData.singleton.peacockHealth / 100) + (1 - (GameData.singleton.peacockComfort / 100))) * 0.5f;
                    break;
                
            }
        }
        */

        float baseNumFeathers = Mathf.Lerp(0, 20, 0.5f * ((GameData.singleton.peacockHappiness / 100) + (GameData.singleton.peacockComfort / 100)));
        if(GameData.singleton.peacockQuarterlyFoodType == FoodType.FT_DELUXE)
        {
            baseNumFeathers += 10;
        }

        if(GameData.singleton.peacockQuarterlyExtras[(int)PeacockExtraType.ET_HORMONES])
        {
            baseNumFeathers *= 1.5f;
        }

        GameData.singleton.peacockReportFeatherCounts.Clear();
        int totalFeathers = Mathf.RoundToInt(baseNumFeathers * Random.Range(0.9f, 1.1f));
        for(int i = 0; i < producableResources.Length; ++i)
        {
            int resource = (int)producableResources[i];
            int numFeathers = Mathf.RoundToInt(totalFeathers * productionDistribution[i]);
            GameData.singleton.peacockReportFeatherCounts.Add(new ResourceAndCount((ResourceType)resource, numFeathers));
            GameData.singleton.resources[resource] += numFeathers;
        }

        string extraDesc = "Gave it ";
        int numExtrasSoFar = 0;
        if(GameData.singleton.peacockNumQuarterlyExtras == 0)
        {
            extraDesc += "nothing extra";
        }
        else
        {
            for(int i = 0; i < GameData.singleton.peacockQuarterlyExtras.Length; ++i)
            {
                if(GameData.singleton.peacockQuarterlyExtras[i])
                {
                    string name = ((PeacockExtraType)i).GetName();
                    if(GameData.singleton.peacockNumQuarterlyExtras == 1)
                    {
                        extraDesc += name;
                        break;
                    }
                    else if(GameData.singleton.peacockNumQuarterlyExtras == 2)
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
                    else // GameData.singleton.peacockNumQuarterlyExtras == 3
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
        GameData.singleton.peacockReportExtraDesc = extraDesc;

        string generalDesc;
        List<string> goodStrings = new List<string>();
        List<string> badStrings = new List<string>();
        if(GameData.singleton.peacockHealth < 25)
        {
            badStrings.Add("very sick");
        }
        else if(GameData.singleton.peacockHealth < 50)
        {
            badStrings.Add("a little sick");
        }
        else
        {
            goodStrings.Add("healthy");
        }

        if(GameData.singleton.peacockComfort <= 25)
        {
            badStrings.Add("very uncomfortable");
        }
        else if(GameData.singleton.peacockComfort <= 40)
        {
            badStrings.Add("a bit distressed");
        }
        else if(GameData.singleton.peacockComfort <= 60)
        {
            goodStrings.Add("comfortable");
        }
        else
        {
            goodStrings.Add("very relaxed");
        }

        if(GameData.singleton.peacockHappiness <= 25)
        {
            badStrings.Add("quite upset");
        }
        else if(GameData.singleton.peacockHappiness <= 40)
        {
            badStrings.Add("a little sad");
        }
        else if(GameData.singleton.peacockHappiness <= 60)
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
        GameData.singleton.peacockReportGeneralDesc = generalDesc;
    }
}
