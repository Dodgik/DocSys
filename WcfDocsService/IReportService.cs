using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfDocsService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IReportService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IReportService
    {
        [OperationContract]
        string GetDocTControlledReport(int departmentID, DateTime currentDate, DateTime endDate);
    }
}
