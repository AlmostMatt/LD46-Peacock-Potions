using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
public class GameData
{
    // everything we need saved goes in here.
    // gameplay code accesses it directly

    public static GameData singleton = new GameData();
    
    // GameState
    public GameState.GameStage currentStage = GameState.GameStage.GS_MAIN_MENU;
    public int quarter = 0;
    public float quarterTime = 0; // seconds elapsed in the current quarter
    public int yearsSkipped = 0;
    public int year
    {
        get { return 452 + (quarter / 4); }
    }
    public int elapsedYears
    {
        get { return yearsSkipped + (quarter / 4); }
    }
    public GameState.Season season // TODO: just make Season an enum in its own file
    {
        get { return (GameState.Season)(quarter % 4); }
    }
    public bool reachedEndOfLife = false;

    // BusinessState
    public int rent = 0;
    public int money = 0;
    public int[] resources = new int[(int)ResourceType.RT_MAX];
    public int[] inventory = new int[(int)ProductType.PT_MAX];
    public int[] potionPrices = new int[(int)ProductType.PT_MAX];
    public bool missedRent = false;
    
    // BusinessState.QuarterlyReport
    public int[] unsoldPotions = new int[(int)ProductType.PT_MAX];
    public int[] quarterlyReportSalePrices = new int[(int)ProductType.PT_MAX];
    public int[] quarterlySales = new int[(int)ProductType.PT_MAX];
    public int[] unfulfilledDemand = new int[(int)ProductType.PT_MAX];
    public int[] miscLosses = new int[(int)ProductType.PT_MAX];
    public int initialBalance = 0;
    public int finalBalance = 0;
    public int livingExpenses = 0;
    public int eventIncome = 0;
    public int eventExpenses = 0;

    // CustomerState
    public int totalPopulation = 1000;
    public float storePopularity = 0;
    public float[] productDemand = new float[(int)ProductType.PT_MAX];
    public float[] optimalPrices = new float[(int)ProductType.PT_MAX];
    // recomputed each quarter by MainGameSystem
    public int[] quarterlyCustomers = new int[(int)ProductType.PT_MAX];
    public int totalQuarterlyCustomers = 0;

    // RelationshipState
    public bool wifeMarried = false;
    public float wifeRelationship = 0;
    public bool wifeUsedLovePotion;
    public float sonRelationship = 0;
    public bool sonWasBorn = false;
    public float cactusRelationship;

    // Peacock
    private float mPeacockHealth = 75;
    public float peacockHealth
    {
        get { return mPeacockHealth; }
        set { mPeacockHealth = Mathf.Clamp(value, 0, 100); }
    }
    public float peacockHappiness = 25;
    public float peacockComfort = 35;
    public FoodType peacockQuarterlyFoodType = FoodType.FT_BASIC;
    public PeacockActivityType peacockQuarterlyActivity = PeacockActivityType.PA_SING;
    public int peacockQuarterlyTotalCost = FoodType.FT_BASIC.GetPrice();
    private int mPeacockQuarterlyFoodCost = FoodType.FT_BASIC.GetPrice();
    public int peacockQuarterlyFoodCost
    {
        get { return mPeacockQuarterlyFoodCost; }
        set
        {
            peacockQuarterlyTotalCost -= mPeacockQuarterlyFoodCost;
            mPeacockQuarterlyFoodCost = value;
            peacockQuarterlyTotalCost += value;
        }
    }
    public int peacockNumQuarterlyExtras = 0;
    public bool[] peacockQuarterlyExtras = new bool[(int)PeacockExtraType.ET_MAX];
    public float[] peacockFeatherProduction = new float[Peacock.producableResources.Length];
    public bool peacockDied = false;

    // peacock report    
    public string peacockReportFoodDesc = "Fed it good food.";
    public string peacockReportActivityDesc = "Read it a story every night.";
    public string peacockReportExtraDesc = "Gave it nothing extra.";
    public string peacockReportGeneralDesc = "It looks healthy, relaxed, and happy.";
    public List<ResourceAndCount> peacockReportFeatherCounts = new List<ResourceAndCount>();

    public static bool LoadGame()
    {
        string saveFilename = Application.persistentDataPath + "/pp_savegame.dat";
        Debug.Log("save file is " + saveFilename);
        if(File.Exists(saveFilename))
        {
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Open(saveFilename, FileMode.Open, FileAccess.Read);
                singleton = (GameData)bf.Deserialize(file);
                file.Close();
                return true;
            }
            catch(Exception e)
            {
                Debug.Log("exception loading game: " + e);
                if(file != null)
                    file.Close();
            }
        }

        return false;
    }

    public static bool SaveGame()
    {
        string saveFilename = Application.persistentDataPath + "/pp_savegame.dat";
        File.Delete(saveFilename);
        try
        {
            FileStream file = File.Open(saveFilename, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, singleton);
            return true;
        }
        catch(Exception e)
        {
            Debug.Log("couldn't save game");
            Debug.Log(e.ToString());
        }

        return false;
    }
}
