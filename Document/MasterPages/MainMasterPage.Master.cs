using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using BizObj.Data;
using BizObj.Document;
using PermissionMembership;

namespace Document.MasterPages
{
    public partial class MainMasterPage : UI.DocumentMasterPage
    {
        protected void InsertAppSettings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("var appSettings = {");
            stringBuilder.Append(String.Format("rootUrl: \"{0}\"", RootFullURL));
            stringBuilder.Append("}");

            HtmlGenericControl scriptControl = new HtmlGenericControl("script");
            scriptControl.Attributes["type"] = "text/javascript";
            scriptControl.InnerHtml = stringBuilder.ToString();
            Page.Header.Controls.AddAt(0, scriptControl);
        }

        protected void InsertNavigationSettings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("var navigationSettings = {");
            string menuList = String.Empty;
            string menuTemplateList = String.Empty;

            MembershipUser mu = Membership.GetUser();
            if (mu != null && mu.ProviderUserKey != null)
            {
                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                try
                {
                    connection.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        trans = connection.BeginTransaction();
                                                
                        DataTable dt = AccessUser.RoleAndObjectsByUserList(trans, (Guid)mu.ProviderUserKey, Department.ObjectTypeID).Table;
                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                Guid objectID = (Guid)row["ObjectID"];
                                Department department = new Department(objectID, mu.UserName);
                                menuList += "{";
                                menuList += String.Format(" id:{0}, name: \"{1}\" ", department.ID, department.ShortName);
                                menuList += "},";
                            }
                        }

                        DataTable mt = AccessUser.RoleAndObjectsByUserList(trans, (Guid)mu.ProviderUserKey, MenuTemplate.ObjectTypeID).Table;
                        if (mt != null)
                        {
                            foreach (DataRow row in mt.Rows)
                            {
                                Guid objectID = (Guid)row["ObjectID"];
                                MenuTemplate menuTemp = new MenuTemplate(objectID, mu.UserName);
                                menuTemplateList += "{";
                                menuTemplateList += String.Format(" id:{0}, name: \"{1}\", systemName: \"{2}\"  ", menuTemp.ID, menuTemp.Name, menuTemp.SystemName);
                                menuTemplateList += "},";
                            }
                        }
                        /*
                        List<Department> dl = Department.GetList(mu.UserName);
                        foreach (Department department in dl)
                        {
                            if (Permission.IsUserPermission(trans, (Guid) mu.ProviderUserKey, department.ObjectID, (int)Department.ActionType.View))
                            {
                                menuList += "{";
                                menuList += String.Format(" id:{0}, name: \"{1}\" ", department.ID, department.ShortName);
                                menuList += "},";
                            }
                        }
                        */
                        
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
            char[] charsToTrim = { ',' };
            stringBuilder.Append(String.Format("menuList: [{0}],", menuList.TrimEnd(charsToTrim)));
            stringBuilder.Append(String.Format("templateList: [{0}]", menuTemplateList.TrimEnd(charsToTrim)));
            stringBuilder.Append("}");

            HtmlGenericControl scriptControl = new HtmlGenericControl("script");
            scriptControl.Attributes["type"] = "text/javascript";
            scriptControl.InnerHtml = stringBuilder.ToString();
            Page.Header.Controls.AddAt(1, scriptControl);
        }

        protected string FormatDictionaryList(DataTable dt, string idColumnName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow r = dt.Rows[i];
                sb.Append("{ id: " + r[idColumnName] + ", ");
                sb.Append("name: \"" + r["Name"].ToString().Replace("'", "\\'").Replace("\"", "\\\"") + "\"");
                if (dt.Columns.Contains("DepartmentID")) {
                    sb.Append(", departmentId: " + r["DepartmentID"]);
                }
                sb.Append("}");
                if (i != dt.Rows.Count - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        protected void InsertAppData()
        {
            StringBuilder appDataSb = new StringBuilder();
            appDataSb.Append("window.appData = {");
            appDataSb.Append("dictionaries: {");
            appDataSb.Append("inputDocTypes: [" + FormatDictionaryList(InputDocType.GetAll(), "InputDocTypeID") + "], ");
            appDataSb.Append("inputMethods: [" + FormatDictionaryList(InputMethod.GetAll(), "InputMethodID") + "], ");
            appDataSb.Append("inputSigns: [" + FormatDictionaryList(InputSign.GetAll(), "InputSignID") + "], ");
            appDataSb.Append("inputSubjectTypes: [" + FormatDictionaryList(InputSubjectType.GetAll(), "InputSubjectTypeID") + "], ");
            appDataSb.Append("deliveryTypes: [" + FormatDictionaryList(DeliveryType.GetAll(), "DeliveryTypeID") + "] ,");
            appDataSb.Append("socialStatuses: [" + FormatDictionaryList(SocialStatus.GetAll(), "SocialStatusID") + "], ");
            appDataSb.Append("branchTypes: [" + FormatDictionaryList(BranchType.GetAll(), "BranchTypeID") + "], ");
            appDataSb.Append("socialCategories: [" + FormatDictionaryList(SocialCategory.GetAll(), "SocialCategoryID") + "], ");
            appDataSb.Append("cardStatuses: [" + FormatDictionaryList(CardStatus.GetAll(), "CardStatusID") + "], ");
            appDataSb.Append("labels: [" + FormatDictionaryList(Label.GetList(), "LabelID") + "], ");
            appDataSb.Append("docStatuses: [" + FormatDictionaryList(DocStatus.GetAll(), "DocStatusID") + "]");
            appDataSb.Append("}");
            appDataSb.Append("}");

            HtmlGenericControl scriptControl = new HtmlGenericControl("script");
            scriptControl.Attributes["type"] = "text/javascript";
            scriptControl.InnerHtml = appDataSb.ToString();
            Page.Header.Controls.AddAt(0, scriptControl);
        }
        
        protected void InsertUserData()
        {
            StringBuilder userDataSb = new StringBuilder();
            userDataSb.Append("var userdata = {");
            Department userDepartment = null;
            int userDepartmentId = 0;

            MembershipUser mu = Membership.GetUser();
            if (mu != null && mu.ProviderUserKey != null)
            {
                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                try
                {
                    connection.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        trans = connection.BeginTransaction();
                        /*
                        DataTable dt = AccessUser.RoleAndObjectsByUserList(trans, (Guid)mu.ProviderUserKey, Department.ObjectTypeID).Table;
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    Guid objectID = (Guid) row["ObjectID"];
                                    userDepartment = new Department(objectID, mu.UserName);
                                }
                            }
                            else
                            {
                                Worker worker = new Worker(trans, (Guid)mu.ProviderUserKey);
                                Post post = new Post(trans, worker.PostID, mu.UserName);
                                userDepartment = new Department(trans, post.DepartmentID);
                                
                                userDataSb.Append(String.Format("worker: {0},", new JavaScriptSerializer().Serialize(worker)));
                            }
                        }
                        */
                        Worker worker = new Worker(trans, (Guid)mu.ProviderUserKey);
                        Post post = new Post(trans, worker.PostID, mu.UserName);
                        userDepartment = new Department(trans, post.DepartmentID);

                        userDataSb.Append(String.Format("worker: {0},", new JavaScriptSerializer().Serialize(worker)));
                        
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


            if (userDepartment != null)
            {
                userDataSb.Append(String.Format("departmentId: {0},", userDepartment.ID));
                userDataSb.Append(String.Format(" departmentName: \"{0}\"", userDepartment.Name));
            }

            userDataSb.Append("}");

            HtmlGenericControl scriptControl = new HtmlGenericControl("script");
            scriptControl.Attributes["type"] = "text/javascript";
            scriptControl.InnerHtml = userDataSb.ToString();
            Page.Header.Controls.AddAt(1, scriptControl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsLogged())
            {
                LoginStatusMain.Visible = false;
            }

            InsertAppData();
            InsertAppSettings();
            InsertNavigationSettings();
            InsertUserData();
        }

        public override void SetErrorMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override void SetErrorMessage(List<string> messages)
        {
            throw new NotImplementedException();
        }

        public override void SetMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override void SetMessage(List<string> messages)
        {
            throw new NotImplementedException();
        }

        public override void SetWarningMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override void SetWarningMessage(List<string> messages)
        {
            throw new NotImplementedException();
        }
    }
}