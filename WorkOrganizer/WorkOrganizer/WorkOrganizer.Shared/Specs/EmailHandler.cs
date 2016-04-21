using System;
using System.Collections.Generic;
using System.Linq;
using static MoneyLib.MoneyCalculator;

namespace WorkOrganizer.Specs
{
    internal abstract class EmailHandler
    {
        protected abstract List<WorkEvent> TreatList(List<WorkEvent> list);
        internal abstract string MakeText(List<WorkEvent> list, Database db, Owner Owner);

        protected static string GetEnding(List<WorkEvent> list)
        {
            int SumMoneyUnits = list.Sum(we => we.SumUnits());
            int SumMoneyCents = list.Sum(we => we.SumCents());
            string TotalMoney = GenerateMoneyString(SumMoneyUnits, SumMoneyCents, MoneyFormat.UU_COMA_CC_MM, "€");
            return "Total: " + TotalMoney + "\nGrata pela atenção, Vanessa Barroso";
        }
    }
}