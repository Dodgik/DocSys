using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class Citizen
    {
        private struct SpNames
        {
            public const string Get = "usp_Citizen_Get";
            public const string Insert = "usp_Citizen_Insert";
            public const string Update = "usp_Citizen_Update";
            public const string Delete = "usp_Citizen_Delete";
            public const string List = "usp_Citizen_List";
        }

        public const int ObjectTypeID = 27;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@CitizenID", SqlDbType.Int)]
        //[FieldAttribute("CitizenID")]
        public int ID { get; set; }

        //[ParamAttribute("@FirstName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("FirstName")]
        public string FirstName { get; set; }

        //[ParamAttribute("@MiddleName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("MiddleName")]
        public string MiddleName { get; set; }

        //[ParamAttribute("@LastName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("LastName")]
        public string LastName { get; set; }

        //[ParamAttribute("@Address", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("Address")]
        public string Address { get; set; }

        //[ParamAttribute("@PhoneNumber", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("PhoneNumber")]
        public string PhoneNumber { get; set; }

        //[ParamAttribute("@CityObjectID", SqlDbType.Int)]
        //[FieldAttribute("CityObjectID")]
        public int CityObjectID { get; set; }

        //[ParamAttribute("@HouseNumber", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("HouseNumber")]
        public string HouseNumber { get; set; }

        //[ParamAttribute("@Corps", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("Corps")]
        public string Corps { get; set; }

        //[ParamAttribute("@ApartmentNumber", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("ApartmentNumber")]
        public string ApartmentNumber { get; set; }

        //[ParamAttribute("@Work", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("Work")]
        public string Work { get; set; }

        //[ParamAttribute("@Sex", SqlDbType.Int)]
        //[FieldAttribute("Sex")]
        public int Sex { get; set; }

        //[ParamAttribute("@SocialStatusID", SqlDbType.Int)]
        //[FieldAttribute("SocialStatusID")]
        public int SocialStatusID { get; set; }

        
        public int[] SocialCategories { get; set; }

        public string CityObjectName { get; set; }

        public string CityObjectTypeShortName { get; set; }

        
        [ScriptIgnore]
        public string UserName { get; set; }
        #endregion
        
        #region Constructors

        public Citizen()
        {
            
        }

        public Citizen(string userName)
        {
            UserName = userName;
        }

        public Citizen(int id, string userName): this(null, id, userName)
        {
            
        }

        public Citizen(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            SocialCategories = SocialCategoryList.GetSocialCategoryIDList(trans, ID);
            if (CityObjectID > 0)
            {
                CityObject cityObject = new CityObject(trans, CityObjectID, userName);
                CityObjectName = cityObject.Name;
                CityObjectType cityObjectType = new CityObjectType(trans, cityObject.TypeID, userName);
                CityObjectTypeShortName = cityObjectType.ShortName;
            }
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int citizenId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }

            SqlParameter[] prms = new SqlParameter[13];
            prms[0] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[0].Value = citizenId;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 100);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@HouseNumber", SqlDbType.NVarChar, 50);
            prms[7].Direction = ParameterDirection.Output;

            prms[8] = new SqlParameter("@Corps", SqlDbType.NVarChar, 50);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@ApartmentNumber", SqlDbType.NVarChar, 50);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@Work", SqlDbType.NVarChar, 100);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@Sex", SqlDbType.Int);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@SocialStatusID", SqlDbType.Int);
            prms[12].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = citizenId;
            FirstName = (string)prms[1].Value;
            MiddleName = (string)prms[2].Value;
            LastName = (string)prms[3].Value;
            Address = (string)prms[4].Value;
            PhoneNumber = (string)prms[5].Value;
            CityObjectID = (int)prms[6].Value;
            HouseNumber = (string)prms[7].Value;
            Corps = (string)prms[8].Value;
            ApartmentNumber = (string)prms[9].Value;
            Work = (string)prms[10].Value;
            Sex = (int)prms[11].Value;
            SocialStatusID = (int)prms[12].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[13];
            prms[0] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Value = FirstName;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Value = MiddleName;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Value = LastName;

            prms[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 100);
            prms[4].Value = Address;

            prms[5] = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50);
            prms[5].Value = PhoneNumber;

            prms[6] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[6].Value = CityObjectID;

            prms[7] = new SqlParameter("@HouseNumber", SqlDbType.NVarChar, 50);
            prms[7].Value = HouseNumber;

            prms[8] = new SqlParameter("@Corps", SqlDbType.NVarChar, 50);
            prms[8].Value = Corps;

            prms[9] = new SqlParameter("@ApartmentNumber", SqlDbType.NVarChar, 50);
            prms[9].Value = ApartmentNumber;

            prms[10] = new SqlParameter("@Work", SqlDbType.NVarChar, 100);
            prms[10].Value = Work;

            prms[11] = new SqlParameter("@Sex", SqlDbType.Int);
            prms[11].Value = Sex;

            prms[12] = new SqlParameter("@SocialStatusID", SqlDbType.Int);
            prms[12].Value = SocialStatusID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int)prms[0].Value;

            return ID;
        }

        public int Insert()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Insert(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            return ID;
        }

        public void Update(SqlTransaction trans)
        {
            if (!CanUpdate(UserName))
            {
                throw new AccessException(UserName, "Update");
            }

            SqlParameter[] prms = new SqlParameter[13];
            prms[0] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Value = FirstName;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Value = MiddleName;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Value = LastName;

            prms[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 100);
            prms[4].Value = Address;

            prms[5] = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50);
            prms[5].Value = PhoneNumber;

            prms[6] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[6].Value = CityObjectID;

            prms[7] = new SqlParameter("@HouseNumber", SqlDbType.NVarChar, 50);
            prms[7].Value = HouseNumber;

            prms[8] = new SqlParameter("@Corps", SqlDbType.NVarChar, 50);
            prms[8].Value = Corps;

            prms[9] = new SqlParameter("@ApartmentNumber", SqlDbType.NVarChar, 50);
            prms[9].Value = ApartmentNumber;

            prms[10] = new SqlParameter("@Work", SqlDbType.NVarChar, 100);
            prms[10].Value = Work;

            prms[11] = new SqlParameter("@Sex", SqlDbType.Int);
            prms[11].Value = Sex;

            prms[12] = new SqlParameter("@SocialStatusID", SqlDbType.Int);
            prms[12].Value = SocialStatusID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Update, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Update, prms);
        }

        public void Update()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Update(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion
        
        #region Static Public Methods

        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset(SpNames.List).Tables[0];
        }

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            if (!CanDelete(userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
        }

        public static void Delete(int id, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, id, userName);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }
            
        public static bool CanInsert(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
    }
}