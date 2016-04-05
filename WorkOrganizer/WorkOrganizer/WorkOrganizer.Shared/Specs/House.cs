using System.Runtime.Serialization;

namespace WorkOrganizer.Specs
{
    [DataContractAttribute]
    public class House
    {
        [DataMemberAttribute]
        private int idHouse;
        public int IdHouse
        {
            get
            {
                return idHouse;
            }
            set
            {
                if (idHouse == -1)
                    idHouse = value;
            }
        }

        [DataMemberAttribute]
        public string Name { get; private set; }

        [DataMemberAttribute]
        public int IdOwner { get; private set; }

        [DataMemberAttribute]
        public bool IsInvisible { get; set; }

        public House (string name, int idOwner)
        {
            idHouse = -1;
            Name = name;
            IdOwner = idOwner;
        }
    }
}