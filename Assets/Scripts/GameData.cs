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
    public GameStage currentStage = GameStage.GS_MAIN_MENU;
    public int quarter = 0;
    public float quarterTimeElapsed = 0; // seconds elapsed in the current quarter
    public int yearsSkipped = 0;
    public int year
    {
        get { return 452 + (quarter / 4); }
    }
    private int dayOfQuarter
    {
        get
        {
            float elapsedTime = quarterTimeElapsed / BusinessSystem.QuarterTotalTime;
            int totalDays = 90;
            // timeElapsed of 0 should be day 1, timeElapsed of 0.999 should round down to 90
            int dayOfQuarter = 1 + (int)((totalDays) * elapsedTime);
            // handle potential edge case of day N+1 showing on the final frame
            if (dayOfQuarter > totalDays)
            {
                dayOfQuarter = totalDays;
            }
            return dayOfQuarter;
        }
    }
    public string month
    {
        get
        {
            // rounding down - to [0,1,2]
            int monthInt = ((dayOfQuarter-1) / 30);
            monthInt += (3 * (quarter % 4));
            // TODO: make fictional month names?
            // Starting in spring 
            return new string[] {
                // Spring
                "Mar",
                "Apr",
                "May",
                // Summer
                "Jun",
                "Jul",
                "Aug",
                // Fall
                "Sep",
                "Oct",
                "Nov",
                // Winter
                "Dec",
                "Jan",
                "Feb",
            }[monthInt];
        }
    }
    public int dayOfMonth
    {
        get
        {
            // TODO: make it so months have variable number of days
            return 1 + ((dayOfQuarter-1) % 30);
        }
    }
    public int elapsedYears
    {
        get { return yearsSkipped + (quarter / 4); }
    }
    public Season season // TODO: just make Season an enum in its own file
    {
        get { return (Season)(quarter % 4); }
    }
    public bool reachedEndOfLife = false;

    // Progression
    public bool[] feathersUnlocked = new bool[(int)FeatherType.FT_MAX];
    public bool[] potionsUnlocked = new bool[(int)PotionType.PT_MAX];

    // BusinessState
    public int rent = 0;
    public int money = 0;
    public int[] feathersOwned = new int[(int)FeatherType.FT_MAX];
    public int[] potionsOwned = new int[(int)PotionType.PT_MAX];
    public int[] potionPrices = new int[(int)PotionType.PT_MAX];
    public int lastMissedRentQuarter = -1;
    public bool missedRent = false; // set to true if you lost the game due to missing rent
    
    // BusinessState.QuarterlyReport
    public int[] unsoldPotions = new int[(int)PotionType.PT_MAX];
    public int[] quarterlyReportSalePrices = new int[(int)PotionType.PT_MAX];
    public int[] quarterlySales = new int[(int)PotionType.PT_MAX];
    public int[] unfulfilledDemand = new int[(int)PotionType.PT_MAX];
    public int[] miscLosses = new int[(int)PotionType.PT_MAX];
    public int initialBalance = 0;
    public int finalBalance = 0;
    public int livingExpenses = 0;
    public int eventIncome = 0;
    public int eventExpenses = 0;

    // CustomerState
    public int totalPopulation = 1000;
    public float storePopularity = 0;
    public float[] productDemand = new float[(int)PotionType.PT_MAX];
    public float[] optimalPrices = new float[(int)PotionType.PT_MAX];
    // recomputed each quarter by MainGameSystem
    public int[] quarterlyCustomers = new int[(int)PotionType.PT_MAX];
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
    public List<FeatherAndCount> peacockReportFeatherCounts = new List<FeatherAndCount>();

    // EventState
    public List<ScheduledEvent.ScheduledEventSaveData> eventSaveData = new List<ScheduledEvent.ScheduledEventSaveData>();

    // specific events
    public int outOfStockEventCooldown = 0;



    // save game stuff. could be in a diff file if we want
    private static string saveFilename = Application.persistentDataPath + "/pp_savegame.dat";
    private static string backupSaveFilename = Application.persistentDataPath + "/pp_savegame_bk.dat";
    public static bool LoadGame()
    {        
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

                // TODO: check for a backup file and try to use it?
            }
        }

        return false;
    }

    public static bool SaveGame()
    {
        EventState.SaveData(); // TODO: think about if this belongs here or not

        // back up existing save
        try
        {
            File.Delete(backupSaveFilename);
            if(File.Exists(saveFilename))
            {
                File.Move(saveFilename, backupSaveFilename);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("couldn't save game. couldn't backup the current save file.");
            Debug.LogError(e.ToString());

            return false;
        }

        // make a new save
        try
        { 
            FileStream file = File.Open(saveFilename, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, singleton);
            file.Close();
        }
        catch(Exception e)
        {
            Debug.LogError("couldn't write a new save file.");
            Debug.LogError(e.ToString());

            // try to restore the backup
            try
            {
                File.Move(backupSaveFilename, saveFilename);
            }
            catch(Exception restoreException)
            {
                Debug.LogError("failed to restore the backup save file. oh no.");
                Debug.LogError(restoreException.ToString());
            }

            return false;
        }
        
        return false;
    }

    public static void EraseGame()
    {
        try
        {
            File.Delete(saveFilename);
            File.Delete(backupSaveFilename);
        }
        catch(Exception e)
        {
            Debug.LogError("failed to delete save file");
            Debug.LogError(e.ToString());
        }

        singleton = new GameData();
    }
}
