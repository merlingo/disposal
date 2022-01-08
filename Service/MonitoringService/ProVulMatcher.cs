using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    public class ProVulMatcher : Filter
    {
        //VulnerabilityParser'dan sonra gelir. Vulnerability hangi ürüne ait oldugunu bulur.
        string url, objectName;
        public ProVulMatcher(string url, string objectName)
        {
            //https://cve.circl.lu/api/cve/:vid adresine istek gönderilerek response json'dan vulnerable_product çekilmesi ve : ile seperate edilince 0dan başlayınca 3. vendor 4.product 5.version
            this.url = url;
            this.objectName = objectName;
        }
        public override Chunk filter(Chunk datasource, EventBus eb)
        {
            //datasource'dan cveid alınır. istek gonderilir. vulnproductlar ogrenilir. her biri cveid,product,version olarak veritabanina main/match/m ile kayıt edilir.
            VulnerabilityDataModel vdm = (VulnerabilityDataModel)datasource.dm;
            string url = "https://cve.circl.lu/api/";
            string func = "cve/" + vdm.cve;
            string result= SensorExtractor.sendAPIRequest( url,  func);
            if (result == "null")
                return datasource;
            JObject input = JObject.Parse(result);
            JArray references = (JArray)input["vulnerable_product"];

            List<string> productList = references.Select(c => (string)c).ToList();
            foreach (string p in productList)
            {
                string[] ps = p.Split(':');
                string json = "{\"cveid\":\"" + vdm.cve + "\",\"product\":\"" + ps[4] + "\",\"version\":\"" + ps[5] + "\"}";
                List<MatcherDataModel> vulns = (List<MatcherDataModel>)DataModel.isExistInRepo<MatcherDataModel>("http://localhost:5000/main/match/m", "cveid=" + vdm.cve+"&product="+ps[4]+"&version="+ps[5]);
                if (vulns == null)
                {

                    Event e = new Event(json, DateTime.Now, EventBus.GetLocalIPAddress(), objectName, "Elaborator", "main/match/m");
                    eb.eventBasedTrigger(e);
                }

                
            }

            return datasource;
        }
    }
}
