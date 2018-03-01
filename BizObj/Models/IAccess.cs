namespace BizObj.Models
{
    interface IAccess
    {
        int ObjectTypeID { get; }
        
        string UserName { get; set; }

        bool CanRead(string userName);

        bool CanWrite(string userName);
    }
}