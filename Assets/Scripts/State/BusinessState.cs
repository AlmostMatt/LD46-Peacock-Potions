﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BusinessState
{
    public static int rent = 0;
    public static float money = 0;
    public static Peacock peacock = new Peacock();    
    public static int[] resources = new int[(int)ResourceType.RT_MAX];
    public static int[] inventory = new int[(int)ProductType.PT_MAX];
    // Set a default price (if not a multiple of 10, modify increments in RenderFunctions)
    public static float[] prices = Enumerable.Repeat(20f, (int)ProductType.PT_MAX).ToArray();
    
    public class QuarterlyReport
    {
        // Inventory as of end-of-quarter
        public int[] unsoldPotions = new int[(int)ProductType.PT_MAX];
        public float[] salePrices = new float[(int)ProductType.PT_MAX];
        public int[] sales = new int[(int)ProductType.PT_MAX];
        public int[] unfulfilledDemand = new int[(int)ProductType.PT_MAX];
        public int[] miscLosses = new int[(int)ProductType.PT_MAX];

        public int livingExpenses = 0;
    }
    public static QuarterlyReport quarterlyReport = new QuarterlyReport();

    public struct PerItemReport
    {
        public ProductType productType;
        public int endOfQStock;
        public int numSold;
        public int numLost;
        public int salePrice; // the price at time of sale (not necessarily the price for the next quarter)
    }

    public static List<PerItemReport> GetPerItemReports()
    {
        List<PerItemReport> reports = new List<PerItemReport>();
        for (int product = 0; product < (int)ProductType.PT_MAX; product++) {
            PerItemReport report = new PerItemReport();
            report.productType = (ProductType) product;
            if (quarterlyReport != null)
            {
                report.endOfQStock = quarterlyReport.unsoldPotions[product];
                report.numSold = quarterlyReport.sales[product];
                report.numLost = quarterlyReport.miscLosses[product];
                report.salePrice = (int)quarterlyReport.salePrices[product];
            }
            else
            {
                // do I need to handle the no-report case?
                report.salePrice = (int)prices[product];
            }
            reports.Add(report);
        }
        return reports;
    }
}
