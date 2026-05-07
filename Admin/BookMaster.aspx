<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookMaster.aspx.cs" Inherits="Admin.BookMaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>BookMaster</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="../assets/css/vendors/select2.css" />
    <link rel="stylesheet" href="../assets/css/bootstrap-multiselect.min.css" />
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
    <style>
        .form-label {
            display: block;
        }

        .btn-group {
            width: -webkit-fill-available;
        }

        label.form-check-label {
            color: black !important;
        }

        .multiselect-native-select {
            width: 100%;
            /* set height */
        }

        .multiselect-container {
            width: 100% !important;
            max-height: 200px !important; /* adjust if needed */
            overflow-y: auto !important;
        }
            /* FIX search bar: make it sticky */
            .multiselect-container .multiselect-filter {
                width: 100% !important;
                position: sticky;
                top: 0;
                background: #fff;
                z-index: 100;
            }

            /* Hide "select all" if you don't use it */
            .multiselect-container .multiselect-item.multiselect-all {
                display: none;
            }

        .wrap-header {
            width: 240px;
            white-space: normal; /* allow wrapping */
            word-break: normal;
            overflow-wrap: normal;
            text-align: center
        }

        /* Cell */
        .wraping-cell {
            width: 240px;
            white-space: normal; /* allow wrapping */
            word-break: normal; /* DO NOT break words */
            overflow-wrap: normal; /* wrap only at spaces */

            line-height: 1.5;
            padding: 6px 8px;
            vertical-align: top;
        }

        .icon-btn {
            width: 38px;
            height: 38px;
            background-color: #0ea5a0; /* teal matching header */
            border-radius: 8px;
            color: #ffffff;
            text-decoration: none;
            transition: all 0.2s ease-in-out;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
        }

            .icon-btn i {
                font-size: 18px;
            }

       

        .drawer {
            position: fixed;
            top: 80px; /* below header */
            right: -400px;
            width: 350px;
            max-height: 70vh; /* 🔥 important */
            background: #fff;
            box-shadow: -3px 0 10px rgba(0,0,0,0.2);
            transition: right 0.3s ease;
            z-index: 9999;
            border-radius: 10px 0 0 10px;
            overflow: hidden;
        }

        .drawer-body {
            max-height: 60vh;
            overflow-y: auto;
        }

        .drawer.open {
            right: 0;
        }

        .drawer-header {
            padding: 10px;
            background: #02a2b9;
            color: #fff;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }



        .drawer-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.4);
            display: none;
            z-index: 9998;
        }

            .drawer-overlay.show {
                display: block;
            }

        .close-btn {
            cursor: pointer;
            font-size: 18px;
        }

    </style>

</head>
<body>
    <uc:header id="Header" runat="server" />
    <form id="form1" runat="server">

        <div class="page-body">
            <asp:HiddenField ID="hdnReceiptPath" runat="server" />
            <asp:HiddenField ID="hdnEbookPath" runat="server" />
            <div class="container-fluid pt-2">
                <input type="hidden" id="hdnBookID" runat="server" />
                <div class="row align-items-center mb-2">
                    <asp:HiddenField runat="server" ID="hfRemoveColumnsCSV" Value="BookId,Active,ActiveStatus,ebook,ReceiptCount" />

                    <div class="col d-flex justify-content-start">
                        <a href="BookMaster.aspx"
                            class="text-primary text-decoration-none fw-semibold fs-6"
                            id="lnkback" runat="server" visible="false">← Back
                        </a>
                    </div>

                    <div class="col d-flex justify-content-end gap-2">
                        <asp:Button ID="btnAddBooks" runat="server" Text="Add Books" CssClass="btn btn-success" OnClick="btnAddBooks_Click" />
                        <asp:Button ID="btnBulkUpload" runat="server" Text="Bulk Upload" CssClass="btn btn-outline-success" OnClick="btnBulkUpload_Click" />
                    </div>
                </div>
                <div class="card" id="divBookGrid" runat="server" visible="false">
                    <div class="card-header bg-primary px-3 py-2">
                        <div class="row align-items-center justify-content-between">
                            <div class="col-auto">
                                <h3 class="card-title mb-0">Book Details</h3>
                            </div>
                            <div class="col-auto text-end">
                                <asp:LinkButton ID="lnkDownloadCSV" runat="server" OnClick="btnDownloadCSV_Click" ToolTip="Download CSV">
                                    <asp:Image ID="imgDownload" runat="server" ImageUrl="../assets/images/icons/csvdownload.png" AlternateText="Download" CssClass="icon img-fluid"
                                        Width="35" Height="35" />
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="card-body p-3">
                        <!-- ---------- SEARCH AREA (Not Scrollable) ---------- -->
                        <div class="row mb-3 align-items-end g-2">
                            <!-- Left controls -->
                            <div class="col-12 col-lg d-flex flex-wrap gap-2 align-items-end">
                                <div class="col-12 col-sm-6 col-md-3">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="form-select w-100">
                                        <asp:ListItem Value="">------- Search By -------</asp:ListItem>
                                        <asp:ListItem Value="Category">Category Name</asp:ListItem>
                                        <asp:ListItem Value="BookTitle">Book Title</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-12 col-sm-6 col-md-3">
                                    <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control w-100"
                                        Placeholder="Enter search text" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-auto">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary w-100" OnClick="btnSearch_Click" />
                                </div>
                                <div class="col-auto">
                                    <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="btn btn-secondary w-100" OnClick="btnClearSearch_Click" />
                                </div>
                            </div>
                            <div class="col-12 col-lg-auto ms-lg-auto text-lg-end" id="divPageSize" runat="server">
                                Show 
                               <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                                   OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                                   CssClass="form-select d-inline w-auto">
                               </asp:DropDownList>
                                entries
   
                            </div>
                            <!-- Right record count -->

                        </div>

                        <!-- ---------- ONLY GRIDVIEW SCROLLS ---------- -->
                        <div class="table-responsive" style="overflow-y: auto; white-space: nowrap;">
                            <asp:GridView ID="gvBookMaster" runat="server" CssClass="table table-bordered table-striped text-center" AutoGenerateColumns="False"
                                OnRowCommand="gvBookMaster_RowCommand" AllowPaging="True" OnPageIndexChanging="gvBookMaster_PageIndexChanging"
                                PagerSettings-Visible="false" EmptyDataText="No records found">
                                <Columns>
                                    <asp:BoundField DataField="Sno" HeaderText="S.No." />
                                    <asp:BoundField DataField="BookID" Visible="false" />
                                    <asp:BoundField DataField="ISBN" HeaderText="ISBN" />
                                    <%-- <asp:BoundField DataField="Book Title" HeaderText="Book Title" />--%>

                                    <asp:TemplateField HeaderText="Book Title">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate><%# Eval("Book Title") %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Category">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate><%# Eval("Category") %>  </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Authors">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate><%# Eval("Authors") %>  </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Publisher">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate><%# Eval("Publisher") %>  </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Edition" HeaderText="Edition" />

                                    <asp:TemplateField HeaderText="Total Copies">
                                        <HeaderStyle CssClass="wrap-header" />
                                        <ItemTemplate><%# Eval("Total Copies") %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ActiveStatus" HeaderText="Status" />
                                    <asp:BoundField DataField="Active" Visible="false" />
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditBook" CommandArgument='<%# Eval("BookID") %>'
                                                CssClass="btn btn-sm btn-primary me-2" ToolTip="Edit Book"> <i class="iconly-Edit icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteBook" CommandArgument='<%# Eval("BookID") %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("Active")) ? "btn btn-sm btn-danger" : "btn btn-sm btn-secondary disabled" %>'
                                                ToolTip='<%# Convert.ToBoolean(Eval("Active"))  ? "Click to deactivate"  : "Already inactive" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("Active"))  ? "return confirm(\"Are you sure you want to deactivate this book?\");" : "return false;" %>'>
                                                <i class="iconly-Delete icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Additonal Purchase">
                                        <HeaderStyle CssClass="wrap-header" />
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkPurchase"
                                                runat="server"
                                                CssClass='<%# Convert.ToBoolean(Eval("Active")) ? "btn btn-sm btn-success" : "btn btn-sm btn-secondary disabled" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("Active")) ? "return true;" : "return false;" %>'
                                                ToolTip='<%# Convert.ToBoolean(Eval("Active")) ? "New Purchase" : "Book is inactive" %>'
                                                CommandName="NewPurchase"
                                                CommandArgument='<%# Eval("BookID") %>'>
    <i class="iconly-Buy icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Receipts">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate>

                                            <!-- View Button -->
                                            <asp:LinkButton ID="lnkViewReceipts" runat="server"
                                                CommandName="ViewReceipts"
                                                CommandArgument='<%# Eval("BookID") %>'
                                                  CssClass='<%# Convert.ToBoolean(Eval("Active")) ? "btn btn-sm btn-info" : "btn btn-sm btn-info disabled" %>'
            OnClientClick='<%# Convert.ToBoolean(Eval("Active")) ? "" : "return false;" %>'
            ToolTip='<%# Convert.ToBoolean(Eval("Active")) ? "View Receipts" : "Book is inactive" %>'>
            <i class="iconly-Document icli"></i>
                                            </asp:LinkButton>

                                            <!-- No Receipts Text -->
                                            <asp:Label ID="lblNoReceipts" runat="server"
                                               
                                                Visible='<%# Convert.ToInt32(Eval("ReceiptCount")) == 0 %>'>
                                            </asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ebook">
                                        <ItemStyle CssClass="wraping-cell" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEbook" runat="server" CommandName="DownloadEbook" CommandArgument='<%# Eval("ebook") %>'
                                                Visible='<%# !string.IsNullOrEmpty(Eval("Ebook").ToString()) %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("Active")) ? "btn btn-sm btn-info me-2" : "btn btn-sm btn-info disabled me-2" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("Active")) ? "" : "return false;" %>' ToolTip='<%# Convert.ToBoolean(Eval("Active")) ? "Download EBook" : "Book is inactive" %>'>
                                                <i class="iconly-Download icli"></i>
                                            </asp:LinkButton>
                                            <asp:Label ID="lblNoEbook" runat="server" Text="Not Available" Visible='<%# string.IsNullOrEmpty(Eval("Ebook").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="d-flex flex-column flex-md-row justify-content-between align-items-center mb-2 gap-2">
                            <div class="text-center text-md-start mt-2">
                                <asp:Label ID="lblPageInfo" runat="server" CssClass="fw-bold text-primary fs-6"></asp:Label>
                            </div>
                            <!-- ✅ Pagination -->
                            <div class="pager-fixed d-flex flex-wrap justify-content-center justify-content-md-start">
                                <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPage" runat="server"
                                            CssClass='<%# (bool)Eval("IsActive") ? "page-btn active" : "page-btn" %>'
                                            CommandName='<%# Eval("Command") %>'
                                            CommandArgument='<%# Eval("PageIndex") %>'
                                            Enabled='<%# Eval("Enabled") %>'
                                            Text='<%# Eval("Text") %>'>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <!-- ✅ Page Info -->


                        </div>
                    </div>
                </div>
                <div class="card" id="divBulkUpload" runat="server" visible="false">

                    <!-- Card Header -->
                    <div class="card-header bg-primary p-3">
                        <h3 class="card-title mb-0 text-white">Bulk Upload Books
                        </h3>
                    </div>

                    <!-- Card Body -->
                    <div class="card-body">

                        <!-- ================= Excel Upload Section ================= -->
                        <div class="mb-4">

                            <!-- Row 1 : Label only -->
                            <div class="row mb-2">
                                <div class="col-12">
                                    <label class="form-label fw-bold">Upload Excel File  </label>
                                </div>
                            </div>

                            <!-- Row 2 : FileUpload + Buttons -->
                            <div class="row align-items-center g-3">
                                <!-- File upload -->
                                <div class="col-12 col-md-4">
                                    <asp:FileUpload ID="fuExcel" runat="server" CssClass="form-control" />

                                </div>
                                <!-- Upload Excel -->
                                <div class="col-6 col-md-auto">
                                    <asp:Button ID="btnUploadExcel" runat="server" Text="Upload Excel" CssClass="btn btn-success w-100" OnClick="btnUploadExcel_Click" />
                                </div>
                                <!-- Download Sample -->
                                <div class="col-6 col-md-auto">
                                    <asp:Button ID="btnDownloadSample" runat="server" Text="Download Sample Excel" CssClass="btn btn-info w-100" OnClick="btnDownloadSample_Click" />
                                </div>
                                <!-- Download Error -->
                                <div class="col-12 col-md-auto">
                                    <asp:Button ID="btnDownloadError" runat="server" Text="Download Error File" CssClass="btn btn-danger w-100" OnClick="btnDownloadError_Click" Visible="false" />
                                </div>
                            </div>
                            <div class="p-2 rounded small" style="font-size: 14px;">
                                <div class="fw-semibold mb-2" style="color: #dc3545;">Instructions:  </div>
                                <ul class="mb-0 ps-3" style="list-style-type: disc; padding-left: 20px;">
                                    <li>Download the <b>Sample Excel</b> file and use the exact same format (Do not rename, remove, or reorder columns)</li>

                                    <li>Upload only <b>.xlsx</b> files (Maximum size: 5 MB)</li>

                                    <li><b>Required columns:</b> ISBN, BookTitle, CategoryName, MainAuthor, Language, Publisher, YearPublished, Edition, Price, TotalCopies, TaxCheck</li>

                                    <li><b>Optional columns:</b> CoAuthor, ShelfLocation</li>

                                    <li><b>ISBN:</b> Enter a valid unique ISBN number  
Example: <b>9781234567890</b></li>

                                    <li><b>Book details example:</b>
                                        BookTitle: <b>C# Basics</b>
                                        CategoryName: <b>Computer Science</b>
                                        MainAuthor: <b>R. Kumar</b></li>

                                    <li><b>Numeric fields:</b>
                                        YearPublished → Example: <b>2022</b>
                                        Price → Example: <b>450.00</b>
                                        TotalCopies → Example: <b>10</b></li>

                                    <li><b>Edition format:</b> Use standard values only Examples: <b>1st, 2nd, 3rd, 4th</b></li>

                                    <li><b>Tax rules:</b>
                                        TaxCheck = <b>1</b> → Tax applicable (**TaxPercent is required**)  
                                        TaxCheck = <b>0</b> → Tax not applicable (**leave TaxPercent empty or 0**)  
                                        Example: <b>1, 5</b></li>

                                    <li><b>CoAuthor:</b> Enter multiple names separated by commas Example: <b>A. Kumar, B. Raj</b></li>

                                    <li><b>Duplicate rules:</b>
                                        ❌ Same ISBN is not allowed, ❌ Same BookTitle + Publisher + Edition combination is not allowed</li>

                                    <li>If upload validation fails, download the error file, correct the data, and upload again</li>
                                </ul>
                            </div>
                        </div>

                        <hr />

                        <!-- ================= ZIP Upload Section ================= -->
                        <div class="row align-items-end g-3">
                            <!-- ZIP Label + Upload -->
                            <div class="col-12 col-md-5">
                                <label class="form-label fw-bold">Upload ZIP (Receipts) </label>
                                <asp:FileUpload ID="fuZip" runat="server" CssClass="form-control" />

                            </div>
                            <!-- Upload ZIP -->
                            <div class="col-6 col-md-auto">
                                <asp:Button ID="btnUploadZip" runat="server" Text="Upload ZIP" CssClass="btn btn-primary w-100" OnClick="btnUploadZip_Click" />
                            </div>

                            <!-- Download ZIP Error -->
                            <div class="col-6 col-md-auto">
                                <asp:Button ID="btnDownloadZipError" runat="server" Text="Download ZIP Errors" CssClass="btn btn-danger w-100"
                                    Visible="false" OnClick="btnDownloadZipError_Click" />
                            </div>
                        </div>
                        <div class="p-2 rounded small" style="font-size: 14px;">
                            <div class="fw-semibold mb-2" style="color: #dc3545;">Instructions:  </div>
                            <ul class="mb-0 ps-3" style="list-style-type: disc; padding-left: 20px;">
                               <li>Create a <b>ZIP file</b> containing one folder named <b>Receipts</b></li>

                                <li>The ZIP must contain <b>only the Receipts folder</b> (Do not include any other folders or files)</li>

                                <li>Inside the <b>Receipts</b> folder, upload only image files in these formats:  
                                <b>.jpg, .jpeg, .png</b></li>

                                <li>Rename each image file using the book ISBN only  
                                Example: <b>9781234567890.jpg</b></li>

                                <li><b>ISBN must contain only 10 to 13 digits</b>  
                                ❌ No spaces  
                                ❌ No special characters  
                                ❌ No extra text in file name</li>

                                <li>Each image file must:  
                                ✔ Be less than <b>2 MB</b>  
                                ✔ Not be empty/corrupted</li>

                                <li>Ensure each image file name matches an ISBN from the uploaded Excel file</li>

                                <li>If validation fails, download the error file, correct the issues, and upload again</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="card" id="divForm" runat="server" visible="false">
                    <div class="card-header bg-primary p-3">
                        <h3 class="card-title mb-0">Book Entry</h3>
                    </div>
                    <div class="card-body">

                        <h5 class="text-primary mb-3 border-bottom pb-1 fw-bold">Book Details</h5>
                        <div class="row">
                            <div class="col-md-4 ">
                                <label class="form-label">Book Title <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtBookTitle" runat="server" CssClass="form-control" placeholder="Enter Book Title" MaxLength="50"></asp:TextBox>
                            </div>
                            <!-- ISBN -->
                            <div class="col-md-4 ">
                                <label class="form-label">ISBN Number <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtISBN" runat="server" CssClass="form-control" MaxLength="13" placeholder="Enter ISBN Number"></asp:TextBox>
                            </div>
                            <!-- Category (searchable dropdown) -->
                            <div class="col-md-4">
                                <label class="form-label">Category <span style="color: red">*</span></label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select js-example-basic-single col-sm-12" data-live-search="true">
                                </asp:DropDownList>
                            </div>
                            <!-- Main Author -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Main Author<span style="color: red">*</span></label>
                                <asp:ListBox ID="lstMainAuthor" runat="server" CssClass="form-select" SelectionMode="Multiple"></asp:ListBox>
                            </div>
                            <!-- Co-Author -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Co-Author</label>
                                <asp:ListBox ID="lstCoAuthor" runat="server" CssClass="form-select" SelectionMode="Multiple"></asp:ListBox>
                            </div>
                            <!-- Language -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Language <span style="color: red">*</span></label>
                                <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Select Language" Value="" Selected="True" Disabled="True"></asp:ListItem>
                                    <asp:ListItem Text="English" Value="English" />
                                    <asp:ListItem Text="Tamil" Value="Tamil" />
                                    <asp:ListItem Text="Hindi" Value="Hindi" />
                                    <asp:ListItem Text="Malayalam" Value="Malayalam" />
                                    <asp:ListItem Text="Kannada" Value="Kannada" />
                                    <asp:ListItem Text="Telugu" Value="Telugu" />
                                </asp:DropDownList>
                            </div>
                            <!-- Publisher -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Publisher Name <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtPublisher" runat="server" CssClass="form-control" placeholder="Enter Publisher Name" MaxLength="50"></asp:TextBox>
                            </div>
                            <!-- Year Published -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Year Published <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtYearPublished" runat="server" CssClass="form-control" MaxLength="4" placeholder="Enter Year (YYYY)" oninput="this.value = this.value.replace(/[^0-9]/g, '')"></asp:TextBox>
                            </div>
                            <!-- Edition -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Edition <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtEdition" runat="server" CssClass="form-control" MaxLength="20" placeholder="Enter Edition"></asp:TextBox>
                            </div>
                        </div>

                        <h5 class="text-primary mt-1 mb-3 border-bottom pb-1 fw-bold">Pricing Details</h5>
                        <div class="row">
                            <!-- Price -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Price<span style="color: red">*</span> </label>
                                <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Price per book" MaxLength="8" onkeypress="return allowPriceChars(event);"
                                    onpaste="return false;" oninput="calculateAmounts();"></asp:TextBox>
                            </div>

                            <!-- Total Copies -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Total Copies <span style="color: red">*</span></label>
                                <asp:TextBox ID="txtTotalCopies" runat="server" CssClass="form-control" placeholder="Enter Total Copies"
                                    MaxLength="10" oninput="calculateAmounts();">   </asp:TextBox>
                            </div>
                            <!-- Total Price-->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Total Price<span style="color: red">*</span> </label>
                                <asp:TextBox ID="txtTotalPrice" runat="server"
                                    CssClass="form-control" MaxLength="8" ReadOnly="true" onkeypress="return allowPriceChars(event);"
                                    onpaste="return false;"></asp:TextBox>
                            </div>
                            <!-- Tax-->
                            <div class="col-md-4 mb-3">
                                <label class="form-label d-block">Add Tax</label>
                                <div style="display: flex; align-items: flex-start; gap: 5px;">
                                    <!-- Checkbox + Label -->
                                    <div class="form-check ps-0 pt-2">
                                        <asp:CheckBox ID="chkTax" runat="server" onclick="toggleTaxBox(this);" />
                                        <label class="form-check-label ms-2" for="chkTax">Include Tax</label>
                                    </div>

                                    <!-- Tax Amount Textbox (Right Side) -->
                                    <div id="taxBoxContainer" style="display: none; min-width: 180px;">
                                        <asp:TextBox ID="txtTaxPercent" runat="server"
                                            CssClass="form-control"
                                            placeholder="Tax % e.g. 7"
                                            MaxLength="8"
                                            onkeypress="return allowPriceChars(event);"
                                            onpaste="return false;"
                                            oninput="calculateAmounts();">
                                        </asp:TextBox>
                                    </div>
                                </div>

                                <!-- Error Message -->
                                <span id="taxError" style="color: red; display: none;">Tax Percent is required
                                </span>

                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Tax Amount</label>
                                <input type="text" id="txtTaxAmount" runat="server" class="form-control" readonly="readonly" />
                            </div>
                            <!--fine amount-->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Final Amount<span style="color: red">*</span> </label>
                                <input type="text" id="txtfinalAmount" runat="server" class="form-control" readonly="readonly" />
                            </div>
                        </div>

                        <h5 class="text-primary mt-1 mb-3 border-bottom pb-1 fw-bold">Uploads</h5>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Upload Receipt  </label>
                                <asp:FileUpload ID="fureceipt" runat="server" CssClass="form-control" />
                                <small class="text-muted">Allowed: jpg, jpeg, png| Max size: 2MB
                                </small>
                                <div id="receiptContainer" runat="server">
                                </div>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="form-label">Upload eBook</label>
                                <asp:FileUpload ID="fuEbook" runat="server" CssClass="form-control" accept=".pdf,.ppt,.pptx" />
                                <small class="text-muted">Allowed: PDF, PPT, PPTX | Max size: 10MB</small>
                                <span id="ebookError" class="text-danger" style="display: none;"></span>
                            </div>
                        </div>

                        <h5 class="text-primary mt-1 mb-3 border-bottom pb-1 fw-bold">Other Details</h5>
                        <div class="row">
                            <!-- Shelf Location -->
                            <div class="col-md-4 mb-3">
                                <label class="form-label">
                                    Shelf Location
                                </label>
                                <asp:TextBox ID="txtShelfLocation" runat="server"
                                    CssClass="form-control" MaxLength="50" placeholder="Enter Shelf Location"></asp:TextBox>
                            </div>
                            <!-- Status -->
                            <div class="col-md-4">
                                <label class="form-label d-block">Status <span style="color: red">*</span></label>
                                <div class="form check form-check-inline ">
                                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                                    <label class="form-check-label ms-2" for="chkActive">Active</label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer text-end">
                        <asp:Button ID="btnSave" runat="server" Text="Save"
                            CssClass="btn btn-primary me-2" OnClientClick="return validateAll();"
                            OnClick="btnSave_Click" />
                        <asp:Button ID="btnUpdate" runat="server" Text="Update"
                            CssClass="btn btn-success me-2" OnClientClick="return validateAll();"
                            OnClick="Update_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="Clear_Click"
                            CssClass="btn btn-warning me-2" OnClientClick="clearForm(); return false;" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                            CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                    </div>
                </div>
                <div class="modal fade" id="purchaseModal" tabindex="-1">
                    <div class="modal-dialog modal-lg modal-dialog-centered">
                        <div class="modal-content shadow border-0">

                            <!-- Header -->
                            <div class="modal-header bg-success text-white ">
                                <h6 class="modal-title ">New Book Purchase -
                                    <asp:Label ID="lblModalTitle" runat="server"></asp:Label></h6>

                                <button type="button" class="btn-close btn-close-white pt-0 " data-bs-dismiss="modal"></button>
                            </div>

                            <!-- Body -->
                            <div class="modal-body">

                                <div class="row g-3">

                                    <!-- Total Copies -->
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">
                                            Price <span
                                                style="color: red">*</span></label>
                                        <asp:TextBox ID="copyPrice" runat="server" CssClass="form-control" placeholder="Price per book" MaxLength="8" onkeypress="return allowPriceChars(event);"
                                            onpaste="return false;" oninput="calculatePurchaseAmount();">
                                        </asp:TextBox>
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">
                                            Total Copies <span
                                                style="color: red">*</span></label>
                                        <asp:TextBox ID="textTotalCopies" runat="server" CssClass="form-control" placeholder="Enter Total Copies"
                                            MaxLength="10" oninput="calculatePurchaseAmount();">
                                        </asp:TextBox>
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">
                                            Total Price<span
                                                style="color: red">*</span>

                                        </label>
                                        <asp:TextBox ID="PriceAmount" runat="server"
                                            CssClass="form-control" onkeypress="return allowPriceChars(event);"
                                            ReadOnly="true" onpaste="return false;">
                                        </asp:TextBox>
                                    </div>
                                    <div class="col-md-6 mb-">

                                        <label class="form-label d-block">Add Tax</label>

                                        <div style="display: flex; gap: 5px; width: 100%">

                                            <!-- Checkbox + Label -->
                                            <div class="form-check ps-0 pt-2">
                                                <asp:CheckBox ID="chkTaxIn" runat="server" onclick="toggleTaxBox1(this);" />
                                                <label class="form-check-label ms-2" for="chkTax">Include Tax</label>
                                            </div>

                                            <!-- Tax Amount Textbox (Right Side) -->
                                            <div id="taxContainerBox" style="display: none; min-width: 200px;">
                                                <asp:TextBox ID="txtPercent" runat="server"
                                                    CssClass="form-control"
                                                    placeholder="Tax % e.g. 5"
                                                    MaxLength="8"
                                                    onkeypress="return allowPriceChars(event);"
                                                    onpaste="return false;"
                                                    oninput="calculatePurchaseAmount();">
                                                </asp:TextBox>
                                            </div>

                                        </div>

                                        <!-- Error Message -->
                                        <span id="tax1Error" style="color: red; display: none;">Tax amount is required
                                        </span>

                                    </div>

                                    <!-- Add Tax -->
                                    <div class="col-md-6">
                                        <label class="form-label">Tax Amount</label>
                                        <input type="text" id="txtAmount" runat="server" class="form-control" readonly />
                                    </div>

                                    <!-- Final Amount -->
                                    <div class="col-md-6">
                                        <label class="form-label">Final Amount</label>
                                        <input type="text" id="finalAmount" runat="server" class="form-control" readonly />
                                    </div>

                                    <!-- Upload -->
                                    <div class="col-md-6">
                                        <label class="form-label">Upload Receipt</label>
                                        <asp:FileUpload ID="AdditionalPurchaseReceipt" runat="server" CssClass="form-control" />
                                        <small class="text-muted">jpg, jpeg, png| Max size: 2MB</small>
                                    </div>

                                </div>

                                <!-- Error -->
                                <div id="formError" class="text-danger mt-2" style="display: none;"></div>

                            </div>

                            <!-- Footer -->
                            <div class="modal-footer py-2">

                                <asp:Button ID="btnSavePurchase" runat="server"
                                    Text="Save Purchase"
                                    CssClass="btn btn-success"
                                    OnClick="btnPurchaseSave_Click"
                                    UseSubmitBehavior="false"
                                    OnClientClick="if (!validatePurchase()) return false;" />
                                <button type="button" class="btn btn-outline-secondary"
                                    data-bs-dismiss="modal">
                                    Cancel
                                </button>
                            </div>

                        </div>
                    </div>
                </div>
                <!-- Overlay -->
                <div id="drawerOverlay" class="drawer-overlay" onclick="closeDrawer()"></div>
                <!-- Drawer -->
                <div id="receiptDrawer" class="drawer">
                    <div class="drawer-header">
                        <h5>Receipts</h5>
                        <span class="close-btn" onclick="closeDrawer()">✖</span>
                    </div>

                    <div id="drawerContent" class="p-2" runat="server" clientidmode="Static"></div>
                    <!-- Receipts will load here -->
                </div>
            </div>
        </div>
    </form>
    <uc:footer id="Footer1" runat="server" />
    <script src="../assets/js/select2/select2.full.min.js"></script>
    <script src="../assets/js/select2/select2-custom.js"></script>
    <script src="../assets/js/bootstrap-multiselect.min.js"></script>
    <script src="../assets/js/custom-inputsearch.js"></script>
    <script src="../assets/js/Sweetalert2.js"></script>

    <!-- Client-side validation and selectpicker init -->
    <script>
        $(document).ready(function () {
            $('#lstMainAuthor').multiselect({
                enableFiltering: true,
                buttonTextAlignment: 'left'
            });
        });
        $(document).ready(function () {
            $('#lstCoAuthor').multiselect({
                enableFiltering: true,
                buttonTextAlignment: 'left',

            });
        });
        function validateAll() {
            return validateBookMaster()
                && validateFiles();
        }
        function validateFiles() {

            var receipt = document.getElementById('<%= fureceipt.ClientID %>');
            var ebook = document.getElementById('<%= fuEbook.ClientID %>');
            var ebookError = document.getElementById("ebookError");

            // Receipt validation
            if (receipt.files.length > 0) {
                var file = receipt.files[0];
                var allowedTypes = ["image/jpeg", "image/png"];

                if (!allowedTypes.includes(file.type)) {
                    AlertMessage("Invalid receipt file type (jpg, png)", "error");
                    return false;
                }

                var sizeKB = file.size / 1024;
                if (sizeKB > 2048) {
                    AlertMessage("Receipt file size must not exceed 2 MB.", "error");
                    return false;
                }
            }

            // Ebook validation
            if (ebook.files.length > 0) {
                var file = ebook.files[0];
                var ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();

                var allowedExt = [".pdf", ".ppt", ".pptx"];

                if (!allowedExt.includes(ext)) {
                    ebookError.innerText = "Invalid ebook format";
                    ebookError.style.display = "block";
                    return false;
                }

                var sizeMB = file.size / (1024 * 1024);
                if (sizeMB > 10) {
                    ebookError.innerText = "Max size is 10MB";
                    ebookError.style.display = "block";
                    return false;
                }

                ebookError.style.display = "none";
            }

            return true;
        }
        function toggleTaxBox1(checkbox) {
            var container = document.getElementById("taxContainerBox");
            var taxBox = document.getElementById('<%= txtPercent.ClientID %>');
            var error = document.getElementById("tax1Error");

            if (checkbox.checked) {
                container.style.display = "block";
                taxBox.focus();
            } else {
                container.style.display = "none";
                taxBox.value = "";
                error.style.display = "none";
            }

            calculatePurchaseAmount();
        }
        function validateTax1() {
            var chk = document.getElementById('<%= chkTaxIn.ClientID %>');
            var taxPercentBox = document.getElementById('<%= txtPercent.ClientID %>');
            var error = document.getElementById("taxError");

            if (chk.checked) {
                var tax = taxPercentBox.value.trim();

                if (tax === "") {
                    error.innerText = "Tax percentage is required";
                    error.style.display = "inline";
                    taxPercentBox.focus();
                    return false;
                }

                if (isNaN(tax) || tax <= 0 || tax > 100) {
                    error.innerText = "Enter valid tax % (1 - 100)";
                    error.style.display = "inline";
                    taxPercentBox.focus();
                    return false;
                }
            }

            error.style.display = "none";
            return true;
        }
        function validateBookMaster() {

            function showErr(msg, id) {
                AlertMessage(msg, "error");
                if (id) document.getElementById(id).focus();
                return false;
            }

            var title = document.getElementById('<%= txtBookTitle.ClientID %>').value.trim();
            if (title === "") return showErr("Book Title is required.", '<%= txtBookTitle.ClientID %>');

            var isbn = document.getElementById('<%= txtISBN.ClientID %>').value.trim();
            if (isbn === "") return showErr("ISBN is required.", '<%= txtISBN.ClientID %>');
            if (!/^[A-Za-z0-9-]+$/.test(isbn)) return showErr("Invalid ISBN.", '<%= txtISBN.ClientID %>');
            var category = $('#<%= ddlCategory.ClientID %>').val();
            if (!category) return showErr("Select category.");
            var authors = $('#<%= lstMainAuthor.ClientID %>').val();
            if (!authors || authors.length === 0) return showErr("Select at least one author.");

            var language = document.getElementById('<%= ddlLanguage.ClientID %>');
            if (language.selectedIndex === 0) {
                return showErr("Select Language.", '<%= ddlLanguage.ClientID %>');
            }

            /* ✅ ---------------- Publisher Name ---------------- */
            var publisher = document.getElementById('<%= txtPublisher.ClientID %>').value.trim();
            if (publisher === "") {
                return showErr("Publisher name is required.", '<%= txtPublisher.ClientID %>');
            }

            var publisherPattern = /^[A-Za-z0-9 .,&-]+$/;
            if (!publisherPattern.test(publisher)) {
                return showErr("Invalid Publisher Name.", '<%= txtPublisher.ClientID %>');
            }

            var year = document.getElementById('<%= txtYearPublished.ClientID %>').value.trim();
            if (year === "" || !/^\d{4}$/.test(year))
                return showErr("Enter valid Year (YYYY).", '<%= txtYearPublished.ClientID %>');
            /* ✅ ---------------- Edition ---------------- */
            var edition = document.getElementById('<%= txtEdition.ClientID %>').value.trim();

            if (edition === "") {
                return showErr("Edition is required.", '<%= txtEdition.ClientID %>');
            }

            // Match number + suffix
            var match = edition.toLowerCase().match(/^(\d+)(st|nd|rd|th)$/);

            if (!match) {
                return showErr("Invalid Edition format (use 1st, 2nd, 3rd...)", '<%= txtEdition.ClientID %>');
            }

            var number = parseInt(match[1], 10);
            var suffix = match[2];

            // Correct suffix logic
            var expected;

            if (number % 100 >= 11 && number % 100 <= 13) {
                expected = "th";
            } else {
                switch (number % 10) {
                    case 1: expected = "st"; break;
                    case 2: expected = "nd"; break;
                    case 3: expected = "rd"; break;
                    default: expected = "th"; break;
                }
            }

            if (suffix !== expected) {
                return showErr("Invalid Edition suffix (e.g., 1st, 2nd, 3rd, 4th)", '<%= txtEdition.ClientID %>');
            }
            var total = document.getElementById('<%= txtTotalCopies.ClientID %>').value.trim();
            if (total == "") {
                return showErr("Total Copies is Required", '<%= txtTotalCopies.ClientID %>');
            }
            if (!/^[0-9]+$/.test(total))
                return showErr("Total Copies must be whole number.", '<%= txtTotalCopies.ClientID %>');

            // ✅ Receipt validation added to main validation
            //if (!validateReceipt()) return false;
            if (!validateTax()) return false;
            return true;
        }
        function allowPriceChars(evt) {
            const key = evt.key;
            const input = evt.target.value;

            if (key === "Backspace" || key === "Delete" || key === "Tab" ||
                key === "ArrowLeft" || key === "ArrowRight") {
                return true;
            }

            if (key >= '0' && key <= '9') {
                if (input.includes('.')) {
                    const parts = input.split('.');
                    if (evt.target.selectionStart > input.indexOf('.')) {
                        return parts[1].length < 2;
                    }
                }
                return true;
            }

            if (key === '.') {
                return !input.includes('.');
            }

            return false;
        }
        function toggleTaxBox(checkbox) {
            var container = document.getElementById("taxBoxContainer");
            var taxPercentBox = document.getElementById('<%= txtTaxPercent.ClientID %>');
            var error = document.getElementById("taxError");

            if (checkbox.checked) {
                container.style.display = "block";
                taxPercentBox.focus();
            } else {
                container.style.display = "none";
                // taxPercentBox.value = ""; ❌ removed
                document.getElementById('<%= txtTaxAmount.ClientID %>').value = "0.00";
                error.style.display = "none";
            }

            calculateAmounts();
        }
        function validateTax() {
            var chk = document.getElementById('<%= chkTax.ClientID %>');
            var taxPercentBox = document.getElementById('<%= txtTaxPercent.ClientID %>');
            var error = document.getElementById("taxError");

            if (chk.checked) {
                var tax = taxPercentBox.value.trim();

                if (tax === "") {
                    error.innerText = "Tax percentage is required";
                    error.style.display = "inline";
                    taxPercentBox.focus();
                    return false;
                }

                if (isNaN(tax) || tax <= 0 || tax > 100) {
                    error.innerText = "Enter valid tax % (1 - 100)";
                    error.style.display = "inline";
                    taxPercentBox.focus();
                    return false;
                }
            }

            error.style.display = "none";
            return true;
        }
        function openPurchaseModal() {
            var myModal = new bootstrap.Modal(document.getElementById('purchaseModal'));
            myModal.show();
        }
        function validatePurchase() {

            function showErr(msg, id) {
                var errorDiv = document.getElementById("formError");
                errorDiv.innerText = msg;
                errorDiv.style.display = "block";

                if (id) {
                    document.getElementById(id).focus();
                }

                return false;
            }

            // ✅ Price validation
            var price = document.getElementById('<%= copyPrice.ClientID %>').value.trim();

            if (price === "" || isNaN(price) || parseFloat(price) <= 0) {
                return showErr("Enter valid price", '<%= copyPrice.ClientID %>');
            }

            // ✅ Total copies validation
            var copies = document.getElementById('<%= textTotalCopies.ClientID %>').value.trim();

            if (!/^[0-9]+$/.test(copies)) {
                return showErr("Enter valid total copies", '<%= textTotalCopies.ClientID %>');
            }

            // ✅ Tax validation
            var chk = document.getElementById('<%= chkTaxIn.ClientID %>');
            var taxBox = document.getElementById('<%= txtPercent.ClientID %>');

            if (chk.checked) {
                var tax = taxBox.value.trim();

                if (tax === "") {
                    return showErr("Tax % is required", '<%= txtPercent.ClientID %>');
                }

                if (isNaN(tax) || tax <= 0 || tax > 100) {
                    return showErr("Enter valid tax % (1–100)", '<%= txtPercent.ClientID %>');
                }
            }

            // ✅ Clear error if everything is valid
            document.getElementById("formError").style.display = "none";

            return true;
        }
        function calculatePurchaseAmount() {

            var price = parseFloat(document.getElementById('<%= copyPrice.ClientID %>').value) || 0;
            var copies = parseInt(document.getElementById('<%= textTotalCopies.ClientID %>').value) || 0;

            var totalPriceBox = document.getElementById('<%= PriceAmount.ClientID %>');
            var finalAmountBox = document.getElementById('finalAmount');
            var taxAmountBox = document.getElementById('txtAmount');

            var chkTax = document.getElementById('<%= chkTaxIn.ClientID %>');
            var taxBox = document.getElementById('<%= txtPercent.ClientID %>');

            var total = price * copies;
            totalPriceBox.value = total.toFixed(2);

            var taxAmount = 0;

            if (chkTax.checked) {
                var taxPercent = parseFloat(taxBox.value);
                if (!isNaN(taxPercent) && taxPercent > 0) {
                    taxAmount = (total * taxPercent) / 100;
                }
            }

            taxAmountBox.value = taxAmount.toFixed(2);

            var finalAmount = total + taxAmount;
            finalAmountBox.value = finalAmount.toFixed(2);
        }
        function calculateAmounts() {

            var price = parseFloat(document.getElementById('<%= txtPrice.ClientID %>').value) || 0;
            var copies = parseInt(document.getElementById('<%= txtTotalCopies.ClientID %>').value) || 0;

            var totalPriceBox = document.getElementById('<%= txtTotalPrice.ClientID %>');
            var finalAmountBox = document.getElementById('<%= txtfinalAmount.ClientID %>');

            var chkTax = document.getElementById('<%= chkTax.ClientID %>');
            var taxPercentBox = document.getElementById('<%= txtTaxPercent.ClientID %>');
            var taxAmountBox = document.getElementById('<%= txtTaxAmount.ClientID %>');

            // Total Price
            var total = price * copies;
            totalPriceBox.value = total > 0 ? total.toFixed(2) : '';

            var taxAmount = 0;

            // Tax Calculation
            if (chkTax.checked) {
                var taxPercent = parseFloat(taxPercentBox.value) || 0;

                if (taxPercent > 0) {
                    taxAmount = (total * taxPercent) / 100;
                }
            }
            // Tax Amount
            taxAmountBox.value = taxAmount > 0 ? taxAmount.toFixed(2) : '';

            // Final Amount
            var finalAmount = total + taxAmount;
            finalAmountBox.value = finalAmount > 0 ? finalAmount.toFixed(2) : '';
        }

        function deleteReceipt(receiptId, bookId) {

            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to undo this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {

                if (result.isConfirmed) {

                    $.ajax({
                        type: "POST",
                        url: "BookMaster.aspx/DeleteReceipt",
                        data: JSON.stringify({ receiptId: receiptId }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        success: function (response) {

                            if (response.d == 1) {

                                Swal.fire({
                                    icon: "success",
                                    title: "Deleted!",
                                    text: "Receipt has been deleted.",
                                    timer: 1500,
                                    showConfirmButton: false
                                });

                                setTimeout(() => {
                                    location.reload();
                                }, 1500);

                            } else {

                                Swal.fire({
                                    icon: "error",
                                    title: "Failed",
                                    text: "Delete failed."
                                });

                            }
                        },

                        error: function () {

                            Swal.fire({
                                icon: "error",
                                title: "Error",
                                text: "Error deleting receipt."
                            });

                        }
                    });

                }
            });
        }
        function downloadReceipt(path) {

            if (!path) {
                alert("File not found");
                return;
            }

            // redirect to handler
            window.location.href = "ResolveUrl(path)?file=" + encodeURIComponent(path);
        }

        function openDrawer() {
            document.getElementById("receiptDrawer").classList.add("open");
            document.getElementById("drawerOverlay").classList.add("show");
        }

        function closeDrawer() {
            document.getElementById("receiptDrawer").classList.remove("open");
            document.getElementById("drawerOverlay").classList.remove("show");
        }

        window.onload = function () {

            var chk = document.getElementById('<%= chkTax.ClientID %>');
            var container = document.getElementById("taxBoxContainer");

            // ✅ Restore tax box visibility
            if (chk.checked) {
                container.style.display = "block";
            } else {
                container.style.display = "none";
            }

            // ✅ Recalculate all values
            calculateAmounts();
        };
    </script>

</body>
</html>
