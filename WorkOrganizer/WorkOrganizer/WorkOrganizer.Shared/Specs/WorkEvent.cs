using System;

namespace WorkOrganizer.Specs
{
    public class WorkEvent
    {
        public Guid Id { get; private set; }
        public DateTime Time { get; private set; }
        public int IdHouse { get; private set; }
        public string Note { get; private set; }
        public int MoneyUnits { get; private set; }
        public int MoneyCents { get; private set; }

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