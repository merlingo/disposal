using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    public class MatcherDataModel : DataModel
    {
        public String cveid { get; set; }
        public String product { get; set; }
        public String version { get; set; }


        public override string toJson()
        {
            string json = "{\"product\":\"" + product + "\", \"version\":\"" + version + "\", \"cveid\":\"" + cveid + "\"}";

            return json;
        }
    }
}
