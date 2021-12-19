using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Xml;


namespace MonitoringService
{
    public class Agent : AbsCollector
//  AbsCollector yapısında bir sınıftır
//  2 görevi vardır: eventleri sunucuya gndermek ve sensörün başlatılması için config dosyasının indirilmesi
//  update fonksiyonu ile Eventleri alıp sunucuya gönderir.gndermek için sendData fonksiyonunu kullanır.Bu fonksiyon POST http yapar.
//  ip ve port bilgileri ile config.xml dosyası okunarak construct edilir.
//  getConfig ile sunucudan config.xml dosyası indirilir ve servis klasörüne kayıt edilir.
    {
        string ip;
        int port;
        string configlink;
        public Agent(string ip,int port) : base()
        {
            this.ip = ip;
            this.port = port;
        }
        public override void update(Event e)
        {
            //Eventleri alıp sunucuya gönderir.gndermek için sendData fonksiyonunu kullanır.Bu fonksiyon POST http yapar.
            string url = "http://"+ ip+":"+port.ToString()+"/"+e.link;
            try
            {
                string data = "";
                if (e.link.Contains("event"))
                    data = e.toJson();
                else
                    data = e.rawData;
                sendData(url, "POST", data);

            }
            catch(Exception ex)
            {
                Logging.WriteToFile("HATA: Agent.update - Veri gönderilirken hata oluştu: " + e.type + "     msg:" + ex.Message);
            }

        }
        

        public string sendData(string url,string method,string data)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            string result;
            //Logging.WriteToFile("agenttan bildiriliyor. Event gönderiliyor url:" + url+" / Method:"+method);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
             
                streamWriter.Write(data);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                 result = streamReader.ReadToEnd();
            }
            return result;
        }

        public string getConfig(string link)
        {
            //sunucudan config.xml dosyasının get http ile indirilmesi
            string url = "http://" + ip + ":" + port.ToString() + link;
            try
            {
                string config = sendData(url, "GET","");
                return config;
            }
            catch (Exception ex)
            {
                Logging.WriteToFile("HATA: Agent.getConfig - Veri gönderilirken hata oluştu: " + link + "     msg:" + ex.Message);
            }
            return "";
        }

        private void saveConfigFile(string config)
        {
            //xml dosyasını kaydet
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(config);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create("configGotten.xml", settings);
            doc.Save(writer);
        }
        public void installSensor()
        {
            //yeni sensor servisi başlatma - python dosyasına ihtiyaç vardır.
            string config = getConfig(configlink);
            if(config!="")
                saveConfigFile(config);
        }
        public void logEvent()
        {
            //verilerin kayıt edilmesi

        }
        public void EventSıkıstır()
        {
            //verinin sıkıştırılarak saklanması
        }
    }
}
