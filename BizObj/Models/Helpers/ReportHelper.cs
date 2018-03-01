using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace BizObj.Models.Helpers
{
    public static class ReportHelper
    {
        public static string BuildReport(string xml, string templateUrl)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            return BuildReport(xmlDocument, templateUrl);
        }

        public static string BuildReport(XmlDocument xmlDocument, string templateUrl)
        {
            XmlDocument xsltTemplate = new XmlDocument();
            xsltTemplate.Load(templateUrl);

            XslCompiledTransform xsltTransformer = new XslCompiledTransform();
            xsltTransformer.Load(xsltTemplate);

            using (StringWriter sw = new StringWriter())
            {
                xsltTransformer.Transform(xmlDocument, null, sw);
                return sw.ToString();
            }
        }

        public static string BuildReport(string xml, XmlReader reader)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            return BuildReport(xmlDocument, reader);
        }

        public static string BuildReport(XmlDocument xmlDocument, XmlReader reader)
        {
            XslCompiledTransform xsltTransformer = new XslCompiledTransform();
            xsltTransformer.Load(reader);

            using (StringWriter sw = new StringWriter())
            {
                xsltTransformer.Transform(xmlDocument, null, sw);
                return sw.ToString();
            }
        }

        public static void Test(string[] args)
        {
            // ë
            char[] ca = Encoding.Unicode.GetChars(new byte[] {0xeb, 0x00});
            var sw = new StreamWriter(@"c:/helloworld.rtf");
            sw.WriteLine(@"{\rtf
                        {\fonttbl {\f0 Times New Roman;}}
                        \f0\fs60 H" +
                         GetRtfUnicodeEscapedString(new String(ca)) + @"llo, World!
                        }");
            sw.Close();
        }

        public static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        public static string EscapeStringForRtf(string s)
        {
            var sb = new StringBuilder();

            foreach (var c in s)
            {
                if (c == 0x5c || c == 0x7b || c == 0x7d)
                {
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static void Test2()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<event>This is a Test</event>";

            // Encode the XML string in a UTF-8 byte array
            byte[] encodedString = Encoding.UTF8.GetBytes(xml);

            // Put the byte array into a stream and rewind it to the beginning
            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;

            // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(ms);

        }

        public static string UnEscOut(string s)
        {
            string result = String.Empty;
			byte [] bt=Encoding.GetEncoding(1251).GetBytes(s);
            foreach (byte b in bt)
                result = result + (b < 128 ? ((char) b).ToString() : "\\'" + b.ToString("x"));

            return result;
        }
        
    }
}
