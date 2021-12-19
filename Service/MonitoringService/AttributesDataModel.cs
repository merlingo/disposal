using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitoringService
{
    public class AttributesDataModel : DataModel
    {
        public String type { get; set; }
        public String source { get; set; }
        public IList<Attribute> attributes { get; set; }

        public AttributesDataModel()
        {

        }
        public AttributesDataModel(string type,string source,List<String> attrs)
        {
            this.type = type; this.source = source;
            attributes = new List<Attribute>();
            foreach (var a in attrs)
            {
                attributes.Add(new Attribute(a));
            }
        }
        public override string toJson()
        {
            List<Attribute> attList = new List<Attribute>();
            string r = "{\"type\":\"" + type + "\",\"source\":\""+source+"\",\"attributes\":[";

            foreach (var a in attributes)
            {
                r = r + a.ToString() + ",";
            }
            r = r.Remove(r.Length - 1, 1) + "]}";
            return r;
        }
        public static AttributesDataModel isExistInRepo(string url, string type)
        {
            //TODO: bu fonksiyon genel olarak abstract datamodel'in template'li fonksiyonu olmalı
            //send get request to get attr list by using type
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url+"?type="+type);
            httpWebRequest.Method = "GET";
            string response;
            //Logging.WriteToFile("agenttan bildiriliyor. Event gönderiliyor url:" + url+" / Method:"+method);

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            AttributesDataModel attrs = null;
            if (response != "[]")
                 attrs = JsonSerializer.Deserialize<IList<AttributesDataModel>>(response)[0];

            return attrs;
        }

    }
    public class Attribute
    {
        public String value { get; set; }
        public String desc { get; set; }
        public String header { get; set; }
        public String type { get; set; }

        public Attribute()
        {

        }
        public Attribute(string v)
        {
            value = v;
            header = v.Split('.').Last();
            desc = "";
            type = "String";
        }
        public Attribute(string v, string d, string h, string t)
        {
            value = v; desc = d; header = h; type = t;
        }
        public override string ToString()
        {
            return "{\"value\":\"" + value + "\",\"desc\":\"" + desc + "\",\"header\":\"" + header + "\",\"type\":\"" + type + "\"}";
        }

       
    }
}
