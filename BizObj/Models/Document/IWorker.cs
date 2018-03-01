namespace BizObj.Models.Document
{
    interface IWorker
    {
        #region Properties

        int ID { get; set; }

        string FirstName { get; set; }

        string MiddleName { get; set; }

        string LastName { get; set; }

        int PostID { get; set; }

        bool IsDismissed { get; set; }

        string PostName { get; set; }

        #endregion
    }
}