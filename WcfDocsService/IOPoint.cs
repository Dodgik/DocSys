using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfDocsService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IOPoint" в коде и файле конфигурации.
    [ServiceContract]
    public interface IOPoint
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        UriTemplate = "/GetTourListJSONP/")]
        cTourList GetTourListJsonp();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
        RequestFormat = WebMessageFormat.Xml,
        ResponseFormat = WebMessageFormat.Xml,
        UriTemplate = "/GetTourListXML/")]
        cTourList GetTourListXml();
    }


    // Используйте контракт данных, как показано в примере ниже, чтобы добавить составные типы к операциям служб.
    [CollectionDataContract]
    public class cTourList : Collection<cTour>
    {
    }

    [DataContract]
    public class cTour
    {
        int _ID = 0;
        string _description = "";

        [DataMember]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [DataMember]
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
