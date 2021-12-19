using System;


namespace MonitoringService
{
    public class TagDataModel : DataModel
    {
        public String id { get; set; }
        public String founded { get; set; }
        public String cveid { get; set; }
        public String name { get; set; }


        public override string toJson()
        {
            string json = "{\"id\":\"" + id + "\", \"founded\":\"" + founded + "\", \"cveid\":\"" + cveid + "\",  \"name\":\"" + name + "\"}";

            return json;
        }
    }
}
