using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace MonitoringService
{
    public abstract class DataModel
    {
        public abstract string toJson();

        public static IList<T> isExistInRepo<T>(string url, string parameters)
        {
            //TODO: bu fonksiyon genel olarak abstract datamodel'in template'li fonksiyonu olmalı
            //send get request to get attr list by using type
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "?"+ parameters);
            httpWebRequest.Method = "GET";
            string response;
            //Logging.WriteToFile("agenttan bildiriliyor. Event gönderiliyor url:" + url+" / Method:"+method);
            //TODO: try catch
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            
            IList<T> datamodels = null;
            if (response != "[]")
                datamodels = JsonSerializer.Deserialize<IList<T>>(response);

            return datamodels;
        }
    }
}