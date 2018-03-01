using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace BizObj.Models.Helpers
{
    /// <summary>
    /// Выполняет XML сериализацию и десериализацию
    /// над объектами, которые удовлетворяют требования XML-сериализации.
    /// Предназначен для высокой производительности не выходя з области.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Содержит список XML-сериализаторов
        /// </summary>
        private static readonly Dictionary<Type, XmlSerializer> XmlFormatter;

        static XmlHelper()
        {
            XmlFormatter = new Dictionary<Type, XmlSerializer>();
        }

        /// <summary>
        /// Получает сериализатор для указанного типа.
        /// Если сериализатор не доступен он будет создан.
        /// </summary>
        private static XmlSerializer GetFormatter(Type objType)
        {
            if (!XmlFormatter.ContainsKey(objType))
                XmlFormatter.Add(objType, new XmlSerializer(objType));
            return XmlFormatter[objType];
        }
        
        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        public static string Serialize<T>(T obj) where T : new()
        {
            using (StringWriter sw = new StringWriter())
            {
                GetFormatter(obj.GetType()).Serialize(sw, obj);
                return sw.ToString();
            }
        }
        
        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        
        public static string Serialize<T>(T obj, Encoding encoding) where T : new()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, encoding))
                {
                    GetFormatter(obj.GetType()).Serialize(sw, obj);
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms, encoding))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
        
        /*
        public static string Serialize<T>(T obj, Encoding encoding) where T : new()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Encoding = Encoding.UTF8;
                
                //using (XmlTextWriter sw = new XmlTextWriter(ms, new UTF8Encoding()))
                using (XmlWriter sw = XmlWriter.Create(ms, xws))
                {
                    GetFormatter(obj.GetType()).Serialize(sw, obj);
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms, new UTF8Encoding()))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
        */
        /*
        public static string Serialize<T>(T obj, Encoding encoding) where T : new()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, encoding))
                {
                    GetFormatter(obj.GetType()).Serialize(sw, obj);
                    return Encoding.UTF8.GetString(ms.GetBuffer());
                }
            }
        }
        */
        /*
        /// <summary>
        /// Сериализует объект в строку XML
        /// </summary>
        public static string Serialize<T>(T obj, Encoding encoding) where T : new()
        {
             // Creates an instance of the XmlSerializer class.
        XmlTypeMapping myMapping = (new SoapReflectionImporter().ImportTypeMapping()
        XmlSerializer mySerializer =  
        new XmlSerializer(myMapping);

            StringBuilder sb = new  StringBuilder();

            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = Formatting.None;
                GetFormatter(obj.GetType()).Serialize(writer, obj, null, "utf-8");
                return sb.ToString();
            }
        }
        */
        /*
        public static string Serialize<T>(T obj, Encoding encoding) where T : new()
        {
            TextWriter writer = new StringWriter();
            //writer.Encoding = new UTF8Encoding();
            XmlTextWriter xmlWriter = new XmlTextWriter(writer, new UTF8Encoding());
            GetFormatter(obj.GetType()).Serialize(writer, obj);
            
            return writer.ToString();
        }
        */

        /// <summary>
        /// Десериализует строку XML в объект заданного типа
        /// </summary>
        public static T Deserialize<T>(string objectAsXml) where T : new()
        {
            if (!String.IsNullOrEmpty(objectAsXml))
            {
                using (StringReader sr = new StringReader(objectAsXml))
                {
                    return (T)GetFormatter(typeof(T)).Deserialize(sr);
                }
            }

            return default(T);
        }
        
        /// <summary>
        /// Преобразует Xml строку в Xml документ
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>
        /// Объект <see cref="T:System.Xml.XmlDocument"/>, содержащий Xml документ.
        /// </returns>
        public static XmlDocument ToXmlDocument(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        /// <summary>
        /// Преобразует объект в Xml документ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// Объект <see cref="T:System.Xml.XmlDocument"/>, содержащий Xml документ.
        /// </returns>
        public static XmlDocument ToXmlDocument<T>(T obj) where T : new()
        {
            string xml = Serialize(obj);
            
            return ToXmlDocument(xml);
        }

        /// <summary>
        /// Преобразует объект в Xml документ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns>
        /// Объект <see cref="T:System.Xml.XmlDocument"/>, содержащий Xml документ.
        /// </returns>
        public static XmlDocument ToXmlDocument<T>(T obj, Encoding encoding) where T : new()
        {
            string xml = Serialize(obj, encoding);
            
            return ToXmlDocument(xml);
        }

        /// <summary>
        /// Changes the XML encoding.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument.</param>
        /// <param name="newEncoding">The new encoding.</param>
        /// <returns></returns>
        public static XmlDocument ChangeXmlEncoding(XmlDocument xmlDoc, string newEncoding)
        {
            if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration) xmlDoc.FirstChild;
                xmlDeclaration.Encoding = newEncoding;
            }
            return xmlDoc;
        }
    }
}