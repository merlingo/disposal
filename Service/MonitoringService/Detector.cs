using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace MonitoringService
{
    public class FileDetector
    {
        string fileType;
        string objectName, isFieldAttr, rootnode;
        public FileDetector(string filetype, string objectName, string isFieldAttr, string rn)
        {
            //eger csv ise basliktan attr list cikarilir. xml ise attrTree cikarilir. json ise attrTree cikarilir. object name ile veritabanina cekilir. Veritabani icin agent kullanilir.
            this.fileType = filetype;
            this.objectName = objectName;
            this.isFieldAttr = isFieldAttr;
            this.rootnode = rn;
        }
        //Parçalı datastream'de her bir objeyi tespit etme ve objeden attrlist'i cikarma


        public Metadata detect(Stream datasource, EventBus eb)
        {
            //Eğer obje db'de yoksa attributes çikarilir kayit edilir. varsa oradan çekilir.
            Metadata metadata = new Metadata();
            metadata.ObjectName = objectName;
            metadata.fileType = fileType;
            metadata.isFieldAttr = isFieldAttr;

            //if it exists in db, then get it from there and dont send post by using eventbus
            var attrs = AttributesDataModel.isExistInRepo("http://localhost:5000/main/attribute/m", objectName);
            if (attrs != null)
            {
                metadata.attributes = attrs;
            }
            else
            {


                if (fileType == "CSV")
                {
                    //csv dosyasinin en ustundeki satirda virgulle attributeslar ayrilmistir. 
                    StreamReader sr = new StreamReader(datasource);
                    string data = sr.ReadLine();//ilk satirda fieldname'ler var.
                    metadata.attributes = new AttributesDataModel(objectName, objectName + "." + fileType, new List<string>(data.Split(',')));

                    sr.Close();

                }
                else if (fileType == "XML")
                {
                    //get only level 1 field names as attributes
                    XmlReader xml = XmlReader.Create(datasource);
                    metadata.attributes = new AttributesDataModel(objectName, objectName + "." + fileType, XmlHelper.getNodes(xml, rootnode));
                    xml.Close();
                }
                else if (fileType == "JSON")
                {
                    StreamReader sr = new StreamReader(datasource);
                    string data = sr.ReadToEnd();
                    metadata.attributes = new AttributesDataModel(objectName, objectName + "." + fileType, JsonHelper.GetFieldNames(data));
                    sr.Close();
                }
                else
                {

                }

                Event e = new Event(metadata.attributes.toJson(), DateTime.Now, EventBus.GetLocalIPAddress(), objectName, "Elaborator", "main/attribute/m");
                eb.eventBasedTrigger(e);
            }
            return metadata;
        }
    }

}
