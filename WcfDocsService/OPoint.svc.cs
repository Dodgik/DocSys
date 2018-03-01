using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfDocsService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "OPoint" в коде, SVC-файле и файле конфигурации.
    public class OPoint : IOPoint
    {
        public cTourList GetTourListJsonp()
        {
            return CreateTourList();
        }

        public cTourList GetTourListXml()
        {
            return CreateTourList();
        }

        private cTourList CreateTourList()
        {
            cTourList oTourList = new cTourList();
            oTourList.Add(new cTour() { ID = 1, description = "Barcelona" });
            oTourList.Add(new cTour() { ID = 2, description = "Paris" });
            oTourList.Add(new cTour() { ID = 3, description = "Rome" });
            oTourList.Add(new cTour() { ID = 4, description = "London" });
            oTourList.Add(new cTour() { ID = 5, description = "Moscow" });

            return oTourList;
        }
    }
}
