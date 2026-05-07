using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class BookAvailability : System.Web.UI.Page
    {
        private string[] lblErrorMsg = new string[20];
        MasterBO objMasterBO = new MasterBO();
        CommonBO objCommonBO = new CommonBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorMessages();
                hfIsFirstTime.Value = "0";
                if (!IsPostBack)
                {
                    BindPageSizeDropdown();
                    hfIsFirstTime.Value = "1";
                    LoadCategories();
                    BindBookAvailability(false);
                }
            }
            catch (Exception ex) 
            {
                MyExceptionLogger.Publish(ex);
               // ShowToastr(lblErrorMsg[5], "error");
            }
        }

        private void ErrorMessages()
        {
            lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "ERRBA0001");   //Please select at least one category
            lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "ERRBA0002");   //Please select a Search By option.
            lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "ERRBA0003");   //Please enter a search value.
            lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "ERRBA0004");   //Search value must contain at least 2 characters.
            lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "ERRBA0005");   //Error occurred while loading book availability.
            lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "ERRBA0006");   //Error occurred while searching books.
            lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "ERRBA0007");   //No records found.
            lblErrorMsg[7] = CommonFunction.GetErrorMessage("", "ERRBA0008");   //No books available for the selected category.
            lblErrorMsg[8] = CommonFunction.GetErrorMessage("", "ERRBA0009");   //No data available to download.
            lblErrorMsg[9] = CommonFunction.GetErrorMessage("", "ERRBA0010");   //Books loaded successfully.
            lblErrorMsg[10]= CommonFunction.GetErrorMessage("", "ERRBA0011");   //Book availability exported successfully.
        }
        private void LoadCategories()
        {
            try
            {
                using (DataSet ds = objMasterBO.CategoryMaster("SELECT"))
                {
                    ddlCategory.Items.Clear();

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ddlCategory.DataSource = ds.Tables[0];
                        ddlCategory.DataTextField = "CategoryName";
                        ddlCategory.DataValueField = "CategoryID";
                        ddlCategory.DataBind();
                    }
                    foreach (ListItem item in ddlCategory.Items)
                    {
                        item.Selected = true;
                    }
                    BindBookAvailability(false);
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        //private void BindPageSizeDropdown()
        //{
        //    DataTable dt = objMasterBO.GetConfigValues("GET_PAGESIZE", "PageSizeOptions");

        //    ddlPageSize.DataSource = dt;
        //    ddlPageSize.DataTextField = "ConfigValue";
        //    ddlPageSize.DataValueField = "ConfigValue";
        //    ddlPageSize.DataBind();

        //    // 🔹 Optional default value
        //    ddlPageSize.SelectedValue = "10";
        //}

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
                gvbookA.PageSize = Convert.ToInt32(defaultValue);
            }
        }

        private void SetPageInfo(int totalRows)
        {
            if (totalRows == 0)
            {
                lblPageInfo.Text = "";
                return;
            }

            int start = (gvbookA.PageIndex * gvbookA.PageSize) + 1;
            int end = Math.Min(start + gvbookA.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start} to {end} of {totalRows} entries";
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvbookA.PageSize = pageSize;
            }
            else
            {
                gvbookA.PageSize = 10; // default fallback
            }

            // 🔹 Reset to first page
            gvbookA.PageIndex = 0;

            BindBookAvailability();

            if (ViewState["BookAvailabilityDS"] != null)
            {
                DataTable dt = (DataTable)ViewState["BookAvailabilityDS"];
                SetPageInfo(dt.Rows.Count);
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            var selectedCategories = ddlCategory.Items
                .Cast<ListItem>()
                .Where(i => i.Selected)
                .ToList();


            if (selectedCategories.Count == 0)
            {
                gvbookA.Visible = false;
                gvbookA.DataSource = null;
                gvbookA.DataBind();

                //lblRecordCount.Text = string.Empty;
                lblTotalBooks.Text = string.Empty;

                pnlGrid.Visible = false;

                // ✅ ADD THIS LINE (CRITICAL FIX)
                ViewState["BookAvailabilityDS"] = null;

                rptPager.Visible = false;

                divPageSize.Visible = false;

                ShowToastr(lblErrorMsg[0], "error");
                return;
            }
            else
            {
                gvbookA.Visible = true;
                rptPager.Visible = true;
            }
                BindBookAvailability(false);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            IsRefresh = true;
            ViewState["ISBN"] = null;
            ViewState["BookTitle"] = null;
            ViewState["Author"] = null;
            ViewState["Year"] = null;
            ViewState["Publisher"] = null;

            gvbookA.PageIndex = 0;

            foreach (ListItem item in ddlCategory.Items)
            {
                item.Selected = true;
            }
            // divPageSize.Visible = true;
            gvbookA.Visible = true;
            BindBookAvailability(false);
            Response.Redirect(Request.RawUrl);
        }

        protected void BindBookAvailability(bool showMessage = true)
        {
            try
            {
                if (!IsPostBack && !showMessage)
                {
                    showMessage = false;
                }

                if (!IsRefresh)
                {
                    SaveGridFilters();
                }

                var selectedCategories = ddlCategory.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Text)
                    .ToList();

                if (showMessage && selectedCategories.Count == 0)
                {
                    ShowToastr(lblErrorMsg[0], "error");
                    return;
                }

                string categoryNames = string.Join(",", selectedCategories);

                string isbn = ViewState["ISBN"]?.ToString();
                string bookTitle = ViewState["BookTitle"]?.ToString();
                string authorName = ViewState["Author"]?.ToString();
                string publisherName = ViewState["Publisher"]?.ToString();

                int? yearPublished = null;
                int y;

                if (int.TryParse(Convert.ToString(ViewState["Year"]), out y))
                {
                    yearPublished = y;
                }

                using (DataSet ds = objCommonBO.GetBookAvailability(
                    categoryNames, isbn, bookTitle, authorName, yearPublished, publisherName))
                {
                    pnlGrid.Visible = true;
                    

                    gvbookA.DataSource = ds?.Tables[0];
                    gvbookA.DataBind();

                    int totalCopiesSum = 0;

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        totalCopiesSum = ds.Tables[0].AsEnumerable()
                            .Sum(row => row.Field<int>("TotalCopies"));
                    }
                    int totalRecords = ds.Tables[0].Rows.Count;
                    SetPageInfo(totalRecords);
                    divPageSize.Visible = totalRecords > 0;
                    int pageSize = gvbookA.PageSize;
                    int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                    if (ds.Tables[0].Rows.Count > gvbookA.PageSize)
                    {
                        CommonFunction.BuildPager(rptPager,totalPages, gvbookA.PageIndex);
                        rptPager.Visible = true;
                    }
                    else
                    {
                        rptPager.Visible = false;
                    }
                    
                    RestoreGridFilters();

                    int recordCount = ds?.Tables[0].Rows.Count ?? 0;
                    lblTotalBooks.Text = "Total No. of Book Copies: " + totalCopiesSum;
                    //lblRecordCount.Text = "No of Records : " + recordCount;

                    ViewState["BookAvailabilityDS"] = recordCount > 0 ? ds.Tables[0] : null;

                    if (!showMessage) return;

                    if (recordCount == 0)
                    {
                        ShowToastr(lblErrorMsg[7], "warning"); // No books available for selected category
                    }
                }
            }
            catch (Exception ex)
            {
                pnlGrid.Visible = false;
                ClearGrid();
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[4], "error");
            }
        }

        private void SaveGridFilters()
        {
            if (gvbookA.HeaderRow == null) return;

            ViewState["ISBN"] =
                (gvbookA.HeaderRow.FindControl("txtFilterISBN") as TextBox)?.Text?.Trim();

            ViewState["BookTitle"] =
                (gvbookA.HeaderRow.FindControl("txtFilterBookTitle") as TextBox)?.Text?.Trim();

            ViewState["Author"] =
                (gvbookA.HeaderRow.FindControl("txtFilterAuthor") as TextBox)?.Text?.Trim();

            ViewState["Year"] =
                (gvbookA.HeaderRow.FindControl("txtFilterYear") as TextBox)?.Text?.Trim();

            ViewState["Publisher"] =
                (gvbookA.HeaderRow.FindControl("txtFilterPublisher") as TextBox)?.Text?.Trim();
        }

        private void RestoreGridFilters()
        {
            if (gvbookA.HeaderRow == null) return;

            (gvbookA.HeaderRow.FindControl("txtFilterISBN") as TextBox).Text =
                ViewState["ISBN"]?.ToString();

            (gvbookA.HeaderRow.FindControl("txtFilterBookTitle") as TextBox).Text =
                ViewState["BookTitle"]?.ToString();

            (gvbookA.HeaderRow.FindControl("txtFilterAuthor") as TextBox).Text =
                ViewState["Author"]?.ToString();

            (gvbookA.HeaderRow.FindControl("txtFilterYear") as TextBox).Text =
                ViewState["Year"]?.ToString();

            (gvbookA.HeaderRow.FindControl("txtFilterPublisher") as TextBox).Text =
                ViewState["Publisher"]?.ToString();
        }

        //protected void btnDownloadCSV_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var selectedCategories = ddlCategory.Items.Cast<ListItem>()
        //            .Where(i => i.Selected)
        //            .Select(i => i.Text)
        //            .ToList();

        //        if (selectedCategories.Count == 0)
        //        {
        //            ShowToastr(lblErrorMsg[0], "error");
        //            return;
        //        }

        //        string categoryNames = string.Join(",", selectedCategories);

        //        string isbn = ViewState["ISBN"]?.ToString();
        //        string bookTitle = ViewState["BookTitle"]?.ToString();
        //        string authorName = ViewState["Author"]?.ToString();
        //        string publisherName = ViewState["Publisher"]?.ToString();

        //        int? yearPublished = null;
        //        int y;
        //        if (int.TryParse(Convert.ToString(ViewState["Year"]), out y))
        //        {
        //            yearPublished = y;
        //        }

        //        using (DataSet ds = objCommonBO.GetBookAvailability(
        //            categoryNames, isbn, bookTitle, authorName, yearPublished, publisherName))
        //        {
        //            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        //            {
        //                ShowToastr(lblErrorMsg[8], "warning");
        //                return;
        //            }

        //            DataTable sourceTable = ds.Tables[0];

        //            if (sourceTable.Columns.Contains("ISBN"))
        //            {
        //                foreach (DataRow row in sourceTable.Rows)
        //                {
        //                    row["ISBN"] = "\t" + row["ISBN"];
        //                }
        //            }

        //            string removeColumns = hfRemoveColumnsCSV.Value;
        //            if (!string.IsNullOrWhiteSpace(removeColumns))
        //            {
        //                foreach (string col in removeColumns.Split(','))
        //                {
        //                    if (sourceTable.Columns.Contains(col))
        //                        sourceTable.Columns.Remove(col);
        //                }
        //            }

        //            StringBuilder sb = CommonFunction.CSVFileGenerationWithoutHeader(sourceTable, "BookAvailability");

        //            Response.Clear();
        //            Response.Buffer = true;
        //            Response.AddHeader("content-disposition", "attachment;filename=BookAvailability.csv");
        //            Response.Charset = "";
        //            Response.ContentType = "text/csv";
        //            Response.Write(sb.ToString());
        //            Response.End();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionLogger.Publish(ex);
        //        ShowToastr("Failed to download CSV.", "error");
        //    }
        //}
        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            DataTable sourceTable = null;   // ✅ declare outside

            try
            {
                var selectedCategories = ddlCategory.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Text)
                    .ToList();

                if (selectedCategories.Count == 0)
                {
                    ShowToastr(lblErrorMsg[0], "error");
                    return;
                }

                string categoryNames = string.Join(",", selectedCategories);

                string isbn = ViewState["ISBN"]?.ToString();
                string bookTitle = ViewState["BookTitle"]?.ToString();
                string authorName = ViewState["Author"]?.ToString();
                string publisherName = ViewState["Publisher"]?.ToString();

                int? yearPublished = null;
                int y;
                if (int.TryParse(Convert.ToString(ViewState["Year"]), out y))
                {
                    yearPublished = y;
                }

                using (DataSet ds = objCommonBO.GetBookAvailability(categoryNames, isbn, bookTitle, authorName, yearPublished, publisherName))
                {
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        ShowToastr(lblErrorMsg[8], "warning");
                        return;
                    }

                    sourceTable = ds.Tables[0]; // ✅ reference

                    if (sourceTable.Columns.Contains("ISBN"))
                    {
                        foreach (DataRow row in sourceTable.Rows)
                        {
                            row["ISBN"] = "\t" + row["ISBN"];
                        }
                    }

                    string removeColumns = hfRemoveColumnsCSV.Value;
                    if (!string.IsNullOrWhiteSpace(removeColumns))
                    {
                        foreach (string col in removeColumns.Split(','))
                        {
                            if (sourceTable.Columns.Contains(col))
                                sourceTable.Columns.Remove(col);
                        }
                    }

                    StringBuilder sb =
                        CommonFunction.CSVFileGenerationWithoutHeader(sourceTable, "BookAvailability");

                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=BookAvailability.csv");
                    Response.Charset = "";
                    Response.ContentType = "text/csv";
                    Response.Write(sb.ToString());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr("Failed to download CSV.", "error");
            }
            //finally
            //{
            //    // ✅ MANUAL DATATABLE CLEANUP
            //    if (sourceTable != null)
            //    {
            //        sourceTable.Clear();          // remove rows
            //        sourceTable.Columns.Clear(); // remove schema
            //        sourceTable = null;          // release reference
            //    }
            //}
        }



        protected void gvbookA_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvbookA.PageIndex = e.NewPageIndex;

            if (ViewState["BookAvailabilityDS"] != null)
            {
                DataTable dt = (DataTable)ViewState["BookAvailabilityDS"];

                gvbookA.DataSource = dt;
                gvbookA.DataBind();

                RestoreGridFilters();

                //lblRecordCount.Text = "No of Records : " + dt.Rows.Count;

                int totalCopiesSum = dt.AsEnumerable().Sum(row => row.Field<int>("TotalCopies"));
                lblTotalBooks.Text = "Total No. of Book Copies: " + totalCopiesSum;
            }
            CommonFunction.BuildPager(rptPager,gvbookA.PageCount, gvbookA.PageIndex);

            // ✅ ALSO update page info
            if (ViewState["BookAvailabilityDS"] != null)
            {
                DataTable dt = (DataTable)ViewState["BookAvailabilityDS"];
                SetPageInfo(dt.Rows.Count);
            }



            //if (ViewState["BookAvailabilityDS"] != null)
            //{
            //    DataSet ds = (DataSet)ViewState["BookAvailabilityDS"];
            //    gvbookA.DataSource = ds.Tables[0];
            //    gvbookA.DataBind();
            //    RestoreGridFilters();
            //    lblRecordCount.Text = "No of Records : " + ds.Tables[0].Rows.Count;
            //}
        }

        protected void gvBookAvailability_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DownloadEbook")
                {
                    string filePath = e.CommandArgument.ToString();

                    if (string.IsNullOrEmpty(filePath))
                    {
                        ShowToastr("File not found.", "error");
                        return;
                    }

                    string fullPath = Server.MapPath(filePath);
                    string fileName = Path.GetFileName(fullPath);

                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.TransmitFile(fullPath);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedCategories = ddlCategory.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .ToList();

                if (selectedCategories.Count == 0)
                {
                    ShowToastr(lblErrorMsg[0], "error");
                    return;
                }

                if (!HasAnyGridFilter())
                {
                    ShowToastr("Please enter at least one filter value", "warning");
                    return;
                }
                TextBox txtISBN = gvbookA.HeaderRow?.FindControl("txtFilterISBN") as TextBox;
                TextBox txtYear = gvbookA.HeaderRow?.FindControl("txtFilterYear") as TextBox;

                string isbn = txtISBN?.Text.Trim();
                string year = txtYear?.Text.Trim();

                if (!string.IsNullOrEmpty(isbn) && !isbn.All(char.IsDigit))
                {
                    ShowToastr("ISBN must contain numbers only.", "warning");
                    return;
                }

                if (!string.IsNullOrEmpty(year))
                {
                    if (!year.All(char.IsDigit))
                    {
                        ShowToastr("Year must contain numbers only.", "warning");
                        return;
                    }

                    if (year.Length != 4)
                    {
                        ShowToastr("Year must be a 4-digit value.", "warning");
                        return;
                    }
                }

                hfIsFirstTime.Value = "0";
                gvbookA.PageIndex = 0;
                BindBookAvailability(false);
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr(lblErrorMsg[5], "error");
            }
        }

        private bool HasAnyGridFilter()
        {
            if (gvbookA.HeaderRow == null)
                return false;

            TextBox txtISBN = gvbookA.HeaderRow.FindControl("txtFilterISBN") as TextBox;
            TextBox txtBookTitle = gvbookA.HeaderRow.FindControl("txtFilterBookTitle") as TextBox;
            TextBox txtAuthor = gvbookA.HeaderRow.FindControl("txtFilterAuthor") as TextBox;
            TextBox txtYear = gvbookA.HeaderRow.FindControl("txtFilterYear") as TextBox;
            TextBox txtPublisher = gvbookA.HeaderRow.FindControl("txtFilterPublisher") as TextBox;

            bool hasText =
                !string.IsNullOrWhiteSpace(txtISBN?.Text) ||
                !string.IsNullOrWhiteSpace(txtBookTitle?.Text) ||
                !string.IsNullOrWhiteSpace(txtAuthor?.Text) ||
                !string.IsNullOrWhiteSpace(txtPublisher?.Text)||
                !string.IsNullOrWhiteSpace(txtYear?.Text);

            return hasText;
        }
        private bool IsRefresh
        {
            get { return ViewState["IsRefresh"] != null && (bool)ViewState["IsRefresh"]; }
            set { ViewState["IsRefresh"] = value; }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            IsRefresh = true;

            ViewState["ISBN"] = null;
            ViewState["BookTitle"] = null;
            ViewState["Author"] = null;
            ViewState["Year"] = null;
            ViewState["Publisher"] = null;

            foreach (ListItem item in ddlCategory.Items)
            {
                item.Selected = true;
            }
            //divPageSize.Visible = true;
            gvbookA.PageIndex = 0;
            BindBookAvailability(false);
            IsRefresh = false;
        }

        private void ClearGrid()
        {
            gvbookA.DataSource = null;
            gvbookA.DataBind();
        }

        private void ShowToastr(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});",
                true
            );
        }
       
      
        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (ViewState["BookAvailabilityDS"] == null)
            {
                gvbookA.DataSource = null;
                gvbookA.DataBind();
                rptPager.Visible = false;
                return;
            }

            int newIndex = Convert.ToInt32(e.CommandArgument);
            gvbookA.PageIndex = newIndex;

            DataTable dt = (DataTable)ViewState["BookAvailabilityDS"];

            gvbookA.DataSource = dt;
            gvbookA.DataBind();

            RestoreGridFilters();

            // ✅ ADD THIS (THIS IS YOUR MISSING PIECE)
            SetPageInfo(dt.Rows.Count);

            CommonFunction.BuildPager(rptPager,gvbookA.PageCount, gvbookA.PageIndex);
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objMasterBO != null & objCommonBO!=null)
                {                    
                    objMasterBO.ReleaseResources();
                    objMasterBO = null;
                    objCommonBO.ReleaseResources();
                    objCommonBO=null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}
