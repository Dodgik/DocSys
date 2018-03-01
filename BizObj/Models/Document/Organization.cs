using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Document;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    public class Organization : ComponentData
    {
        [ScriptIgnore]
        public override int ObjectTypeID
        {
            get { return 12; }
        }

        private struct SpNames
        {
            public const string Get = "usp_Organization_Get";
            public const string GetByName = "usp_Organization_GetByName";
            public const string Insert = "usp_Organization_Insert";
            public const string Update = "usp_Organization_Update";
            public const string Delete = "usp_Organization_Delete";
            public const string Search = "usp_Organization_Search";
            public const string Page = "usp_Organization_Page";
            public const string Replace = "usp_Organization_Replace";
        }
        
        #region Properties

        //[ParamAttribute("@OrganizationID", SqlDbType.Int)]
        //[FieldAttribute("OrganizationID")]
        public int ID { get; set; }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 256)]
        //[FieldAttribute("Name")]
        public string Name { get; set; }

        //[ParamAttribute("@OrganizationTypeID", SqlDbType.Int)]
        //[FieldAttribute("OrganizationTypeID")]
        public int OrganizationTypeID { get; set; }

        public int DepartmentID { get; set; }
        public int WorkerID { get; set; }

        [ScriptIgnore]
        public Worker Worker { get; set; }
        #endregion

        #region Constructors

        public Organization()
        {
            
        }

        public Organization(string userName): base(userName)
        {
            
        }

        public Organization(int organizationId, string userName): this(null, organizationId, userName)
        {
            
        }

        public Organization(SqlTransaction trans, int organizationId, string userName): this(userName)
        {
            Init(trans, organizationId);
        }
        #endregion

        #region Private Methods

        public override void Init(SqlTransaction trans, int organizationId)
        {
            base.Init(trans, organizationId);
            
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[0].Value = organizationId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = organizationId;
            Name = (string)prms[1].Value;
            OrganizationTypeID = (int)prms[2].Value;
            DepartmentID = (int)prms[3].Value;
            WorkerID = (int)prms[4].Value;
        }

        #endregion
        
        #region Public Methods

        public override int Insert(SqlTransaction trans)
        {
            base.Insert(trans);

            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            prms[2].Value = OrganizationTypeID;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Value = DepartmentID;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Value = WorkerID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int)prms[0].Value;

            return ID;
        }

        public override void Update(SqlTransaction trans)
        {
            base.Update(trans);

            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            prms[2].Value = OrganizationTypeID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Update, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Update, prms);
        }


        public static DataSet GetPage(SqlTransaction trans, PageSettings pageSettings)
        {
            SqlParameter[] sps = new SqlParameter[8];
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

            sps[6] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            int organizationTypeID;
            if (pageSettings.Where.HasRule("OrganizationTypeID") && Int32.TryParse(pageSettings.Where.GetRule("OrganizationTypeID").Data, out organizationTypeID))
            {
                sps[6].Value = organizationTypeID;
            }
            else
                sps[6].Value = DBNull.Value;

            sps[7] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            int departmentId;
            if (pageSettings.Where.HasRule("DepartmentID") && Int32.TryParse(pageSettings.Where.GetRule("DepartmentID").Data, out departmentId))
            {
                sps[7].Value = departmentId;
            }
            else
                sps[7].Value = DBNull.Value;

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

        public static DataSet Search(SqlTransaction trans, string organizationName, int organizationTypeID, int departmentId)
        {
            SqlParameter[] sps = new SqlParameter[3];
            sps[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            sps[0].Value = organizationName;

            sps[1] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            if (organizationTypeID > 0)
                sps[1].Value = organizationTypeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            if (departmentId > 0)
                sps[2].Value = departmentId;
            else
                sps[2].Value = DBNull.Value;
            
            if (trans == null)
                return SPHelper.ExecuteDataset(SpNames.Search, sps);
            return SPHelper.ExecuteDataset(trans, SpNames.Search, sps);
        }

        public static DataSet Search(string organizationName, int organizationTypeID, int departmentId)
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

                    ds = Search(trans, organizationName, organizationTypeID, departmentId);

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


        public static Organization GetByName(SqlTransaction trans, string name, string userName)
        {
            Organization org = null;

            SqlParameter[] sps = new SqlParameter[3];
            sps[0] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            sps[0].Direction = ParameterDirection.Output;

            sps[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            sps[1].Value = name;

            sps[2] = new SqlParameter("@OrganizationTypeID", SqlDbType.Int);
            sps[2].Direction = ParameterDirection.Output;
            
            SPHelper.ExecuteNonQuery(trans, SpNames.GetByName, sps);
            if (sps[0].Value != DBNull.Value)
            {
                org = new Organization(userName);
                org.ID = (int) sps[0].Value;
                org.Name = (string)sps[1].Value;
                org.OrganizationTypeID = (int)sps[2].Value;
            }

            return org;
        }

        #endregion
        
        #region Static Public Methods

        public override void Delete(SqlTransaction trans)
        {
            base.Update(trans);

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, ID);
        }

        public static void Delete(SqlTransaction trans, int id)
        {
            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
        }

        public static void Delete(int id)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, id);

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

        public static void Replace(SqlTransaction trans, int organizationId, int toOrganizationId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[0].Value = organizationId;

            prms[1] = new SqlParameter("@ToOrganizationID", SqlDbType.Int);
            prms[1].Value = toOrganizationId;


            SPHelper.ExecuteNonQuery(trans, SpNames.Replace, prms);
        }

        public static void Replace(int organizationId, int toOrganizationId)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Replace(trans, organizationId, toOrganizationId);

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
    }
}