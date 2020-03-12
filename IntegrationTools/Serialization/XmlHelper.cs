using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using IntegrationTools.DataConversion;
using System.Xml.XPath;

namespace IntegrationTools.Serialization
{
    public class XmlHelper
    {
        StringHelper stringhelper = new StringHelper();
        public void SerializeXmlFile<T>(object o, string filePath)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream())
                {
                    XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
                    xs.Serialize(xtw, o);
                    File.WriteAllText(filePath, stringhelper.UTF8ByteArrayToString(((MemoryStream)xtw.BaseStream).ToArray()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public string DeserializeToXml<T>(object o)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
                xs.Serialize(xtw, o);
                return stringhelper.UTF8ByteArrayToString(((MemoryStream)xtw.BaseStream).ToArray());
            }
        }

        public T Serialize<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(xml))
            {
                return (T)xs.Deserialize(sr);
            }
        }

        public string RemoveXmlNamespace(string xml)
        {
            //look for a better way to do this
            return xml.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", string.Empty).
                Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty).
                Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty);
        }

        public string GetFieldAsString<T>(object o, string fieldName)
        {
            string strValue = null;
            XmlDocument doc = new XmlDocument();
            XPathNavigator nav = doc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                XmlSerializer ser = new XmlSerializer(o.GetType());
                ser.Serialize(writer, o);
            }
            XmlNode node = doc.FirstChild;
            if (node.HasChildNodes)
            {
                XmlNode fieldNode = node[fieldName];
                if (fieldNode != null)
                    strValue = fieldNode.InnerText;
            }
            return strValue;
        }

        public T DeserializeXmlFile<T>(string filePath)
        {
            try
            {
                object obj = null;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    obj = (new XmlSerializer(typeof(T))).Deserialize(sr);
                }
                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
