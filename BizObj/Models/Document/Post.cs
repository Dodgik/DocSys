using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    public class Post
    {
        private struct StoredProcedures
        {
            public const string GetPost = "usp_Post_Get";
            public const string InsertPost = "usp_Post_Insert";
            public const string UpdatePost = "usp_Post_Update";
            public const string DeletePost = "usp_Post_Delete";
            public const string GetList = "usp_Post_List";
        }
        
        public const int ObjectTypeID = 11;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@PostID", SqlDbType.Int)]
        //[FieldAttribute("PostID")]
        private int _id;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 256)]
        //[FieldAttribute("Name")]
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        //[ParamAttribute("@DepartmentID", SqlDbType.Int)]
        //[FieldAttribute("DepartmentID")]
        private int _departmentID;
        public int DepartmentID
        {
            get
            {
                return _departmentID;
            }
            set
            {
                _departmentID = value;
            }
        }

        //[ParamAttribute("@IsVacant", SqlDbType.Int)]
        //[FieldAttribute("IsVacant")]
        private bool _isVacant;
        public bool IsVacant
        {
            get
            {
                return _isVacant;
            }
            set
            {
                _isVacant = value;
            }
        }

        //[ParamAttribute("@PostTypeID", SqlDbType.Int)]
        //[FieldAttribute("PostTypeID")]
        private int _postTypeID;
        public int PostTypeID
        {
            get
            {
                return _postTypeID;
            }
            set
            {
                _postTypeID = value;
            }
        }

        [ScriptIgnore]
        public string UserName { get; set; }

        #endregion
        
        #region Constructors

        public Post()
        {

        }

        public Post(string userName)
        {
            UserName = userName;
        }

        public Post(int postId, string userName): this(userName)
        {
            Init(null, postId);
        }

        public Post(SqlTransaction trans, int postId, string userName): this(userName)
        {
            Init(trans, postId);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int postId)
        {
            /*if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }*/
            
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[0].Value = postId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@IsVacant", SqlDbType.Bit);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@PostTypeID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;
            
            if (trans == null)
                SPHelper.ExecuteNonQuery(StoredProcedures.GetPost, prms);
            else
                SPHelper.ExecuteNonQuery(trans, StoredProcedures.GetPost, prms);
            
            ID = postId;
            Name = (string)prms[1].Value;
            DepartmentID = (int)prms[2].Value;
            IsVacant = (bool)prms[3].Value;
            PostTypeID = (int)prms[4].Value;
        }

        #endregion


        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Value = DepartmentID;

            prms[3] = new SqlParameter("@IsVacant", SqlDbType.Bit);
            prms[3].Value = IsVacant;

            prms[4] = new SqlParameter("@PostTypeID", SqlDbType.Int);
            prms[4].Value = PostTypeID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(StoredProcedures.InsertPost, prms);
            else
                SPHelper.ExecuteNonQuery(trans, StoredProcedures.InsertPost, prms);

            ID = (int)prms[0].Value;

            return _id;
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

            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, -1);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Value = DepartmentID;

            prms[3] = new SqlParameter("@IsVacant", SqlDbType.Bit);
            prms[3].Value = IsVacant;

            prms[4] = new SqlParameter("@PostTypeID", SqlDbType.Int);
            prms[4].Value = PostTypeID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(StoredProcedures.UpdatePost, prms);
            else
                SPHelper.ExecuteNonQuery(trans, StoredProcedures.UpdatePost, prms);
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


        public static IEnumerable<Post> GetPosts(SqlTransaction trans, Post postFilter)
        {
            Collection<Post> posts = new Collection<Post>();
            using (SqlDataReader r = GetReader(trans, postFilter))
            {
                Post post;
                while (r.Read())
                {
                    post = new Post();
                    post.ID = (int)r["PostID"];
                    post.Name = Convert.ToString(r["Name"]);
                    post.DepartmentID = (int)r["DepartmentID"];
                    post.IsVacant = (bool)r["IsVacant"];
                    post.PostTypeID = (int)r["PostTypeID"];
                    posts.Add(post);
                }
            }
            return posts;
        }
        public static IEnumerable<Post> GetPosts(Post postFilter)
        {
            IEnumerable<Post> posts;
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    posts = GetPosts(trans, postFilter);

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
            return posts;
        }

        public static SqlDataReader GetReader(SqlTransaction trans, Post postFilter)
        {
            SqlParameter[] p = new SqlParameter[2];
            p[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            p[0].Value = postFilter.DepartmentID;

            p[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            p[1].Value = postFilter.Name;

            if (trans == null)
                return SPHelper.ExecuteReader(StoredProcedures.GetList, p);
            return SPHelper.ExecuteReader(trans, StoredProcedures.GetList, p);
        }
        public static SqlDataReader GetReader(Post postFilter)
        {
            return GetReader(null, postFilter);
        }
        #endregion


        #region Static Public Methods

        public static void Delete(SqlTransaction trans, int postId, string userName)
        {
            if (!CanDelete(trans, postId, userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, StoredProcedures.DeletePost, postId);
        }

        public static void Delete(int postId, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, postId, userName);

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

        public static int GetDepartmentID(SqlTransaction trans, int postId)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[0].Value = postId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@IsVacant", SqlDbType.Bit);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@PostTypeID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(StoredProcedures.GetPost, prms);
            else
                SPHelper.ExecuteNonQuery(trans, StoredProcedures.GetPost, prms);

            return (int)prms[2].Value;
        }
        
        public static bool CanInsert(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(SqlTransaction trans, int postId, string userName)
        {
            return Department.CanDelete(trans, GetDepartmentID(trans, postId), userName) && Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
    }
}