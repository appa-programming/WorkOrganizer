using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrganizer.Specs
{
    class Type3EmailHandler : EmailHandler
    {
        protected override List<WorkEvent> TreatList(List<WorkEvent> list)
        {
            return list.OrderBy(we => we.Time).ToList();
        }

        /*Boa noite Diogo,
        segue o descritivo deste mês:
        1 - 1
        2 - 1
        8 - 2
        18 - 2
        20 - 2
        20 - 1 (obras)
        22 - 1
        24 - 1
        25 - 1
        26 - 2
        27 - 1
        28 - 1
        29 - 1
        30 - 1
        Total : 533€
        Grata pela atenção,  Vanessa Barroso*/
        internal override string MakeText(List<WorkEvent> list, Database db, Owner Owner)
        {
            StringBuilder Resp = new StringBuilder();
            Resp.Append("Boa noite " + Owner.Name + ",\nsegue o descritivo deste mês:\n");

            List<WorkEvent> TreatedList = TreatList(list);
            var Collection = TreatedList.GroupBy(we => we.Time.Day);

            foreach (var group in Collection)
            {
                int CountCleaning = 0;
                bool HasStairCleaning = false;
                bool HasConstructionCleaning = false;
                foreach (var workEvent in group)
                {
                    if (workEvent.CleaningMoneyUnits > 0 || workEvent.CleaningMoneyCents > 0)
                        CountCleaning++;
                    if (workEvent.ConstructionCleaningMoneyUnits > 0 || workEvent.ConstructionCleaningMoneyCents > 0)
                        HasConstructionCleaning = true;
                    if (workEvent.StairsMoneyUnits > 0 || workEvent.StairsMoneyCents > 0)
                        HasStairCleaning = true;
                }
                if (CountCleaning == 0)
                    continue;

                string Extra = "\n";
                if (HasConstructionCleaning && HasStairCleaning)
                    Extra = " (obras, escadas)\n";
                else if (HasConstructionCleaning)
                    Extra = " (obras)\n";
                else if (HasStairCleaning)
                    Extra = " (escadas)\n";
                Resp.Append(group.Key + " - " + CountCleaning.ToString() + Extra);
            }
            Resp.Append(GetEnding(list));

            return Resp.ToString();
        }
    }
}
