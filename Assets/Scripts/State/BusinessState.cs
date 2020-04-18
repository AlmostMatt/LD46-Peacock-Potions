using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessState
{
    public static float money = 0; // TODO: maybe we make a "currency" or "resources" array, if needed
    public static Peacock peacock = new Peacock();
    // public static bool[] animals = new bool[(int)AnimalType.AT_MAX]; // TODO: this could be ints for health, or maybe structs if need to track more per-animal
    public static int[] resources = new int[(int)ResourceType.RT_MAX];
    public static int[] inventory = new int[(int)ProductType.PT_MAX];
    public static float[] prices = new float[(int)ProductType.PT_MAX];

    public static int peacockFood = 0;

    public class QuarterlyReport
    {
        public int[] production = new int[(int)ProductType.PT_MAX];
        public int[] sales = new int[(int)ProductType.PT_MAX];
        public int[] unfulfilledDemand = new int[(int)ProductType.PT_MAX];
        public int[] miscLosses = new int[(int)ProductType.PT_MAX];

        public int livingExpenses = 0;
    }
    public static QuarterlyReport quarterlyReport;

    public struct PerItemReport
    {
        public ProductType itemType;
        public int previousStock;
        public int currentStock;
        public int numProduced;
        public int numSold;
        public int numLost;
    }

    public static List<PerItemReport> GetPerItemReports()
    {
        List<PerItemReport> reports = new List<PerItemReport>();
        for (int product = 0; product < (int)ProductType.PT_MAX; product++) {
            PerItemReport report = new PerItemReport();
            report.itemType = (ProductType) product;
            report.currentStock = inventory[product];
            if (quarterlyReport != null)
            {
                report.numProduced = quarterlyReport.production[product];
                report.numSold = quarterlyReport.sales[product];
                report.numLost = quarterlyReport.miscLosses[product];
                report.previousStock = report.currentStock - report.numProduced + report.numSold + report.numLost;
                // TODO: only add non-zero reports
            }
            reports.Add(report);
        }
        return reports;
    }
}
