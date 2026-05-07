using BLL;
using Library;
using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace LMS
{
    public partial class LMSLogin : System.Web.UI.Page
    {
        LoginBO objLoginBo = new LoginBO();
        private string[] lblErrorMsg = new string[10];

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    // Disable caching
                    Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                    Response.Cache.SetValidUntilExpires(false);
                    Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();
                    Response.AppendHeader("Pragma", "no-cache");

                    // Session reset
                    Session.Clear();
                    Session.RemoveAll();
                    Session.Abandon();
                    FormsAuthentication.SignOut();
                }
                lblErrorMsg = new string[10];
                // Load error messages
                lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "ERRLOG001"); //Please enter Student id
                lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRLOG002"); // Please enter Password.
                lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRLOG003"); // Invalid Student id or Password
                lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRLOG004"); //Please enter admin id
                lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRLOG005"); // Invalid Admin id or Password
                lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRLOG006"); // Your account has been locked. Please contact the administrator.
                lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "ERRLOG007"); // Unauthorized login attempt.
               
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }


        protected void btnStudentLogin_Click(object sender, EventArgs e)
        {
            SignInUser(
                userType: 1,
                strUserId: txtStudentID.Text.Trim(),
                strPassword:txtStudentPwd.Text.Trim(),
                loginIdEmptyMsg: lblErrorMsg[0],
               
                passwordEmptyMsg: lblErrorMsg[1],
                invalidLoginMsg: lblErrorMsg[2],
                validateLoginIdFormat: CommonFunction.IsAlphaNumeric
            );
        }
        protected void btnAdminLogin_Click(object sender, EventArgs e)
        {
            SignInUser(
                userType:2,
                strUserId: txtAdminID.Text.Trim(),
                strPassword: txtAdminPwd.Text.Trim(),
                loginIdEmptyMsg: lblErrorMsg[3],
               
                passwordEmptyMsg: lblErrorMsg[1],
                invalidLoginMsg: lblErrorMsg[4],
                validateLoginIdFormat: CommonFunction.IsAlphaNumeric
            );
        }

        private void SignInUser(int userType,
            string strUserId, string strPassword,
            string loginIdEmptyMsg, 
            string passwordEmptyMsg, string invalidLoginMsg,
            Func<string, bool> validateLoginIdFormat)
        {
            try
            {
                // 1️⃣ UserID required  
                if (string.IsNullOrWhiteSpace(strUserId))
                {
                    ShowAlert(loginIdEmptyMsg, "error");
                    return;
                }

                // 2️⃣ UserID must be alphanumeric
                if (!validateLoginIdFormat(strUserId))
                {
                    ShowAlert(invalidLoginMsg, "error");
                    return;
                }

                // 3️⃣ Password required
                if (string.IsNullOrWhiteSpace(strPassword))
                {
                    ShowAlert(passwordEmptyMsg, "error");
                    return;
                }

                // 4️⃣ Validate login using stored procedure
                using (DataSet ds = objLoginBo.CheckLogin(userType,strUserId, Security.CryptTripleDES(true, strPassword)))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int count = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                        if (count > 0)
                        {
                            Session["AdminUserName"] = strUserId;
                            if (userType == 1)
                            {
                                int roleType = Convert.ToInt32(ds.Tables[0].Rows[0]["RoleType"]);
                                string redirectPage = ds.Tables[0].Rows[0]["RedirectPage"].ToString(); 
                                Session["AdminUserID"] = ds.Tables[0].Rows[0]["StudentUserID"];
                                Session["MemberID"] = ds.Tables[0].Rows[0]["StudentID"].ToString();
                                Session["AdminUserRoleID"] = roleType;
                                Session["RedirectPage"] = redirectPage;
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                    1, strUserId,
                                    DateTime.Now,
                                    DateTime.Now.AddMinutes(Session.Timeout),
                                    false, "Student",
                                    FormsAuthentication.FormsCookiePath
                                );

                                string encTicket = FormsAuthentication.Encrypt(ticket);
                                Response.Cookies.Add(
                                    new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                                );

                                Response.Redirect(redirectPage, true);
                                return;
                            }

                            // 🔹 ADMIN LOGIN (CHECK RoleType)
                            if (userType == 2)
                            {
                                int roleType = Convert.ToInt32(ds.Tables[0].Rows[0]["RoleType"]);
                                string redirectPage = ds.Tables[0].Rows[0]["RedirectPage"].ToString();
                                Session["AdminUserID"] = ds.Tables[0].Rows[0]["UserID"];
                                Session["MemberID"] = ds.Tables[0].Rows[0]["EmployeeCode"].ToString();
                                Session["AdminUserRoleID"] = roleType;
                                Session["RedirectPage"] = redirectPage;
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                    1, strUserId,
                                    DateTime.Now,
                                    DateTime.Now.AddMinutes(Session.Timeout),
                                    false,"Admin",
                                    FormsAuthentication.FormsCookiePath
                                );

                                string encTicket = FormsAuthentication.Encrypt(ticket);
                                Response.Cookies.Add(
                                    new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                                );

                                // 🔀 Redirect based on RoleType
                               Response.Redirect(redirectPage, true);
                                return;
                            }
                        }
                        else
                        {
                            //ClearFields();
                            ShowAlert(invalidLoginMsg, "error");
                            return;
                        }
                    }
                    else
                    {

                       // ClearFields();
                        ShowAlert(invalidLoginMsg, "error");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        private void ClearFields()
        {
            txtStudentID.Text = "";
            txtStudentPwd.Text = "";
            txtAdminID.Text="";
            txtAdminPwd.Text="";
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objLoginBo != null)
                {
                    objLoginBo.ReleaseResources();
                    objLoginBo = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        private void ShowAlert(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});", true);
        }
    }
}
