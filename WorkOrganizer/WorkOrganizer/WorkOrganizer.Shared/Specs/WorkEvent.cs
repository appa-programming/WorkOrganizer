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
        public int MoneyUnits { get; set; }
        [DataMemberAttribute]
        public int MoneyCents { get; set; }

        public WorkEvent(DateTime time, int idHouse, string note, int moneyUnits, int moneyCents)
        {
            Id = Guid.NewGuid();
            Time = time;
            IdHouse = idHouse;
            Note = note;
            MoneyUnits = moneyUnits;
            MoneyCents = moneyCents;
        }

        internal void EditTo(WorkEvent ev)
        {
            Time = ev.Time;
            IdHouse = ev.IdHouse;
            Note = ev.Note;
            MoneyUnits = ev.MoneyUnits;
            MoneyCents = ev.MoneyCents;
        }
    }
}