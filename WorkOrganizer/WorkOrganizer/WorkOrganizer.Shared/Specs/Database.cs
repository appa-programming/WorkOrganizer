using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkOrganizer.Specs
{
    public class Database
    {
        public List<House> Houses { get; private set; }
        public List<Owner> Owners { get; private set; }

        public async void Load()
        {
            var Hs = (await IOHandler.ReadJsonAsync<House>("wo_Houses.json"));
            if (Hs != null)
                Houses = Hs.ToList();
            else
                Houses = new List<House>();
            var Os = (await IOHandler.ReadJsonAsync<Owner>("wo_Owners.json"));
            if (Os != null)
                Owners = Os.ToList();
            else
                Owners = new List<Owner>();
        }

        async void SaveHouses()
        {
            await IOHandler.WriteJsonAsync<House>("wo_Houses.json", Houses);
        }

        async void SaveOwners()
        {
            await IOHandler.WriteJsonAsync<Owner>("wo_Houses.json", Owners);
        }

    }
}
