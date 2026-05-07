using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class RoleMenuMapping : System.Web.UI.Page
    {
        MasterBO objMasterBO = new MasterBO();
        AdminBO objBO = new AdminBO();
        private string[] lblErrorMsg = new string[20];
        int adminUserId;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorLog();
                adminUserId =  Convert.ToInt32(Session["AdminUserID"] ?? 0);

                if (!IsPostBack)
                {
                    LoadRoleTypes();
                    BindPageSizeDropdown();
                    pnlMenuList.Visible=false;
                    divPageSize.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowToastr(CommonFunction.GetErrorMessage("", "ERR999") + " " + ex.Message, "error");
            }
        }

        private void ErrorLog()
        {
            try
            {
                lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRMENU016"); //Unexpected error occurred
                lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRMENU022"); //No record Found
                lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRRRMM005"); //Role Menu Updated Successfully.
                
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[1], "error");
            }
        }
        private void LoadRoleTypes()
        {
            try
            {
                using (DataSet ds = objBO.GetRolesForDropdown())
                {
                    ddlRoleType.Items.Clear();

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ddlRoleType.DataSource = ds.Tables[0];
                        ddlRoleType.DataTextField = "UserRole"; 
                        ddlRoleType.DataValueField = "RoleID";
                        ddlRoleType.DataBind();
                    }

                    ddlRoleType.Items.Insert(0,
                        new ListItem("-- Select Role --", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[1] + " " + ex.Message, "error");
            }
        }
        private void BindPageSizeDropdown()
        {
            DataTable dt = objMasterBO.GetConfigValues("GET_PAGESIZE", "PageSizeOptions");

            ddlPageSize.DataSource = dt;
            ddlPageSize.DataTextField = "ConfigValue";
            ddlPageSize.DataValueField = "ConfigValue";
            ddlPageSize.DataBind();

            // ✅ Get default from SP (SequenceNo = 1 OR IsDefault = 1)
            DataRow defaultRow = dt.Rows[0]; // because ORDER BY SequenceNo

            if (defaultRow != null)
            {
                string defaultValue = defaultRow["ConfigValue"].ToString();

                ddlPageSize.SelectedValue = defaultValue;

                // 🔥 CONNECT GRID HERE
                gvRoleMenu.PageSize = Convert.ToInt32(defaultValue);
            }
        }



        protected void ddlRoleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gvRoleMenu.PageIndex = 0;
                if (ddlRoleType.SelectedValue == "0")
                {
                    gvRoleMenu.DataSource = null;
                    gvRoleMenu.DataBind();
                    pnlMenuList.Visible=false;
                    divPageSize.Visible = false;
                    return;
                }
                pnlMenuList.Visible=true;
                LoadMenuByRole();
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[1] + " " + ex.Message, "error");
            }
        }

        private void LoadMenuByRole()
        {
            try
            {
                if (ddlRoleType.SelectedValue=="0")
                {
                    pnlMenuList.Visible=false;
                    return;
                }

                int roleID = Convert.ToInt32(ddlRoleType.SelectedValue);
                using (DataSet ds = objBO.SearchRoleMenu(roleID))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        gvRoleMenu.DataSource = dt;
                        gvRoleMenu.DataBind();

                        int totalRecords = dt.Rows.Count;
                        int pageSize = gvRoleMenu.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                        // ✅ Page info
                        SetPageInfo(totalRecords);

                        // ✅ ALWAYS show dropdown when data exists
                        divPageSize.Visible = true;

                        // ✅ Pager only if needed
                        if (totalPages > 1)
                        {
                            CommonFunction.BuildPager(rptPager, totalPages, gvRoleMenu.PageIndex);
                            rptPager.Visible = true;
                        }
                        else
                        {
                            rptPager.Visible = false;
                        }
                    }
                    else
                    {
                        gvRoleMenu.DataSource = null;
                        gvRoleMenu.DataBind();

                        rptPager.Visible = false;
                        lblPageInfo.Text = "";
                        divPageSize.Visible = false;

                        ShowToastr(lblErrorMsg[2], "info"); //No Record Found
                    }
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        pnlMenuList.Visible = true;   // ✅ HERE
                    }
                    else
                    {
                        pnlMenuList.Visible = false;  // ✅ FIX
                    }
                   
                    
                }
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[1]+ " " + ex.Message, "error");
            }
        }

        protected void chkAllow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)sender;
                GridViewRow row = (GridViewRow)chk.NamingContainer;

                int menuID = Convert.ToInt32(gvRoleMenu.DataKeys[row.RowIndex].Value);
                int roleID = Convert.ToInt32(ddlRoleType.SelectedValue);
                int aUserID = Convert.ToInt32(Session["AUserID"]);

                int isChecked = chk.Checked ? 1 : 0;

                using (DataSet ds = objBO.SaveUpdateRoleMenu(roleID, menuID, 0, isChecked, aUserID))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        int MsgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                        {
                            ShowToastr(lblErrorMsg[3], "success");
                        }
                    }
                    LoadMenuByRole();
                    //lblRecordCount.Text = gvRoleMenu.Rows.Count + " Records found";
                }
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[1] + " " + ex.Message, "error");
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ddlRoleType.SelectedIndex = 0;
                gvRoleMenu.PageIndex = 0;
                gvRoleMenu.DataSource = null;
                gvRoleMenu.DataBind();
                
                pnlMenuList.Visible=false;
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[1] + " " + ex.Message, "error");
            }
        }

        protected void gvMenu_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRoleMenu.PageIndex = e.NewPageIndex;
            LoadMenuByRole();

        }
        



        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            gvRoleMenu.PageIndex = newIndex;
            LoadMenuByRole();
        }

        private void ShowToastr(string message, string type)
        {
            message = (message ?? "").Replace("'", "\\'").Replace("\"", "\\\"").Replace(Environment.NewLine, " ").Trim();
            ScriptManager.RegisterStartupScript(this.Page, GetType(), Guid.NewGuid().ToString(), "$(function(){AlertMessage('" + message + "','" + type.ToLower() + "')});", true);
        }

        private void SetPageInfo(int totalRows)
        {
            if (totalRows == 0)
            {
                lblPageInfo.Text = "";
                return;
            }

            int start = (gvRoleMenu.PageIndex * gvRoleMenu.PageSize) + 1;
            int end = Math.Min(start + gvRoleMenu.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start}–{end} of {totalRows} entries";
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvRoleMenu.PageSize = pageSize;
            }
            else
            {
                gvRoleMenu.PageSize = 5; // default fallback
            }

            // 🔹 Reset to first page
            gvRoleMenu.PageIndex = 0;
            LoadMenuByRole();

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
