using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class Department
    {
        private struct SpNames
        {
            public const string Get = "usp_Department_Get";
            public const string GetByObjectID = "usp_Department_GetByObjectID";
            public const string Insert = "usp_Department_Insert";
            public const string Update = "usp_Department_Update";
            public const string Delete = "usp_Department_Delete";
            public const string Page = "usp_Department_Page";
        }
        
        public const int ObjectTypeID = 29;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        public enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties
        //[ParamAttribute("@DepartmentID", SqlDbType.Int)]
        //[FieldAttribute("DepartmentID")]
        public int ID { get; set; }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("Name")]
        public string Name { get; set; }

        //[ParamAttribute("@ShortName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("ShortName")]
        public string ShortName { get; set; }

        //[ParamAttribute("@ObjectID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("ObjectID")]
        public Guid ObjectID { get; set; }

        //[ParamAttribute("@ParrentDepartmentID", SqlDbType.Int)]
        //[FieldAttribute("ParrentDepartmentID")]
        public int ParrentDepartmentID { get; set; }


        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public Department()
        {

        }

        public Department(string userName)
        {
            UserName = userName;
        }

        public Department(SqlTransaction trans, int departmentId, string userName): this(userName)
        {
            Init(trans, departmentId);
        }

        public Department(SqlTransaction trans, int departmentId)
        {
            Init(trans, departmentId);
        }

        public Department(SqlTransaction trans, Guid objectID, string userName): this(userName)
        {
            Init(trans, objectID);
        }

        public Department(int departmentId, string userName): this(null, departmentId, userName)
        {
            
        }

        public Department(Guid objectID, string userName): this(null, objectID, userName)
        {
            
        }

        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[0].Value = departmentId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@ShortName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = departmentId;
            Name = (string)prms[1].Value;
            ShortName = (string)prms[2].Value;
            ObjectID = (Guid)prms[3].Value;
            ParrentDepartmentID = (int)prms[4].Value;
        }

        private void Init(SqlTransaction trans, Guid objectID)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@ShortName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = objectID;

            prms[4] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByObjectID, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByObjectID, prms);

            ID = (int)prms[0].Value;
            Name = (string)prms[1].Value;
            ShortName = (string)prms[2].Value;
            ObjectID = objectID;
            ParrentDepartmentID = (int)prms[4].Value;
        }
        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            AccessObject accessObject = new AccessObject(trans.Connection.ConnectionString);
            accessObject.Id = Guid.NewGuid();
            accessObject.Name = Name;
            accessObject.ObjectTypeId = ObjectTypeID;
            accessObject.ObjectStateId = StateIDAll;
            ObjectID = accessObject.Insert(trans);
            

            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@ShortName", SqlDbType.NVarChar, 50);
            prms[2].Value = ShortName;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = ObjectID;

            prms[4] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            prms[4].Value = ParrentDepartmentID;

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
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@ShortName", SqlDbType.NVarChar, 50);
            prms[2].Value = ShortName;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = ObjectID;

            prms[4] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            prms[4].Value = ParrentDepartmentID;

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

        public static List<Department> GetList(SqlTransaction trans, string userName)
        {
            /*if (!CanView(userName))
            {
                throw new AccessException(userName, "Get list");
            }*/
            List<Department> dl = new List<Department>();
            DataTable dt;
            if (trans == null)
            {
                dt = SPHelper.ExecuteDataset("usp_Department_List").Tables[0];
            }
            else
            {
                dt = SPHelper.ExecuteDataset(trans, "usp_Department_List").Tables[0];
            }

            foreach (DataRow dr in dt.Rows)
            {
                Department department = new Department(userName);
                department.ID = (int) dr["DepartmentID"];
                department.Name = (string)dr["Name"];
                department.ShortName = (string)dr["ShortName"];
                department.ObjectID = (Guid) dr["ObjectID"];

                dl.Add(department);
            }

            return dl;
        }

        public static List<Department> GetList(string userName)
        {
            return GetList(null, userName);
        }

        public static DataSet Search(SqlTransaction trans, string name, int departmentId)
        {
            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            sps[0].Value = name;

            sps[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[1].IsNullable = true;
            if (departmentId > 0) {
                sps[1].Value = departmentId;
            } else {
                sps[1].Value = DBNull.Value;
            }

            return SPHelper.ExecuteDataset(trans, "usp_Department_Search", sps);
        }

        public static DataSet Search(string name, int departmentId)
        {
            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = Search(trans, name, departmentId);

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

            return ds;
        }

        public static void Delete(SqlTransaction trans, int departmentId, string userName)
        {
            if (!CanDelete(trans, departmentId, userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, departmentId);
        }

        public static void Delete(int departmentId, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, departmentId, userName);

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


        public static Guid GetObjectID(SqlTransaction trans, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[0].Value = departmentId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@ShortName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            return (Guid) prms[3].Value;
        }


        public static DataSet GetPage(SqlTransaction trans, PageSettings pageSettings)
        {
            SqlParameter[] sps = new SqlParameter[7];
            sps[0] = new SqlParameter("@PageIndex", SqlDbType.Int);
            sps[0].Value = pageSettings.PageIndex;

            sps[1] = new SqlParameter("@SortColumnName", SqlDbType.VarChar, 50);
            sps[1].Value = pageSettings.SortColumn;

            sps[2] = new SqlParameter("@SortOrderBy", SqlDbType.VarChar, 4);
            sps[2].Value = pageSettings.SortOrder;

            sps[3] = new SqlParameter("@NumberOfRows", SqlDbType.Int);
            sps[3].Value = pageSettings.PageSize;

            sps[4] = new SqlParameter("@TotalRecords", SqlDbType.Int);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            sps[5].Value = pageSettings.Where.HasRule("Name")
                                ? pageSettings.Where.GetRule("Name").Data
                                : String.Empty;

            sps[6] = new SqlParameter("@ParrentDepartmentID", SqlDbType.Int);
            sps[6].IsNullable = true;
            int departmentId;
            if (pageSettings.Where.HasRule("ParrentDepartmentID") && Int32.TryParse(pageSettings.Where.GetRule("ParrentDepartmentID").Data, out departmentId))
            {
                sps[6].Value = departmentId;
            }
            else
                sps[6].Value = DBNull.Value;

            DataSet ds = SPHelper.ExecuteDataset(trans, SpNames.Page, sps);

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;

            return ds;
        }

        public static DataSet GetPage(PageSettings pageSettings)
        {
            DataSet ds;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = GetPage(trans, pageSettings);

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

            return ds;
        }



        public static bool CanInsert(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.Insert);
        }

        public static bool CanUpdate(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.Update);
        }

        public static bool CanDelete(SqlTransaction trans, int departmentId, string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(trans, userName, GetObjectID(trans, departmentId), (int)ActionType.Delete);
        }

        public static bool CanView(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.View);
        }
        #endregion
    }
}