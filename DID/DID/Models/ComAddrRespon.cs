namespace DID.Models
{
    public class ComAddrRespon
    {
        public Dictionary<int, string> province_list
        {
            get; set;
        }
        public Dictionary<int, string> city_list
        {
            get; set;
        }
        public Dictionary<int, string> county_list
        {
            get; set;
        }
    }

    public class Area
    {
        public string code
        {
            get; set;
        }

        public string name
        {
            get; set;
        }
    }
}
