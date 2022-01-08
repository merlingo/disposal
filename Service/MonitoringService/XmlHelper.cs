using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
namespace MonitoringService
{
    class XmlHelper
    {
        public static List<string> getNodes(string xmlFileNAme)
        {
            List<string> nodeName = new List<string>();

            XElement xDoc = XElement.Load(xmlFileNAme);

            List<String> distinctDocs = xDoc.Descendants().GroupBy(x => {
                string atname = x.Name.ToString();
                XElement y = x.Parent;
                while (y!=null)
                {
                    atname = y.Name +"."+ atname;
                    y = y.Parent;
                }
                return atname; })
                .Select(g=>g.Key).ToList();
            Console.WriteLine(distinctDocs.ToString());
            distinctDocs.Insert(0, xDoc.Name.ToString());
            return distinctDocs;
        }
        public static List<string> getNodes(System.Xml.XmlReader xmlReader, string rootnode)
        {
            List<string> nodeName = new List<string>();

            XElement xDoc = XElement.Load(xmlReader);

            List<String> distinctDocs = xDoc.Descendants(rootnode).First().Descendants().GroupBy(x => {
                string atname = x.Name.ToString();
                XElement y = x.Parent;
                while (y != null)
                {
                    atname = y.Name + "." + atname;
                    y = y.Parent;
                }
                return atname;
            })
                .Select(g => g.Key).ToList();
            Console.WriteLine(distinctDocs.ToString());
            //distinctDocs.Insert(0, xDoc.Name.ToString());
            return distinctDocs;
        }
    }

    class JsonHelper
    {
        public static IEnumerable<string> getObjects(string inputs, string underField="")
        {
            bool hasValues = true;
            dynamic input;
            if (underField == "")
            {
                input =(JToken) Newtonsoft.Json.JsonConvert.DeserializeObject(inputs);

            }
            else
            {
                 input = JObject.Parse(inputs)[underField];
            }
            //hasValues = input.HasValues;
            //input = input.First;
            foreach(var x in input)
            {
                yield return x.ToString();
            }
            //while (hasValues)
            //{
            //    yield return input.ToString();
            //    dynamic next = input.Next;
            //    input = next;
            //    hasValues = input!=null;
            //}

          

        }

        public static List<string> GetFieldNames(dynamic input)
        {
            List<string> fieldNames = new List<string>();

            try
            {
                if (input.Trim().Equals("[]"))
                    return null;
                // Deserialize the input json string to an object
                input = Newtonsoft.Json.JsonConvert.DeserializeObject(input);

                // Json Object could either contain an array or an object or just values
                // For the field names, navigate to the root or the first element
                input = input.Root ?? input.First ?? input;

                if (input != null)
                {
                    // Get to the first element in the array
                    bool isArray = true;
                    while (isArray)
                    {
                        input = input.First ?? input;
                        
                        if (input.GetType() == typeof(Newtonsoft.Json.Linq.JObject) ||
                        input.GetType() == typeof(Newtonsoft.Json.Linq.JValue) ||
                        input == null)
                            isArray = false;
                    }

                    // check if the object is of type JObject. 
                    // If yes, read the properties of that JObject
                    if (input.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                    {
                        // Create JObject from object
                        Newtonsoft.Json.Linq.JObject inputJson =
                            Newtonsoft.Json.Linq.JObject.FromObject(input);

                        // Read Properties
                        var properties = inputJson.Properties();

                        // Loop through all the properties of that JObject
                        foreach (var property in properties)
                        {
                            // Check if there are any sub-fields (nested)
                            // i.e. the value of any field is another JObject or another JArray
                            if (property.Value.GetType() == typeof(Newtonsoft.Json.Linq.JObject) ||
                            property.Value.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
                            {
                                // If yes, enter the recursive loop to extract sub-field names
                                var subFields = GetFieldNames(property.Value.ToString());

                                if (subFields != null && subFields.Count() > 0)
                                {
                                    // join sub-field names with field name 
                                    //(e.g. Field1.SubField1, Field1.SubField2, etc.)
                                    fieldNames.AddRange(
                                        subFields
                                        .Select(n =>
                                        string.IsNullOrEmpty(n) ? property.Name :
                                     string.Format("{0}.{1}", property.Name, n)));
                                }
                            }
                            else
                            {
                                // If there are no sub-fields, the property name is the field name
                                fieldNames.Add(property.Name);
                            }
                        }
                    }
                    else
                        if (input.GetType() == typeof(Newtonsoft.Json.Linq.JValue))
                    {
                        // for direct values, there is no field name
                        fieldNames.Add(string.Empty);
                    }
                }
            }
            catch
            {
                throw;
            }

            return fieldNames;
        }
    }
}
