using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Helpers
{
    public static class HelperSerialization
    {
        public static T XMLFileDeserialization<T>(string xmlfilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(xmlfilePath);
            T obj = (T)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }
        public static string XMLSerialization(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
        public static T XMLDeserialization<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);

            }
        }
        public static bool XMLTryDeserialization<T>(string xml, out T result, out string error)
        {
            result = default;
            error = "";            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                try { result = (T)serializer.Deserialize(reader); } catch (Exception e)
                {
                    error += e.Message + Environment.NewLine;
                    if (e.InnerException != null) error += e.InnerException.Message;                  
                    return false;
                }
                if (result==null)
                {
                    error = "Неправильный формат объекта!";                    
                    return false;
                }
                return true;
            }
        }     


        public static string JSONSerializationWithLowerFirst(object obj)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, serializerSettings);             
        }

        public static string JSONSerialization(object obj) { return JsonConvert.SerializeObject(obj); }        
        public static T JSONDeserialization<T>(string json) { return JsonConvert.DeserializeObject<T>(json); }
        public static T JSONReSerialization<T>(object obj) { return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj)); }

        public static bool JSONTryDeserialization<T>(string json, out T obj, out string error)
        {
            obj = default;
            error = "";
            try
            {
                obj = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                obj = default;
                error += e.Message + Environment.NewLine;
                if (e.InnerException != null) error += e.InnerException.Message;
                return false;
            }
            return true;
        }
        public static bool TryJSONDeserialization<T>(string json, out T obj)
        {
            try
            {                
                obj = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                obj = default;
                return false;
            }            
            return true;
        }
        public static bool TryJSONFileDeserialization<T>(string fileName, out T obj)
        {
            obj = default;
            if (!File.Exists(fileName)) return false;
            try
            {
                string text = File.ReadAllText(fileName);
                return (HelperSerialization.TryJSONDeserialization<T>(text, out obj));                 
            }
            catch (Exception e)
            {
                obj = default;
                return false;
            }            
        }
    }
}
