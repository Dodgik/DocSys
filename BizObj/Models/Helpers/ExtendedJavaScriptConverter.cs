using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace BizObj.Models.Helpers
{
    public class ExtendedJavaScriptConverter<T> : JavaScriptConverter where T : new()
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] { typeof(T) }; }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            T p = new T();

            var props = typeof(T).GetProperties();

            foreach (string key in dictionary.Keys)
            {
                var prop = props.Where(t => t.Name == key).FirstOrDefault();
                if (prop != null)
                {
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        prop.SetValue(p, DateTime.Parse(dictionary[key] as string, DateTimeFormatInfo.CurrentInfo), null);
                    }
                    else
                    {

                        prop.SetValue(p, dictionary[key], null);
                    }
                }
            }

            return p;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            T p = (T)obj;
            IDictionary<string, object> serialized = new Dictionary<string, object>();

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.PropertyType == typeof(DateTime))
                {
                    serialized[pi.Name] = ((DateTime)pi.GetValue(p, null)).ToString(CultureInfo.CurrentUICulture);
                }
                else
                {
                    serialized[pi.Name] = pi.GetValue(p, null);
                }

            }

            return serialized;
        }

        public static JavaScriptSerializer GetSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new ExtendedJavaScriptConverter<T>() });

            return serializer;
        }
    }
}
