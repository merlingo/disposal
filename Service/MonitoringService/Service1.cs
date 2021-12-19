using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace MonitoringService
{
    public partial class Service1 : ServiceBase
    {
        //Timer collectTimer = new Timer(); // name space(using System.Timers;)  
        //Timer sendDataTimer = new Timer(); // name space(using System.Timers;)  

        EventBus eb;
        string configfilepath = "config.xml";
        //ReadConfig rd;
        public Service1()
        {
            eb = new EventBus(configfilepath);
            InitializeComponent();
            //rd = new ReadConfig(configfilepath);
            //foreach (AbsCollector agent in rd.GetAgents())
            //    eb.register(agent);

            //foreach (AbsSensor sensor in rd.GetSensors())
            //{
            //    eb.register(sensor);
            //    //eb.trigger(1, sensor.Id);
            //    //((PyBasedSensor)sensor).sensorStop();
            //}
            //collectTimer.Elapsed += new ElapsedEventHandler(OnElapsedTimeCollect);
            //collectTimer.Interval = 200000; //number in milisecinds   10min
            //sendDataTimer.Elapsed += new ElapsedEventHandler(OnElapsedTimeSendData);
            //sendDataTimer.Interval = 300000; //number in milisecinds   15min
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                Logging.WriteToFile("Service is beginning at " + DateTime.Now);
                eb.start();
                Logging.WriteToFile("Service has been started at " + DateTime.Now);
            }
            catch (Exception e)
            {
                Logging.WriteToFile("Service is started Error " + e.Message);

            }
           

            //foreach (AbsSensor sensor in eb.sensors)
            //{
            //    sensor.sensorStart();
            //}

            //collectTimer.Enabled = true;

            //sendDataTimer.Enabled = true;
        }
        protected override void OnStop()
        {
            try
            {
                Logging.WriteToFile("Service is stopped at " + DateTime.Now);
                eb.stop();
            }
            catch(Exception e)
            {
                Logging.WriteToFile("Service is stopped Error " + DateTime.Now);

            }
            //collectTimer.Enabled = false;
            //sendDataTimer.Enabled = false;
            //foreach (AbsSensor sensor in eb.sensors)
            //{
            //    sensor.sensorStop();
            //    //eb.trigger(1, sensor.Id);
            //    //((PyBasedSensor)sensor).sensorStop();
            //}
        }
        private void OnElapsedTimeCollect(object source, ElapsedEventArgs e)
        {
            Logging.WriteToFile("Service is collecting data at " + DateTime.Now);
            eb.collectDataTrigger();
        }
        private void OnElapsedTimeSendData(object source, ElapsedEventArgs e)
        {

            Logging.WriteToFile("Service is sending data at " + DateTime.Now);
            eb.sendDataTrigger();
        }
        
    }
}
