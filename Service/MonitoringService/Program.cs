using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MonitoringService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application as library.
        /// </summary>
        static bool devamMi = true;

        static void Main()
        {
            EventBus eb;
            string configfilepath = "config.xml";
            //Event e = new Event("mert nar", DateTime.Now, "8.8.8.8", "deneme", "dut", "m/n/s");
            //Console.WriteLine(e.toJson());
            eb = new EventBus(configfilepath);
            ReadConfig rc = new ReadConfig(configfilepath);
            List<Elaborator> els = new List<Elaborator>();
            foreach(Elaborator el in rc.GetElaborators())
            {
                el.setEvenBus(eb);
                els.Add(el);
            }
            Logging.WriteToFile("Service is beginning at " + DateTime.Now);

            //buyuk dosya bolme nesnelendirme parse  toplama ve db kaydetme
            foreach(Elaborator e in els)
                e.pipe();
        }
       
    }
}
