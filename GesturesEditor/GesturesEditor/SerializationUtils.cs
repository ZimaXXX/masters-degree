using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.IO;

namespace GesturesEditor
{
    public class SerializationUtils
    {
        /// <summary>
        /// Serialize the ArrayList
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeArrayList(ArrayList obj, Type type)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            Type[] extraTypes = new Type[1];
            extraTypes[0] = type;
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), extraTypes);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            try
            {
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                doc.Load(stream);
                //doc.Save("C:\\ocr.xml");
                return doc.InnerXml;
            }
            catch { throw; }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        /// <summary>
        /// DeSerialize serialized string
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        public static ArrayList DeSerializeArrayList(string serializedData, Type type)
        {
            ArrayList list = new ArrayList();
            Type[] extraTypes = new Type[1];
            extraTypes[0] = type;
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), extraTypes);
            XmlReader xReader = XmlReader.Create(new StringReader(serializedData));
            try
            {
                object obj = serializer.Deserialize(xReader);
                list = (ArrayList)obj;
            }
            catch
            {
                throw;
            }
            finally
            {
                xReader.Close();
            }
            return list;
        }
    }
}
