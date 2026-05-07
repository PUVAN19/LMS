using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class Menumaster : System.Web.UI.Page
    {
        MasterBO objMasterBO = new MasterBO();

        AdminBO objAdminBO = new AdminBO();
        private string[] lblErrorMsg = new string[20];
        int intAdminUserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            intAdminUserID = Convert.ToInt32(Session["AdminUserID"]);
            try
            {
                ErrorLog();

                if (!IsPostBack)
                {
                    BindPageSizeDropdown();
                    parentMenuDiv.Visible = false;
                    BindGrid();
                    IsDefault.Checked = false;
                    BindParentMenus();
                }

            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9], "error");
            }
        }

        private void ErrorLog()
        {
            try
            {

                lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "ERRMENU001"); // Menu Name is required.
                lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRMENU002"); // Invalid Menu Name.
                lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRMENU005"); // Active missing.
                lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRMENU014"); // Duplicate (Menu name already exists.)
                lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRMENU016"); // Unexpected error
                lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRMENU015"); // Menu not found
                lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "ERRMENU012"); // Menu updated successfully (used as generic)
                lblErrorMsg[7] = CommonFunction.GetErrorMessage("", "ERRMENU013"); // Menu deleted successfully (used as generic)
                lblErrorMsg[8] = CommonFunction.GetErrorMessage("", "ERRMENU011"); // Menu added successfully (used as generic)
                lblErrorMsg[9] = CommonFunction.GetErrorMessage("", "ERRMENU016"); // General/unexpected error fallback
                lblErrorMsg[10]= CommonFunction.GetErrorMessage("", "ERRMENU004"); // page name error
                lblErrorMsg[11]= CommonFunction.GetErrorMessage("", "ERRMENU008"); // page name error 
                lblErrorMsg[12]= CommonFunction.GetErrorMessage("", "ERRMENU017"); // Select SearchBy
                lblErrorMsg[13]= CommonFunction.GetErrorMessage("", "ERRMENU018"); // Format invalid for MenuName
                lblErrorMsg[14]= CommonFunction.GetErrorMessage("", "ERRMENU019"); // Format invalid for PageName
                lblErrorMsg[15]= CommonFunction.GetErrorMessage("", "ERRMENU020"); // Search Cleared
                lblErrorMsg[16]= CommonFunction.GetErrorMessage("", "ERRMENU021"); // Enter Search Value.
                lblErrorMsg[17]= CommonFunction.GetErrorMessage("", "ERRMENU022"); // No Record Found
                lblErrorMsg[18]= CommonFunction.GetErrorMessage("", "ERRMENU023"); // Select Parent Menu.
                lblErrorMsg[19]= CommonFunction.GetErrorMessage("", "ERRMENU024"); // PageName is Required to check Default Page.
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9], "error");
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
                gvMenu.PageSize = Convert.ToInt32(defaultValue);
            }
        }
        protected void chkIsActive_CheckedChanged(object sender, EventArgs e)
        {
            // If you need to handle anything when active checkbox changes
            bool isActive = chkIsActive.Checked;
        }


        protected void chkIsChild_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                parentMenuDiv.Visible = chkIsChild.Checked;

                if (!chkIsChild.Checked)
                {
                    ddlParentMenu.SelectedValue="0";
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9] + ": " + ex.Message, "error");
            }
        }
        private int GetNextSequenceNo(int parentMenuID)
        {
            using (DataSet ds = objAdminBO.MenuMaster(
                "SELECT_CHILD", 0, "", "", parentMenuID, true, false, 0, true, intAdminUserID))
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count == 0)
                    return 1;

                return dt.Rows.Count + 1;
            }
        }

        private int GetNextParentSequence()
        {
            using (DataSet ds = objAdminBO.MenuMaster(
                "SELECT_PARENT", 0, "", "", 0, false, false, 0, true, intAdminUserID))
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count == 0)
                    return 1;

                return dt.Rows.Count + 1;
            }
        }


        #region Bind Methods
        private void BindParentMenus()
        {
            try
            {
                ddlParentMenu.Items.Clear();
                ddlParentMenu.Items.Add(new ListItem("-- Select Parent Menu --", "0"));

                using (DataSet ds = objAdminBO.MenuMaster("SELECT_PARENT", 0, "", "", 0, false, false, 0, true, intAdminUserID))
                {
                    DataTable dt = ds.Tables[0];

                    foreach (DataRow dr in dt.Rows)
                    {
                        ddlParentMenu.Items.Add(new ListItem(
                            dr["MenuName"].ToString(),
                            dr["MenuID"].ToString()
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[4] + ": " + ex.Message, "error");
            }
        }
     
        private void BindGrid()
        {
            try
            {
                string searchBy = ddlSearchBy.SelectedValue;
                string searchValue = txtSearchValue.Text.Trim();

                using (DataSet ds =
                    (!string.IsNullOrEmpty(searchBy) && !string.IsNullOrEmpty(searchValue))
                        ? objAdminBO.SearchMenuMaster(searchBy, searchValue)
                        : objAdminBO.GetMenuMasterGrid()
                )
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];   // ✅ FIXED

                        gvMenu.DataSource = dt;
                        gvMenu.DataBind();

                        divGrid.Visible = true;

                        int totalRecords = dt.Rows.Count;
                        int pageSize = gvMenu.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                        // ✅ Page info
                        SetPageInfo(totalRecords);

                        // ✅ ALWAYS show dropdown when data exists
                        divPageSize.Visible = true;

                        // ✅ Pager only if needed
                        if (totalPages > 1)
                        {
                            CommonFunction.BuildPager(rptPager, totalPages, gvMenu.PageIndex);
                            rptPager.Visible = true;
                        }
                        else
                        {
                            rptPager.Visible = false;
                        }
                    }
                    else
                    {
                        gvMenu.DataSource = null;
                        gvMenu.DataBind();

                        rptPager.Visible = false;
                        lblPageInfo.Text = "";
                        divPageSize.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[4] + ": " + ex.Message, "error");
            }
        }
        #endregion

        #region Insert / Update

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string menuName = txtMenuName.Text.Trim();
                string pageName = txtPageName.Text.Trim();
                bool isChild = chkIsChild.Checked;
                bool IsDefaultPage = IsDefault.Checked;
                bool isActive = chkIsActive.Checked;

                int parentId = Convert.ToInt32(ddlParentMenu.SelectedValue);
                // ✅ Default page validation
                if (IsDefaultPage && string.IsNullOrWhiteSpace(pageName))
                {
                    ShowToastr(lblErrorMsg[19], "error"); // "Page name is required to check DeafaultPage."
                    return;
                }

                // ===== VALIDATION =====
                if (string.IsNullOrWhiteSpace(menuName))
                {
                    ShowToastr(lblErrorMsg[0], "error");
                    return;
                }

                if (!Regex.IsMatch(menuName, @"^[A-Za-z\s]+$"))
                {
                    ShowToastr(lblErrorMsg[1], "error");
                    return;
                }

                if (isChild)
                {
                    if (string.IsNullOrWhiteSpace(pageName))
                    {
                        ShowToastr(lblErrorMsg[11], "error");
                        return;
                    }
                    // PageName validation (only .aspx allowed)
                    if (!Regex.IsMatch(pageName, @"^[A-Za-z0-9_]+\.aspx$", RegexOptions.IgnoreCase))
                    {
                        ShowToastr(lblErrorMsg[10], "error"); // Invalid Page Name
                        return;
                    }

                    if (parentId == 0)
                    {
                        ShowToastr(lblErrorMsg[18], "error");
                        return;
                    }
                }
                else
                {

                    if (txtPageName.Text.Length > 0)
                    {
                        if (!Regex.IsMatch(txtPageName.Text.Trim(), @"^[A-Za-z0-9_]+\.aspx$", RegexOptions.IgnoreCase))
                        {
                            ShowToastr(lblErrorMsg[10], "error");
                            return;
                        }
                    }
                }

                int sequenceNo = isChild
                    ? GetNextSequenceNo(parentId)
                    : GetNextParentSequence();

                using (DataSet ds = objAdminBO.MenuMaster("INSERT", 0, menuName, pageName, parentId, isChild, IsDefaultPage, sequenceNo, isActive, intAdminUserID))
                {
                    int resultCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"]);

                    switch (resultCode)
                    {
                        case 0:
                            ClearFields();
                            BindGrid();
                            BindParentMenus();
                            ShowToastr(lblErrorMsg[8], "success"); // Menu added
                            break;

                        case 1:
                            ShowToastr(lblErrorMsg[0], "error");
                            break;

                        case 2:
                            ShowToastr(lblErrorMsg[3], "warning");
                            break;

                        default:
                            ShowToastr(lblErrorMsg[4], "error");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9], "error");
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hfMenuID.Value))
                {
                    ShowToastr("No record selected for update.", "error");
                    return;
                }

                string menuName = txtMenuName.Text.Trim();
                string pageName = txtPageName.Text.Trim();
                bool isChild = chkIsChild.Checked;
                bool IsDefaultPage = IsDefault.Checked;
                int parentId = 0;
                int.TryParse(ddlParentMenu.SelectedValue, out parentId);
                bool isActive = chkIsActive.Checked;
                int menuId = Convert.ToInt32(hfMenuID.Value);

                // ✅ Default page validation
                if (IsDefaultPage && string.IsNullOrWhiteSpace(pageName))
                {
                    ShowToastr(lblErrorMsg[19], "error"); // "Page name is required to check Default Page."
                    return;
                }


                if (string.IsNullOrWhiteSpace(menuName))
                {
                    ShowToastr(lblErrorMsg[0], "error");
                    return;
                }

                if (!Regex.IsMatch(menuName, @"^[A-Za-z\s&/]+$"))
                {
                    ShowToastr(lblErrorMsg[1], "error");
                    return;
                }

                if (isChild)
                {
                    if (string.IsNullOrWhiteSpace(pageName))
                    {
                        ShowToastr(lblErrorMsg[11], "error"); // Please enter Page Name
                        return;
                    }

                    if (parentId==0)
                    {
                        ShowToastr(lblErrorMsg[18], "error");  //Select Parent Menu
                        return;
                    }
                }

                int sequenceNo;

                if (isChild)
                {
                    sequenceNo = GetNextSequenceNo(parentId);
                }
                else
                {
                    sequenceNo = GetNextParentSequence();
                }
                string modifiedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "";

                using (DataSet ds = objAdminBO.MenuMaster(
                    "UPDATE",menuId,menuName,pageName, parentId,isChild,IsDefaultPage,sequenceNo,isActive,intAdminUserID))
                {
                    chkIsActive.Checked = true;
                    int resultCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"]);

                    switch (resultCode)
                    {
                        case 0:
                            ClearFields();
                            BindGrid();
                            BindParentMenus();
                            btnSubmit.Visible = true;
                            btnUpdate.Visible = false;
                            ShowToastr(lblErrorMsg[6], "success"); // updated
                            break;

                        case 2:
                            ShowToastr(lblErrorMsg[3], "warning"); // duplicate
                            break;

                        case 3:
                            ShowToastr(lblErrorMsg[5], "error"); // not found
                            break;

                        default:
                            ShowToastr(lblErrorMsg[4], "error");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[6] + ": " + ex.Message, "error");
            }
        }
        #endregion

        #region Grid Events
        protected void gvMenu_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMenu.PageIndex = e.NewPageIndex;
            BindGrid();

        }

        protected void gvMenu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditMenu")
            {
                try
                {
                    int menuId = Convert.ToInt32(e.CommandArgument);

                    using (DataSet ds = objAdminBO.GetMenuByID(menuId))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];

                            hfMenuID.Value = dr["MenuID"].ToString();
                            txtMenuName.Text = dr["MenuName"].ToString();
                            txtPageName.Text = dr["PageName"].ToString();

                            chkIsChild.Checked = Convert.ToBoolean(dr["IsChildMenu"]);
                            IsDefault.Checked = Convert.ToBoolean(dr["IsDefaultPage"]);
                            parentMenuDiv.Visible = chkIsChild.Checked;

                            // ✅ PARENT MENU FALLBACK HANDLING
                            string parentValue = dr["ParentMenuID"] != DBNull.Value
                                ? dr["ParentMenuID"].ToString()
                                : "0";

                            if (ddlParentMenu.Items.FindByValue(parentValue) != null)
                            {
                                ddlParentMenu.SelectedValue = parentValue;
                            }
                            else
                            {
                                ddlParentMenusFallbackSelect(parentValue);
                            }

                            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

                            btnSubmit.Visible = false;
                            btnUpdate.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyExceptionLogger.Publish(ex);
                    ShowToastr(lblErrorMsg[5] + ": " + ex.Message, "error");
                }
            }
        }


        protected void gvMenu_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int menuId = Convert.ToInt32(gvMenu.DataKeys[e.RowIndex].Value);

                using (DataSet ds = objAdminBO.MenuMaster("DELETE", menuId))
                {
                    int resultCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"]);

                    if (resultCode == 0)
                    {
                        BindGrid();
                        ShowToastr(lblErrorMsg[7], "success");
                    }
                    else if (resultCode == 3)
                    {
                        ShowToastr(lblErrorMsg[5], "error");
                    }
                    else
                    {
                        ShowToastr(lblErrorMsg[4], "error");
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9] + ": " + ex.Message, "error");
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchBy = ddlSearchBy.SelectedValue;
                string searchValue = txtSearchValue.Text.Trim();

                if (string.IsNullOrEmpty(searchBy))
                {
                    ShowToastr(lblErrorMsg[12], "warning");
                    return;
                }

                if (string.IsNullOrEmpty(searchValue))
                {
                    ShowToastr(lblErrorMsg[16], "error");
                    return;
                }

                if (searchBy == "MenuName")
                {
                    if (!Regex.IsMatch(searchValue, @"^[A-Za-z\s]+$"))
                    {
                        ShowToastr(lblErrorMsg[13], "error"); // Format invalid for MenuName
                        return;
                    }
                }

                if (searchBy == "PageName")
                {
                    if (!Regex.IsMatch(searchValue, @"^[A-Za-z0-9_]+\.aspx$", RegexOptions.IgnoreCase))
                    {
                        ShowToastr(lblErrorMsg[14], "error"); // Format invalid for PageName
                        return;
                    }
                }
                gvMenu.PageIndex = 0;
                BindGrid();
               

                if (gvMenu.Rows.Count == 0)
                {
                    ShowToastr(lblErrorMsg[17], "warning"); // No Record Found.
                }

            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9] + ": " + ex.Message, "error");
            }
        }


        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            ddlSearchBy.SelectedIndex = 0;
            txtSearchValue.Text = "";
            chkIsActive.Checked = true;
            gvMenu.PageIndex = 0;
            BindGrid();

        }

        protected void gvMenu_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool isActive = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "IsActive"));

                LinkButton deleteBtn = (LinkButton)e.Row.FindControl("btnDelete");

                if (!isActive)
                {
                    deleteBtn.Enabled = false;
                    deleteBtn.CssClass += " disabled";
                    deleteBtn.OnClientClick = "return false;";

                    deleteBtn.Style.Add("opacity", "0.4");
                    deleteBtn.Style.Add("cursor", "not-allowed");
                }
            }
        }

        #endregion
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFields();
                btnSubmit.Visible = true;
                btnUpdate.Visible = false;
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[9] + ": " + ex.Message, "error");
            }
        }

        private void ClearFields()
        {
            txtMenuName.Text = "";
            txtPageName.Text = "";
            chkIsActive.Checked = true;
            chkIsChild.Checked = false;
            IsDefault.Checked = false;
            parentMenuDiv.Visible = false;
            if (ddlParentMenu.Items.FindByValue("0") != null)
                ddlParentMenu.SelectedValue = "0";
            hfMenuID.Value = "";
        }

        private void ddlParentMenusFallbackSelect(string value)
{
    try
    {
        if (!string.IsNullOrEmpty(value))
        {
            ddlParentMenu.Items.Add(
                new ListItem("Parent (ID: " + value + ")", value)
            );
            ddlParentMenu.SelectedValue = value;
        }
    }
    catch (Exception ex)
    {
        MyExceptionLogger.Publish(ex);
    }
}


    


        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            gvMenu.PageIndex = newIndex;
            BindGrid();
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

            int start = (gvMenu.PageIndex * gvMenu.PageSize) + 1;
            int end = Math.Min(start + gvMenu.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start}–{end} of {totalRows} entries";
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvMenu.PageSize = pageSize;
            }
            else
            {
                gvMenu.PageSize = 5; // default fallback
            }

            // 🔹 Reset to first page
            gvMenu.PageIndex = 0;
            BindGrid();

        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objAdminBO != null)
                {
                    objAdminBO.ReleaseResources();
                    objAdminBO = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}
