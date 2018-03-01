using System.Data.SqlClient;

namespace BizObj.Models.Document
{
    interface IComponent
    {
        void Init(SqlTransaction trans, int id);
        int Insert(SqlTransaction trans);
        void Update(SqlTransaction trans);
        void Delete(SqlTransaction trans);
    }
}