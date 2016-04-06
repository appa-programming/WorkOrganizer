using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkOrganizer.Specs
{
    public class Database
    {
        public List<House> Houses { get; private set; }
        public List<Owner> Owners { get; private set; }
        public DateTime WorkEventsDate { get; set; }
        public List<WorkEvent> WorkEvents { get; private set; }
        public bool IsLoaded { get; private set; }

        public Database()
        {
            IsLoaded = false;
        }

        public async Task<bool> Load()
        {
            //Houses = new List<House>();
            //AddHouse(new House("QtaConchas", 1));
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
            //await SaveWorkEvents(Today.Month, Today.Year);
            if (!await LoadWorkEvents(Today.Month, Today.Year))
                WorkEvents = new List<WorkEvent>();
            IsLoaded = true;
            return true;
        }

        #region Houses
        async Task<bool> SaveHouses()
        {
            return await IOHandler.WriteJsonAsync<House>("wo_Houses.json", Houses);
        }
        public async Task<DatabaseMessage> AddHouse(House house)
        {
            if (Houses.Any(h => h.Name == house.Name && !h.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The House '" + house.Name + "' already exists.");
            }
            else
            {
                Houses.Add(house);
                house.IdHouse = Houses.Count;
                Houses = Houses.OrderBy(h => h.IdOwner).ThenBy(h => h.Name).ToList();
                if (await SaveHouses())
                    return new DatabaseMessage();
                else
                    return new DatabaseMessage(DatabaseMessageState.ERROR, "Error at Database, please repeat the request.");
            }
        }
        public async Task<DatabaseMessage> RemoveHouse(int id)
        {
            Houses.FirstOrDefault(h => h.IdOwner == id).IsInvisible = true;
            await SaveHouses();
            return new DatabaseMessage();
        }
        public  async Task<DatabaseMessage> EditHouse(int idHouse, House house)
        {
            if (Houses.Any(h => h.Name == house.Name && !h.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The House '" + house.Name + "' already exists.");
            }
            else
            {
                Houses.RemoveAll(o => o.IdHouse == idHouse);
                Houses.Add(house);
                house.IdHouse = idHouse;
                Houses = Houses.OrderBy(h => h.IdOwner).ThenBy(h => h.Name).ToList();
                if (await SaveHouses())
                    return new DatabaseMessage();
                else
                    return new DatabaseMessage(DatabaseMessageState.ERROR, "Error at Database, please repeat the request.");
            }
        }

        internal List<House> GetOwnersHouses(int idOwner)
        {
            return Houses.FindAll(h => h.IdOwner == idOwner).ToList();
        }
        #endregion
        #region Owners
        async Task<bool> SaveOwners()
        {
            return await IOHandler.WriteJsonAsync<Owner>("wo_Owners.json", Owners);
        }
        public async Task<DatabaseMessage> AddOwner(Owner owner)
        {
            if (Owners.Any(o => o.Name == owner.Name && !o.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The Owner '" + owner.Name + "' already exists.");
            }
            else
            {
                Owners.Add(owner);
                owner.IdOwner = Owners.Count;
                Owners = Owners.OrderBy(o => o.Name).ToList();
                if (await SaveOwners())
                    return new DatabaseMessage();
                else
                    return new DatabaseMessage(DatabaseMessageState.ERROR, "Error at Database, please repeat the request.");
            }
        }
        public async Task<DatabaseMessage> EditOwner(int idOwner, Owner owner)
        {
            if (Owners.Any(o => o.Name == owner.Name && !o.IsInvisible))
            {
                return new DatabaseMessage(DatabaseMessageState.ERROR, "The Owner '" + owner.Name + "' already exists.");
            }
            else
            {
                Owners.RemoveAll(o => o.IdOwner == idOwner);
                Owners.Add(owner);
                owner.IdOwner = idOwner;
                Owners = Owners.OrderBy(o => o.Name).ToList();
                if (await SaveOwners())
                    return new DatabaseMessage();
                else
                    return new DatabaseMessage(DatabaseMessageState.ERROR, "Error at Database, please repeat the request.");
            }
        }

        public async Task<DatabaseMessage> RemoveOwner(int id)
        {
            Owners.FirstOrDefault(o => o.IdOwner == id).IsInvisible = true;
            await SaveOwners();
            return new DatabaseMessage();
        }

        internal Owner GetOwnerOfHouse(House house)
        {
            return Owners.FirstOrDefault(o => o.IdOwner == house.IdOwner);
        }
        #endregion
        #region WorkEvents
        private async Task<bool> LoadWorkEvents(int month, int year)
        {
            try
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
                    return true;
                }
                else
                {
                    WorkEvents = new List<WorkEvent>();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> LoadWorkEvents()
        {
            return (await LoadWorkEvents(WorkEventsDate.Month, WorkEventsDate.Year));
        }

        private async void AddWorkEvent(WorkEvent ev)
        {
            WorkEvents.Add(ev);
            WorkEvents = WorkEvents.OrderBy(w => w.Time).ToList();
            await SaveWorkEvents(WorkEventsDate.Month, WorkEventsDate.Year);
        }

        public async void EditWorkEvent(Guid id, WorkEvent ev)
        {
            var we = WorkEvents.FirstOrDefault(w => w.Id == id);
            bool HasCalendarChanges = !(we.Time.Month == ev.Time.Month && we.Time.Year == ev.Time.Year);
            we.EditTo(ev);
            if (HasCalendarChanges)
            {
                await SaveSpecificWorkEvent(we);
                WorkEvents.Remove(we);
            }
            await SaveWorkEvents(WorkEventsDate.Month, WorkEventsDate.Year);
        }

        public async Task<bool> SaveSpecificWorkEvent(WorkEvent we)
        {
            try
            {
                if (we.Time.Month == WorkEventsDate.Month &&
                    we.Time.Year == WorkEventsDate.Year)
                {
                    AddWorkEvent(we);
                }
                else
                {
                    DateTime OldWorkEventsDate = WorkEventsDate;
                    List<WorkEvent> OldWorkEvents = WorkEvents;
                    await LoadWorkEvents(we.Time.Month, we.Time.Year);
                    WorkEvents.Add(we);
                    WorkEvents = WorkEvents.OrderBy(w => w.Time).ToList();
                    await SaveWorkEvents(we.Time.Month, we.Time.Year);
                    WorkEvents = OldWorkEvents;
                    WorkEventsDate = OldWorkEventsDate;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SaveWorkEvents(int month, int year)
        {
            try
            {
                await IOHandler.WriteJsonAsync<WorkEvent>("wo_WorkEvents_" + year.ToString() + "_" + month.ToString() + ".json", WorkEvents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        
        public List<WorkEvent> GetWorkEventsInTheHousesThisMonth(List<House> ownersHouses)
        {
            List<WorkEvent> Resp = new List<WorkEvent>();
            foreach(var h in ownersHouses)
            {
                Resp.AddRange(GetWorkEventsInTheHouseThisMonth(h));
            }
            return Resp;
        }
        public IEnumerable<WorkEvent> GetWorkEventsInTheHouseThisMonth(House h)
        {
            return WorkEvents.FindAll(we => we.IdHouse == h.IdHouse);
        }
        #endregion
    }
}
