using System;
using System.IO;
using System.Collections.Generic;

using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MonitoringService
{
    public class ProductParser : Filter
    {
        //chunk.rawData daki her vendor(ya da product) icin ayrinti degeri almak icin request gonderilir. her vendor-product icin yeni model olusturulur.
        string fileType;
        string objectName, GetDetailURL, Direction;
        public ProductParser(string filetype, string objectName, string Direction, string GetDetailURL)
        {
            //eger csv ise basliktan attr list cikarilir. xml ise attrTree cikarilir. json ise attrTree cikarilir. object name ile veritabanina cekilir. Veritabani icin agent kullanilir.
            this.fileType = filetype;
            this.objectName = objectName;
            this.GetDetailURL = GetDetailURL;
            this.Direction = Direction;
        }
        public override Chunk filter(Chunk chunk, EventBus eb)
        {
            string vendor = chunk.rawString;
            List<ProductParser> products = (List<ProductParser>)DataModel.isExistInRepo<ProductParser>("http://localhost:5000/main/vulnerability/m", "vendor=" + vendor);

            if (products.Count >= 0)
                return chunk;
            if (Direction.Trim().Equals("VendorToProduct"))
            {

                string r = SensorExtractor.sendAPIRequest(GetDetailURL, chunk.rawString);
                if (r.Trim() == "{}")
                    //data gelmedi. Demekki url sıkıntılı bir product veya vendor gelmiyor. bir işlem yapilmadan chunk geri döndürülür. Eğer ekstra bir filter eklenirse diye chunk null olarak dndürülebilr.
                    return null;
                JObject input = JObject.Parse(r);
                JArray references = (JArray)input["product"];

                List<string> productList = references.Select(c => (string)c).ToList();
                foreach (string p in productList)
                {
                    string json = "{\"vendor\":\"" + chunk.rawString + "\", \"name\":\"" + p + "\"}";
                    Event e = new Event(json, DateTime.Now, EventBus.GetLocalIPAddress(), objectName, "Elaborator", "main/product/m");
                    eb.eventBasedTrigger(e);
                }
            }


            return chunk;
        }
    }

    public class VulnerabilityParser : Filter
    {
        string fileType;
        string objectName;
        public VulnerabilityParser(string filetype, string objectName)
        {
            //eger csv ise basliktan attr list cikarilir. xml ise attrTree cikarilir. json ise attrTree cikarilir. object name ile veritabanina cekilir. Veritabani icin agent kullanilir.
            this.fileType = filetype;
            this.objectName = objectName;
        }
        //attrlist ve rawData'dan obje oluşturma
        public override Chunk filter(Chunk chunk, EventBus eb)
        {
            VulnerabilityDataModel vulParsedObject = new VulnerabilityDataModel();
            vulParsedObject.rawData = chunk.rawString;
            vulParsedObject.type = chunk.Metadata.ObjectName;
            vulParsedObject.Source = chunk.Metadata.ObjectName;
            if (fileType == "CSV")
            {

            }
            else if (fileType == "XML")
            {
                //attribute'lari cekilmis objeden vulnerability objesi olusturma ve veritabanina kayit etme
                XElement xDoc = XElement.Parse(chunk.rawString);
                // XElement e = xDoc.Descendants("Vulnerability").First();
                if (xDoc.Elements("CVE").Any())
                    vulParsedObject.cve = xDoc.Element("CVE").Value;
                else if (xDoc.Elements(chunk.Metadata.attributes.attributes.Single(a => a.header == "CVE").value).Any())
                {
                    vulParsedObject.cve = xDoc.Element(chunk.Metadata.attributes.attributes.Single(a => a.header == "CVE").value).Value;
                }

                if (xDoc.Elements("Notes").Any())
                    vulParsedObject.description = xDoc.Element("Notes").Value;
                else if (xDoc.Elements(chunk.Metadata.attributes.attributes.Single(a => a.header == "Description").value).Any())
                {
                    vulParsedObject.description = xDoc.Element(chunk.Metadata.attributes.attributes.Single(a => a.header == "Description").value).Value;
                }

                if (xDoc.Elements("Reference").Any())
                    vulParsedObject.reference = xDoc.Element("Reference").Value;
                else if (xDoc.Elements(chunk.Metadata.attributes.attributes.Single(a => a.header == "Reference").value).Any())
                {
                    vulParsedObject.reference = xDoc.Element(chunk.Metadata.attributes.attributes.Single(a => a.header == "Reference").value).Value;
                }



            }
            else if (fileType == "JSON")
            {
                JObject input = JObject.Parse(chunk.rawString);

                vulParsedObject.cve = (string)input["id"]; //id
                vulParsedObject.description = (string)input["summary"]; //summary
                JArray references = (JArray)input["references"];

                List<string> referencesList = references.Select(c => (string)c).ToList();
                vulParsedObject.reference = String.Join("|", referencesList); //references

            }
            else
            {

            }
            chunk.dm = vulParsedObject;
            List<VulnerabilityDataModel> vulns =(List<VulnerabilityDataModel>) DataModel.isExistInRepo<VulnerabilityDataModel>("http://localhost:5000/main/vulnerability/m", "cve="+vulParsedObject.cve);
            if (vulns==null)//veritabaninda boyle bir vulnerability kayit edildiyse tekrar kayit edilmesin
            {
                Event e = new Event(vulParsedObject.toJson(), DateTime.Now, EventBus.GetLocalIPAddress(), objectName, "Elaborator", "main/vulnerability/m");
                eb.eventBasedTrigger(e);
            }
            else
            {
                //daha onceden islenmis veri - oyuzden tekrar analiz edilmesine gerek yok
                return null;
            }
            
            return chunk;
        }
    }
}
