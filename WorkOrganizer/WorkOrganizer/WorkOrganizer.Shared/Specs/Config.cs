using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WorkOrganizer.Specs
{
    [DataContractAttribute]
    public class Config
    {
        [DataMemberAttribute]
        public List<string> CheckInValues { get; private set; }
        [DataMemberAttribute]
        public List<string> Stairs { get; private set; }
        [DataMemberAttribute]
        public List<string> Cleaning { get; private set; }
        [DataMemberAttribute]
        public List<string> ConstructionCleaning { get; private set; }
        [DataMemberAttribute]
        public string Laundry { get; private set; }

        public Config(List<string> ci, List<string> s, List<string> c, List<string> cc, string l)
        {
            CheckInValues = ci;
            Stairs = s;
            Cleaning = c;
            ConstructionCleaning = cc;
            Laundry = l;
        }
    }
}