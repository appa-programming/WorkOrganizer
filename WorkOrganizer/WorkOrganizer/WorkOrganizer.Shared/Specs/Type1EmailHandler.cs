using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MoneyLib.MoneyCalculator;

namespace WorkOrganizer.Specs
{
    class Type1EmailHandler : EmailHandler
    {
        protected override List<WorkEvent> TreatList(List<WorkEvent> list)
        {
            return list.OrderBy(we => we.IdHouse).ToList();
        }

        internal override string MakeText(List<WorkEvent> list, Database db, Owner Owner)
        {
            StringBuilder Resp = new StringBuilder();
            Resp.Append("Boa noite " + Owner.Name + ",\nsegue o descritivo deste mês de " + PortugueseUtils.GetMonthName(list[0].Time) + ":\n");

            List<WorkEvent> TreatedList = TreatList(list);
            var Collection = TreatedList.GroupBy(we => we.IdHouse);

            foreach (var group in Collection)
            {
                List<string> Notes = new List<string>();
                
                var House = App.DB.Houses.FirstOrDefault(h => h.IdHouse == group.Key);
                Resp.Append(House.Name + ": ");

                int CountCleaning = 0;
                int CountConstructionCleaning = 0;
                int CountStairsCleaning = 0;
                int CheckInMoneyUnits = 0;
                int CheckInMoneyCents = 0;
                int LaundryMoneyUnits = 0;
                int LaundryMoneyCents = 0;

                foreach (var workEvent in group)
                {
                    if (workEvent.CleaningMoneyUnits > 0 || workEvent.CleaningMoneyCents > 0)
                        CountCleaning++;
                    if (workEvent.ConstructionCleaningMoneyUnits > 0 || workEvent.ConstructionCleaningMoneyCents > 0)
                        CountConstructionCleaning++;
                    if (workEvent.StairsMoneyUnits > 0 || workEvent.StairsMoneyCents > 0)
                        CountStairsCleaning++;
                    CheckInMoneyUnits += workEvent.CheckInMoneyUnits;
                    CheckInMoneyCents += workEvent.CheckInMoneyCents;
                    LaundryMoneyUnits += workEvent.LaundryMoneyUnits;
                    LaundryMoneyCents += workEvent.LaundryMoneyCents;
                    if (workEvent.Note.Trim() != "")
                        Notes.Add("Nota do dia " + workEvent.Time.ToString("dd/MM/yyyy") + " às " +
                            workEvent.Time.ToString("hh:mm") + ": " + workEvent.Note);
                }
                string CheckInMoneyString = GenerateMoneyString(CheckInMoneyUnits, CheckInMoneyCents, MoneyFormat.UU_COMA_CC_MM, "€");
                string LaundryMoneyString = GenerateMoneyString(LaundryMoneyUnits, LaundryMoneyCents, MoneyFormat.UU_COMA_CC_MM, "€");

                if (CountCleaning > 0)
                    Resp.Append("Intervenções: " + CountCleaning + " ");
                if (CountConstructionCleaning > 0)
                    Resp.Append("Intervenção de obra: " + CountConstructionCleaning + " ");
                if (CountStairsCleaning > 0)
                    Resp.Append("Intervenção de escadas: " + CountStairsCleaning + " ");
                if (LaundryMoneyUnits > 0 || LaundryMoneyCents > 0)
                    Resp.Append("Lavandaria: " + LaundryMoneyString + " ");
                if (CheckInMoneyUnits > 0 || CheckInMoneyCents > 0)
                    Resp.Append("Check in: " + CheckInMoneyString + " ");
                Resp.Append("\n");
                foreach (var note in Notes)
                {
                    Resp.Append(note + "\n");
                }
            }
            Resp.Append(GetEnding(list));
        
            return Resp.ToString();
        }
    }
}
