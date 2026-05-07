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
    public partial class CategoryMaster : System.Web.UI.Page
    {
        MasterBO objMasterBO = new MasterBO();

        private string[] lblErrorMsg = new string[10];
        int intAdminUserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            intAdminUserID = Convert.ToInt32(Session["AdminUserID"]);
            if (!IsPostBack)
            {
                BindPageSizeDropdown();
                BindCategoryGrid();

            }
            lblErrorMsg = new string[30];
            lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "SUSCM01");
            lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "SUSCM02");
            lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "SUSCM03");

            lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRCM01");
            lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRCM02");
            lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRCM03");
            lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "ERRCM04");
            lblErrorMsg[7] = CommonFunction.GetErrorMessage("", "ERRCM05");
            lblErrorMsg[8] = CommonFunction.GetErrorMessage("", "ERRCM06");
            lblErrorMsg[9] = CommonFunction.GetErrorMessage("", "ERRCM07");
            lblErrorMsg[10] = CommonFunction.GetErrorMessage("", "ERRCM08");



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
                gvCategory.PageSize = Convert.ToInt32(defaultValue);
            }
        }
        private void BindCategoryGrid()
        {
            try
            {
                using (DataSet ds = objMasterBO.CategoryMaster("SELECT"))
                {

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        gvCategory.DataSource = ds.Tables[0];
                        gvCategory.DataBind();
                        int totalRecords = dt.Rows.Count;
                        int pageSize = gvCategory.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                        // ✅ Page info (like Book Grid)
                        SetPageInfo(totalRecords);

                        // ✅ Pager logic
                        if (totalPages > 1)
                        {
                            CommonFunction.BuildPager(rptPager,totalPages, gvCategory.PageIndex);
                            rptPager.Visible = true;
                            divPageSize.Visible = true;
                        }
                        else
                        {
                            rptPager.Visible = false;
                        }

                    }
                    else
                    {
                        gvCategory.DataSource = null;
                        gvCategory.DataBind();
                        rptPager.Visible = false;
                        // ✅ Clear page info
                        lblPageInfo.Text = "";
                        divPageSize.Visible = false;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                //ShowAlert("Failed to load grid", "error");
            }
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string CategoryName = txtCategoryName.Text.Trim();
                string Description = txtDescription.InnerText.Trim();
                // Category Name - Required
                if (string.IsNullOrWhiteSpace(CategoryName))
                {
                    ShowAlert(lblErrorMsg[3], "error");
                    txtCategoryName.Focus();
                    return;
                }
                if (CategoryName.Length < 3)
                {
                    ShowAlert(lblErrorMsg[10], "error");
                    txtCategoryName.Focus();
                    return;
                }
                
                    // Category Name - Pattern check (alphabets, space, hyphen, apostrophe)
                 if (!Regex.IsMatch(CategoryName, @"^[A-Za-z0-9 &\-]+$"))
                {
                    ShowAlert(lblErrorMsg[4], "error");
                    txtCategoryName.Focus();
                    return;
                }
                // Description - Required
                if (string.IsNullOrWhiteSpace(Description))
                {
                    ShowAlert(lblErrorMsg[5], "error");
                    txtDescription.Focus();
                    return;
                }
                if (Description.Length < 5)
                {
                    ShowAlert(lblErrorMsg[6], "error");
                    txtDescription.Focus();
                    return;
                }
                // Max length
                if (Description.Length > 300)
                {
                    ShowAlert(lblErrorMsg[7], "error");
                    txtDescription.Focus();
                    return;
                }
                // Allowed characters
                if (!Regex.IsMatch(Description, @"^[A-Za-z0-9 .,()'""-:;!?]+$"))
                {
                    ShowAlert(lblErrorMsg[8], "error");
                    txtDescription.Focus();
                    return;
                }

                using (DataSet ds = objMasterBO.CategoryMaster("INSERT", 0, CategoryName, Description, true, intAdminUserID))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                        if (msgCode == 2)
                        {
                            ShowAlert(lblErrorMsg[9], "error");
                        }
                        if (msgCode == 1)
                        {
                            ShowAlert(lblErrorMsg[0], "success");
                            ClearFormFields();
                            BindCategoryGrid();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        protected void gvCategory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string CategoryID = e.CommandArgument.ToString();
                int updatedBy = Convert.ToInt32(Session["UserID"]);
                if (e.CommandName == "EditCategory")
                {
                    using (DataSet ds = objMasterBO.CategoryMaster("SELECTBYID", Convert.ToInt32(CategoryID)))
                    {
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            hdnCategoryID.Value = dr["CategoryID"].ToString();
                            txtCategoryName.Text = dr["CategoryName"].ToString();
                            txtDescription.InnerText = dr["Description"].ToString();
                            chkActive.Checked = Convert.ToBoolean(dr["Active"]);
                            btnAdd.Visible = false;
                            btnUpdate.Visible = true;
                        }
                    }
                }
                if (e.CommandName == "DeleteCategory")
                {
                    using (DataSet ds = objMasterBO.CategoryMaster("DELETE", Convert.ToInt32(CategoryID), "", "", false, intAdminUserID))
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                        if (msgCode == 1)
                        {
                            ShowAlert(lblErrorMsg[2], "success");
                            BindCategoryGrid();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                int CategoryID = Convert.ToInt32(hdnCategoryID.Value);
                string CategoryName = txtCategoryName.Text.Trim();
                string Description = txtDescription.InnerText;
                bool isUpdate = btnAdd.Text == "Save";
                bool Active = chkActive.Checked;


                using (DataSet ds = objMasterBO.CategoryMaster("UPDATE", Convert.ToInt32(hdnCategoryID.Value), CategoryName, Description, chkActive.Checked, intAdminUserID))
                {
                    int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                    if (msgCode == 2)
                    {
                        ShowAlert(lblErrorMsg[9], "error");
                    }
                    if (msgCode == 1)
                    {
                        ShowAlert(lblErrorMsg[1], "success");
                        ClearFormFields();
                        BindCategoryGrid();
                        btnAdd.Visible = true;
                        btnUpdate.Visible = false;
                        btnAdd.Text = "Save";

                    }
                  
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }


        private void ShowAlert(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});",
                true
            );
        }

        private void ClearFormFields()
        {
            txtCategoryName.Text = "";
            txtDescription.InnerText = "";
        }
        protected void Clear_Click(object sender, EventArgs e)
        {
            chkActive.Checked=true;
            ClearFormFields();
            btnAdd.Visible=true;
            btnUpdate.Visible= false;
        }

        protected void gvCategory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategory.PageIndex = e.NewPageIndex;
            BindCategoryGrid();

        }

       


        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            gvCategory.PageIndex = newIndex;
            BindCategoryGrid();
        }
        private void SetPageInfo(int totalRows)
        {
            if (totalRows == 0)
            {
                lblPageInfo.Text = "";
                return;
            }

            int start = (gvCategory.PageIndex * gvCategory.PageSize) + 1;
            int end = Math.Min(start + gvCategory.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start}–{end} of {totalRows} entries";
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvCategory.PageSize = pageSize;
            }
            else
            {
                gvCategory.PageSize = 5; // default fallback
            }

            // 🔹 Reset to first page
            gvCategory.PageIndex = 0;
            BindCategoryGrid();

        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objMasterBO != null)
                {
                    objMasterBO.ReleaseResources();
                    objMasterBO = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}
