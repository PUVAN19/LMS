using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class IssueDashboard : System.Web.UI.Page

    {
        MasterBO objMasterBO = new MasterBO();
        CommonBO objCommonBO = new CommonBO();
        int AdminUserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            AdminUserID = Convert.ToInt32(Session["AdminUserID"]);
            if (!IsPostBack)
            {
                BindPageSizeDropdown();
                LoadDashboard();
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
                gvBooks.PageSize = Convert.ToInt32(defaultValue);
            }
        }
        private void LoadDashboard()
        {
            try
            {
                using (DataSet ds = objCommonBO.GetBookIssueDashboard("TOTAL_COUNT"))
                {

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        lblTotalBooks.Text    = dt.Rows[0]["TotalBooks"].ToString();
                        lblIssuedBooks.Text   = GetCount("ISSUED_COUNT");
                        lblDueBooks.Text      = GetCount("DUE_COUNT");
                        lblReturnedBooks.Text = GetCount("RETURNED_COUNT");

                        BindGrid("TOTAL_GRID", true);
                    }
                    else
                    {
                       // divGrid.Visible = false;
                        ClearGrid();
                       
                            //gvCategory.DataSource = null;
                            //gvCategory.DataBind();
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
            }
        }

        private string GetCount(string action)
        {
            using (DataSet ds = objCommonBO.GetBookIssueDashboard(action))
            {
                return (ds != null && ds.Tables[0].Rows.Count > 0)
                       ? ds.Tables[0].Rows[0][0].ToString()
                       : "0";

            }
        }
        private void SetGridTitle(string type)
        {
            switch (type.ToUpper())
            {
                case "TOTAL_GRID":
                    lblGridTitle.InnerText = "Total Issued Books Details";
                    break;
                case "ISSUED_GRID":
                    lblGridTitle.InnerText = "Issued Book Details";
                    break;
                case "DUE_GRID":
                    lblGridTitle.InnerText = "Due Book Details";
                    break;
                case "RETURNED_GRID":
                    lblGridTitle.InnerText = "Returned Book Details";
                    break;
                default:
                    lblGridTitle.InnerText = "Book Details";
                    break;
            }
        }
        private void BindGrid(string type, bool resetPageIndex = false)
        {
            rptPager.Visible = false;
            CurrentGridType = type;
            SetGridTitle(type);
            SetCSVHiddenColumns(type);

            if (!IsRefresh)
                SaveGridFilters();

            DataSet ds = objCommonBO.GetBookIssueDashboard(type);

            using (ds)
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    ApplyFiltersUsingViewState(ref ds);

                    DataView dv = ds.Tables[0].DefaultView;

                    // ✅ Bind grid always
                    gvBooks.DataSource = dv;
                    gvBooks.DataBind();

                    if (resetPageIndex)
                        gvBooks.PageIndex = 0;

                    int intRowCount = dv.Count;

                    // ✅ HANDLE EMPTY DATA FIRST
                    if (intRowCount == 0)
                    {
                        rptPager.Visible = false;
                        divPageSize.Visible = false;
                        //lnkDownloadCSV.Visible = false;
                        lblPageInfo.Text = "";
                        ViewState["GridData"] = null;
                        return;
                    }

                    // ✅ DATA EXISTS → SHOW CONTROLS
                    divPageSize.Visible = true;
                    lnkDownloadCSV.Visible = true;

                    // ✅ Page Info (AFTER confirming data exists)
                    SetPageInfo(intRowCount);

                    // ✅ Column visibility
                    gvBooks.Columns[10].Visible = true;
                    gvBooks.Columns[11].Visible = false;
                    gvBooks.Columns[12].Visible = false;

                    switch (type.ToUpper())
                    {
                        case "TOTAL_GRID":
                            lblGridTitle.InnerText = "Total Issued Books Details";
                            gvBooks.Columns[11].Visible = true;
                            break;

                        case "ISSUED_GRID":
                            lblGridTitle.InnerText = "Issued Book Details";
                            gvBooks.Columns[10].Visible = false;
                            break;

                        case "DUE_GRID":
                            lblGridTitle.InnerText = "Due Book Details";
                            gvBooks.Columns[10].Visible = false;
                            gvBooks.Columns[12].Visible = true;
                            break;

                        case "RETURNED_GRID":
                            lblGridTitle.InnerText = "Returned Book Details";
                            gvBooks.Columns[12].Visible = false;
                            break;
                    }

                    // ✅ Save for CSV
                    ViewState["GridData"] = dv.ToTable();

                    // ✅ Pager Logic
                    if (intRowCount > gvBooks.PageSize)
                    {
                        CommonFunction.BuildPager(rptPager, gvBooks.PageCount, gvBooks.PageIndex);
                        rptPager.Visible = true;
                    }
                    else
                    {
                        rptPager.Visible = false;
                    }

                    RestoreGridFilters();
                }
                else
                {
                    // ✅ COMPLETELY NULL DATASET
                    gvBooks.DataSource = new DataTable();
                    gvBooks.DataBind();

                    rptPager.Visible = false;
                    divPageSize.Visible = false;
                    lnkDownloadCSV.Visible = false;
                    lblPageInfo.Text = "";
                    ViewState["GridData"] = null;
                }
            }
        }
     
        protected void gvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Fine")
            {
                int rowIndex = ((GridViewRow)((LinkButton)e.CommandSource).NamingContainer).RowIndex;

                int bookIssueId = Convert.ToInt32(e.CommandArgument);
                hfBookIssueID.Value = bookIssueId.ToString();

                hfBookPrice.Value = gvBooks.DataKeys[rowIndex]["Price"].ToString();

                lblFineBookTitle.Text = gvBooks.DataKeys[rowIndex]["Book Title"].ToString();
                lblFineMemberID.Text  = gvBooks.DataKeys[rowIndex]["MemberID"].ToString();
                lblFineDueDate.Text   = Convert.ToDateTime(gvBooks.DataKeys[rowIndex]["Due Date"]).ToString("dd-MMM-yyyy");
                lblBookPrice.Text = "₹ " + Convert.ToDecimal(gvBooks.DataKeys[rowIndex]["Price"]).ToString("0");

                int overdueDays = Convert.ToInt32(gvBooks.DataKeys[rowIndex]["OverdueDays"]);

                if (overdueDays > 0)
                {
                    lblOverdueDays.Text = "Book is overdue by " + overdueDays + " days";
                }
                else
                {
                    lblOverdueDays.Text = "";
                }

                txtFineAmount.Text = "";

                ScriptManager.RegisterStartupScript(
                      Page,
                      Page.GetType(),
                      "ShowModal",
                      "$(document).ready(function(){ $('#FineModal').modal('show'); });",
                      true);
            }
        }

        protected void btnCollectFine_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFineAmount.Text))
                {
                    ShowToastr("Please enter fine amount.", "warning");
                    return;
                }

                int bookIssueId = Convert.ToInt32(hfBookIssueID.Value);
                decimal fineAmount = Convert.ToDecimal(txtFineAmount.Text);
                decimal bookPrice = Convert.ToDecimal(hfBookPrice.Value);

                if (fineAmount > bookPrice)
                {
                    ShowToastr("Fine amount cannot be greater than Book Price.", "error");
                    return;
                }

                using (DataSet ds = objCommonBO.UpdateFineAmount("UPDATE_FINE", bookIssueId, fineAmount, AdminUserID))
                {
                        ShowToastr("Fine collected successfully.", "success");
                        txtFineAmount.Text = "";
                }

                BindGrid(CurrentGridType, false);
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr("Failed to collect fine.", "error");
            }
        }

        protected void CardTotalBooks_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ClearAllFilters();
            CurrentGridType = "TOTAL_GRID";
            BindGrid(CurrentGridType, true);
            IsRefresh = false;
        }

        protected void CardIssuedBooks_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ClearAllFilters();
            CurrentGridType = "ISSUED_GRID";
            BindGrid(CurrentGridType, true);
            IsRefresh = false;
        }

        protected void CardDueBooks_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ClearAllFilters();
            CurrentGridType = "DUE_GRID";
            BindGrid(CurrentGridType, true);
            IsRefresh = false;
        }

        protected void CardReturnedBooks_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ClearAllFilters();
            CurrentGridType = "RETURNED_GRID";
            BindGrid(CurrentGridType, true);
            IsRefresh = false;
        }


        private string FilterISBN
        {
            get { return ViewState["ISBN"]?.ToString(); }
            set { ViewState["ISBN"] = value; }
        }

        private string FilterBookTitle
        {
            get { return ViewState["BookTitle"]?.ToString(); }
            set { ViewState["BookTitle"] = value; }
        }

        private string FilterMemberID
        {
            get { return ViewState["MemberID"]?.ToString(); }
            set { ViewState["MemberID"] = value; }
        }

        private string FilterMemberType
        {
            get { return ViewState["MemberType"]?.ToString(); }
            set { ViewState["MemberType"] = value; }

        }

        private void SaveGridFilters()
        {
            if (gvBooks.HeaderRow == null) return;

            FilterISBN = (gvBooks.HeaderRow.FindControl("txtFilterISBN") as TextBox)?.Text.Trim();
            FilterBookTitle = (gvBooks.HeaderRow.FindControl("txtFilterBookTitle") as TextBox)?.Text.Trim();
            FilterMemberID = (gvBooks.HeaderRow.FindControl("txtFilterMemberID") as TextBox)?.Text.Trim();
            FilterMemberType = (gvBooks.HeaderRow.FindControl("txtFilterMemberType") as TextBox)?.Text.Trim();
        }
        private void RestoreGridFilters()
        {
            if (gvBooks.HeaderRow == null) return;

            TextBox txtISBN = gvBooks.HeaderRow.FindControl("txtFilterISBN") as TextBox;
            if (txtISBN != null)
                txtISBN.Text = FilterISBN ?? string.Empty;

            TextBox txtTitle = gvBooks.HeaderRow.FindControl("txtFilterBookTitle") as TextBox;
            if (txtTitle != null)
                txtTitle.Text = FilterBookTitle ?? string.Empty;

            TextBox txtMemberID = gvBooks.HeaderRow.FindControl("txtFilterMemberID") as TextBox;
            if (txtMemberID != null)
                txtMemberID.Text = FilterMemberID ?? string.Empty;

            TextBox txtMemberType = gvBooks.HeaderRow.FindControl("txtFilterMemberType") as TextBox;
            if (txtMemberType != null)
                txtMemberType.Text = FilterMemberType ?? string.Empty;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvBooks.PageIndex = 0;
            SaveGridFilters();
            BindGrid(CurrentGridType, false);
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ClearAllFilters();
            gvBooks.PageIndex = 0;
            BindGrid(CurrentGridType, true);
            IsRefresh = false;
        }

        private void ClearAllFilters()
        {
            FilterISBN = string.Empty;
            FilterBookTitle = string.Empty;
            FilterMemberID = string.Empty;
            FilterMemberType = string.Empty;
        }


        private string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Replace("'", "''");
        }

        private void ApplyFiltersUsingViewState(ref DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0) return;
            if (string.IsNullOrWhiteSpace(FilterISBN) &&
                string.IsNullOrWhiteSpace(FilterBookTitle) &&
                string.IsNullOrWhiteSpace(FilterMemberID) &&
                string.IsNullOrWhiteSpace(FilterMemberType))
            {
                ds.Tables[0].DefaultView.RowFilter = string.Empty;
                return;
            }

            string filter = "1=1";

            if (!string.IsNullOrWhiteSpace(FilterISBN))
                filter += $" AND ISBN LIKE '%{Safe(FilterISBN)}%'";

            if (!string.IsNullOrWhiteSpace(FilterBookTitle))
                filter += $" AND [Book Title] LIKE '%{Safe(FilterBookTitle)}%'";

            if (!string.IsNullOrWhiteSpace(FilterMemberID))
                filter += $" AND MemberID LIKE '%{Safe(FilterMemberID)}%'";

            if (!string.IsNullOrWhiteSpace(FilterMemberType))
                filter += $" AND [Member Type] LIKE '%{Safe(FilterMemberType)}%'";

            ds.Tables[0].DefaultView.RowFilter = filter;
        }

        protected string GetStatusCssClass(object dataItem)
        {
            string status = DataBinder.Eval(dataItem, "Status").ToString();
            DateTime dueDate = Convert.ToDateTime(DataBinder.Eval(dataItem, "Due Date"));

            if (status == "Returned")
                return "badge bg-success";

            if (dueDate < DateTime.Today)
                return "badge bg-danger";

            return "badge bg-warning";
        }
        protected void Filter_TextChanged(object sender, EventArgs e)
        {
            gvBooks.PageIndex = 0;
            BindGrid(CurrentGridType);
        }


        private string CurrentGridType
        {
            get { return ViewState["CurrentGridType"]?.ToString() ?? "TOTAL_GRID"; }
            set { ViewState["CurrentGridType"] = value; }
        }

        private bool IsRefresh
        {
            get
            {
                return ViewState["IsRefresh"] != null && (bool)ViewState["IsRefresh"];
            }
            set
            {
                ViewState["IsRefresh"] = value;
            }
        }

        private void SetCSVHiddenColumns(string gridType)
        {
            gridType = gridType.ToUpper();

            if (gridType == "TOTAL_GRID")
            {
                hfRemoveColumnsCSV.Value = "IssueID,BookID";
            }
            else if (gridType == "ISSUED_GRID")
            {
                hfRemoveColumnsCSV.Value = "IssueID,BookID,Returned Date,Status";
            }
            else if (gridType == "DUE_GRID")
            {
                hfRemoveColumnsCSV.Value = "IssueID,BookID,Returned Date,Status";
            }
            else if (gridType == "RETURNED_GRID")
            {
                hfRemoveColumnsCSV.Value = "IssueID,BookID,Status,RenewalCount";
            }
            else
            {
                hfRemoveColumnsCSV.Value = "";
            }
        }

        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable gridData = ViewState["GridData"] as DataTable;

                if (gridData == null || gridData.Rows.Count == 0)
                {
                    ShowToastr("No data available for export.", "warning");
                    return;
                }

                // ✅ CLONE STRUCTURE + DATA
                DataTable csvTable = gridData.Copy();

                DataColumn snoCol = new DataColumn("S.No", typeof(int));
                csvTable.Columns.Add(snoCol);
                snoCol.SetOrdinal(0);

                int index = 1;
                foreach (DataRow row in csvTable.Rows)
                    row["S.No"] = index++;

                if (csvTable.Columns.Contains("ISBN"))
                {
                    foreach (DataRow row in csvTable.Rows)
                        row["ISBN"] = "\t" + row["ISBN"];
                }

                if (!string.IsNullOrWhiteSpace(hfRemoveColumnsCSV.Value))
                {
                    foreach (string col in hfRemoveColumnsCSV.Value.Split(','))
                    {
                        string colName = col.Trim();
                        if (csvTable.Columns.Contains(colName))
                            csvTable.Columns.Remove(colName);
                    }
                }

                StringBuilder sb = CommonFunction.CSVFileGeneration(csvTable, "");

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader(
                    "content-disposition",
                    "attachment;filename=BookDetails.csv"
                );
                Response.ContentType = "text/csv";
                Response.Write(sb.ToString());

                // ✅ SAFER TERMINATION
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                // ✅ CORRECT CLEANUP
                csvTable.Dispose();                 // you own this
                ViewState.Remove("GridData");       // release reference
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                //ShowToastr("Failed to download CSV.", "error");
            }
        }

     

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);
            gvBooks.PageIndex = newIndex;
            BindGrid(CurrentGridType, false);
        }

        private void ClearGrid()
        {
            gvBooks.DataSource = null;
            gvBooks.DataBind();
            lblTotalBooks.Text = "0";
            lblIssuedBooks.Text = "0";
            lblDueBooks.Text = "0";
            lblReturnedBooks.Text = "0";
            lblGridTitle.InnerText = "Books";
           
        }

        private void ShowToastr(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});",
                true
            );
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
            BindGrid(CurrentGridType, true);

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
