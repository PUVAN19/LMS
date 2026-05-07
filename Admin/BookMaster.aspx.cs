using BLL;
using Library;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace Admin
{
    public partial class BookMaster : System.Web.UI.Page
    {
        MasterBO objMasterBO = new MasterBO();
        private string[] lblErrorMsg = new string[100];
        int intAdminUserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            intAdminUserID = Convert.ToInt32(Session["AdminUserID"]);


            // ✅ DELETE RECEIPT
            if (!string.IsNullOrEmpty(Request.QueryString["deleteReceiptId"]))
            {
                int receiptId;
                if (int.TryParse(Request.QueryString["deleteReceiptId"], out receiptId))
                {
                    objMasterBO.DeleteReceipt(receiptId, intAdminUserID);
                }
            }

            // ✅ RELOAD RECEIPTS
            if (Request["__EVENTTARGET"] == "ReloadReceipts")
            {
                int bookId;

                // ✅ Get from EVENTARGUMENT (not hidden field)
                if (int.TryParse(Request["__EVENTARGUMENT"], out bookId))
                {
                    LoadReceipts(bookId);
                    LoadBookForEdit(bookId);

                    divForm.Visible = true;
                    divBookGrid.Visible = false;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                }
            }

            if (!IsPostBack)
            {
                BindPageSizeDropdown();
                BindCategory();
                BindAuthorList();
                BindBookGrid();

                if (!string.IsNullOrEmpty(Request.QueryString["BookID"]))
                {
                    int bookId;
                    if (int.TryParse(Request.QueryString["BookID"], out bookId))
                    {
                        LoadBookForEdit(bookId);
                        LoadReceipts(bookId); // ✅ ADD THIS
                        btnSave.Visible = false;
                        btnUpdate.Visible = true;
                    }
                }
            }

            LoadErrorMessages();
        }
        private void LoadErrorMessages()
        {
            //lblErrorMsg = new string[50];
            lblErrorMsg[0] = CommonFunction.GetErrorMessage("", "ERRCOM"); //NO records Found
            lblErrorMsg[1] = CommonFunction.GetErrorMessage("", "BKM001"); // ISBN required
            lblErrorMsg[2] = CommonFunction.GetErrorMessage("", "BKM002"); // Invalid ISBN
            lblErrorMsg[3] = CommonFunction.GetErrorMessage("", "BKM003"); // ISBN < 10
            lblErrorMsg[4] = CommonFunction.GetErrorMessage("", "BKM004"); // ISBN > 13
            lblErrorMsg[5] = CommonFunction.GetErrorMessage("", "BKM005"); // Title required
            lblErrorMsg[6] = CommonFunction.GetErrorMessage("", "BKM006"); // Invalid title
            lblErrorMsg[7] = CommonFunction.GetErrorMessage("", "BKM007"); // Category
            lblErrorMsg[8] = CommonFunction.GetErrorMessage("", "BKM008"); // Author
            lblErrorMsg[9] = CommonFunction.GetErrorMessage("", "BKM009"); // Language
            lblErrorMsg[10] = CommonFunction.GetErrorMessage("", "BKM010"); // Publisher required
            lblErrorMsg[11] = CommonFunction.GetErrorMessage("", "BKM011"); // Invalid publisher
            lblErrorMsg[12] = CommonFunction.GetErrorMessage("", "BKM012"); // Year required
            lblErrorMsg[13] = CommonFunction.GetErrorMessage("", "BKM013"); // 4-digit rule
            lblErrorMsg[14] = CommonFunction.GetErrorMessage("", "BKM014"); // Year > now
            lblErrorMsg[15] = CommonFunction.GetErrorMessage("", "BKM015"); // Year < 1900
            lblErrorMsg[16] = CommonFunction.GetErrorMessage("", "BKM016"); // Edition
            lblErrorMsg[17] = CommonFunction.GetErrorMessage("", "BKM017"); // Price
            lblErrorMsg[18] = CommonFunction.GetErrorMessage("", "BKM018"); // Total copies
            lblErrorMsg[19] = CommonFunction.GetErrorMessage("", "BKM019"); // Available copies
            lblErrorMsg[20] = CommonFunction.GetErrorMessage("", "SUSBOOK01");//insert success
            lblErrorMsg[21] = CommonFunction.GetErrorMessage("", "SUSBOOK02");//update success
            lblErrorMsg[22] = CommonFunction.GetErrorMessage("", "SUSBOOK03");//delete success
            lblErrorMsg[23] = CommonFunction.GetErrorMessage("", "WARBOOK01");//ISBN WARNING
            lblErrorMsg[24] = CommonFunction.GetErrorMessage("", "BKM020");//Enter a valid decimal price (up to 2 digits).
            lblErrorMsg[25] = CommonFunction.GetErrorMessage("", "BKM021");//Tax % is required.
            lblErrorMsg[26] = CommonFunction.GetErrorMessage("", "BKM022");//Enter valid Tax % (1–100).
            lblErrorMsg[27] = CommonFunction.GetErrorMessage("", "BKM023");//Invalid receipt file type
            lblErrorMsg[28] = CommonFunction.GetErrorMessage("", "BKM024");//Receipt size must be between 50KB and 2MB
            lblErrorMsg[29] = CommonFunction.GetErrorMessage("", "BKM025");//Invalid ebook file type
            lblErrorMsg[30] = CommonFunction.GetErrorMessage("", "BKM026");//Ebook max size is 10MB
            lblErrorMsg[31] = CommonFunction.GetErrorMessage("", "BKM027");//Failed to save book
            lblErrorMsg[32] = CommonFunction.GetErrorMessage("", "BKM028");//File not found.
            lblErrorMsg[33] = CommonFunction.GetErrorMessage("", "BKM029");//File does not exist on server.
            lblErrorMsg[34] = CommonFunction.GetErrorMessage("", "BKM030");//Invalid Book ID.
            lblErrorMsg[35] = CommonFunction.GetErrorMessage("", "BKM035");//Please select a search option.
            lblErrorMsg[36] = CommonFunction.GetErrorMessage("", "BKM036");//Please enter a search value.
            lblErrorMsg[37] = CommonFunction.GetErrorMessage("", "BKM037");//Category name must start with a letter and contain only valid characters.
            lblErrorMsg[38] = CommonFunction.GetErrorMessage("", "BKM038");//Book title must start with a letter and contain only valid characters.
            lblErrorMsg[39] = CommonFunction.GetErrorMessage("", "BKM039");//No data available for export.
            lblErrorMsg[40] = CommonFunction.GetErrorMessage("", "BKM040");//Please select an Excel file.
            lblErrorMsg[41] = CommonFunction.GetErrorMessage("", "BKM041");//Only .xlsx files are allowed.
            lblErrorMsg[42] = CommonFunction.GetErrorMessage("", "BKM042");//File size exceeds limit
            lblErrorMsg[43] = CommonFunction.GetErrorMessage("", "BKM043");//Uploaded Excel file is empty.
            lblErrorMsg[44] = CommonFunction.GetErrorMessage("", "BKM044");//Invalid Excel format. Please download the sample file.
            lblErrorMsg[45] = CommonFunction.GetErrorMessage("", "BKM045");//Validation failed. Please download the error file.

            lblErrorMsg[46] = CommonFunction.GetErrorMessage("", "BKM046");//Bulk insert failed. Please try again.
            lblErrorMsg[47] = CommonFunction.GetErrorMessage("", "BKM047");//Bulk insert completed successfully.
            lblErrorMsg[48] = CommonFunction.GetErrorMessage("", "BKM031");//book purchase successfullly
            lblErrorMsg[49] = CommonFunction.GetErrorMessage("", "BKM032");//Invalid Excel file. Please Upload Correct file
            lblErrorMsg[50] = CommonFunction.GetErrorMessage("", "BKM048");//All purchases already have receipts. Delete one to upload new.
            lblErrorMsg[51] = CommonFunction.GetErrorMessage("", "BKM049");//This purchase already has a receipt.
            lblErrorMsg[52] = CommonFunction.GetErrorMessage("", "BKM050");//Please upload ZIP file.
            lblErrorMsg[53] = CommonFunction.GetErrorMessage("", "BKM051");//Only ZIP file allowed.
            lblErrorMsg[54] = CommonFunction.GetErrorMessage("", "BKM052");//ZIP must contain a Receipts folder.
            lblErrorMsg[55] = CommonFunction.GetErrorMessage("", "BKM053");//Receipts folder is empty.
            lblErrorMsg[56] = CommonFunction.GetErrorMessage("", "BKM054");//Validation failed. Please download the error file
            lblErrorMsg[57] = CommonFunction.GetErrorMessage("", "BKM055");//Receipts uploaded successfully.
            lblErrorMsg[58] = CommonFunction.GetErrorMessage("", "BKM056");//Error file not found
            lblErrorMsg[59] = CommonFunction.GetErrorMessage("", "BKM057");//Duplicate book (same Title + Publisher + Edition)

        }


        private void BindPageSizeDropdown()
        {
            using (DataTable dt = objMasterBO.GetConfigValues("GET_PAGESIZE", "PageSizeOptions"))
            {
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
                    gvBookMaster.PageSize = Convert.ToInt32(defaultValue);
                }
            }


        }
        private void BindCategory()
        {
            try
            {
                using (DataSet ds = objMasterBO.CategoryMaster("SELECT"))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ddlCategory.DataSource = ds.Tables[0];
                        ddlCategory.DataTextField = "CategoryName";
                        ddlCategory.DataValueField = "CategoryID";
                        ddlCategory.DataBind();
                    }
                    ddlCategory.Items.Insert(0, new ListItem("Select Category", ""));
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);

            }
        }
        private void BindAuthorList()
        {
            try
            {
                using (DataSet ds = objMasterBO.AuthorMaster("SELECT"))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        // ✅ Filter Main Authors
                        DataView dvMain = new DataView(dt);
                        dvMain.RowFilter = "AuthorType = 'Main'";

                        lstMainAuthor.DataSource = dvMain;
                        lstMainAuthor.DataTextField = "AuthorName";
                        lstMainAuthor.DataValueField = "AuthorID";
                        lstMainAuthor.DataBind();

                        // ✅ Filter Co Authors
                        DataView dvCo = new DataView(dt);
                        dvCo.RowFilter = "AuthorType = 'Co-Author'";

                        lstCoAuthor.DataSource = dvCo;
                        lstCoAuthor.DataTextField = "AuthorName";
                        lstCoAuthor.DataValueField = "AuthorID";
                        lstCoAuthor.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        private void BindBookGrid()
        {
            try
            {
                using (DataSet ds = objMasterBO.BookMaster("SELECT"))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {

                        DataTable dt = ds.Tables[0];

                        gvBookMaster.DataSource = dt;
                        gvBookMaster.DataBind();

                        int totalRecords = dt.Rows.Count;
                        int pageSize = gvBookMaster.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                        divBookGrid.Visible = totalRecords > 0;
                        divPageSize.Visible = totalRecords > 0;
                        // ✅ ADD HERE
                        SetPageInfo(totalRecords);

                        // ✅ Pager Logic (ONLY ONCE)
                        if (totalPages > 1)
                        {
                            CommonFunction.BuildPager(rptPager,totalPages, gvBookMaster.PageIndex);
                            rptPager.Visible = true;
                        }
                        else
                        {
                            rptPager.Visible = false;
                        }
                    }
                    else
                    {
                        gvBookMaster.DataSource = null;
                        gvBookMaster.DataBind();

                        divBookGrid.Visible = false;
                        rptPager.Visible = false;

                        // ✅ Optional: clear page info
                        lblPageInfo.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        private List<string> GetSelectedAuthors()
        {
            List<string> list = new List<string>();

            // ✅ Main Authors
            foreach (ListItem item in lstMainAuthor.Items)
            {
                if (item.Selected)
                    list.Add(item.Value);
            }

            // ✅ Co Authors (NOT mandatory)
            foreach (ListItem item in lstCoAuthor.Items)
            {
                if (item.Selected)
                    list.Add(item.Value);
            }

            return list;
        }
        private bool ValidateUserInputs(string title, string isbn, int categoryId, string language, string publisher, string yearText, string edition, string priceText, string totalCopiesText, string authorIdsCsv)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ShowAlert(lblErrorMsg[5], "error");
                txtBookTitle.Focus();
                return false;
            }

            string titlePattern = @"^[\p{L}0-9\s\.,:'&()\-/+#]+$";
            if (!Regex.IsMatch(title, titlePattern))
            {
                ShowAlert(lblErrorMsg[6], "error");
                txtBookTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(isbn))
            {
                ShowAlert(lblErrorMsg[1], "error");
                txtISBN.Focus();
                return false;
            }

            string isbnPattern = @"^(?:\d[\- ]?){9}[\dX]$|^(?:\d[\- ]?){13}$";
            if (!Regex.IsMatch(isbn, isbnPattern))
            {
                ShowAlert(lblErrorMsg[2], "error");
                txtISBN.Focus();
                return false;
            }

            if (isbn.Length < 10 || isbn.Length > 13)
            {
                ShowAlert(lblErrorMsg[3], "error");
                txtISBN.Focus();
                return false;
            }

            if (categoryId == 0)
            {
                ShowAlert(lblErrorMsg[7], "error");
                ddlCategory.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(authorIdsCsv))
            {
                ShowAlert(lblErrorMsg[8], "error");
                return false;
            }

            if (ddlLanguage.SelectedIndex == 0)
            {
                ShowAlert(lblErrorMsg[9], "error");
                ddlLanguage.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(publisher))
            {
                ShowAlert(lblErrorMsg[10], "error");
                txtPublisher.Focus();
                return false;
            }

            string publisherPattern = @"^[A-Za-z0-9 .,&\-]+$";
            if (!Regex.IsMatch(publisher, publisherPattern))
            {
                ShowAlert(lblErrorMsg[11], "error");
                txtPublisher.Focus();
                return false;
            }

            if (!Regex.IsMatch(yearText, @"^\d{4}$"))
            {
                ShowAlert(lblErrorMsg[13], "error");
                txtYearPublished.Focus();
                return false;
            }

            int publishedYear = int.Parse(yearText);
            if (publishedYear > DateTime.Now.Year || publishedYear < 1900)
            {
                ShowAlert(lblErrorMsg[14], "error");
                txtYearPublished.Focus();
                return false;
            }

            if (!CommonFunction.IsValidEdition(edition))
            {
                ShowAlert(lblErrorMsg[16], "error");
                txtEdition.Focus();
                return false;
            }


            decimal price;
            if (!decimal.TryParse(priceText, out price) || price <= 0 || decimal.Round(price, 2) != price)
            {
                ShowAlert(lblErrorMsg[24], "error");
                txtPrice.Focus();
                return false;
            }
            int totalCopies = 0;
            if (!int.TryParse(totalCopiesText, out totalCopies) || totalCopies <= 0)
            {
                ShowAlert(lblErrorMsg[18], "error");
                txtTotalCopies.Focus();
                return false;
            }
            decimal taxPercent = 0;

            if (chkTax.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtTaxPercent.Text))
                {
                    ShowAlert(lblErrorMsg[25], "error");
                    txtTaxPercent.Focus();
                    return false;
                }

                if (!decimal.TryParse(txtTaxPercent.Text, out taxPercent) || taxPercent <= 0 || taxPercent > 100)
                {
                    ShowAlert(lblErrorMsg[26], "error");
                    txtTaxPercent.Focus();
                    return false;
                }
            }

            return true;
        }
        private decimal CalculateTotalPrice(decimal price, int copies)
        {
            return price * copies;
        }
        private decimal CalculateFinalAmount(decimal totalPrice, bool isTaxChecked, decimal taxPercent,
                                                     out decimal taxAmount)
        {
            taxAmount = 0;

            if (isTaxChecked && taxPercent > 0)
            {
                taxAmount = (totalPrice * taxPercent) / 100;
            }

            return totalPrice + taxAmount;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string authorIdsCsv = string.Join(",", GetSelectedAuthors());
            string title = txtBookTitle.Text.Trim();
            string isbn = txtISBN.Text.Trim();
            int categoryId = string.IsNullOrEmpty(ddlCategory.SelectedValue) ? 0 : Convert.ToInt32(ddlCategory.SelectedValue);
            string language = ddlLanguage.SelectedValue;
            string publisher = txtPublisher.Text.Trim();
            string yearText = txtYearPublished.Text.Trim();
            string edition = txtEdition.Text.Trim();
            string priceText = txtPrice.Text.Trim();
            string totalCopiesText = txtTotalCopies.Text.Trim();
            bool isTax = chkTax.Checked;
            string shelfLocation = txtShelfLocation.Text.Trim();
            bool active = chkActive.Checked;

            if (!ValidateUserInputs(title, isbn, categoryId, language, publisher,
                yearText, edition, priceText, totalCopiesText, authorIdsCsv))
            {
                return;
            }
            int publishedYear = int.Parse(yearText);
            decimal price = decimal.Parse(priceText);
            int totalCopies = int.Parse(totalCopiesText);
            string receiptPath = Session["ReceiptPath"]?.ToString() ?? "";
            string ebookPath = Session["EbookPath"]?.ToString() ?? "";
            string UploadedFileName = Session["ReceiptName"]?.ToString() ?? "";
            // Receipt Upload
            if (fureceipt.HasFile)
            {
                string originalFileName = Path.GetFileName(fureceipt.FileName); // ✅ store original name
                string ext = Path.GetExtension(originalFileName).ToLower();

                string[] allowedExt = { ".jpg", ".jpeg", ".png" };

                if (!allowedExt.Contains(ext))
                {
                    ShowAlert(lblErrorMsg[27], "error");
                    return;
                }

                if (fureceipt.PostedFile.ContentLength > 2 * 1024 * 1024)
                {
                    ShowAlert(lblErrorMsg[28], "error");
                    return;
                }

                // ✅ Use ISBN + timestamp
                string fileName = $"Receipt_{isbn}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";

                string folderPath = Server.MapPath("~/Uploads/Receipts/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                fureceipt.SaveAs(Path.Combine(folderPath, fileName));

                // ✅ Save values
                receiptPath = "~/Uploads/Receipts/" + fileName;
                UploadedFileName = originalFileName;

                Session["ReceiptPath"] = "~/Uploads/Receipts/" + fileName;
                Session["ReceiptName"] = originalFileName;
            }

            // Ebook Upload
            if (fuEbook.HasFile)
            {
                string ext = Path.GetExtension(fuEbook.FileName).ToLower();
                string[] allowedExt = { ".pdf", ".ppt", ".pptx" };

                if (!allowedExt.Contains(ext))
                {
                    ShowAlert(lblErrorMsg[29], "error");
                    return;
                }
                if (fuEbook.PostedFile.ContentLength > 10 * 1024 * 1024)
                {
                    ShowAlert(lblErrorMsg[30], "error");
                    return;
                }
                string fileName = "Ebook_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ext;

                string folderPath = Server.MapPath("~/Uploads/Ebooks/");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                fuEbook.SaveAs(Path.Combine(folderPath, fileName));

                ebookPath = "~/Uploads/Ebooks/" + fileName;
                Session["EbookPath"] = "~/Uploads/Ebooks/" + fileName;
            }

            // ✅ ================= FILE UPLOAD END =================

            // ✅ Calculate in backend
            // ✅ Calculate Total Price
            decimal totalPrice = CalculateTotalPrice(price, totalCopies);

            // ✅ Get Tax Percentage
            decimal taxPercent = 0;

            if (chkTax.Checked)
            {
                decimal.TryParse(txtTaxPercent.Text, out taxPercent);
            }

            // ✅ Calculate TaxAmount + FinalAmount (ONLY ONE METHOD CALL)
            decimal taxAmount;
            decimal finalAmount = CalculateFinalAmount(totalPrice, chkTax.Checked, taxPercent, out taxAmount);

            try
            {
                using (DataSet ds = objMasterBO.BookMaster("INSERT", 0, isbn, categoryId, title, language, publisher, publishedYear, edition, price,
               totalCopies, totalPrice, isTax, taxPercent, taxAmount, finalAmount, receiptPath, ebookPath, shelfLocation, active, authorIdsCsv, intAdminUserID,UploadedFileName))
                {
                    int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                    if (msgCode == 1)
                    {
                        ShowAlert(lblErrorMsg[20], "success");
                        ClearFormFields();
                        BindBookGrid();
                        divBookGrid.Visible = true;
                        divForm.Visible = false;
                        btnAddBooks.Visible = true;
                        Session.Remove("ReceiptPath");
                        Session.Remove("EbookPath");
                        Session.Remove("ReceiptName");
                    }
                    if (msgCode == 2)
                    {
                        ShowAlert(lblErrorMsg[23], "error");
                    }
                    if (msgCode == 5)
                    {
                        ShowAlert(lblErrorMsg[59], "error");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);

            }
        }
        protected void gvBookMaster_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DownloadEbook")
                {
                    string filePath = e.CommandArgument.ToString();

                    if (string.IsNullOrEmpty(filePath))
                    {
                        ShowAlert(lblErrorMsg[32], "error");
                        return;
                    }

                    string fullPath = Server.MapPath(filePath);

                    if (!File.Exists(fullPath))
                    {
                        ShowAlert(lblErrorMsg[33], "error");
                        return;
                    }

                    string fileName = Path.GetFileName(fullPath);

                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.TransmitFile(fullPath);
                    Response.End();
                    return; // ✅ IMPORTANT
                }

                // ✅ 2. STOP FOR PAGING
                if (e.CommandName == "Page")
                    return;

                // ✅ 3. HANDLE BOOK-BASED COMMANDS
                int bookId;
                if (!int.TryParse(e.CommandArgument.ToString(), out bookId))
                {
                    ShowAlert(lblErrorMsg[34], "error");
                    return;
                }

                // ✅ 4. VIEW RECEIPTS (Drawer)
                if (e.CommandName == "ViewReceipts")
                {
                    // Get receipts first
                    DataSet ds = objMasterBO.BookMaster("SELECTRECEIPTS", bookId);

                    // ❌ If no receipts → DO NOTHING
                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return;
                    }

                    // ✅ Load and open only if data exists
                    LoadReceipts(bookId);

                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "OpenDrawer", "setTimeout(function(){ openDrawer(); }, 100);", true);

                    return;
                }

                if (e.CommandName == "EditBook")
                {
                    LoadBookForEdit(bookId);
                    LoadReceiptsForEdit(bookId);
                    divForm.Visible = true;
                    divBookGrid.Visible = false;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                }
                else if (e.CommandName == "DeleteBook")
                {
                    using (DataSet ds = objMasterBO.BookMaster("DELETE", bookId))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                            if (msgCode == 1)
                            {
                                ShowAlert(lblErrorMsg[22], "success");
                                BindBookGrid();
                            }
                            else
                            {
                                ShowAlert(lblErrorMsg[0], "error");
                            }
                        }
                    }
                }
                if (e.CommandName == "NewPurchase")
                {
                    // ✅ Store BookID (important)
                    ViewState["PurchaseBookID"] = bookId;
                   
                    ClearPurchaseModal();
                    using (DataSet ds = objMasterBO.BookMaster("SELECTBYID", bookId))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow row = ds.Tables[0].Rows[0];

                            string bookTitle = row["BookTitle"].ToString();
                            string isbn = row["ISBN"].ToString();

                            // ✅ NOW it's valid
                            ViewState["PurchaseISBN"] = isbn;

                            // ✅ Set label
                            lblModalTitle.Text = bookTitle + " (" + isbn + ")";
                        }
                    }
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShowModal", "$(document).ready(function(){ $('#purchaseModal').modal('show'); });"
                        , true);
                }
            }


            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        protected void LoadReceipts(int bookId)
        {
            try
            {
                using (DataSet ds = objMasterBO.BookMaster("SELECTRECEIPTS", bookId))
                {
                    StringBuilder sb = new StringBuilder();

                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        string path = row["ReceiptPath"]?.ToString();
                        string fileName = Path.GetFileName(path);
                        sb.Append($@"
<a href='{ResolveUrl(path)}' download 
   style='display:flex; align-items:center; 
          padding:8px 10px; margin-bottom:6px; 
          border:1px solid #eee; border-radius:6px; 
          background:#fafafa; text-decoration:none; color:#333;' 

   onmouseover=""this.style.background='#D3D3D3'"" 
   onmouseout=""this.style.background='#fafafa'"">

    <span style='font-size:16px; margin-right:8px;'>📄</span>

    <span style='white-space:nowrap; overflow:hidden; text-overflow:ellipsis; flex:1;'>
        {fileName}
    </span>

</a>");
                    }

                    drawerContent.InnerHtml = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
        private void LoadReceiptsForEdit(int bookId)
        {
            try
            {
                using (DataSet ds = objMasterBO.BookMaster("SELECTRECEIPTS", bookId))
                {
                    StringBuilder sb = new StringBuilder();

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            string path = row["ReceiptPath"].ToString();
                            string id = row["ReceiptID"].ToString();
                            string fileName = Path.GetFileName(path);

                            sb.Append($@"
<div class='d-flex justify-content-between align-items-center border rounded px-2 py-2 mb-2 bg-light'>

    <div class='d-flex align-items-center text-truncate'>
        <i class='bi bi-file-earmark-text me-2 text-primary'></i>

        <a href='{ResolveUrl(path)}'
           download='{fileName}'
           class='text-decoration-none text-truncate'
           style='max-width:250px;' title='{fileName}'>
           {fileName}
        </a>
    </div>

    <i class='fa-solid fa-circle-xmark text-danger ms-2'
       style='cursor:pointer; font-size:16px;'
       onclick='deleteReceipt({id}, {bookId})'></i>

</div>");
                        }
                    }
                    else
                    {
                        sb.Append("<span class='text-muted'>No receipts found</span>");
                    }

                    // 👉 IMPORTANT: bind to edit container (NOT drawer)
                    receiptContainer.InnerHtml = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
       

        [System.Web.Services.WebMethod]
        public static int DeleteReceipt(int receiptId)
        {
            try
            {
                MasterBO obj = new MasterBO();
                int adminUserId = Convert.ToInt32(HttpContext.Current.Session["AdminUserID"]);

                return obj.DeleteReceipt(receiptId, adminUserId);
            }
            catch
            {
                return 0;
            }
        }


        protected void Update_Click(object sender, EventArgs e)
        {
            try
            {
                int bookID = Convert.ToInt32(hdnBookID.Value);
                string isbn = txtISBN.Text.Trim();
                string title = txtBookTitle.Text.Trim();
                int categoryId = string.IsNullOrEmpty(ddlCategory.SelectedValue) ? 0 : Convert.ToInt32(ddlCategory.SelectedValue);
                string language = ddlLanguage.SelectedValue;
                string publisher = txtPublisher.Text.Trim();
                string yearText = txtYearPublished.Text.Trim();
                string edition = txtEdition.Text.Trim();
                string priceText = txtPrice.Text.Trim();
                string totalCopiesText = txtTotalCopies.Text.Trim();
                string shelfLocation = txtShelfLocation.Text.Trim();
                bool Active = chkActive.Checked;
                string authorIdsCsv = string.Join(",", GetSelectedAuthors());

                if (!ValidateUserInputs(title, isbn, categoryId, language, publisher,
                    yearText, edition, priceText, totalCopiesText, authorIdsCsv))
                {
                    return;
                }

                int publishedYear = int.Parse(yearText);
                decimal price = decimal.Parse(priceText);
                int totalCopies = int.Parse(totalCopiesText);
                string receiptPath = "";
                string ebookPath = "";
                string UploadedFileName = "";

                if (fureceipt.HasFile)
                {
                    string originalFileName = Path.GetFileName(fureceipt.FileName); // ✅ store original name
                    string ext = Path.GetExtension(originalFileName).ToLower();

                    string[] allowedExt = { ".jpg", ".jpeg", ".png" };

                    if (!allowedExt.Contains(ext))
                    {
                        ShowAlert(lblErrorMsg[27], "error");
                        return;
                    }

                    if (fureceipt.PostedFile.ContentLength > 2 * 1024 * 1024)
                    {
                        ShowAlert(lblErrorMsg[28], "error");
                        return;
                    }

                    // ✅ Use ISBN + timestamp
                    string fileName = $"Receipt_{isbn}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";

                    string folderPath = Server.MapPath("~/Uploads/Receipts/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    fureceipt.SaveAs(Path.Combine(folderPath, fileName));

                    // ✅ Save values
                    receiptPath = "~/Uploads/Receipts/" + fileName;
                    UploadedFileName = originalFileName;
                }

                // Ebook Upload
                if (fuEbook.HasFile)
                {
                    string ext = Path.GetExtension(fuEbook.FileName).ToLower();

                    string[] allowedExt = { ".pdf", ".ppt", ".pptx" };

                    if (!allowedExt.Contains(ext))
                    {
                        ShowAlert(lblErrorMsg[29], "error");
                        return;
                    }

                    if (fuEbook.PostedFile.ContentLength > 10 * 1024 * 1024)
                    {
                        ShowAlert(lblErrorMsg[30], "error");
                        return;
                    }

                    string fileName = "Ebook_" + DateTime.Now.Ticks + ext;
                    string folderPath = Server.MapPath("~/Uploads/Ebooks/");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    fuEbook.SaveAs(Path.Combine(folderPath, fileName));

                    ebookPath = "~/Uploads/Ebooks/" + fileName;
                }

                // ✅ ================= FILE UPLOAD END =================
                decimal totalPriceCalc = CalculateTotalPrice(price, totalCopies);
                bool isTax = chkTax.Checked;
                decimal taxPercent = 0;
                decimal taxAmount = 0;

                // ✅ Get Tax Percentage from correct field
                if (chkTax.Checked)
                {
                    decimal.TryParse(txtTaxPercent.Text, out taxPercent);
                }

                // ✅ Calculate Tax Amount
                if (chkTax.Checked && taxPercent > 0)
                {
                    taxAmount = (totalPriceCalc * taxPercent) / 100;
                }
                else
                {
                    taxAmount = 0;
                    taxPercent = 0;
                }

                // ✅ Final Amount
                decimal finalAmount = totalPriceCalc + taxAmount;
                using (DataSet ds = objMasterBO.BookMaster(
                    "UPDATE", bookID, isbn, categoryId, title, language, publisher, Convert.ToInt32(yearText),
                    edition, Convert.ToDecimal(priceText), Convert.ToInt32(totalCopiesText), totalPriceCalc, isTax, taxPercent, taxAmount, finalAmount, receiptPath, ebookPath, shelfLocation,
                    Active, authorIdsCsv, intAdminUserID,UploadedFileName))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);
                        if (msgCode == 2)
                        {
                            ShowAlert(lblErrorMsg[23], "error");
                        }
                        if (msgCode == 1)
                        {
                            ShowAlert(lblErrorMsg[21], "success");
                            LoadReceiptsForEdit(bookID);
                            ClearFormFields();
                            divBookGrid.Visible = true;
                            divForm.Visible = false;
                            btnAddBooks.Visible = true;
                            BindBookGrid();
                        }
                        if(msgCode == 3)
                        {
                            ShowAlert(lblErrorMsg[50], "error");
                        }
                        if (msgCode == 4)
                        {
                            ShowAlert(lblErrorMsg[51], "error");
                        }
                        if (msgCode == 5)
                        {
                            ShowAlert(lblErrorMsg[59], "error");
                        }
                        

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);

            }
        }
        protected void btnPurchaseSave_Click(object sender, EventArgs e)
        {
            try
            {
                string isbn = ViewState["PurchaseISBN"]?.ToString();
                if (ViewState["PurchaseBookID"] == null)
                {
                    ShowErrorAndKeepModal("Invalid Book");
                    return;
                }

                int bookId = Convert.ToInt32(ViewState["PurchaseBookID"]);

                string priceText = copyPrice.Text.Trim();
                string qtyText = textTotalCopies.Text.Trim();
                bool isTaxIncluded = chkTaxIn.Checked;

                decimal price;
                int quantity;

                // ✅ Price validation
                if (!decimal.TryParse(priceText, out price) || price <= 0)
                {
                    ShowErrorAndKeepModal(lblErrorMsg[24]);
                    copyPrice.Focus();
                    return;
                }

                // ✅ Quantity validation
                if (!int.TryParse(qtyText, out quantity) || quantity <= 0)
                {
                    ShowErrorAndKeepModal(lblErrorMsg[18]);
                    textTotalCopies.Focus();
                    return;
                }

                // ✅ Tax validation
                decimal taxPercent = 0;

                if (isTaxIncluded)
                {
                    if (string.IsNullOrWhiteSpace(txtPercent.Text))
                    {
                        ShowErrorAndKeepModal(lblErrorMsg[25]);
                        txtPercent.Focus();
                        return;
                    }

                    if (!decimal.TryParse(txtPercent.Text, out taxPercent) || taxPercent <= 0 || taxPercent > 100)
                    {
                        ShowErrorAndKeepModal(lblErrorMsg[26]);
                        txtPercent.Focus();
                        return;
                    }
                }

                // ✅ File Upload validation (FIXED)
                string receiptPath = "";
                string UploadedFileName = "";

                if (AdditionalPurchaseReceipt.HasFile)
                {
                    string originalFileName = Path.GetFileName(AdditionalPurchaseReceipt.FileName); // ✅ store original name
                    string ext = Path.GetExtension(originalFileName).ToLower();

                    string[] allowedExt = { ".jpg", ".jpeg", ".png" };

                    if (!allowedExt.Contains(ext))
                    {
                        ShowAlert(lblErrorMsg[27], "error");
                        return;
                    }

                    if (AdditionalPurchaseReceipt.PostedFile.ContentLength > 2 * 1024 * 1024)
                    {
                        ShowAlert(lblErrorMsg[28], "error");
                        return;
                    }

                    // ✅ Use ISBN + timestamp
                    string fileName = $"Receipt_{isbn}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";

                    string folderPath = Server.MapPath("~/Uploads/Receipts/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    AdditionalPurchaseReceipt.SaveAs(Path.Combine(folderPath, fileName));

                    // ✅ Save values
                    receiptPath = "~/Uploads/Receipts/" + fileName;
                    UploadedFileName = originalFileName;
                }


                // ✅ Calculations
                decimal totalPrice = price * quantity;
                decimal taxAmount = (isTaxIncluded && taxPercent > 0) ? (totalPrice * taxPercent) / 100 : 0;
                decimal finalAmount = totalPrice + taxAmount;

                // ✅ Insert
                using (DataSet ds = objMasterBO.BookPurchaseLog_Insert(bookId, price, quantity, isTaxIncluded, taxPercent, taxAmount, finalAmount,
                    receiptPath, intAdminUserID, UploadedFileName
                ))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);

                        if (msgCode == 1)
                        {
                            ShowAlert(lblErrorMsg[48], "success");
                            ClearPurchaseModal();
                            BindBookGrid();
                            ScriptManager.RegisterStartupScript(this, this.GetType(),
                             "CloseModal",
                             @"var modalEl = document.getElementById('purchaseModal');
                              var modal = bootstrap.Modal.getInstance(modalEl);
                              if (modal) modal.hide();",
                             true);
                        }
                        else
                        {
                            ShowErrorAndKeepModal("Failed to save purchase");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                //ShowErrorAndKeepModal(lblErrorMsg[31]);
            }
        }

        private void ClearPurchaseModal()
        {
            copyPrice.Text = "";
            textTotalCopies.Text = "";
            PriceAmount.Text = "";
            txtPercent.Text = "";
            chkTaxIn.Checked = false;

            // ✅ Clear file upload (important)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "clearFile", "$('#FileUpload1').val('');", true);

            // ✅ Clear calculated fields
            ScriptManager.RegisterStartupScript(this, this.GetType(), "clearFields", "$('#txtAmount').val(''); $('#finalAmount').val('');", true);
        }

        private void ShowErrorAndKeepModal(string message)
        {
            ShowAlert(message, "error");

            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "KeepModalOpen",
                @"setTimeout(function() {
            openPurchaseModal();
            calculatePurchaseAmount();
        }, 100);",
                true);
        }
        private DataTable GetBookById(int bookId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DataSet ds = objMasterBO.BookMaster("SELECTBYID", bookId))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
            return dt;
        }

        private void LoadBookForEdit(int bookId)
        {
            var dt = GetBookById(bookId);
            if (dt.Rows.Count == 0) return;

            var dr = dt.Rows[0];
            hdnBookID.Value = dr["BookID"].ToString();
            txtISBN.Text = dr["ISBN"].ToString();
            txtBookTitle.Text = dr["BookTitle"].ToString();
            ddlCategory.SelectedValue = dr["CategoryID"].ToString();
            ddlLanguage.SelectedValue = dr["Language"].ToString();
            txtPublisher.Text = dr["PublisherName"].ToString();
            txtYearPublished.Text = dr["YearPublished"].ToString();
            txtEdition.Text = dr["Edition"].ToString();
            txtPrice.Text = dr["Price"].ToString();
            txtTotalCopies.Text = dr["TotalCopies"].ToString();
            txtShelfLocation.Text = dr["ShelfLocation"].ToString();
            chkActive.Checked = Convert.ToBoolean(dr["Active"]);
            txtTotalPrice.Text = dr["TotalPrice"].ToString();

            // ✅ Tax values
            chkTax.Checked = Convert.ToBoolean(dr["TaxCheck"]);
            txtTaxPercent.Text = dr["TaxPercentage"].ToString();
            txtAmount.Value = dr["TaxAmount"].ToString();
            finalAmount.Value = dr["FinalAmount"].ToString();

            // ✅ Show tax UI if needed
            if (chkTax.Checked)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "fixTaxUI",
                    "setTimeout(function(){ $('#taxBoxContainer').show(); }, 200);", true);
            }

            //✅ Recalculate after UI ready
            ScriptManager.RegisterStartupScript(this, GetType(), "recalc",
                "setTimeout(function(){ calculateAmounts(); }, 300);", true);

            LoadAuthorsForBook(bookId);

        }

        private void LoadAuthorsForBook(int bookId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (DataSet ds = objMasterBO.BookAuthor("SELECTBYBOOK", bookId))
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }

                foreach (ListItem li in lstMainAuthor.Items) li.Selected = false;
                foreach (ListItem li in lstCoAuthor.Items) li.Selected = false;

                foreach (DataRow r in dt.Rows)
                {
                    string aid = r["AuthorID"].ToString();

                    var mainItem = lstMainAuthor.Items.FindByValue(aid);
                    if (mainItem != null) mainItem.Selected = true;

                    var coItem = lstCoAuthor.Items.FindByValue(aid);
                    if (coItem != null) coItem.Selected = true;
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
            txtISBN.Text = "";
            txtBookTitle.Text = "";
            ddlCategory.SelectedIndex = 0;
            foreach (ListItem li in lstMainAuthor.Items) li.Selected = false;
            foreach (ListItem li in lstCoAuthor.Items) li.Selected = false;
            ddlLanguage.SelectedIndex = 0;
            txtPublisher.Text = "";
            txtYearPublished.Text = "";
            txtEdition.Text = "";
            txtPrice.Text = "";
            txtTotalCopies.Text = "";
            txtShelfLocation.Text = "";
            chkActive.Checked = true;
            chkTax.Checked = false;
            txtTaxPercent.Text = "";
            txtTotalPrice.Text = "";
            txtfinalAmount.Value = "";
            txtTaxAmount.Value = "";
           
            hdnBookID.Value = "0";
            receiptContainer.InnerHtml = "";
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            ClearFormFields();
        }

        protected void btnAddBooks_Click(object sender, EventArgs e)
        {
            ClearFormFields();
            divBookGrid.Visible = false;
            divForm.Visible = true;
            btnAddBooks.Visible = false;
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            btnBulkUpload.Visible = true;
            divBulkUpload.Visible = false;
            lnkback.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearFormFields();
            BindBookGrid();
            divForm.Visible = false;

            //divBookGrid.Visible = true;
            btnAddBooks.Visible = true;
            lnkback.Visible = false;
        }
        protected void gvBookMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBookMaster.PageIndex = e.NewPageIndex;

            if (ViewState["SearchBy"] != null && ViewState["SearchValue"] != null)
            {
                BindSearchGrid(); // ✅ KEEP FILTER
            }
            else
            {
                BindBookGrid();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchBy = ddlSearchBy.SelectedValue;
                string searchValue = txtSearchValue.Text.Trim();

                // ✅ 1. Validate dropdown
                if (string.IsNullOrEmpty(searchBy))
                {
                    ShowAlert(lblErrorMsg[35], "warning");
                    return;
                }

                // ✅ 2. Validate input
                if (string.IsNullOrEmpty(searchValue))
                {
                    ShowAlert(lblErrorMsg[36], "error");
                    return;
                }

                // ✅ 3. 🔥 ADD THIS VALIDATION BACK

                if (searchBy == "Category")
                {
                    // Only letters, spaces, basic symbols
                    if (!Regex.IsMatch(searchValue, @"^[A-Za-z][A-Za-z\s\.,:'\-]*$"))
                    {
                        ShowAlert(lblErrorMsg[37], "error");
                        txtSearchValue.Focus();
                        return;
                    }
                }

                if (searchBy == "BookTitle")
                {
                    // Allow letters + numbers
                    if (!Regex.IsMatch(searchValue, @"^[\p{L}0-9\s\.,:'&()\-/+#]+$"))
                    {
                        ShowAlert(lblErrorMsg[38], "error");
                        txtSearchValue.Focus();
                        return;
                    }
                }
                // ✅ 4. Store AFTER validation
                ViewState["SearchBy"] = searchBy;
                ViewState["SearchValue"] = searchValue;
                // ✅ 5. Reset page
                gvBookMaster.PageIndex = 0;
                // ✅ 6. Bind
                BindSearchGrid();
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            ddlSearchBy.SelectedIndex = 0;
            txtSearchValue.Text = "";
            ViewState["SearchBy"] = null;
            ViewState["SearchValue"] = null;
            gvBookMaster.PageIndex = 0;
            // ✅ SHOW AGAIN
            divPageSize.Visible = true;
            BindBookGrid();
        }


        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            DataTable sourceTable = null;
            StringBuilder sb = null;

            try
            {
                DataSet ds;

                // ✅ CHECK IF SEARCH IS APPLIED
                if (ViewState["SearchBy"] != null && ViewState["SearchValue"] != null)
                {
                    string searchBy = ViewState["SearchBy"].ToString();
                    string searchValue = ViewState["SearchValue"].ToString();

                    // ✅ GET FILTERED DATA
                    ds = objMasterBO.BookMaster("SEARCH", 0, "", 0, "", "", "", 0, "", 0, 0, 0, true, 0, 0, 0, "", "", "", true, "", intAdminUserID,"", searchBy, searchValue);
                }
                else
                {
                    // ✅ NO SEARCH → GET ALL DATA
                    ds = objMasterBO.BookMaster("SELECT");
                }

                // ✅ VALIDATION
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    ShowAlert(lblErrorMsg[39], "warning");
                    return;
                }

                // ✅ COPY TABLE
                sourceTable = ds.Tables[0].Copy();

                // ✅ ISBN FIX
                if (sourceTable.Columns.Contains("ISBN"))
                {
                    foreach (DataRow row in sourceTable.Rows)
                    {
                        row["ISBN"] = "\t" + row["ISBN"];
                    }
                }

                // ✅ REMOVE HIDDEN COLUMNS
                string removeColumns = hfRemoveColumnsCSV.Value;
                if (!string.IsNullOrWhiteSpace(removeColumns))
                {
                    foreach (string col in removeColumns.Split(','))
                    {
                        string colName = col.Trim();
                        if (sourceTable.Columns.Contains(colName))
                            sourceTable.Columns.Remove(colName);
                    }
                }

                // ✅ GENERATE CSV
                sb = CommonFunction.CSVFileGenerationWithoutHeader(sourceTable, "BookMaster");

                Response.Clear();
                Response.Buffer = true;

                // ✅ OPTIONAL: DIFFERENT NAME FOR FILTERED FILE
                string fileName = "BookMaster";
                if (ViewState["SearchBy"] != null)
                {
                    fileName += "_Filtered";
                }

                Response.AddHeader("content-disposition", $"attachment;filename={fileName}.csv");
                Response.ContentType = "text/csv";
                Response.Write(sb.ToString());

                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
            finally
            {
                sourceTable = null;
                sb = null;
            }
        }
   

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            gvBookMaster.PageIndex = newIndex;

            if (ViewState["SearchBy"] != null && ViewState["SearchValue"] != null)
            {
                BindSearchGrid(); // ✅ KEEP SEARCH RESULT
            }
            else
            {
                BindBookGrid(); // ✅ NORMAL GRID
            }
        }
        private void BindSearchGrid()
        {
            if (ViewState["SearchBy"] == null || ViewState["SearchValue"] == null)
                return;

            string searchBy = ViewState["SearchBy"].ToString();
            string searchValue = ViewState["SearchValue"].ToString();

            using (DataSet ds = objMasterBO.BookMaster("SEARCH", 0, "", 0, "", "", "", 0, "", 0, 0, 0, true, 0, 0, 0, "", "", "", true, "", intAdminUserID,"",
                searchBy, searchValue,0))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    gvBookMaster.DataSource = ds.Tables[0];
                    gvBookMaster.DataBind();

                    int totalRows = ds.Tables[0].Rows.Count;

                    // ✅ Show page info
                    lblPageInfo.Visible = true;
                    divPageSize.Visible = true;
                    SetPageInfo(totalRows);

                    int pageSize = gvBookMaster.PageSize;
                    int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                    rptPager.Visible = totalPages > 1;

                    if (totalPages > 1)
                    {
                        CommonFunction.BuildPager(rptPager,totalPages, gvBookMaster.PageIndex);
                    }
                }
                else
                {
                    gvBookMaster.DataSource = null;
                    gvBookMaster.DataBind();

                    // ❌ Hide when no data
                    lblPageInfo.Visible = false;
                    lblPageInfo.Text = "";

                    rptPager.Visible = false;
                    divPageSize.Visible = false;
                    ShowAlert(lblErrorMsg[0], "warning");
                }
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize;

            // 🔹 Safe conversion (avoids runtime error)
            if (int.TryParse(ddlPageSize.SelectedValue, out pageSize) && pageSize > 0)
            {
                gvBookMaster.PageSize = pageSize;
            }


            // 🔹 Reset to first page
            gvBookMaster.PageIndex = 0;

            // 🔹 Rebind data
            if (ViewState["SearchBy"] != null && ViewState["SearchValue"] != null)
            {
                BindSearchGrid();
            }
            else
            {
                BindBookGrid();
            }
        }
        private void SetPageInfo(int totalRows)
        {
            if (totalRows == 0)
            {
                lblPageInfo.Visible = false;
                return;
            }
            else
            {
                lblPageInfo.Visible = true;
            }

            int start = (gvBookMaster.PageIndex * gvBookMaster.PageSize) + 1;
            int end = Math.Min(start + gvBookMaster.PageSize - 1, totalRows);

            lblPageInfo.Text = $"Showing {start}–{end} of {totalRows} entries";
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

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            ClearFormFields();
            divBookGrid.Visible = false;
            divForm.Visible = false;
            divBulkUpload.Visible = true;
            btnAddBooks.Visible = true;
            btnBulkUpload.Visible = false;
            lnkback.Visible = true;
        }
      
        private Dictionary<string, int> GetCategoryDictionary()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            DataSet ds = objMasterBO.CategoryMaster("SELECT");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    dict[row["CategoryName"].ToString()] =
                        Convert.ToInt32(row["CategoryID"]);
                }
            }

            return dict;
        }
        private Dictionary<string, int> GetAuthorDictionary()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            DataSet ds = objMasterBO.AuthorMaster("SELECT");

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    dict[row["AuthorName"].ToString()] =
                        Convert.ToInt32(row["AuthorID"]);
                }
            }

            return dict;
        }

        private List<string> ValidateExcelRow(int rowNumber,string title,string isbn,int categoryId,
    string language,string publisher,string yearText,string edition,string priceText,string totalCopiesText,
    string taxCheckText,string taxPercentText,string authorIdsCsv)
        {
            List<string> errors = new List<string>();

            // Trim values
            title = title?.Trim();
            isbn = isbn?.Trim();
            language = language?.Trim();
            publisher = publisher?.Trim();
            yearText = yearText?.Trim();
            edition = edition?.Trim();
            priceText = priceText?.Trim();
            totalCopiesText = totalCopiesText?.Trim();
            taxPercentText = taxPercentText?.Trim();
            authorIdsCsv = authorIdsCsv?.Trim();

            // =========================
            // Required Field Validation
            // =========================

            if (string.IsNullOrWhiteSpace(title))
                errors.Add($"Row {rowNumber}: Title is required");

            if (string.IsNullOrWhiteSpace(isbn))
                errors.Add($"Row {rowNumber}: ISBN is required");

            if (categoryId == 0)
                errors.Add($"Row {rowNumber}: Category is required");

            if (string.IsNullOrWhiteSpace(authorIdsCsv))
                errors.Add($"Row {rowNumber}: Author is required");

            if (string.IsNullOrWhiteSpace(publisher))
                errors.Add($"Row {rowNumber}: Publisher is required");

            if (string.IsNullOrWhiteSpace(yearText))
                errors.Add($"Row {rowNumber}: Year is required");

            if (string.IsNullOrWhiteSpace(edition))
                errors.Add($"Row {rowNumber}: Edition is required");

            if (string.IsNullOrWhiteSpace(priceText))
                errors.Add($"Row {rowNumber}: Price is required");

            if (string.IsNullOrWhiteSpace(totalCopiesText))
                errors.Add($"Row {rowNumber}: Total Copies is required");

            // =========================
            // Max Length Validation
            // =========================

            ValidateMaxLength(errors, rowNumber, "ISBN", isbn, 20);
            ValidateMaxLength(errors, rowNumber, "Title", title, 150);
            ValidateMaxLength(errors, rowNumber, "Language", language, 50);
            ValidateMaxLength(errors, rowNumber, "Publisher", publisher, 100);
            ValidateMaxLength(errors, rowNumber, "Edition", edition, 20);
            // =========================
            // ISBN Validation
            // =========================

            if (!string.IsNullOrWhiteSpace(isbn) &&
                !Regex.IsMatch(isbn, @"^\d{13}$"))
            {
                errors.Add($"Row {rowNumber}: ISBN must be exactly 13 digits");
            }

            // =========================
            // Year Validation
            // =========================

            if (!string.IsNullOrWhiteSpace(yearText))
            {
                int year;
                if (!int.TryParse(yearText, out year))
                {
                    errors.Add($"Row {rowNumber}: Year must be numeric");
                }
                else if (year < 1900 || year > DateTime.Now.Year)
                {
                    errors.Add($"Row {rowNumber}: Year must be between 1900 and {DateTime.Now.Year}");
                }
            }

            // =========================
            // Edition Validation
            // =========================

            if (!string.IsNullOrWhiteSpace(edition) &&
                !CommonFunction.IsValidEdition(edition))
            {
                errors.Add($"Row {rowNumber}: Edition must be like 1st, 2nd, 3rd, 4th");
            }

            // =========================
            // Price Validation
            // =========================

            if (!string.IsNullOrWhiteSpace(priceText))
            {
                decimal price;

                if (!decimal.TryParse(priceText, out price))
                {
                    errors.Add($"Row {rowNumber}: Price must be numeric");
                }
                else if (price <= 0)
                {
                    errors.Add($"Row {rowNumber}: Price must be greater than 0");
                }
                else if (price > 99999999.99m)
                {
                    errors.Add($"Row {rowNumber}: Price exceeds allowed limit");
                }
            }

            // =========================
            // Total Copies Validation
            // =========================

            if (!string.IsNullOrWhiteSpace(totalCopiesText))
            {
                int copies;

                if (!int.TryParse(totalCopiesText, out copies))
                {
                    errors.Add($"Row {rowNumber}: Total Copies must be numeric");
                }
                else if (copies <= 0)
                {
                    errors.Add($"Row {rowNumber}: Total Copies must be greater than 0");
                }
            }

            // =========================
            // Tax Validation
            // =========================

            bool isTax = taxCheckText == "1";

            if (isTax)
            {
                if (string.IsNullOrWhiteSpace(taxPercentText))
                {
                    errors.Add($"Row {rowNumber}: Tax Percent is required");
                }
                else
                {
                    decimal taxPercent;

                    if (!decimal.TryParse(taxPercentText, out taxPercent))
                    {
                        errors.Add($"Row {rowNumber}: Tax Percent must be numeric");
                    }
                    else if (taxPercent <= 0 || taxPercent > 100)
                    {
                        errors.Add($"Row {rowNumber}: Tax Percent must be between 1 and 100");
                    }
                }
            }

            return errors;
        }
        private void ValidateMaxLength(
    List<string> errors,
    int rowNumber,
    string fieldName,
    string value,
    int maxLength)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength)
            {
                errors.Add("Row " + rowNumber + ": " + fieldName +
                           " cannot exceed " + maxLength + " characters");
            }
        }
        //protected void btnUploadExcel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // 🔴 FILE VALIDATION
        //        if (!fuExcel.HasFile)
        //        {
        //            ShowAlert(lblErrorMsg[40], "error");
        //            return;
        //        }

        //        string ext = Path.GetExtension(fuExcel.FileName).ToLower();
        //        if (ext != ".xlsx")
        //        {
        //            ShowAlert(lblErrorMsg[41], "error");
        //            return;
        //        }

        //        if (fuExcel.PostedFile.ContentLength > 5 * 1024 * 1024)
        //        {
        //            ShowAlert(lblErrorMsg[42], "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 PATH SETUP
        //        //-----------------------------------
        //        string uploadPath = Server.MapPath("~/BulkUpload/Upload/");
        //        string errorPath = Server.MapPath("~/BulkUpload/Error/");

        //       // ClearFolder(uploadPath);
        //        ClearFolder(errorPath);

        //        string fileName = "Books_" + DateTime.Now.Ticks + ".xlsx";
        //        string fullPath = Path.Combine(uploadPath, fileName);

        //        try
        //        {
        //            fuExcel.SaveAs(fullPath);
        //        }
        //        catch (Exception ex)
        //        {
        //            MyExceptionLogger.Publish(ex);
        //            ShowAlert("File upload failed.", "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 READ EXCEL
        //        //-----------------------------------
        //        DataTable excelDt = ReadExcel(fullPath);

        //        if (excelDt.Rows.Count == 0)
        //        {
        //            ShowAlert(lblErrorMsg[43], "error");
        //            return;
        //        }

        //        if (!ValidateHeaders(excelDt))
        //        {
        //            ShowAlert(lblErrorMsg[44], "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 PREPARE
        //        //-----------------------------------
        //        List<string> errorList = new List<string>();
        //        int rowNumber = 2;

        //        var categoryDict = GetCategoryDictionary();
        //        var authorDict = GetAuthorDictionary();
        //        HashSet<string> validLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //        foreach (ListItem item in ddlLanguage.Items)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.Value))
        //            {
        //                validLanguages.Add(item.Value);
        //            }
        //        }
        //        DataTable bulkDt = CreateBulkTable();

        //        //-----------------------------------
        //        // 🔴 DUPLICATE CHECK (EXCEL)
        //        //-----------------------------------
        //        var isbnSet = new HashSet<string>();
        //        var bookSet = new HashSet<string>(); // Title + Publisher + Edition

        //        //-----------------------------------
        //        // 🔴 LOOP
        //        //-----------------------------------
        //        foreach (DataRow row in excelDt.Rows)
        //        {
        //            // Skip empty rows
        //            if (row.ItemArray.All(x => string.IsNullOrWhiteSpace(x?.ToString())))
        //            {
        //                rowNumber++;
        //                continue;
        //            }

        //            try
        //            {
        //                string isbn = row["ISBN"].ToString().Trim();
        //                string title = row["BookTitle"].ToString().Trim();
        //                string categoryName = row["CategoryName"].ToString().Trim();
        //                string mainAuthor = row["MainAuthor"].ToString().Trim();
        //                string coAuthor = row["CoAuthor"].ToString().Trim();
        //                string language = row["Language"].ToString().Trim();
        //                string publisher = row["Publisher"].ToString().Trim();
        //                string yearText = row["YearPublished"].ToString().Trim();
        //                string edition = row["Edition"].ToString().Trim();
        //                string priceText = row["Price"].ToString().Trim();
        //                string totalCopiesText = row["TotalCopies"].ToString().Trim();
        //                string taxCheckText = row["TaxCheck"].ToString().Trim();
        //                string taxPercentText = row["TaxPercent"].ToString().Trim();
        //                string shelf = row["ShelfLocation"].ToString().Trim();
        //                string activeText = row["ActiveStatus"].ToString().Trim();

        //                //-----------------------------------
        //                // 🔴 CATEGORY VALIDATION
        //                //-----------------------------------
        //                int categoryId = 0;
        //                if (!categoryDict.TryGetValue(categoryName, out categoryId))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Category '{categoryName}'");
        //                    rowNumber++;
        //                    continue;
        //                }

        //                //-----------------------------------
        //                // 🔴 AUTHOR VALIDATION
        //                //-----------------------------------
        //                List<int> authorIds = new List<int>();

        //                if (!authorDict.ContainsKey(mainAuthor))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Main Author '{mainAuthor}'");
        //                    rowNumber++;
        //                    continue;
        //                }
        //                else
        //                {
        //                    authorIds.Add(authorDict[mainAuthor]);
        //                }

        //                if (!string.IsNullOrEmpty(coAuthor))
        //                {
        //                    foreach (var a in coAuthor.Split(','))
        //                    {
        //                        string name = a.Trim();
        //                        if (!authorDict.ContainsKey(name))
        //                        {
        //                            errorList.Add($"Row {rowNumber}: Invalid Co-Author '{name}'");
        //                            rowNumber++;
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            authorIds.Add(authorDict[name]);
        //                        }
        //                    }
        //                }
        //                // 🔴 DUPLICATE AUTHOR CHECK
        //                if (authorIds.Count != authorIds.Distinct().Count())
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate authors not allowed");
        //                    rowNumber++;
        //                    continue;
        //                }
        //                // Remove duplicate authors
        //                authorIds = authorIds.Distinct().ToList();
        //                string authorIdsCsv = string.Join(",", authorIds);
        //                language = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(language.ToLower());
        //                if (string.IsNullOrWhiteSpace(language))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Language is required");
        //                    rowNumber++;
        //                    continue;
        //                }

        //                if (!validLanguages.Contains(language))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Language '{language}'");
        //                    rowNumber++;
        //                    continue;
        //                }
        //                // 🔴 NORMALIZE (convert to proper format)

        //                //-----------------------------------
        //                // 🔴 COMMON VALIDATION (REUSABLE)
        //                //-----------------------------------
        //                var rowErrors = ValidateExcelRow(
        //                    rowNumber,
        //                    title, isbn, categoryId,
        //                    language, publisher,
        //                    yearText, edition,
        //                    priceText, totalCopiesText,
        //                    taxCheckText, taxPercentText,
        //                    authorIdsCsv
        //                );

        //                if (rowErrors.Count > 0)
        //                {
        //                    errorList.AddRange(rowErrors);
        //                    rowNumber++;
        //                    continue;
        //                }

        //                //-----------------------------------
        //                // 🔴 DUPLICATE CHECK (EXCEL)
        //                //-----------------------------------
        //                if (!isbnSet.Add(isbn))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate ISBN in Excel");
        //                    rowNumber++;
        //                    continue;
        //                }

        //                var sortedAuthors = authorIds.Distinct().OrderBy(x => x);
        //                authorIdsCsv = string.Join(",", sortedAuthors); // ✅ correct

        //                string bookKey = title.ToLower().Trim() + "_"
        //                               + edition.ToLower().Trim() + "_"
        //                               + authorIdsCsv;

        //                if (!bookSet.Add(bookKey))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate book (Title + Edition + Authors) in Excel");
        //                    rowNumber++;
        //                    continue;
        //                }

        //                //-----------------------------------
        //                // 🔴 SAFE PARSE (AFTER VALIDATION)
        //                //-----------------------------------
        //                int year = int.Parse(yearText);
        //                decimal price = decimal.Parse(priceText);
        //                int copies = int.Parse(totalCopiesText);
        //                bool isTax = taxCheckText == "1";
        //                decimal taxPercent = string.IsNullOrEmpty(taxPercentText) ? 0 : decimal.Parse(taxPercentText);

        //                //-----------------------------------
        //                // 🔴 CALCULATE
        //                //-----------------------------------
        //                decimal totalPrice = price * copies;
        //                decimal taxAmount = isTax ? (totalPrice * taxPercent / 100) : 0;
        //                decimal finalAmount = totalPrice + taxAmount;

        //                //-----------------------------------
        //                // 🔴 ADD TO BULK TABLE
        //                //-----------------------------------
        //                bulkDt.Rows.Add(rowNumber, isbn, title, categoryId, language, publisher, year, edition,
        //                    price, copies, totalPrice, isTax, taxPercent, taxAmount, finalAmount,
        //                    shelf, activeText == "1", authorIdsCsv);

        //                rowNumber++;
        //            }
        //            catch (Exception ex)
        //            {
        //                MyExceptionLogger.Publish(ex);
        //            }
        //        }

        //        //-----------------------------------
        //        // 🔴 STOP IF ERRORS
        //        //-----------------------------------
        //        if (errorList.Count > 0)
        //        {
        //            string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
        //            File.WriteAllLines(errorFile, errorList);

        //            ViewState["ErrorFilePath"] = errorFile;
        //            btnDownloadError.Visible = true;

        //            ShowAlert(lblErrorMsg[45], "error");
        //            return;
        //        }
        //        if (bulkDt.Rows.Count == 0)
        //        {
        //            ShowAlert("No valid data to insert.", "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 DB INSERT
        //        //-----------------------------------

        //            try
        //            {
        //                using (DataTable dbErrors = objMasterBO.BulkInsertBooks(bulkDt, intAdminUserID))
        //                {
        //                    bool hasDbError = false;

        //                    if (dbErrors != null && dbErrors.Rows.Count > 0)
        //                    {
        //                        foreach (DataRow dr in dbErrors.Rows)
        //                        {
        //                            string msg = dr["ErrorMessage"]?.ToString();

        //                            if (!string.IsNullOrWhiteSpace(msg))
        //                            {
        //                                hasDbError = true;
        //                                errorList.Add($"Row {dr["RowNumber"]}: {msg}");
        //                            }
        //                        }
        //                    }

        //                    if (hasDbError)
        //                    {
        //                        string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
        //                        File.WriteAllLines(errorFile, errorList);

        //                        ViewState["ErrorFilePath"] = errorFile;
        //                        btnDownloadError.Visible = true;

        //                        ShowAlert("Some records failed validation.", "error");
        //                        return;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // 🔴 SQL ERROR → LOG ONLY
        //                MyExceptionLogger.Publish(ex);

        //                ShowAlert("Database error occurred. Please contact admin.", "error");
        //                return;
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionLogger.Publish(ex);
        //    }
        //}
        //protected void btnUploadExcel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //-----------------------------------
        //        // 🔴 FILE VALIDATION
        //        //-----------------------------------
        //        if (!fuExcel.HasFile)
        //        {
        //            ShowAlert(lblErrorMsg[40], "error");
        //            return;
        //        }

        //        string ext = Path.GetExtension(fuExcel.FileName).ToLower();
        //        if (ext != ".xlsx")
        //        {
        //            ShowAlert(lblErrorMsg[41], "error");
        //            return;
        //        }

        //        if (fuExcel.PostedFile.ContentLength > 5 * 1024 * 1024)
        //        {
        //            ShowAlert(lblErrorMsg[42], "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 PATH SETUP
        //        //-----------------------------------
        //        string uploadPath = Server.MapPath("~/BulkUpload/Upload/");
        //        string errorPath = Server.MapPath("~/BulkUpload/Error/");

        //        // ❌ DO NOT DELETE uploadPath (prevents file lock issue)
        //        // ClearFolder(uploadPath);

        //        ClearFolder(errorPath); // ✅ safe

        //        string fileName = "Books_" + DateTime.Now.Ticks + ".xlsx";
        //        string fullPath = Path.Combine(uploadPath, fileName);

        //        //-----------------------------------
        //        // 🔴 SAVE FILE
        //        //-----------------------------------
        //        try
        //        {
        //            fuExcel.SaveAs(fullPath);
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowAlert(ex.Message, "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 READ EXCEL
        //        //-----------------------------------
        //        DataTable excelDt;
        //        try
        //        {
        //            excelDt = ReadExcel(fullPath);
        //        }
        //        catch (Exception ex)
        //        {

        //            ShowAlert(ex.Message, "error");
        //            return;
        //        }

        //        if (excelDt.Rows.Count == 0)
        //        {
        //            ShowAlert(lblErrorMsg[43], "error");
        //            return;
        //        }

        //        if (!ValidateHeaders(excelDt))
        //        {
        //            ShowAlert(lblErrorMsg[44], "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // 🔴 PREPARE
        //        //-----------------------------------
        //        List<string> errorList = new List<string>();
        //        int rowNumber = 2;

        //        var categoryDict = GetCategoryDictionary();
        //        var authorDict = GetAuthorDictionary();

        //        HashSet<string> validLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //        foreach (ListItem item in ddlLanguage.Items)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.Value))
        //                validLanguages.Add(item.Value);
        //        }

        //        DataTable bulkDt = CreateBulkTable();

        //        var isbnSet = new HashSet<string>();
        //        var bookSet = new HashSet<string>();

        //        //-----------------------------------
        //        // 🔴 LOOP
        //        //-----------------------------------
        //        foreach (DataRow row in excelDt.Rows)
        //        {
        //            if (row.ItemArray.All(x => string.IsNullOrWhiteSpace(x?.ToString())))
        //            {
        //                rowNumber++;
        //                continue;
        //            }

        //            try
        //            {
        //                string isbn = row["ISBN"].ToString().Trim();
        //                string title = row["BookTitle"].ToString().Trim();
        //                string categoryName = row["CategoryName"].ToString().Trim();
        //                string mainAuthor = row["MainAuthor"].ToString().Trim();
        //                string coAuthor = row["CoAuthor"].ToString().Trim();
        //                string language = row["Language"].ToString().Trim();
        //                string publisher = row["Publisher"].ToString().Trim();
        //                string yearText = row["YearPublished"].ToString().Trim();
        //                string edition = row["Edition"].ToString().Trim();
        //                string priceText = row["Price"].ToString().Trim();
        //                string totalCopiesText = row["TotalCopies"].ToString().Trim();
        //                string taxCheckText = row["TaxCheck"].ToString().Trim();
        //                string taxPercentText = row["TaxPercent"].ToString().Trim();
        //                string shelf = row["ShelfLocation"].ToString().Trim();
        //                string activeText = row["ActiveStatus"].ToString().Trim();

        //                //-----------------------------------
        //                // VALIDATIONS
        //                //-----------------------------------
        //                int categoryId = 0;
        //                if (!categoryDict.TryGetValue(categoryName, out categoryId))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Category '{categoryName}'");
        //                    rowNumber++; continue;
        //                }


        //                List<int> authorIds = new List<int>();

        //                if (!authorDict.ContainsKey(mainAuthor))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Main Author '{mainAuthor}'");
        //                    rowNumber++; continue;
        //                }
        //                authorIds.Add(authorDict[mainAuthor]);

        //                if (!string.IsNullOrEmpty(coAuthor))
        //                {
        //                    foreach (var a in coAuthor.Split(','))
        //                    {
        //                        string name = a.Trim();
        //                        if (!authorDict.ContainsKey(name))
        //                        {
        //                            errorList.Add($"Row {rowNumber}: Invalid Co-Author '{name}'");
        //                            rowNumber++; continue;
        //                        }
        //                        authorIds.Add(authorDict[name]);
        //                    }
        //                }

        //                if (authorIds.Count != authorIds.Distinct().Count())
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate authors not allowed");
        //                    rowNumber++; continue;
        //                }

        //                authorIds = authorIds.Distinct().ToList();
        //                string authorIdsCsv = string.Join(",", authorIds);

        //                language = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(language.ToLower());

        //                if (!validLanguages.Contains(language))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Invalid Language '{language}'");
        //                    rowNumber++; continue;
        //                }

        //                var rowErrors = ValidateExcelRow(
        //                    rowNumber, title, isbn, categoryId,
        //                    language, publisher, yearText, edition,
        //                    priceText, totalCopiesText, taxCheckText, taxPercentText, authorIdsCsv
        //                );

        //                if (rowErrors.Count > 0)
        //                {
        //                    errorList.AddRange(rowErrors);
        //                    rowNumber++; continue;
        //                }

        //                if (!isbnSet.Add(isbn))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate ISBN in Excel");
        //                    rowNumber++; continue;
        //                }

        //                string bookKey = title.ToLower().Trim() + "_" + edition.ToLower().Trim() + "_" + authorIdsCsv;
        //                if (!bookSet.Add(bookKey))
        //                {
        //                    errorList.Add($"Row {rowNumber}: Duplicate book in Excel");
        //                    rowNumber++; continue;
        //                }

        //                //-----------------------------------
        //                // PARSE + ADD
        //                //-----------------------------------
        //                int year = int.Parse(yearText);
        //                decimal price = decimal.Parse(priceText);
        //                int copies = int.Parse(totalCopiesText);
        //                bool isTax = taxCheckText == "1";
        //                decimal taxPercent = string.IsNullOrEmpty(taxPercentText) ? 0 : decimal.Parse(taxPercentText);

        //                decimal totalPrice = price * copies;
        //                decimal taxAmount = isTax ? (totalPrice * taxPercent / 100) : 0;
        //                decimal finalAmount = totalPrice + taxAmount;

        //                bulkDt.Rows.Add(rowNumber, isbn, title, categoryId, language, publisher,
        //                    year, edition, price, copies, totalPrice,
        //                    isTax, taxPercent, taxAmount, finalAmount,
        //                    shelf, activeText == "1", authorIdsCsv);

        //                rowNumber++;
        //            }
        //            catch (Exception ex)
        //            {
        //                //MyExceptionLogger.Publish(ex);
        //                errorList.Add($"Row {rowNumber}: Invalid data format.");
        //                rowNumber++;
        //            }
        //        }

        //        //-----------------------------------
        //        // STOP IF ERRORS
        //        //-----------------------------------
        //        if (errorList.Count > 0)
        //        {
        //            string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
        //            File.WriteAllLines(errorFile, errorList);

        //            ViewState["ErrorFilePath"] = errorFile;
        //            btnDownloadError.Visible = true;

        //            ShowAlert(lblErrorMsg[45], "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // STOP IF NO DATA
        //        //-----------------------------------
        //        if (bulkDt.Rows.Count == 0)
        //        {
        //            ShowAlert("No valid data to insert.", "error");
        //            return;
        //        }

        //        //-----------------------------------
        //        // DB INSERT
        //        //-----------------------------------
        //        try
        //        {

        //            using (DataTable dbResult = objMasterBO.BulkInsertBooks(bulkDt, intAdminUserID))
        //            {
        //                if (dbResult != null && dbResult.Rows.Count > 0)
        //                {
        //                    // ✅ Check if it's an ERROR result (has ErrorMessage column)
        //                    if (dbResult.Columns.Contains("ErrorMessage"))
        //                    {
        //                        foreach (DataRow dr in dbResult.Rows)
        //                        {
        //                            string msg = dr["ErrorMessage"]?.ToString();
        //                            if (!string.IsNullOrWhiteSpace(msg))
        //                            {
        //                                // ✅ Safely check RowNumber column too
        //                                string rowNum = dbResult.Columns.Contains("RowNumber")
        //                                    ? dr["RowNumber"]?.ToString()
        //                                    : "?";

        //                                errorList.Add($"Row {rowNum}: {msg}");
        //                            }
        //                        }

        //                        if (errorList.Count > 0)
        //                        {
        //                            string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
        //                            File.WriteAllLines(errorFile, errorList);

        //                            ViewState["ErrorFilePath"] = errorFile;
        //                            btnDownloadError.Visible = true;

        //                            ShowAlert(lblErrorMsg[45], "error");
        //                            return;
        //                        }
        //                    }
        //                    else if (dbResult.Columns.Contains("InsertedCount"))
        //                    {
        //                        // ✅ SUCCESS result — data was inserted
        //                        // Optionally log: int count = Convert.ToInt32(dbResult.Rows[0]["InsertedCount"]);
        //                    }
        //                    else if (dbResult.Columns.Contains("ErrorMessage") == false
        //                             && dbResult.Columns.Count == 1
        //                             && dbResult.Columns[0].ColumnName == "ErrorMessage")
        //                    {
        //                        // Edge case — already handled above
        //                    }
        //                }

        //                // ✅ SUCCESS
        //                ShowAlert(lblErrorMsg[47], "success");
        //                btnDownloadError.Visible = false;
        //            }

        //            ShowAlert(lblErrorMsg[47], "success");
        //            btnDownloadError.Visible = false;
        //        }
        //        catch (Exception ex)
        //        {
        //            MyExceptionLogger.Publish(ex);
        //            //ShowAlert("Database error occurred. Please contact admin.", "error");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionLogger.Publish(ex);
        //        //ShowAlert("Something went wrong. Please try again.", "error");
        //    }
        //}

        protected void btnUploadExcel_Click(object sender, EventArgs e)
        {
            try
            {
                //-----------------------------------
                // 🔴 FILE VALIDATION (USER ERRORS)
                //-----------------------------------
                if (!fuExcel.HasFile)
                {
                    ShowAlert(lblErrorMsg[40], "error"); // "Please select a file"
                    return;
                }

                string ext = Path.GetExtension(fuExcel.FileName).ToLower();
                if (ext != ".xlsx")
                {
                    ShowAlert(lblErrorMsg[41], "error"); // "Only .xlsx files are allowed"
                    return;
                }

                if (fuExcel.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    ShowAlert(lblErrorMsg[42], "error"); // "File size must be under 5MB"
                    return;
                }

                //-----------------------------------
                // 🔴 PATH SETUP
                //-----------------------------------
                string uploadPath = Server.MapPath("~/BulkUpload/Upload/");
                string errorPath = Server.MapPath("~/BulkUpload/Error/");
                ClearFolder(errorPath);

                string fileName = "Books_" + DateTime.Now.Ticks + ".xlsx";
                string fullPath = Path.Combine(uploadPath, fileName);

                //-----------------------------------
                // 🔴 SAVE FILE
                // ⚙️ SYSTEM ERROR → logger only, user sees friendly message
                //-----------------------------------
                try
                {
                    fuExcel.SaveAs(fullPath);
                }
                catch (Exception ex)
                {
                    MyExceptionLogger.Publish(ex); // 🔒 admin only
                    ShowAlert("File could not be saved. Please try again.", "error"); // 👤 user
                    return;
                }

                //-----------------------------------
                // 🔴 READ EXCEL
                // ✅ USER ERRORS → show directly (wrong format, missing headers, wrong columns)
                // ⚙️ SYSTEM ERRORS → logger only
                //-----------------------------------
                DataTable excelDt;
                try
                {
                    excelDt = ReadExcel(fullPath);
                }
                catch (ExcelFormatException ex)
                {
                    // 👤 USER ERROR — wrong file format/structure
                    ShowAlert(ex.Message, "error");
                    return;
                }
                catch (Exception ex)
                {
                    // 🔒 SYSTEM ERROR — unexpected crash
                    MyExceptionLogger.Publish(ex);
                    ShowAlert("Unable to read the Excel file. Please try again.", "error");
                    return;
                }

                //-----------------------------------
                // 🔴 BASIC DATA CHECKS (USER ERRORS)
                //-----------------------------------
                if (excelDt.Rows.Count == 0)
                {
                    ShowAlert(lblErrorMsg[43], "error"); // "Excel file has no data"
                    return;
                }

                if (!ValidateHeaders(excelDt))
                {
                    ShowAlert(lblErrorMsg[44], "error"); // "Invalid headers. Please use the template."
                    return;
                }

                //-----------------------------------
                // 🔴 PREPARE
                //-----------------------------------
                List<string> errorList = new List<string>();
                int rowNumber = 2;

                var categoryDict = GetCategoryDictionary();
                var authorDict = GetAuthorDictionary();

                HashSet<string> validLanguages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (ListItem item in ddlLanguage.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                        validLanguages.Add(item.Value);
                }

                DataTable bulkDt = CreateBulkTable();
                var isbnSet = new HashSet<string>();
                var bookSet = new HashSet<string>();

                //-----------------------------------
                // 🔴 ROW LOOP
                //-----------------------------------
                foreach (DataRow row in excelDt.Rows)
                {
                    if (row.ItemArray.All(x => string.IsNullOrWhiteSpace(x?.ToString())))
                    {
                        rowNumber++;
                        continue;
                    }

                    try
                    {
                        string isbn = row["ISBN"].ToString().Trim();
                        string title = row["BookTitle"].ToString().Trim();
                        string categoryName = row["CategoryName"].ToString().Trim();
                        string mainAuthor = row["MainAuthor"].ToString().Trim();
                        string coAuthor = row["CoAuthor"].ToString().Trim();
                        string language = row["Language"].ToString().Trim();
                        string publisher = row["Publisher"].ToString().Trim();
                        string yearText = row["YearPublished"].ToString().Trim();
                        string edition = row["Edition"].ToString().Trim();
                        string priceText = row["Price"].ToString().Trim();
                        string totalCopiesText = row["TotalCopies"].ToString().Trim();
                        string taxCheckText = row["TaxCheck"].ToString().Trim();
                        string taxPercentText = row["TaxPercent"].ToString().Trim();
                        string shelf = row["ShelfLocation"].ToString().Trim();
                       // string activeText = row["ActiveStatus"].ToString().Trim();

                        // ✅ CATEGORY
                        int categoryId = 0;
                        if (!categoryDict.TryGetValue(categoryName, out categoryId))
                        {
                            errorList.Add($"Row {rowNumber}: Invalid Category '{categoryName}'");
                            rowNumber++; continue;
                        }

                        // ✅ AUTHORS
                        List<int> authorIds = new List<int>();
                        if (!authorDict.ContainsKey(mainAuthor))
                        {
                            errorList.Add($"Row {rowNumber}: Required Main Author '{mainAuthor}'");
                            rowNumber++; continue;
                        }
                        authorIds.Add(authorDict[mainAuthor]);

                        if (!string.IsNullOrEmpty(coAuthor))
                        {
                            bool coAuthorError = false;
                            foreach (var a in coAuthor.Split(','))
                            {
                                string name = a.Trim();
                                if (!authorDict.ContainsKey(name))
                                {
                                    errorList.Add($"Row {rowNumber}: Invalid Co-Author '{name}'");
                                    coAuthorError = true;
                                    break;
                                }
                                authorIds.Add(authorDict[name]);
                            }
                            if (coAuthorError) { rowNumber++; continue; }
                        }

                        if (authorIds.Count != authorIds.Distinct().Count())
                        {
                            errorList.Add($"Row {rowNumber}: Duplicate authors not allowed");
                            rowNumber++; continue;
                        }

                        authorIds = authorIds.Distinct().ToList();
                        string authorIdsCsv = string.Join(",", authorIds);

                        // ✅ LANGUAGE
                        language = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(language.ToLower());
                        if (!validLanguages.Contains(language))
                        {
                            errorList.Add($"Row {rowNumber}: Invalid Language '{language}'");
                            rowNumber++; continue;
                        }
                        // ✅ TAX CROSS-VALIDATION
                        if (taxCheckText == "0" && !string.IsNullOrEmpty(taxPercentText) && taxPercentText != "0")
                        {
                            errorList.Add($"Row {rowNumber}: Tax Percent must be 0 when Tax Check is disabled");
                            rowNumber++; continue;
                        }

                        // ✅ TAX PERCENT REQUIRED WHEN TAX CHECK IS 1
                        if (taxCheckText == "1" && (string.IsNullOrEmpty(taxPercentText) || taxPercentText == "0"))
                        {
                            errorList.Add($"Row {rowNumber}: Tax Percent is required and must be greater than 0 when Tax Check is enabled");
                            rowNumber++; continue;
                        }

                        // ✅ FIELD VALIDATIONS
                        var rowErrors = ValidateExcelRow(
                            rowNumber, title, isbn, categoryId,
                            language, publisher, yearText, edition,
                            priceText, totalCopiesText, taxCheckText, taxPercentText, authorIdsCsv
                        );
                        if (rowErrors.Count > 0)
                        {
                            errorList.AddRange(rowErrors);
                            rowNumber++; continue;
                        }

                        // ✅ DUPLICATE ISBN IN EXCEL
                        if (!isbnSet.Add(isbn))
                        {
                            errorList.Add($"Row {rowNumber}: Duplicate ISBN in Excel");
                            rowNumber++; continue;
                        }

                        // ✅ DUPLICATE BOOK IN EXCEL
                        string bookKey = title.ToLower().Trim() + "_" + edition.ToLower().Trim() + "_" + authorIdsCsv;
                        if (!bookSet.Add(bookKey))
                        {
                            errorList.Add($"Row {rowNumber}: Duplicate book in Excel");
                            rowNumber++; continue;
                        }

                        // ✅ PARSE + ADD
                        int year = int.Parse(yearText);
                        decimal price = decimal.Parse(priceText);
                        int copies = int.Parse(totalCopiesText);
                        bool isTax = taxCheckText == "1";
                        decimal taxPercent = string.IsNullOrEmpty(taxPercentText) ? 0 : decimal.Parse(taxPercentText);

                        decimal totalPrice = price * copies;
                        decimal taxAmount = isTax ? (totalPrice * taxPercent / 100) : 0;
                        decimal finalAmount = totalPrice + taxAmount;

                        bulkDt.Rows.Add(rowNumber, isbn, title, categoryId, language, publisher,
                            year, edition, price, copies, totalPrice,
                            isTax, taxPercent, taxAmount, finalAmount,
                            shelf,true, authorIdsCsv);

                        rowNumber++;
                    }
                    catch (Exception ex)
                    {
                        // 🔒 SYSTEM ERROR — unexpected parse crash, log it
                        MyExceptionLogger.Publish(ex);
                        // 👤 USER sees row-level message
                        errorList.Add($"Row {rowNumber}: Invalid data format. Please check all fields.");
                        rowNumber++;
                    }
                }

                //-----------------------------------
                // 🔴 STOP IF ROW ERRORS (USER ERRORS → error file + toast)
                //-----------------------------------
                if (errorList.Count > 0)
                {
                    string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
                    File.WriteAllLines(errorFile, errorList);
                    ViewState["ErrorFilePath"] = errorFile;
                    btnDownloadError.Visible = true;
                    ShowAlert(lblErrorMsg[45], "error"); // "Validation errors found. Download the error file."
                    return;
                }

                //-----------------------------------
                // 🔴 NO VALID ROWS (USER ERROR)
                //-----------------------------------
                if (bulkDt.Rows.Count == 0)
                {
                    ShowAlert("No valid data to insert.", "error");
                    return;
                }

                //-----------------------------------
                // 🔴 DB INSERT
                //-----------------------------------
                try
                {
                    using (DataTable dbResult = objMasterBO.BulkInsertBooks(bulkDt, intAdminUserID))
                    {
                        if (dbResult != null && dbResult.Rows.Count > 0)
                        {
                            if (dbResult.Columns.Contains("ErrorMessage"))
                            {
                                // 👤 DB VALIDATION ERRORS → user sees them in error file
                                foreach (DataRow dr in dbResult.Rows)
                                {
                                    string msg = dr["ErrorMessage"]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(msg))
                                    {
                                        string rowNum = dbResult.Columns.Contains("RowNumber")
                                            ? dr["RowNumber"]?.ToString() : "?";
                                        errorList.Add($"Row {rowNum}: {msg}");
                                    }
                                }

                                if (errorList.Count > 0)
                                {
                                    string errorFile = Path.Combine(errorPath, "Error_" + DateTime.Now.Ticks + ".txt");
                                    File.WriteAllLines(errorFile, errorList);
                                    ViewState["ErrorFilePath"] = errorFile;
                                    btnDownloadError.Visible = true;
                                    ShowAlert(lblErrorMsg[45], "error");
                                    return;
                                }
                            }
                        }

                        // ✅ SUCCESS
                        btnDownloadError.Visible = false;
                        ShowAlert(lblErrorMsg[47], "success"); // "Books uploaded successfully."
                    }
                }
                catch (Exception ex)
                {
                    // 🔒 SYSTEM/DB ERROR → logger only
                    MyExceptionLogger.Publish(ex);
                   // ShowAlert("A database error occurred. Please contact the administrator.", "error");
                }
            }
            catch (Exception ex)
            {
                // 🔒 UNEXPECTED SYSTEM ERROR → logger only
                MyExceptionLogger.Publish(ex);
                //ShowAlert("Something went wrong. Please try again.", "error");
            }
        }
        // Add this small class anywhere in your code-behind
        public class ExcelFormatException : Exception
        {
            public ExcelFormatException(string message) : base(message) { }
        }

        private DataTable ReadExcel(string filePath)
        {
            DataTable dt = new DataTable();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                if (package.Workbook.Worksheets.Count == 0)
                    throw new ExcelFormatException("The uploaded file has no worksheets. Please use the correct template.");

                ExcelWorksheet ws = package.Workbook.Worksheets.First();

                if (ws.Dimension == null)
                    throw new ExcelFormatException("The uploaded Excel sheet is empty. Please add data and try again.");

                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;

                if (colCount != 14)
                    throw new ExcelFormatException($"Invalid Excel format. Expected 14 columns but found {colCount}. Please use the correct template.");

                for (int col = 1; col <= colCount; col++)
                {
                    string header = ws.Cells[1, col].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(header))
                        throw new ExcelFormatException($"Column {col} header is missing. Please use the correct template.");
                    dt.Columns.Add(header);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    bool isEmptyRow = true;
                    DataRow dr = dt.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        string value = ws.Cells[row, col].Text?.Trim();
                        if (!string.IsNullOrWhiteSpace(value)) isEmptyRow = false;
                        dr[col - 1] = value;
                    }
                    if (!isEmptyRow) dt.Rows.Add(dr);
                }
            }

            return dt;
        }
     
        private void ClearFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath)) return;

                var files = Directory.GetFiles(folderPath);

                foreach (var file in files)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);

                        // ✅ Delete only OLD files (avoid deleting active file)
                        if (fi.CreationTime < DateTime.Now.AddMinutes(-10))
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // 🔴 Skip locked files silently
                    }
                }
            }
            catch (Exception ex)
            {
                // 🔴 Log only (no UI)
                MyExceptionLogger.Publish(ex);
            }
        }

        private DataTable CreateBulkTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RowNumber", typeof(int));
            dt.Columns.Add("ISBN");
            dt.Columns.Add("BookTitle");
            dt.Columns.Add("CategoryID", typeof(int));
            dt.Columns.Add("Language");
            dt.Columns.Add("PublisherName");
            dt.Columns.Add("YearPublished", typeof(int));
            dt.Columns.Add("Edition");
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("TotalCopies", typeof(int));
            dt.Columns.Add("TotalPrice", typeof(decimal));
            dt.Columns.Add("TaxCheck", typeof(bool));
            dt.Columns.Add("TaxPercentage", typeof(decimal));
            dt.Columns.Add("TaxAmount", typeof(decimal));
            dt.Columns.Add("FinalAmount", typeof(decimal));
            dt.Columns.Add("ShelfLocation");
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("AuthorIDs");

            return dt;
        }
        private bool ValidateHeaders(DataTable dt)
        {
            string[] expectedColumns = {"ISBN","BookTitle","CategoryName","MainAuthor","CoAuthor","Language","Publisher","YearPublished","Edition",
        "Price","TotalCopies","TaxCheck","TaxPercent","ShelfLocation" };

            if (dt.Columns.Count != expectedColumns.Length)
                return false;

            for (int i = 0; i < expectedColumns.Length; i++)
            {
                if (dt.Columns[i].ColumnName.Trim() != expectedColumns[i])
                    return false;
            }
            return true;
        }
        protected void btnDownloadSample_Click(object sender, EventArgs e)
        {
            string samplePath = Server.MapPath("~/BulkUpload/Sample/Sample_Book.xlsx");
            if (File.Exists(samplePath))
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Sample_BookDetails.xlsx");
                Response.WriteFile(samplePath);
                Response.End();
            }
            else
            {
                ShowAlert(lblErrorMsg[48], "error");
            }
        }
        protected void btnDownloadError_Click(object sender, EventArgs e)
        {
            if (ViewState["ErrorFilePath"] != null)
            {
                string filePath = ViewState["ErrorFilePath"].ToString();
                if (File.Exists(filePath))
                {
                    Response.Clear();
                    Response.ContentType = "text/plain";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=Error.txt");
                    Response.WriteFile(filePath);
                    Response.End();
                }
            }
        }

    

        protected void btnUploadZip_Click(object sender, EventArgs e)
        {
            List<string> fileErrors = new List<string>();

            // ✅ Your manual fixed folder
            string bulkRoot = Server.MapPath("~/BulkReceipts/");
            string tempReceipts = Path.Combine(bulkRoot, "Receipts\\");
            string tempExtracted = Path.Combine(bulkRoot, "Extracted\\");
            string tempErrors = Path.Combine(bulkRoot, "Errors\\");

            try
            {
                ViewState["ZipErrorFilePath"] = null;
                btnDownloadZipError.Visible = false;
                // =========================
                // 1. ENSURE FOLDERS EXIST
                // =========================
                Directory.CreateDirectory(bulkRoot);
                Directory.CreateDirectory(tempReceipts);
                Directory.CreateDirectory(tempErrors);

                ClearFolderBulkZip(tempReceipts);
                ClearFolderBulkZip(tempErrors);

                // ✅ Clear Extracted folder before each upload
                if (Directory.Exists(tempExtracted))
                    Directory.Delete(tempExtracted, true);
                Directory.CreateDirectory(tempExtracted);

                // =========================
                // 2. ZIP VALIDATION
                // =========================
                if (!fuZip.HasFile)
                {
                    ShowAlert(lblErrorMsg[52], "error");
                    return;
                }

                string ext = Path.GetExtension(fuZip.FileName).ToLower();
                if (ext != ".zip")
                {
                    ShowAlert(lblErrorMsg[53], "error");
                    return;
                }

                // =========================
                // 3. SAVE ZIP INTO BulkReceipts/
                // =========================
                string zipPath = Path.Combine(bulkRoot, fuZip.FileName);
                fuZip.SaveAs(zipPath);

                // =========================
                // 4. EXTRACT ZIP INTO Extracted/
                // =========================
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(tempExtracted, entry.FullName);
                        string dir = Path.GetDirectoryName(destinationPath);

                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        if (!string.IsNullOrEmpty(entry.Name))
                            entry.ExtractToFile(destinationPath, true);
                    }
                }

                // ✅ Delete zip after extraction
                if (File.Exists(zipPath))
                    File.Delete(zipPath);

                // =========================
                // 5. GET RECEIPTS FOLDER FROM Extracted/
                // =========================
                string sourceReceipts = Directory
                    .GetDirectories(tempExtracted, "Receipts", SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(sourceReceipts))
                {
                    ShowAlert(lblErrorMsg[54], "error");
                    return;
                }

                List<string> receiptFiles = Directory.GetFiles(sourceReceipts).ToList();

                if (receiptFiles.Count == 0)
                {
                    ShowAlert(lblErrorMsg[55], "error");
                    return;
                }

                // =========================
                // 6. FILE VALIDATION
                // =========================
                string[] allowedExt = { ".jpg", ".jpeg", ".png" };
                long maxFileSizeBytes = 2 * 1024 * 1024; // 2MB

                foreach (string file in receiptFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string isbn = Path.GetFileNameWithoutExtension(fileName);
                    string fileExt = Path.GetExtension(fileName).ToLower();
                    long fileSize = new FileInfo(file).Length;

                    if (!allowedExt.Contains(fileExt))
                        fileErrors.Add("Invalid file type: " + fileName);

                    if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^\d{10,13}$"))
                        fileErrors.Add("Invalid ISBN format: " + fileName);

                    if (fileSize == 0)
                        fileErrors.Add("File is empty (0 bytes): " + fileName);

                    if (fileSize > maxFileSizeBytes)
                        fileErrors.Add($"File exceeds 2MB limit ({fileSize / 1024} KB): " + fileName);
                }

                if (fileErrors.Count > 0)
                {
                    // ✅ Uses original 2-arg CreateErrorFile → saves to ~/Temp/ErrorFiles/
                   
                    CreateErrorFile(fileErrors, "ZIP", tempErrors);
                    ShowAlert(lblErrorMsg[56], "error");
                    return;
                }

                // =========================
                // 7. COPY TO Receipts/ + BUILD DATATABLE
                // ✅ Store FINAL path in DB
                // =========================
                DataTable dt = new DataTable();
                dt.Columns.Add("RowNumber", typeof(int));
                dt.Columns.Add("ISBN", typeof(string));
                dt.Columns.Add("ReceiptPath", typeof(string));
                dt.Columns.Add("UploadedFileName", typeof(string));

                int row = 1;

                foreach (string file in receiptFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string isbn = Path.GetFileNameWithoutExtension(fileName);

                    // ✅ Stage into BulkReceipts/Receipts/
                    string tempFilePath = Path.Combine(tempReceipts, fileName);
                    File.Copy(file, tempFilePath, true);

                    // ✅ Final path stored in DB
                    string finalFilePath = "~/Uploads/Receipts/" + fileName;
                    dt.Rows.Add(row++, isbn, finalFilePath, fileName);
                }

                // =========================
                // 8. CALL BLL
                // =========================
                using (DataSet ds = objMasterBO.BulkReceiptUpload(dt, intAdminUserID))
                {
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int msgCode = Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);

                        // ❌ DB VALIDATION FAILED
                        if (msgCode == 0)
                        {
                            List<string> dbErrors = new List<string>();

                            foreach (DataRow r in ds.Tables[0].Rows)
                                dbErrors.Add($"Row {r["RowNumber"]}: {r["ErrorMessage"]}");

                            // ✅ Uses original 2-arg CreateErrorFile → saves to ~/Temp/ErrorFiles/
                            CreateErrorFile(dbErrors, "ZIP", tempErrors);
                            ShowAlert(lblErrorMsg[56], "error");

                            // ✅ Clear staging only, keep ErrorFiles for download
                            ClearFolderBulkZip(tempReceipts);
                            ClearFolderBulkZip(tempExtracted);
                            return;
                        }

                        // ✅ DB SUCCESS → MOVE FILES TO FINAL
                        if (msgCode == 1)
                        {
                            string finalReceiptsPath = Server.MapPath("~/Uploads/Receipts/");

                            if (!Directory.Exists(finalReceiptsPath))
                                Directory.CreateDirectory(finalReceiptsPath);

                            // ✅ Move each file BulkReceipts/Receipts/ → Uploads/Receipts/
                            foreach (DataRow r in dt.Rows)
                            {
                                string fileName = r["UploadedFileName"].ToString();
                                string tempPath = Path.Combine(tempReceipts, fileName);
                                string finalPath = Path.Combine(finalReceiptsPath, fileName);

                                if (!File.Exists(tempPath))
                                    continue;

                                if (File.Exists(finalPath))
                                    File.Delete(finalPath);

                                File.Move(tempPath, finalPath);
                            }

                            // ✅ SUCCESS: Clear all 3 folders
                            ClearFolderBulkZip(tempReceipts);
                            ClearFolderBulkZip(tempExtracted);
                            ClearFolderBulkZip(tempErrors);

                            ShowAlert(lblErrorMsg[57], "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ✅ Clear staging folders only (ErrorFiles kept for download)
                ClearFolderBulkZip(tempReceipts);
                ClearFolderBulkZip(tempExtracted);
                MyExceptionLogger.Publish(ex);
                //ShowAlert("System Error: " + ex.Message, "error");
            }
        }

        // ✅ Clears all files/subfolders inside a folder without deleting the folder itself
        private void ClearFolderBulkZip(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            foreach (string file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                try { File.Delete(file); } catch { }
            }

            foreach (string dir in Directory.GetDirectories(folderPath))
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
        private void CreateErrorFile(List<string> errors, string type, string folderPath)
        {
            if (errors == null || errors.Count == 0)
                return;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = type + "_Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string fullPath = Path.Combine(folderPath, fileName);

            File.WriteAllLines(fullPath, errors);

            if (type == "ZIP")
            {
                ViewState["ZipErrorFilePath"] = fullPath;
                btnDownloadZipError.Visible = true;
            }
            else if (type == "EXCEL")
            {
                ViewState["ErrorFilePath"] = fullPath;
                btnDownloadError.Visible = true;
            }
        }
        protected void btnDownloadZipError_Click(object sender, EventArgs e)
        {
            string filePath = ViewState["ZipErrorFilePath"] as string;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                ShowAlert(lblErrorMsg[58], "error");
                return;
            }

            string fileName = Path.GetFileName(filePath);

            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(filePath);
            Response.End();
        }

    }
}
