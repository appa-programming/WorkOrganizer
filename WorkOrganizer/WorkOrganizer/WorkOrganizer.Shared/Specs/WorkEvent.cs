using System;
using System.Runtime.Serialization;

namespace WorkOrganizer.Specs
{
    [DataContractAttribute]
    public class WorkEvent
    {
        [DataMemberAttribute]
        public Guid Id { get; private set; }
        [DataMemberAttribute]
        public DateTime Time { get; set; }
        [DataMemberAttribute]
        public int IdHouse { get; set; }
        [DataMemberAttribute]
        public string Note { get; set; }

        [DataMemberAttribute]
        public int CheckInMoneyUnits { get; set; }
        [DataMemberAttribute]
        public int CheckInMoneyCents { get; set; }
        [DataMemberAttribute]
        public int StairsMoneyUnits { get; set; }
        [DataMemberAttribute]
        public int StairsMoneyCents { get; set; }
        [DataMemberAttribute]
        public int CleaningMoneyUnits { get; set; }
        [DataMemberAttribute]
        public int CleaningMoneyCents { get; set; }
        [DataMemberAttribute]
        public int ConstructionCleaningMoneyUnits { get; set; }
        [DataMemberAttribute]
        public int ConstructionCleaningMoneyCents { get; set; }
        [DataMemberAttribute]
        public int LaundryMoneyUnits { get; set; }
        [DataMemberAttribute]
        public int LaundryMoneyCents { get; set; }

        public WorkEvent(DateTime time, int idHouse, string note,
                            int checkInMU, int checkInMC,
                            int stairsMU, int stairsMC,
                            int cleaningMU, int cleaningMC,
                            int constructionCleaningMU, int constructionCleaningMC,
                            int laundryMU, int laundryMC)
        {
            Id = Guid.NewGuid();
            Time = time;
            IdHouse = idHouse;
            Note = note;
            CheckInMoneyUnits = checkInMU;
            CheckInMoneyCents = checkInMC;
            StairsMoneyUnits = stairsMU;
            StairsMoneyCents = stairsMC;
            CleaningMoneyUnits = cleaningMU;
            CleaningMoneyCents = cleaningMC;
            ConstructionCleaningMoneyUnits = constructionCleaningMU;
            ConstructionCleaningMoneyCents = constructionCleaningMC;
            LaundryMoneyUnits = laundryMU;
            LaundryMoneyCents = laundryMC;
        }

        internal void EditTo(WorkEvent ev)
        {
            Time = ev.Time;
            IdHouse = ev.IdHouse;
            Note = ev.Note;

            CheckInMoneyUnits = ev.CheckInMoneyUnits;
            CheckInMoneyCents = ev.CheckInMoneyCents;
            StairsMoneyUnits = ev.StairsMoneyUnits;
            StairsMoneyCents = ev.StairsMoneyCents;
            CleaningMoneyUnits = ev.CleaningMoneyUnits;
            CleaningMoneyCents = ev.CleaningMoneyCents;
            ConstructionCleaningMoneyUnits = ev.ConstructionCleaningMoneyUnits;
            ConstructionCleaningMoneyCents = ev.ConstructionCleaningMoneyCents;
            LaundryMoneyUnits = ev.LaundryMoneyUnits;
            LaundryMoneyCents = ev.LaundryMoneyCents;
        }

        internal int SumUnits()
        {
            return CheckInMoneyUnits +
                    StairsMoneyUnits +
                    CleaningMoneyUnits +
                    ConstructionCleaningMoneyUnits +
                    LaundryMoneyUnits;
        }

        internal int SumCents()
        {
            return CheckInMoneyCents +
                    StairsMoneyCents +
                    CleaningMoneyCents +
                    ConstructionCleaningMoneyCents +
                    LaundryMoneyCents;
        }
    }
}