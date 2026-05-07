using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class MemberDashboard : System.Web.UI.Page
    {
        MasterBO objMasterBO = new MasterBO();
        CommonBO objCommonBO = new CommonBO();
        private string[] lblErrorMsg = new string[10];
        //int intAdminUserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            //intAdminUserID = Convert.ToInt32(Session["AdminUserID"]);

            if (Session["MemberID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindPageSizeDropdown();
                BindSummary();

            }
            lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRMEM01");
            lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRMEM02");
            lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "SUSMEM01");
            lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRMEM03");
            lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRMEM04");

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
                gvBooks.PageSize = Convert.ToInt32(defaultValue);
            }
        }

        private void BindSummary()
        {
            try
            {
                string memberID = Session["MemberID"].ToString();

                using (DataSet ds = objCommonBO.GetMemberDashboard(memberID))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];

                        lblBorrowedCount.Text = dr["BorrowedBooksCount"].ToString();
                        lblReturnedCount.Text = dr["ReturnedBooksCount"].ToString();
                        lblDueCount.Text = dr["DueBooksCount"].ToString();

                        int overdueCount = Convert.ToInt32(dr["OverDueBooksCount"]);
                        int finedCount = Convert.ToInt32(dr["FinedBooksCount"]);

                        lblOverDueCount.Text = overdueCount.ToString();

                        string message = "";

                        if (overdueCount > 0 && finedCount == 0)
                        {
                            message = $"You have <b>{overdueCount}</b> overdue book(s). Please return or renew them immediately.";
                        }
                        else if (overdueCount == 0 && finedCount > 0)
                        {
                            message = $"You have <b>{finedCount}</b> book(s) with unpaid fines. Please pay the fine immediately.";
                        }
                        else if (overdueCount > 0 && finedCount > 0)
                        {
                            message = $"You have <b>{overdueCount}</b> overdue book(s) and <b>{finedCount}</b> book(s) with unpaid fines. Please return or renew the books and pay the fine immediately.";
                        }

                        if (!string.IsNullOrEmpty(message))
                        {
                            divOverDueAlert.InnerHtml =
                                $"<i class='fa-regular fa-clock fs-4 text-warning'></i> {message}";
                            divOverDueAlert.Visible = true;
                        }
                        else
                        {
                            divOverDueAlert.Visible = false;
                        }

                        Session["MemberBooks"] = ds.Tables[0];

                        BindGrid(CurrentGrid);
                    }
                    else
                    {
                        ClearGrids();
                        rptPager.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        private void BindGrid(string type, bool resetPageIndex = false)
        {
            DataTable dt = Session["MemberBooks"] as DataTable;

            if (dt == null)
            {
                gvBooks.DataSource = null;
                gvBooks.DataBind();

                rptPager.Visible = false;
                divPageSize.Visible = false;   // ✅ FIX
                lblPageInfo.Text = "";         // ✅ FIX

                return;
            }

            DataView dv = dt.DefaultView;

            switch (type)
            {
                case "BORROWED":
                    dv.RowFilter = "";
                    ConfigureGrid("BORROWED");
                    lblGridTitle.InnerText = "Borrowed Books";
                    break;

                case "RETURNED":
                    dv.RowFilter = "ReturnDate IS NOT NULL";
                    ConfigureGrid("RETURNED");
                    lblGridTitle.InnerText = "Returned Books";
                    break;

                case "DUE":
                    dv.RowFilter = "ReturnDate IS NULL";
                    ConfigureGrid("DUE");
                    lblGridTitle.InnerText = "Due Books";
                    break;
            }

            DataTable filteredTable = dv.ToTable();

            if (resetPageIndex)
                gvBooks.PageIndex = 0;

            gvBooks.DataSource = filteredTable;
            gvBooks.DataBind();

            int totalRows = filteredTable.Rows.Count;

            // ✅ HANDLE EMPTY DATA FIRST
            if (totalRows == 0)
            {
                rptPager.Visible = false;
                divPageSize.Visible = false;   // ✅ FIX
                lblPageInfo.Text = "";         // ✅ FIX
                return;
            }

            // ✅ DATA EXISTS
            divPageSize.Visible = true;        // ✅ FIX

            // ✅ PAGE INFO LABEL
            SetPageInfo(totalRows);            // ✅ FIX

            // ✅ PAGER
            if (totalRows > gvBooks.PageSize)
            {
                CommonFunction.BuildPager(rptPager, gvBooks.PageCount, gvBooks.PageIndex);
                rptPager.Visible = true;
            }
            else
            {
                rptPager.Visible = false;
            }
        }
        private void ConfigureGrid(string mode)
        {
            int issueDateCol = 8;
            int dueDateCol = 9;
            int returnDateCol = 10;
            int payFineCol = 11;     
            int renewalCol = 12;
            int renewalRequestCol = 13;
            int status = 14;
            int rejectReason = 15;

            gvBooks.Columns[issueDateCol].Visible = false;
            gvBooks.Columns[dueDateCol].Visible = false;
            gvBooks.Columns[returnDateCol].Visible = false;
            gvBooks.Columns[renewalCol].Visible = false;
            gvBooks.Columns[renewalRequestCol].Visible = false;
            gvBooks.Columns[status].Visible = false;
            gvBooks.Columns[rejectReason].Visible = false;
            gvBooks.Columns[payFineCol].Visible = false;

            if (mode == "BORROWED")
            {
                gvBooks.Columns[issueDateCol].Visible = true;
                gvBooks.Columns[dueDateCol].Visible = true;
                gvBooks.Columns[returnDateCol].Visible = true;
                gvBooks.Columns[renewalCol].Visible = true;
                gvBooks.Columns[status].Visible = true;
                gvBooks.Columns[rejectReason].Visible = true;
            }
            else if (mode == "RETURNED")
            {
                gvBooks.Columns[returnDateCol].Visible = true;
            }
            else if (mode == "DUE")
            {
                gvBooks.Columns[issueDateCol].Visible = true;
                gvBooks.Columns[dueDateCol].Visible = true;
                gvBooks.Columns[renewalCol].Visible = true;
                gvBooks.Columns[renewalRequestCol].Visible = true;
                gvBooks.Columns[status].Visible = true;
                gvBooks.Columns[rejectReason].Visible = true;
                gvBooks.Columns[payFineCol].Visible = true;  
            }
        }


        protected void Borrowed_Click(object sender, EventArgs e)
        {
            CurrentGrid = "BORROWED";
            BindGrid("BORROWED", true);
        }

        protected void Returned_Click(object sender, EventArgs e)
        {
            CurrentGrid = "RETURNED";
            BindGrid("RETURNED", true);
        }

        protected void Due_Click(object sender, EventArgs e)
        {
            CurrentGrid = "DUE";
            BindGrid("DUE", true);
        }

        private void ClearGrids()
        {
            // Clear GridView data
            gvBooks.DataSource = null;
            gvBooks.DataBind();

            // Reset summary counts
            lblBorrowedCount.Text = "0";
            lblReturnedCount.Text = "0";
            lblDueCount.Text = "0";
            lblOverDueCount.Text = "0";

            // Hide overdue alert
            divOverDueAlert.Visible = false;

            // Clear session data
            Session["MemberBooks"] = null;

            // Reset grid title
            lblGridTitle.InnerText = "Books";
        }

        protected void gvBooks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBooks.PageIndex = e.NewPageIndex;
            BindSummary();
        }
      
        private string CurrentGrid
        {
            get { return Session["CurrentGrid"]?.ToString() ?? "BORROWED"; }
            set { Session["CurrentGrid"] = value; }
        }


        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            gvBooks.PageIndex = newIndex;
            BindSummary();

        }

        protected void gvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "PayFine")
            {
                int bookIssueId;
                if (!int.TryParse(e.CommandArgument.ToString(), out bookIssueId))
                    return;

                ShowAlert("Redirecting to payment...", "info");
            }

            if (e.CommandName == "RenewalRequest")
            {
                // 1️⃣ Get BookIssueID from CommandArgument
                int bookIssueId;
                if (!int.TryParse(e.CommandArgument.ToString(), out bookIssueId))
                {
                    ShowAlert(lblErrorMsg[5], "error");
                    return;
                }

                // 2️⃣ Get data from session
                DataTable dt = Session["MemberBooks"] as DataTable;
                if (dt == null) return;

                // 3️⃣ Find row by BookIssueID
                DataRow row = dt.AsEnumerable()
                                .FirstOrDefault(r => Convert.ToInt32(r["BookIssueID"]) == bookIssueId);
                if (row == null) return;

                // ❌ Renewal not allowed
                if (Convert.ToInt32(row["IsRenewalAllowed"]) == 0)
                {
                    // ✅ ADD HERE
                    lblMaxRenewDays.Text = row["MaxRenewalDays"].ToString();

                    // Show warning modal (not hard-coded)
                    ScriptManager.RegisterStartupScript(
                        Page,
                        Page.GetType(),
                        "ShowRenewalExpiredModal",
                          "$(document).ready(function(){ $('#renewalExpiredModal').modal('show'); });",

                        true
                    );
                    return;
                }
                if (row != null)
                {
                    lblISBN.Text = row["ISBN"].ToString();
                    lblBookTitle.Text = row["BookTitle"].ToString();
                    lblCategory.Text = row["CategoryName"].ToString();
                    lblEdition.Text = row["Edition"].ToString();
                    lblAuthors.Text = row["AuthorNames"].ToString();
                    lblDueDate.Text = Convert.ToDateTime(row["DueDate"])
                                        .ToString("dd-MMM-yyyy");

                    // Store BookID in hidden field for submission
                    hdnBookID.Value = row["BookID"].ToString();
                    hdnBookIssueID.Value = row["BookIssueID"].ToString();

                    txtRenewDays.Text = "";
                }

                // 4️⃣ Show modal
                ScriptManager.RegisterStartupScript(
                    Page,
                    Page.GetType(),
                    "ShowModal",
                    "$(document).ready(function(){ $('#renewalModal').modal('show'); });",
                    true
                );
            }
        }

        protected void gvBooks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;

                string lastStatus = drv["Last Renewal Status"]?.ToString();
                LinkButton btnRenew = (LinkButton)e.Row.FindControl("lnkRenewal");

                if (btnRenew != null &&
                    !string.IsNullOrEmpty(lastStatus) &&
                    lastStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    btnRenew.Visible = false;
                }

                LinkButton btnPayFine = (LinkButton)e.Row.FindControl("lnkPayFine");

                decimal fineAmount = 0;
                string fineStatus = drv["FineStatus"]?.ToString();

                if (drv["FineAmount"] != DBNull.Value)
                    fineAmount = Convert.ToDecimal(drv["FineAmount"]);

                if (btnPayFine != null)
                {
                    if (fineAmount > 0 && fineStatus == "Unpaid")
                    {
                        btnPayFine.Visible = true;
                        btnPayFine.Text = "Pay ₹" + fineAmount.ToString("0.00");
                    }
                    else
                    {
                        btnPayFine.Visible = false;
                    }
                }
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
        protected void btnSubmitRenewal_Click(object sender, EventArgs e)
        {
            try
            {
                string memberID = Session["MemberID"].ToString();
                int bookIssueId = Convert.ToInt32(hdnBookIssueID.Value);

                int noOfDays;
                if (!int.TryParse(txtRenewDays.Text.Trim(), out noOfDays) || noOfDays <= 0)
                {
                    ShowAlert(lblErrorMsg[4], "error");
                    return;
                }

                using (DataSet ds = objCommonBO.BookRenewalRequest(
                    "INSERT",
                    memberID,
                    bookIssueId,
                    noOfDays
                ))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);

                        // ❌ Invalid issue / already returned
                        if (msgCode == -1)
                        {
                            ShowAlert(lblErrorMsg[1], "error");
                        }

                        else if (msgCode == 2)
                        {
                            ShowAlert(lblErrorMsg[2], "error");
                        }

                        else if (msgCode == 1)
                        {
                            ShowAlert(lblErrorMsg[3], "success");

                            CurrentGrid = "DUE";  
                            BindSummary();

                            ScriptManager.RegisterStartupScript(
                                Page,
                                Page.GetType(),
                                "HideModal",
                                "$('#renewalModal').modal('hide');",
                                true
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        private void SetPageInfo(int totalRows)
        {
            if (totalRows == 0)
            {
                lblPageInfo.Text = "";
                return;
            }

            int start = (gvBooks.PageIndex * gvBooks.PageSize) + 1;
            int end = Math.Min(start + gvBooks.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start}–{end} of {totalRows} entries";
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvBooks.PageSize = pageSize;
            }
            else
            {
                gvBooks.PageSize = 5; // default fallback
            }

            // 🔹 Reset to first page
            gvBooks.PageIndex = 0;
            BindSummary();

        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objCommonBO != null)
                {
                    objCommonBO.ReleaseResources();
                    objCommonBO = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}