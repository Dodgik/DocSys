using System;
using System.IO;
using System.Text;
using System.Data;
using System.Runtime.Serialization.Json;


namespace BizObj
{
    public class JSONHelper
    {
        public static string ToJsonString(DataView dataViewToConvert)
        {
            if (dataViewToConvert == null)
            {
                return string.Empty;
            }

            StringBuilder jsonBuilder = new StringBuilder();

            if (dataViewToConvert.Count > 0)
            {
                jsonBuilder.Append("[");
                foreach (DataRowView drv in dataViewToConvert)
                {
                    jsonBuilder.Append("{");
                    DataTable dt = dataViewToConvert.Table;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        jsonBuilder.Append("\"");
                        jsonBuilder.Append(dt.Columns[j].ColumnName);
                        jsonBuilder.Append("\":\"");
                        if (drv[j] != DBNull.Value)
                        {
                            jsonBuilder.Append(drv[j].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "").Replace("\r", ""));
                        }
                        jsonBuilder.Append("\",");
                    }
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("},");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");
            }
            else
            {
                jsonBuilder.Append("[]");
            }
            return jsonBuilder.ToString();
        }

        public static string ToJsonSerialize(Type type, Object object2Serialize)
        {
            MemoryStream outStream = new MemoryStream();
            DataContractJsonSerializer s = new DataContractJsonSerializer(type);
            s.WriteObject(outStream, object2Serialize);
            outStream.Position = 0;
            StreamReader sr = new StreamReader(outStream);
            return sr.ReadToEnd();
        }
    }
}