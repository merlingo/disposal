using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MonitoringService
{

    enum Durum
    {

        aktif,
        pasif,
        yok
    }
    public class PyBasedSensor : AbsSensor
    {
        //sensor duty is to collect data and create time based event
        // python based sensor will collect data by using python scripts
        public string pydirname;
        public string pyfilename;
        public string venvdirname;
        public string type;
        public string c;
        public int limit = -1, reset = -1;
        Durum sensorDurum = Durum.yok;
        private string eventFileName;
        public string vname;
        public int collectTime, sendTime;
        private bool updated = false;
        public PyBasedSensor(string dn, string fn, string vdn, string type, string eventFileName, string command, int ct, int st) : base()
        {
            triggerMethod = "Time";
            pydirname = dn.Trim();
            pyfilename = fn.Trim();
            venvdirname = vdn.Trim();
            this.c = command;
            this.type = type;
            this.eventFileName = eventFileName;
            collectTime = ct;
            sendTime = st;

            // Durum sensorDurum = Durum.pasif;
            vname = "test_venv_" + pyfilename.Split('.')[0].Trim();

        }
        public string getType()
        {
            return type;
        }
        public void sensorLoad()
        {
            if (!Directory.Exists(venvdirname + "\\" + vname))
            {
                //sensor ilk yüklendiğinde çalışır - burda ise venv yaratma ve requirementların yüklenmesi
                CmdService cmdService = new CmdService("cmd.exe");
                string command = "py -m venv " + venvdirname + "\\" + vname + " && " + venvdirname + "\\" + vname + "\\Scripts\\activate && pip install -r " + pydirname + "\\requirements.txt";
                Logging.WriteToFile("PYBASEDSENSOR.SensorLoad: " + command);
                try
                {
                    string result = cmdService.ExecuteCommand(command);

                }
                catch (Exception e)
                {
                    Logging.WriteToFile("PYBASEDSENSOR.SensorLoad ERROR:   " + e.Message);

                }
                Logging.WriteToFile("PYBASEDSENSOR.SensorLoad: ");
                sensorStop(cmdService);

            }
            else
            {
                Logging.WriteToFile("PYBASEDSENSOR: Sensor önceden yüklenmiştir");
            }
            sensorDurum = Durum.pasif;
            //RunPython.createVenv(venvdirname,"test_venv",pydirname);
            //RunPython.installRequirements(pydirname, "requirements.txt");
        }
        public async void sensorLoadAsync()
        {
            await Task.Run(() =>
            {
                sensorLoad();
            });

            //RunPython.createVenv(venvdirname,"test_venv",pydirname);
            //RunPython.installRequirements(pydirname, "requirements.txt");
        }
        public CmdService sensorStart()
        {
            if (sensorDurum == Durum.pasif)
            {
                Logging.WriteToFile("PYBasedSensor.sensorStart: durum pasif. sensor start çalışacak", type);
                //string vname = "test_venv";
                string command = venvdirname + "\\" + vname + "\\Scripts\\activate ";
                try
                {
                    Logging.WriteToFile("PYBasedSensor.Collect: commandline: " + command, type);
                    CmdService cmdService = new CmdService("cmd.exe");
                    Logging.WriteToFile("PYBASEDSENSOR. cmd service başlatıldı ", type);
                    string result = cmdService.ExecuteCommand(command);
                    Logging.WriteToFile("PYBASEDSENSOR.SensorStart: " + result, type);
                    sensorDurum = Durum.aktif;
                    return cmdService;
                }
                catch (Exception e)
                {
                    Logging.WriteToFile("ERROR: PYBASEDSENSOR.SensorStart -  " + e.Message, type);
                    return null;
                }

            }
            else
            {
                Logging.WriteToFile("Sensör zaten çalışıyor ya da daha yüklenmemiş", type);
                return null;
            }

        }
        public void sensorStop(CmdService cmdService)
        {
            //venv deactivate
            if (sensorDurum == Durum.aktif)
            {
                Logging.WriteToFile("PYBASEDSENSOR.SensorStoping: ");


                try
                {
                    //string vname = "test_venv";
                    string command = venvdirname + "\\" + vname + "\\Scripts\\deactivate ";
                    //CmdService cmdService = new CmdService("cmd.exe");

                    string result = cmdService.ExecuteCommand(command);
                    Logging.WriteToFile("PYBASEDSENSOR.SensorStopped: " + result, type);

                    sensorDurum = Durum.pasif;
                }
                catch (Exception e)
                {
                    Logging.WriteToFile("ERROR: PYBASEDSENSOR.Sensor Stop -  " + e.Message, type);

                }
            }
            else
            {
                Logging.WriteToFile("PyBasedSensor.Sensorstop: Sensor çalışmamaktadır", type);
            }
        }
        public override string collect()
        {
            string result = "";
            Logging.WriteToFile("PYBasedSensor.Collect: sensor venv yüklemesi için sensorStart() çalıştırılıyor", type);
            CmdService cmdService = sensorStart();
            if (sensorDurum == Durum.aktif)
            {
                try
                {
                    Logging.WriteToFile("PYBasedSensor.Collect:", type);
                    string commandline = "py " + pydirname.Trim() + "\\" + pyfilename.Trim() + " -r \"" + eventFileName + "\"" + " -o " + "\"" + c + "\"" + (limit > 0 ? " -l " + limit.ToString() : "");//python kodu çalıştır ve sonucunu eventFileName dosyasına yaz
                    Logging.WriteToFile("commandline: " + commandline, type);
                    Logging.WriteToFile("PYBasedSensor.Collect: commandline: "+commandline, type);

                    //result alıp döndürmek yerine, python kodunun sonucu bir dosyaya yazması, bu dosyanın da yüklenip rawData'ya dönüştürülmesi sağlanabilir. böylece her sensör için bir de xml'den eventDosyaAdi alınır.
                    //CmdService cmdService = new CmdService("cmd.exe");

                    result = cmdService.ExecuteCommand(commandline);
                    updated = true;
                    Logging.WriteToFile(result, type);
                }
                catch (Exception e)
                {
                    Logging.WriteToFile("ERROR: PYBASEDSENSOR.Sensor Stop -  " + e.Message, type);

                }

            }
            else
            {
                result = "sensor calışmadı";
                Logging.WriteToFile("Sensor çalışmamaktadır", type);
            }
            sensorStop(cmdService);
            return result;
        }
        public static async Task collectAsync(PyBasedSensor sensor)
        {


            //result alıp döndürmek yerine, python kodunun sonucu bir dosyaya yazması, bu dosyanın da yüklenip rawData'ya dönüştürülmesi sağlanabilir. böylece her sensör için bir de xml'den eventDosyaAdi alınır.
            await Task.Run(() =>
            {
                while (true)
                {
                    Logging.WriteToFile("PYBasedSensor.CollectAsync:");
                    string commandline = "py " + sensor.pydirname.Trim() + "\\" + sensor.pyfilename.Trim() + " -r \"" + sensor.eventFileName + "\"" + " -o " + "\"" + sensor.c + "\"" + (sensor.limit > 0 ? " -l " + sensor.limit.ToString() : "");//python kodu çalıştır ve sonucunu eventFileName dosyasına yaz
                    Logging.WriteToFile("commandline: " + commandline);
                    CmdService cmdService = new CmdService("cmd.exe");
                    //CmdService.ExecuteCommandS(commandline);
                    sensor.updated = true;
                    Thread.Sleep(sensor.collectTime);
                }


            });
        }
        public Event doEvent(Event e = null)
        {
            if (File.Exists(eventFileName) && (updated))
            {//eğer dosya varsa ve son göndermeden buyana güncelleme olduysa
                string rawData = getEventRawData(eventFileName);
                updated = false;
                if (e == null)
                    e = new Event(rawData, DateTime.Now, "", type, "","/monitoring/event/m");
                else
                {
                    e.rawData = rawData;
                    e.type = type;

                }
                return e;
            }
            else
            {
                Logging.WriteToFile("DoEvent: file doesnt exist:" + eventFileName, type);
                e = null;
                //gönderilecek veri yok
                return null;
            }
        }

        private string getEventRawData(string eventFileName)
        {
            //read file and return
            if (File.Exists(eventFileName))
            {
                // Create a file to write to.
                string createText = File.ReadAllText(eventFileName);
                File.Delete(eventFileName);
                return createText;
            }
            Logging.WriteToFile("PyBasedSensor.getEventRawData - file path is not correct. There is no file as " + eventFileName, type);
            throw new Exception("PyBasedSensor.getEventRawData - file path is not correct. There is no file as " + eventFileName);
        }

        public override void publish(ref Event e)
        {
            e = doEvent(e);
        }

        public override int getCollectTime()
        {
            return collectTime;
        }

        public override int getSendTime()
        {
            return sendTime;
        }
    }
}
