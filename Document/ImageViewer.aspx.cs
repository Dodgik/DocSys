using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using BizObj.Data;
using BizObj.Document;
using Custom.UI;
using PermissionMembership;

namespace Document
{
    public partial class ImageViewer : System.Web.UI.Page
    {
        protected void InsertAppSettings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("var appSettings = {");
            stringBuilder.Append(String.Format("rootUrl: \"{0}\"", CurrentTheme.RootFullURL));
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

        protected void Page_Load(object sender, EventArgs e)
        {
            InsertAppSettings();
            InsertNavigationSettings();
        }
    }
}