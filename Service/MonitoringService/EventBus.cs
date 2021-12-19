using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MonitoringService
{
    //ObserverPattern uygulaması, kayitOl,DurumDegistir, tetikle. Modüller bu arayüze kayıtOl fonksiyonu ile bağlanır. Modüller implement ettikleri abstract fonksiyon ve durumlarına göre 
    //(2 durum vardır, her durum için 2 fonksiyon: bir Üretken durumu diğeri ise Üye durumu. Üretken event üretirken üye olayları alır. EventBus’a modüller event basar.
    //Bu event basıldığı anda diğer modüllerin durumlarına göre olayAlma fonksiyonları çalışır. 
   
    public class Event
    {
        //EventBus konektörüne verilen veri yapısı. Çıkarılan veriler(webAPI, file, Crawl) event dönüştürülür ve eventbus'a verilir. eventbus da kayıtlı olan Collector'e verir.
        public string rawData { get; set; } //ham veri - çıkarılan verilerin tamamı buraya koyulur
        public DateTime date { get; set; } //verinin çıkarılma zamanı
        public string src_ip { get; set; } //verinin çıkarıldığı kaynağın ip adresi
        public string type { get; set; } //veri kaynağının tipi
        public string src { get; set; } //hostname
        [JsonIgnore]
        public string link { get; set; } // verinin kayıt edilmesi için gereken link

        public Event()
        {
            date = DateTime.Now;

        }
        public Event(string data)
        {
            date = DateTime.Now;
            rawData = data;
        }
        public Event(string data, DateTime date)
        {
            this.rawData = data;
            this.date = date;
        }
        public Event(string data, DateTime date, string ip, string type, string src, string link)
        {
            this.rawData = data;
            this.date = date;
            this.src_ip = ip;
            this.type = type;
            this.src = src;
            this.link = link;
        }
        public string toJson()
        {

            var json = JsonSerializer.Serialize(this);
            Logging.WriteToFile(json);
            return json;
        }
        public static Event fromJson(string json)
        {
            Event e = JsonSerializer.Deserialize<Event>(json);
            return e;
        }
    }


    public class EventBus
    {
        // colector ve sensor listesi vardir. matris ve config verisine gore, collector'un publish ettigi event'i sensorlere notify eder
        public List<AbsCollector> collectors;
        public List<AbsSensor> sensors;
        int id;
        string host;
        string ip;
        public static int runState { get; private set; }
        public EventBus()
        {
            collectors = new List<AbsCollector>();
            sensors = new List<AbsSensor>();
            host = Dns.GetHostName();
            ip = GetLocalIPAddress();
            runState = -1;
        }
        public EventBus(string configFile)
        {
            collectors = new List<AbsCollector>();
            sensors = new List<AbsSensor>();
            this.host = Dns.GetHostName();
            this.ip = GetLocalIPAddress();
            ReadConfig rd = new ReadConfig(configFile);

            //config dosyası okunur ve tüm observerlar listeye eklenir
            foreach (AbsCollector agent in rd.GetAgents())
                register(agent);

            foreach (AbsSensor sensor in rd.GetSensors())
            {
                register(sensor);

            }
            //her sensor icin ayrı thread fonksiyonları oluşturulur. triggerParallel fonksiyonuyla tüm fonksiyonlar pool'a eklenir.
            runState = -1;
        }
        public EventBus(int id)
        {
            this.id = id;
            collectors = new List<AbsCollector>();
            sensors = new List<AbsSensor>();

        }

        //servis başlatma fonksiyonu - bu fonksiyon ile eventbus'ta kayıtlı olan sensorler ve ilk collector paralel olarak tetiklenir. bu tetiklenme ile sensr bazlı tüm sensorler kendi zaman dilimlerinde devamlı
        //çalışmak üzere paralel olarak çalışmaya başlar.
        public void start()
        {
            
            if (runState <= 0) //sensorler hiç başlamamış (-1) ya da durmuş (0) ise başlat
            {
                runState = 1;
                Logging.WriteToFile("EventBus starts: tasks will start. - " + sensors.Count);
                try
                {
                    for (int i = 0; i < sensors.Count; i++)
                    {
                        //Logging.WriteToFile("EventBus starts: tasks will trigger. - " + ((PyBasedSensor)sensors[i]).type);
                        if (sensors[i].triggerMethod == "Time")
                            EventBus.triggerAsync((PyBasedSensor)sensors[i], (Agent)collectors[0]);
                        //Logging.WriteToFile("EventBus starts: tasks end trigger. - " + ((PyBasedSensor)sensors[i]).type);
                    }
                }

                catch (Exception e)
                {
                    Logging.WriteToFile("start error: " + e.Message.ToString());

                }
            }
            else
            {
                Logging.WriteToFile("EventBus starts: tasks cant start. - " + runState.ToString());

            }
        }
        //stop fonksiyonu ile sensorlerin çalışması durdurulur
        public void stop()
        {
            runState = 0;
        }
        public static int isRunning()
        {
            return runState;
        }

        public bool register(Observer col)
        {
            if (col is AbsSensor)
            {
                Logging.WriteToFile("register sensor - " + ((PyBasedSensor)col).getType());
                sensors.Add((AbsSensor)col);
            }
            else if (col is AbsCollector)
            {
                Logging.WriteToFile("agent register - ");
                collectors.Add((AbsCollector)col);
            }
            else
            {
                //logging here
                Logging.WriteToFile("REGISTER fault: it should be AbsSensor or AbsCollector");
                return false;
            }
            return true;
        }
        public static async Task triggerAsync(PyBasedSensor sensor, Agent agent)
        {
            await Task.Run(() =>
            {
                Logging.WriteToFile("Task on thread " + Thread.CurrentThread.ManagedThreadId + " running.", sensor.type);
                if (EventBus.isRunning() > 0)
                {
                    Logging.WriteToFile("running state 0dan büyük olduğu için başladı", sensor.type);

                    while (true)
                    {
                        try
                        {
                            Thread.Sleep(sensor.getCollectTime());
                            Logging.WriteToFile("sensor collect time bitti.(" + sensor.getCollectTime().ToString() + "). şimdi collect fonksiyonu çalışıyor", sensor.type);
                            sensor.collect();
                            Logging.WriteToFile("collect data in task. collect fonksiyonu çalıştı." + sensor.type + " değerleri için veri dosyası oluşturuldu", sensor.type);

                            Thread.Sleep(sensor.getSendTime());
                            Event e = new Event();
                            e.src = Dns.GetHostName();
                            e.src_ip = GetLocalIPAddress();
                            sensor.dofunc(ref e);

                            if (e == null)
                            {
                                Logging.WriteToFile("continue (sending data) sensor - " + sensor.type, sensor.type);
                                return;
                            }
                            //agent'a gondermeden once elaborator'a verilir. elaborator'de filtrelere sokulur. 
                            agent.dofunc(ref e);
                            Logging.WriteToFile("continue (sending data) sensor in task - " + sensor.type, sensor.type);

                            if (isRunning() == 0)
                            {
                                Logging.WriteToFile("stop trigger async - " + sensor.type, sensor.type);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logging.WriteToFile("ERROR in trigger async" + e.Message, sensor.type);
                        }

                    }
                    Logging.WriteToFile("finish task run async - " + sensor.getType(), sensor.type);
                }
                else
                {
                    Logging.WriteToFile("runing state  küçük", sensor.type);

                }
            });
        }

        public void eventBasedTrigger(Event e)
        {
            foreach (AbsCollector collector in collectors)
                collector.dofunc(ref e);
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            Logging.WriteToFile("EventBUS.GetLocalIPAddress: No network adapters with an IPv4 address in the system!");
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        private Observer findById(int id)
        {
            Observer found = null;
            for (int i = 0; i < collectors.Count; i++)
            {
                if (collectors[i].Id == id)
                {
                    found = collectors[i];
                }
            }
            if (found == null)
            {
                for (int i = 0; i < sensors.Count; i++)
                {
                    if (sensors[i].Id == id)
                    {
                        found = sensors[i];
                    }
                }
            }
            return found;
        }
       

        void collectData(AbsSensor sensor)
        {
            sensor.collect();
        }
        void sendData(AbsSensor sensor)
        {
            Event e = new Event();
            e.src = this.host;
            e.src_ip = this.ip;
            sensor.dofunc(ref e);
            if (e == null)
            {
                Logging.WriteToFile("continue (sending data) sensor - " + ((PyBasedSensor)sensor).getType());
                return;
            }
            for (int j = 0; j < collectors.Count; j++)
            {
                collectors[j].dofunc(ref e);

            }
        }

        //sendDataTrigger: monitoring edilen veri dosyasının sunucuya gönderimi, hangi 
        //collectDataTrigger: monitoring işlemi yapan python kodlarını çalıştırma: collect fonksiyonunun calismasi
        public void collectDataTrigger()
        {
            Logging.WriteToFile("Collecting data - sensors:" + sensors.Count);
            for (int i = 0; i < sensors.Count; i++)
            {
                sensors[i].collect();
            }
        }
        public void sendDataTrigger()
        {
            Logging.WriteToFile("sending data - sensors:" + sensors.Count);
            for (int i = 0; i < sensors.Count; i++)
            {
                Logging.WriteToFile("processing sensor - " + ((PyBasedSensor)sensors[i]).getType());

                Event e = new Event();
                e.src = this.host;
                e.src_ip = this.ip;
                sensors[i].dofunc(ref e);
                if (e == null)
                {
                    Logging.WriteToFile("continue (sending data) sensor - " + ((PyBasedSensor)sensors[i]).getType());
                    continue;
                }
                for (int j = 0; j < collectors.Count; j++)
                {
                    collectors[j].dofunc(ref e);

                }
                //Thread.Sleep(500);
            }

        }
        public void trigger(int direction, int gid = -1, int aid = -1)
        {//gönderen id, alan id
            //config bilgisine göre değişir. eger one to many ise id degeri olan sensor(yani event üretici) publish fonksiyonu calisir. onun doldurdugu event tum collector'ler update edilr
            //eğer tetikleme collector tarafindan geldiyse
            Event e = new Event();
            e.src = this.host;
            e.src_ip = this.ip;
            if (direction > 0)//gönderen tetiklenir, o event alanlara verilir
            {
                if (gid >= 0)
                {
                    AbsSensor sensor = (AbsSensor)findById(gid);
                    sensor.dofunc(ref e); // event olusturuldu
                    //Logging.WriteToFile(e.toJson());
                    if (aid >= 0)
                    {
                        AbsCollector collector = (AbsCollector)findById(aid);
                        collector.dofunc(ref e);
                    }
                    else
                    {
                        for (int i = 0; i < collectors.Count; i++)
                        {
                            collectors[i].dofunc(ref e);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < sensors.Count; i++)
                    {
                        e = new Event();
                        sensors[i].dofunc(ref e);
                        if (aid >= 0)
                        {
                            AbsCollector collector = (AbsCollector)findById(aid);
                            collector.dofunc(ref e);
                        }
                        else
                        {
                            for (int j = 0; j < collectors.Count; j++)
                            {
                                collectors[j].dofunc(ref e);
                            }
                        }
                    }
                }
            }
        }
    }
    public abstract class Observer
    {
        protected static int n_of = 0;
        public abstract void dofunc(ref Event e);
        private int id;
        public int Id { get => id; private set => id = value; }

        public Observer()
        {
            Id = Observer.n_of;
            Observer.n_of++;
        }
        
        public int Count()
        {
            return Observer.n_of;
        }
    }
    public abstract class AbsCollector : Observer
    {
        public override void dofunc(ref Event e)
        {
            update(e);
        }
        public abstract void update(Event e);
    }
    public abstract class AbsSensor : Observer
    {
        public string triggerMethod;
        public override void dofunc(ref Event e)
        {
            publish(ref e);
        }
        public abstract void publish(ref Event e);
        //public abstract void sensorStart();
        //public abstract void sensorStop();
        public abstract int getCollectTime();
        public abstract int getSendTime();
        public abstract string collect();
    }


}
