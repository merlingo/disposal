using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace MonitoringService
{
    public class SensorExtractor : Extractor
    {
        string apiUrl, apiFunc, dataModel,runType;
        int timeSpan;
        string lastCVE="";
        private static int i = 0;
        bool isCont = true;
        public string UnderField { get; set; }
        public SensorExtractor(string apiUrl,string apiFunc, int timeSpan, string dataModel,string RunType)
        {
            this.apiFunc = apiFunc;
            this.apiUrl = apiUrl;
            this.timeSpan = timeSpan;
            this.dataModel = dataModel;
            runType = RunType;
            UnderField = "";

        }
        //api url ve api func kullanilarak gonderilen get request, cevap olarak response string'e donusturulur.
        public static string sendAPIRequest(string url,string func)
        {
            string s = url + "/" + func;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(s);
            httpWebRequest.Method = "GET";
            string result;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
        public bool lastCVEcontrol(Chunk c)
        {
            bool isNew = true;
            if(lastCVE == "")
            {
                //c icindeki cve'yi son cve'ye esit mi diye db'den kontrol et
            }
            isNew = c.rawString.Contains(lastCVE);
            return isNew;
        }
        public void stop()
        {
            isCont = false;
        }
        public override IEnumerable<Chunk> extract(Stream dataSource, Metadata metadata)
        {
            //zaman bazlı çalıştırma ile internetten çekilen veriler eğer db'de olmayan bir cve ise işlenip kayıt edilir. yani elaborator sürecine dahil edilmek üzere chunk'a dönüştürülür.
            if (runType == "TekSefer")
                isCont = false;
            while (true)
            {
                string result = "";
                if (dataSource ==null)
                     result = sendAPIRequest(apiUrl, apiFunc);
                else
                    using (StreamReader streamReader = new StreamReader(dataSource))
                    {
                        result = streamReader.ReadToEnd();
                    }
                foreach (string r in JsonHelper.getObjects(result,UnderField))
                {
                    Chunk c = new Chunk();
                    c.rawString = r;
                    c.Metadata = metadata;
                    c.index = i;
                    i++;
                    if (lastCVEcontrol(c))
                    {
                        yield return c;
                    }
                }
               
                Thread.Sleep(1* timeSpan);
                dataSource = null;
                if (!isCont)
                    break;
            }
        }
    }
}
    