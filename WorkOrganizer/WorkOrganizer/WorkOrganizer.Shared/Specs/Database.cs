using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkOrganizer.Specs
{
    public class Database
    {
        public List<House> Houses { get; private set; }
        public List<Owner> Owners { get; private set; }
        public DateTime WorkEventsDate { get; set; }
        public List<WorkEvent> WorkEvents { get; private set; }


        public async void Load()
        {
            var Hs = (await IOHandler.ReadJsonAsync<House>("wo_Houses.json"));
            if (Hs != null)
                Houses = Hs.ToList();
            else
                Houses = new List<House>();
            string debug = (await IOHandler.ReadXMLAsync("wo_Owners.json"));
            var Os = (await IOHandler.ReadJsonAsync<Owner>("wo_Owners.json"));
            if (Os != null)
                Owners = Os.ToList();
            else
                Owners = new List<Owner>();
            DateTime Today = DateTime.Today;
            LoadWorkEvents(Today.Month, Today.Year);
        }

        #region Houses
        async void SaveHouses()
        {
            await IOHandler.WriteJsonAsync<House>("wo_Houses.json", Houses);
        }
        public DatabaseMessage AddHouse(House house)
        {
            if (Houses.Any(h => h.Name == house.Name && !h.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The House '" + house.Name + "' already exists.");
            }
            else
            {
                Houses.Add(house);
                house.IdHouse = Houses.Count;
                SaveHouses();
                return new DatabaseMessage();
            }
        }
        public DatabaseMessage RemoveHouse(int id)
        {
            Houses.FirstOrDefault(h => h.IdOwner == id).IsInvisible = true;
            SaveHouses();
            return new DatabaseMessage();
        }
        #endregion
        #region Owners
        async void SaveOwners()
        {
            await IOHandler.WriteJsonAsync<Owner>("wo_Owners.json", Owners);
            string debug = (await IOHandler.ReadXMLAsync("wo_Owners.json"));
        }
        public DatabaseMessage AddOwner(Owner owner)
        {
            if (Owners.Any(o => o.Name == owner.Name && !o.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The Owner '" + owner.Name + "' already exists.");
            }
            else
            {
                Owners.Add(owner);
                owner.IdOwner = Owners.Count;
                SaveOwners();
                return new DatabaseMessage();
            }
        }
        public DatabaseMessage RemoveOwner(int id)
        {
            Owners.FirstOrDefault(o => o.IdOwner == id).IsInvisible = true;
            SaveOwners();
            return new DatabaseMessage();
        }
        #endregion
        #region WorkEvents
        async void LoadWorkEvents(int month, int year)
        {
            WorkEventsDate = new DateTime(year, month, 1);
            string fileName = "wo_WorkEvents_" + year.ToString() + "_" + month.ToString() + ".json";
            if (await IOHandler.ExistsFile(fileName))
            {
                var WEs = (await IOHandler.ReadJsonAsync<WorkEvent>(fileName));
                if (WEs != null)
                    WorkEvents = WEs.ToList();
                else
                    WorkEvents = new List<WorkEvent>();
            }
            else
            {
                WorkEvents = new List<WorkEvent>();
            }
        }

        public async void AddWorkEvent(WorkEvent ev)
        {
            WorkEvents.Add(ev);
            WorkEvents = WorkEvents.OrderBy(w => w.Time).ToList();
            SaveWorkEvents(WorkEventsDate.Month, WorkEventsDate.Year);
        }

        public async void EditWorkEvent(Guid id, WorkEvent ev)
        {
            var we = WorkEvents.FirstOrDefault(w => w.Id == id);
            bool HasCalendarChanges = !(we.Time.Month == ev.Time.Month && we.Time.Year == ev.Time.Year);
            we.EditTo(ev);
            if (HasCalendarChanges)
            {
                SaveSpecificWorkEvent(we);
                WorkEvents.Remove(we);
            }
            SaveWorkEvents(WorkEventsDate.Month, WorkEventsDate.Year);
        }

        private async void SaveSpecificWorkEvent(WorkEvent we)
        {
            DateTime OldWorkEventsDate = WorkEventsDate;
            List<WorkEvent> OldWorkEvents = WorkEvents;
            LoadWorkEvents(we.Time.Month, we.Time.Year);
            WorkEvents.Add(we);
            WorkEvents = WorkEvents.OrderBy(w => w.Time).ToList();
            SaveWorkEvents(we.Time.Month, we.Time.Year);
            WorkEvents = OldWorkEvents;
            WorkEventsDate = OldWorkEventsDate;
        }

        private async void SaveWorkEvents(int month, int year)
        {
            await IOHandler.WriteJsonAsync<WorkEvent>("wo_WorkEvents_" + year.ToString() + "_" + month.ToString() + ".json", WorkEvents);
        }
        #endregion
    }
}
