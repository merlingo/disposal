using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.IO;

namespace MonitoringService
{
    public class ReadConfig
    {
        //private List<Tuple<String, Object>> configData;
        //public List<Tuple<String, Object>> Tuples { get { return configData; } }
        XElement xDoc;
        public ReadConfig(string configFile)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(currentDirectory, configFile);
            Logging.WriteToFile("Config dosyası:" + path);
            try
            {
                xDoc = XElement.Load(path);

            }
            catch (Exception e)
            {
                Logging.WriteToFile("HATA: " + e.Message);

            }
            Logging.WriteToFile("Okuma tamam:" + path);
        }
        public string getValue(string name)
        {
            return xDoc.Descendants(name).First().Value;
        }
        //eventbus icinde bulunan tüm elemanları, türlerine göre liste olarak döndür.
        public int sensorCount()
        {
            return xDoc.Descendants("Sensor").Count();
        }
        public int agentCount()
        {
            return xDoc.Descendants("Agent").Count();
        }
        public int fieldCount(String field)
        {
            return xDoc.Descendants(field).Count();
        }
        public IEnumerable<AbsSensor> GetSensors()
        {
            // AbsSensor sensor=null;
            foreach (XElement e in xDoc.Descendants("Sensor"))
            {
                Logging.WriteToFile("sensor configden okunuyor: ");
                AbsSensor sensor = new PyBasedSensor(e.Element("DN").Value,
                    e.Element("FN").Value,
                    e.Element("VDN").Value,
                    e.Element("Type").Value,
                    e.Element("EventFileName").Value,
                    e.Element("Command").Value,
                    int.Parse(e.Element("CollectTime").Value),
                    int.Parse(e.Element("SendTime").Value)
                    );
                Logging.WriteToFile("sensor tamam:" + sensor.GetType());

                if (e.Elements("Limit").Any())
                    ((PyBasedSensor)sensor).limit = int.Parse(e.Element("Limit").Value);
                if (e.Elements("Reset").Any())
                    ((PyBasedSensor)sensor).reset = int.Parse(e.Element("Reset").Value);

                //python sensorlere ozel sensorLoad - calistirilacak python kodu icin virtualEnv hazirlanmasi. Bu asama icin python, pip ve internet gereklidir.
                ((PyBasedSensor)sensor).sensorLoad();
                //((PyBasedSensor)sensor).sensorStart();
                yield return sensor;
            }
        }
        public IEnumerable<Elaborator> GetElaborators()
        {
            // AbsSensor sensor=null;
            foreach (XElement e in xDoc.Descendants("Elaborator"))
            {
                Logging.WriteToFile("elaborator configden okunuyor: ");
                Elaborator elab = new Elaborator(getExtractor(e), getFileDetector(e), getElaboratedFileName(e)
                    , GetFilters(e).ToList(), e.Element("DataType").Value
                    );
               
                yield return elab;
            }
        }
        public IEnumerable<AbsCollector> GetAgents()
        {
            foreach (XElement e in xDoc.Descendants("Agent"))
            {
                yield return new Agent( e.Element("IP").Value, int.Parse(e.Element("Port").Value));
            }
        }
        public string getElaboratedFileName(XElement xElement)
        {
           // XElement e = xElement.Descendants("Elaborator").First();

            return xElement.Element("Filename").Value;
        }
        public Extractor getExtractor(XElement xElement)
        {
            XElement e = xElement.Descendants("Extractor").First();
            string type = e.Element("ExtType").Value;
            if (type.Trim().Equals( "Data"))
            {
                if (e.Elements("Seperator").Any())
                    return new FileExtractor(e.Element("Seperator").Value);
                else
                    return new FileExtractor(e.Element("StartMark").Value, e.Element("EndMark").Value);
            }
            else if (type.Trim().Equals("Sensor"))
            {
                
                    SensorExtractor es = new SensorExtractor(e.Element("APIUrl").Value, e.Element("APIFunc").Value, Int32.Parse( e.Element("TimeSpan").Value),  e.Element("DataModel").Value, e.Element("RunType").Value);
                if (e.Elements("UnderField").Any())
                    es.UnderField = e.Element("UnderField").Value;
                return es;
            }
            else
                return null;
        }
        public FileDetector getFileDetector(XElement xElement)
        {
            if (xElement.Elements("Detector").Any())
            {
                XElement e = xElement.Descendants("Detector").First();

                return new FileDetector(e.Element("FileType").Value, e.Element("ObjectName").Value, e.Element("FieldAttr").Value, e.Element("RootNode").Value);

            }
            else
                return null;

        }
        

        internal IEnumerable<Filter> GetFilters(XElement xElement)
        {
            foreach (XElement e in xElement.Descendants("Filter"))
            {
                String dataType = e.Element("DataType").Value;
                Filter f;
                if (dataType == "FileParser")
                {
                    //attr nereden alinacak; pipe ya da db, objeAdi, baglantilar,
                    f = new VulnerabilityParser(e.Element("FileType").Value, e.Element("ObjectName").Value);
                    yield return f;
                }
                if (dataType == "ProductParser")
                {
                    //attr nereden alinacak; pipe ya da db, objeAdi, baglantilar,
                    f = new ProductParser(e.Element("FileType").Value, e.Element("ObjectName").Value, e.Element("Direction").Value, e.Element("GetDetailURL").Value);
                    yield return f;
                }
                else if (dataType == "FileSearcher")
                {
                    f = new FileSearcher();
                    yield return f;
                }
                else if (dataType == "FileRecorder")
                {
                    f = new FileRecorder();
                    yield return f;
                }
                else if (dataType == "Matcher")
                {
                    f = new ProVulMatcher(e.Element("URL").Value, e.Element("ObjectName").Value);
                    yield return f;
                }
                else if (dataType == "Tagger")
                {
                    f = new Tagger(e.Element("ObjectName").Value);
                    yield return f;
                }
                else
                {
                    //TODO: define a NotDefinedFilterException and raise it here.
                    yield return null;
                }
            }
        }
    }

}
