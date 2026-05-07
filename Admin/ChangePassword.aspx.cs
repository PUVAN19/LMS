using BLL;
using Library;
using System;
using System.Data;
using System.Web.UI;

namespace Admin
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        LoginBO objBO = new LoginBO();

        protected void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtOldPassword.Text))
                {
                    ShowToastr("Enter current password", "error");
                    return;
                }

                if (string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    ShowToastr("Enter new password", "error");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPassword.Text,
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$"))
                {
                    ShowToastr("Password must contain at least 1 uppercase, 1 lowercase, 1 number, 1 special character and minimum 6 characters", "error");
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowToastr("Passwords do not match", "warning");
                    return;
                }

                int userId = Convert.ToInt32(Session["AdminUserID"]);

                string oldEnc = Security.CryptTripleDES(true, txtOldPassword.Text);
                string newEnc = Security.CryptTripleDES(true, txtNewPassword.Text);

                using (DataSet ds = objBO.ChangePassword(userId, oldEnc, newEnc))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int status = Convert.ToInt32(ds.Tables[0].Rows[0]["Status"]);
                        string message = ds.Tables[0].Rows[0]["Message"].ToString();

                        if (status == 1)
                        {
                            ShowToastr(message, "success");
                            txtOldPassword.Text = "";
                            txtNewPassword.Text = "";
                            txtConfirmPassword.Text = "";
                        }
                        else
                        {
                            ShowToastr(message, "error");
                        }
                    }
                    else
                    {
                        ShowToastr("Invalid response from server", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowToastr("Error updating password", "error");
            }
        }

        private void ShowToastr(string msg, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ToastrMsg",
                $"toastr['{type}']('{msg}');", true);
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objBO != null)
                {
                    objBO.ReleaseResources();
                    objBO = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}