using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Admin
{
    public partial class Bookdue : System.Web.UI.Page
    {
        AdminBO objBO = new AdminBO();
        private string[] lblErrorMsg = new string[20];
        int intAdminUserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            intAdminUserID = Convert.ToInt32(Session["AdminUserID"]);

            try
            {
                InitializeErrorMessages();

                if (!IsPostBack)
                {
                    divInputSection.Visible = false;
                    divActionButtons.Visible = false;
                    divDateSection.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[19] + ": " + ex.Message, "error");
            }
        }

        private void InitializeErrorMessages()
        {
            try
            {
                lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "ERRBD001");   //Please enter ID
                lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRBD002");   //No records found
                lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRBD003");   //Selected books marked as returned.
                lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRBD004");   //Selected books renewed.
                lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRBD005");   //Select valid renewal date!
                lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRBD006");   //Error during operation
                lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "ERRMENU016"); //Unexpected error occurred
                lblErrorMsg[7] = CommonFunction.GetErrorMessage("", "ERRBD007");   //Please select member type
                lblErrorMsg[8] = CommonFunction.GetErrorMessage("", "ERRBD008");   //Please select books to process
                lblErrorMsg[9] = CommonFunction.GetErrorMessage("", "ERRBD009");   //Please select a valid return date.
                lblErrorMsg[10] = CommonFunction.GetErrorMessage("", "ERRBD0010"); //Return date cannot be earlier than the Issue Date.
                lblErrorMsg[11] = CommonFunction.GetErrorMessage("", "ERRBD0011"); //Return date cannot be in the future.
                lblErrorMsg[12] = CommonFunction.GetErrorMessage("", "ERRBD0012"); //Please select at least one book.
                lblErrorMsg[13] = CommonFunction.GetErrorMessage("", "ERRBD0013"); //Books marked as returned successfully.
                lblErrorMsg[14] = CommonFunction.GetErrorMessage("", "ERRBD0014"); //Please select a valid renewal date.
                lblErrorMsg[15] = CommonFunction.GetErrorMessage("", "ERRBD0015"); //Renewal date cannot be earlier than today.
                lblErrorMsg[16] = CommonFunction.GetErrorMessage("", "ERRBD0016"); //Renewal date must be AFTER the current Due Date.
                lblErrorMsg[17] = CommonFunction.GetErrorMessage("", "ERRBD0017"); //Books renewed successfully.
                lblErrorMsg[18] = CommonFunction.GetErrorMessage("", "ERRBD0018"); //Invalid id-Enter the id Correctly.
                lblErrorMsg[19] = CommonFunction.GetErrorMessage("", "ERRBD0019"); //General Error
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[19], "error");
            }
        }

        protected void rblMemberType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (IsPostBack && rblMemberType.SelectedIndex != -1 && ViewState["HasSelectedMemberType"] != null)
            //{
            //    ShowToastr("Selection changed. Enter Member ID.", "info");
            //}

            // Mark that user has selected at least once
            ViewState["HasSelectedMemberType"] = true;

            txtMemberID.Text = "";
            txtDate.Text = "";
            divInputSection.Visible = true;
            divDateSection.Visible = false;
            divActionButtons.Visible = false;
            gvBookDues.DataSource = null;
            gvBookDues.DataBind();
            rptPager.Visible = false;

            // Set focus back to MemberID textbox
            txtMemberID.Focus();
            lblEnterId.Text = rblMemberType.SelectedValue == "Student"
                ? "Enter Student ID"
                : "Enter Staff ID";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadBookDues();
            }
            catch (Exception ex)
            {
                ShowToastr(lblErrorMsg[5] + ": " + ex.Message, "error");  //General Error
            }
        }

        private void LoadBookDues()
        {
            // Validate MemberID
            string memberId = txtMemberID.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberId))
            {
                ShowToastr(lblErrorMsg[0], "error");
                return;
            }

            // Validate MemberType
            if (string.IsNullOrWhiteSpace(rblMemberType.SelectedValue))
            {
                ShowToastr(lblErrorMsg[7], "warning"); // "Please select a Member Type"
                return;
            }

            try
            {
                string memberType = rblMemberType.SelectedValue;
                using (DataSet ds = objBO.GetBookDues(memberType, memberId))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        // Check for INVALID_ID
                        if (ds.Tables[0].Columns.Contains("Result") &&
                            ds.Tables[0].Rows[0]["Result"].ToString() == "INVALID_ID")
                        {
                            ShowToastr(lblErrorMsg[18], "error"); // "Invalid ID - Enter correctly"

                            // Clear GridView and hide action sections
                            gvBookDues.DataSource = null;
                            gvBookDues.DataBind();
                            divActionButtons.Visible = false;
                            divDateSection.Visible = false;

                            // Stop further processing
                            return;
                        }

                        // Bind only if rows exist
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            gvBookDues.DataSource = ds;
                            gvBookDues.DataBind();
                            divActionButtons.Visible = true;
                            divDateSection.Visible = true;
                            int totalRecords = ds.Tables[0].Rows.Count;
                            int pageSize = gvBookDues.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                            if (ds.Tables[0].Rows.Count > gvBookDues.PageSize)
                            {
                                CommonFunction.BuildPager(rptPager,totalPages, gvBookDues.PageIndex);
                                rptPager.Visible = true;
                            }
                            else
                            {
                                rptPager.Visible = false;
                            }


                            foreach (GridViewRow row in gvBookDues.Rows)
                            {
                                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                                if (chk != null) chk.Checked = false;
                            }
                        }
                        else
                        {
                            gvBookDues.DataSource = null;
                            gvBookDues.DataBind();
                            divActionButtons.Visible = false;
                            divDateSection.Visible = false;
                            ShowToastr(lblErrorMsg[1], "info"); // "No Records found"
                        }
                        CommonFunction.BuildPager(rptPager,gvBookDues.PageCount, gvBookDues.PageIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[6] + ": " + ex.Message, "error"); // Unexpected error
            }
        }

       

        protected void btnMarkReturned_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime selectedReturnDate;

                // Validate return date textbox
                if (string.IsNullOrWhiteSpace(txtDate.Text))
                {
                    ShowToastr(lblErrorMsg[9], "warning");
                    return;
                }

                if (!DateTime.TryParseExact(
                    txtDate.Text.Trim(),
                    "d-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out selectedReturnDate))
                {
                    ShowToastr(lblErrorMsg[9], "warning");
                    return;
                }

                bool anySelected = false;

                foreach (GridViewRow row in gvBookDues.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {
                        anySelected = true;

                        DateTime issueDate;
                        string issueDateText = row.Cells[6].Text.Replace("&nbsp;", "").Trim();

                        // Validate issue date from GridView
                        if (!DateTime.TryParseExact(
                            issueDateText,
                            "dd-MMM-yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out issueDate))
                        {
                            ShowToastr("Invalid issue date format.", "error");
                            return;
                        }

                        // Return date cannot be earlier than issue date
                        if (selectedReturnDate < issueDate)
                        {
                            ShowToastr(lblErrorMsg[10], "error");
                            return;
                        }

                        // Return date cannot be in future
                        if (selectedReturnDate > DateTime.Today)
                        {
                            ShowToastr(lblErrorMsg[11], "error");
                            return;
                        }

                        // Get Issue ID
                        int issueId = Convert.ToInt32(gvBookDues.DataKeys[row.RowIndex].Value);

                        // Mark returned
                        objBO.MarkBookReturned(issueId, selectedReturnDate, intAdminUserID);
                    }
                }

                if (!anySelected)
                {
                    ShowToastr(lblErrorMsg[12], "warning");
                    return;
                }

                ShowToastr(lblErrorMsg[13], "success");

                LoadBookDues();

                // Clear date textbox
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "clearDateReturned",
                    "document.getElementById('" + txtDate.ClientID + "').value='';",
                    true);
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[6] + ex.Message, "error");
            }
        }
     
        protected void btnMarkRenewed_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime renewalDate;

                // Validate renewal date
                if (string.IsNullOrWhiteSpace(txtDate.Text))
                {
                    ShowToastr(lblErrorMsg[14], "warning");
                    return;
                }

                if (!DateTime.TryParseExact(
                    txtDate.Text.Trim(),
                    "d-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out renewalDate))
                {
                    ShowToastr(lblErrorMsg[14], "warning");
                    return;
                }

                // Renewal date cannot be earlier than today
                if (renewalDate.Date <= DateTime.Today)
                {
                    ShowToastr(lblErrorMsg[15], "error");
                    return;
                }
                //if (renewalDate.Date == DateTime.Today)
                //{
                //    ShowToastr(lblErrorMsg[15], "error");
                //    return;
                //}

                bool anySelected = false;

                foreach (GridViewRow row in gvBookDues.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {
                        anySelected = true;

                        // Get Due Date from DataKeys
                        DateTime dueDate = Convert.ToDateTime(
                            gvBookDues.DataKeys[row.RowIndex]["DueDate"]
                        ).Date;

                        // Renewal date must be strictly greater than due date
                        if (renewalDate.Date <= dueDate)
                        {
                            ShowToastr(lblErrorMsg[16], "error");
                            return;
                        }

                        // Get Issue ID
                        int issueId = Convert.ToInt32(
                            gvBookDues.DataKeys[row.RowIndex]["IssueID"]
                        );

                        // Renew book
                        objBO.RenewBook(issueId, renewalDate.Date, intAdminUserID);
                    }
                }

                if (!anySelected)
                {
                    ShowToastr(lblErrorMsg[12], "warning");
                    return;
                }

                ShowToastr(lblErrorMsg[17], "success");

                LoadBookDues();

                // Clear date textbox
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "clearDateRenewed",
                    "document.getElementById('" + txtDate.ClientID + "').value='';",
                    true);
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[6] + ex.Message, "error");
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                rblMemberType.ClearSelection();
                txtMemberID.Text = "";
                txtDate.Text = "";

                divInputSection.Visible = false;
                divActionButtons.Visible = false;
                divDateSection.Visible = false;

                gvBookDues.DataSource = null;
                gvBookDues.DataBind();
                rptPager.Visible=false;
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[5] + ": " + ex.Message, "error");
            }
        }
        

     

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);
            gvBookDues.PageIndex = newIndex;
            LoadBookDues();
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

        private void ShowToastr(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});",
                true
            );
        }
    }
}
