using System.Runtime.Serialization;

namespace WorkOrganizer.Specs
{
    [DataContractAttribute]
    public class Owner
    {
        [DataMemberAttribute]
        private int idOwner;
        public int IdOwner
        {
            get
            {
                return idOwner;
            }
            set
            {
                if (idOwner == -1)
                    idOwner = value;
            }
        }
        [DataMemberAttribute]
        public string Name { get; private set; }
        [DataMemberAttribute]
        public bool IsInvisible { get; set; }

        public Owner(string name)
        {
            idOwner = -1;
            Name = name;
        }
    }
}