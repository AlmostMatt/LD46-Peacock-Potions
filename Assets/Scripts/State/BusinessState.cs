using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BusinessState
{
    public static void MoneyChangeFromEvent(int eventMoney)
    {
        // TODO: make event income and expenses into [string, amount] pairs so that the event can be named
        if (eventMoney > 0)
        {
            GameData.singleton.eventIncome += eventMoney;
        } else
        {
            // Make expenses a positive number
            GameData.singleton.eventExpenses -= eventMoney;
        }
        GameData.singleton.money += eventMoney;
    }

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

            report.endOfQStock = GameData.singleton.unsoldPotions[product];
            report.numSold = GameData.singleton.quarterlySales[product];
            report.numLost = GameData.singleton.miscLosses[product];
            report.salePrice = (int)GameData.singleton.quarterlyReportSalePrices[product];

            reports.Add(report);
        }
        return reports;
    }
}
