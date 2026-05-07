<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IssueDashboard.aspx.cs" Inherits="Admin.IssueDashboard" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AdminDashboard</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/customPagination.css" rel="stylesheet" />

    <style>
        .required {
            color: red;
        }

        .summary-card {
            border-radius: 12px;
            color: #333;
            min-height: 120px;
            cursor: pointer;
            transition: transform 0.25s ease, box-shadow 0.25s ease;
            border-radius: 12px;
        }

            .summary-card:hover {
                transform: scale(1.04);
                box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
            }

            .summary-card h4 {
                color: #000000;
                font-weight: 600;
            }

            .summary-card h2 {
                color: #000000;
                font-weight: 700;
            }

        .icon {
            width: 75px;
            height: 70px;
            filter: brightness(0);
            display: flex;
            align-items: center;
            justify-content: center;
            transition: transform 0.3s ease;
        }

        .wrap-header {
            width: 240px;
            white-space: normal;
            word-break: normal;
            overflow-wrap: normal;
        }

        .wraping-cell {
            width: 240px;
            white-space: normal;
            word-break: normal;
            overflow-wrap: normal;
            line-height: 1.5;
            padding: 6px 8px;
            vertical-align: top;
        }

        .modal-dialog {
            max-width: 600px !important;
        }

        #gvBooks th,
        #gvBooks td {
            text-align: center;
            vertical-align: middle;
        }

        .bg-total {
            background: linear-gradient(135deg, #4a90e2, #d6e9ff);
        }

        .bg-issued {
            background: linear-gradient(135deg, #43a047, #dcedc8);
        }

        .bg-due {
            background: linear-gradient(135deg, #ef6c00, #ffe0b2);
        }

        .bg-returned {
            background: linear-gradient(135deg, #c2185b, #f8bbd0);
        }

        .search-wrapper {
            position: relative;
        }

            .search-wrapper .clear-btn {
                position: absolute;
                right: 10px;
                top: 50%;
                transform: translateY(-50%);
                cursor: pointer;
                color: red;
                font-size: 18px;
                display: none;
            }


        .clear-btn-inside {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: red;
            display: none;
        }

            .clear-btn-inside:hover {
                color: #a71d2a;
            }


        .clear-filter-btn {
            cursor: pointer;
            display: none;
            font-size: 16px;
        }

        .hide-clear-filter {
            display: none !important;
        }

        .filter-btn {
            display: inline-block;
            font-size: 16px;
            line-height: 1;
        }
    </style>


</head>

<body>
    <uc1:header runat="server" id="Header" />
    <form id="form1" runat="server">
        <div class="page-body">
            <div class="container-fluid pt-2">
                <asp:HiddenField runat="server" ID="hfRemoveColumnsCSV" />
                <div id="divSummary" runat="server" class="row g-3">

                    <!-- Total Books -->
                    <div class="col-lg-3 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardTotalBooks_Click">
                            <div class="card summary-card bg-total shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">Total Issued Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblTotalBooks" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/Totalbooks.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <!-- Issued Books -->
                    <div class="col-lg-3 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardIssuedBooks_Click">
                            <div class="card summary-card bg-issued shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">Issued Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblIssuedBooks" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/Issuedbooks.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>

                    <!-- Due Books -->
                    <div class="col-lg-3 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardDueBooks_Click">
                            <div class="card summary-card bg-due shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">OverDue Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblDueBooks" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/deadline.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>

                    <!-- Returned Books -->
                    <div class="col-lg-3 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardReturnedBooks_Click">
                            <div class="card summary-card bg-returned shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">Returned Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblReturnedBooks" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/book.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <asp:LinkButton
                ID="btnFilterPostBack"
                runat="server"
                OnClick="btnSearch_Click"
                Style="display: none" />

            <div class="card" id="divGrid" runat="server" visible="true">
                <div class="card-header bg-primary px-3 py-2 d-flex justify-content-between align-items-center">
                    <h4 class="card-title mb-0 text-white" id="lblGridTitle" runat="server">All Renewals
                    </h4>

                    <div class="d-flex align-items-center text-white ms-auto gap-3">
                        <div class="d-flex align-items-center" id="divPageSize" runat="server">
                            <span class="me-2">Show</span>
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                                CssClass="form-select form-select-sm w-auto me-2">
                            </asp:DropDownList>
                            <span>entries</span>
                        </div>

                        <asp:LinkButton
                            ID="lnkDownloadCSV"
                            runat="server"
                            OnClick="btnDownloadCSV_Click"
                            ToolTip="Download CSV">

                            <asp:Image
                                ID="imgDownload"
                                runat="server"
                                ImageUrl="~/assets/images/icons/csvdownload.png"
                                AlternateText="Download"
                                CssClass="img-fluid"
                                Width="35"
                                Height="35" />
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body pt-0">
                    <div class="table-responsive align-items-center pt-2">
                        <asp:GridView
                            ID="gvBooks"
                            runat="server"
                            DataKeyNames="IssueID,Book Title,MemberID,Due Date, OverdueDays, Price"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-striped"
                            EmptyDataText="No records found"
                            AllowPaging="true"
                            PagerSettings-Visible="false"
                            OnRowCommand="gvBooks_RowCommand"
                            ClientIDMode="Static">

                            <EmptyDataTemplate>
                                <div class="text-center py-4">
                                    <i class="bi bi-search fs-2 text-muted"></i>
                                    <div class="mt-2 fw-semibold text-muted">
                                        No results found
                                   
                                    </div>
                                    <div class="mt-3">
                                        <asp:LinkButton ID="btnEmptyRefresh"
                                            runat="server"
                                            CssClass="btn btn-primary btn-sm"
                                            ToolTip="Refresh"
                                            OnClick="btnRefresh_Click">
                                    <i class="bi bi-arrow-clockwise"></i> Refresh
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </EmptyDataTemplate>
                            <Columns>


                                <asp:TemplateField HeaderText="S.No.">
                                    <HeaderTemplate>
                                        <div class="fw-bold text-center">S.No.</div>

                                        <div class="d-flex justify-content-center gap-2 mt-2 align-items-center">
                                            <asp:LinkButton ID="btnApplyFilter"
                                                runat="server"
                                                CssClass="text-primary filter-btn"
                                                ToolTip="Apply Filter"
                                                OnClientClick="toggleClearFilterIcon();"
                                                OnClick="btnSearch_Click">
                                             <i class="fa-solid fa-filter"></i>
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnClearFilter"
                                                runat="server"
                                                CssClass="text-danger clear-filter-btn"
                                                Style="display: none;"
                                                ToolTip="Clear Filter"
                                                OnClick="btnRefresh_Click">
                                             <i class="fa-solid fa-circle-xmark"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </HeaderTemplate>


                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:BoundField DataField="IssueID" Visible="false" />
                                <asp:BoundField DataField="BookID" Visible="false" />


                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="fw-bold">ISBN</div>
                                        <div class="search-wrapper input-clear-wrapper">
                                            <asp:TextBox ID="txtFilterISBN"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                placeholder="Search ISBN"
                                                AutoPostBack="false"
                                                MaxLength="13"
                                                onkeypress="return allowOnlyNumbers(event);"
                                                onkeyup="toggleClearIcon(this);" />
                                            <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                        </div>

                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("ISBN") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="fw-bold">Book Title</div>
                                        <div class="search-wrapper input-clear-wrapper">
                                            <asp:TextBox ID="txtFilterBookTitle"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                placeholder="Search Title"
                                                AutoPostBack="false"
                                                onkeypress="return allowAlphaNumeric(event);"
                                                onkeyup="toggleClearIcon(this);" />

                                            <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("Book Title") %>
                                    </ItemTemplate>
                                </asp:TemplateField>



                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="fw-bold">Member ID</div>
                                        <div class="search-wrapper input-clear-wrapper">
                                            <asp:TextBox ID="txtFilterMemberID"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                placeholder="Search Member ID"
                                                AutoPostBack="false"
                                                onkeyup="toggleClearIcon(this);" />
                                            <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("MemberID") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="fw-bold">Member Type</div>
                                        <div class="search-wrapper input-clear-wrapper">
                                            <asp:TextBox ID="txtFilterMemberType"
                                                runat="server"
                                                CssClass="form-control form-control-sm"
                                                placeholder="Search Member Type"
                                                AutoPostBack="false"
                                                onkeypress="return allowAlphaNumeric(event);"
                                                onkeyup="toggleClearIcon(this);" />
                                            <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("Member Type") %>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:BoundField DataField="Issue Date"
                                    HeaderText="Issued On"
                                    DataFormatString="{0:dd-MMM-yyyy}" />

                                <asp:BoundField DataField="Due Date"
                                    HeaderText="Due Date"
                                    DataFormatString="{0:dd-MMM-yyyy}" />

                                <asp:BoundField DataField="RenewalCount" HeaderText="Renewal Count" />

                                <asp:BoundField DataField="Returned Date"
                                    HeaderText="Returned On"
                                    DataFormatString="{0:dd-MMM-yyyy}" />



                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:Label runat="server"
                                            CssClass='<%# GetStatusCssClass(Container.DataItem) %>'
                                            Text='<%# DataBinder.Eval(Container.DataItem, "Status") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Fine Amount">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkFine"
                                            runat="server"
                                            CommandName="Fine"
                                            CommandArgument='<%# Eval("IssueID") %>'
                                            CssClass="btn btn-sm btn-secondary me-2"
                                            ToolTip="Fine">
                                        <span>Collect Fine</span>
                                        </asp:LinkButton>
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
                    </div>



                </div>
            </div>
        </div>
        <div class="modal fade" id="FineModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">Enter Fine Amount</h5>
                        <button type="button" class="btn-close btn-close-white"
                            data-bs-dismiss="modal">
                        </button>
                    </div>

                    <div class="modal-body">

                        <div class="card mb-3">
                            <div class="row mt-2">
                                <div class="col-6">
                                    <h4 class="fw-bold">
                                        <asp:Label ID="lblFineBookTitle" runat="server" />
                                    </h4>
                                    <h5>
                                        <asp:Label ID="lblOverdueDays"
                                            runat="server"
                                            CssClass="text-danger fw-bold" /></h5>
                                </div>
                                <div class="col-3">
                                    <strong>Member ID</strong><br />
                                    <asp:Label ID="lblFineMemberID" runat="server" />
                                </div>

                                <div class="col-3">
                                    <strong>Due Date</strong><br />
                                    <asp:Label ID="lblFineDueDate" runat="server" />
                                </div>

                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-2">
                                <label class="form-label fw-semibold">
                                    Fine Amount (₹)
                               
                                </label>

                                <asp:TextBox ID="txtFineAmount"
                                    runat="server"
                                    CssClass="form-control"
                                    placeholder="Enter fine amount"
                                    MaxLength="5"
                                    onkeypress="return allowOnlyNumbers(event);" />
                            </div>
                            <div class="col-md-6 mb-2">
                                <strong>Book Price</strong><br />
                                <asp:Label ID="lblBookPrice" runat="server"></asp:Label>
                            </div>
                        </div>

                        <asp:HiddenField ID="hfBookIssueID" runat="server" />
                        <asp:HiddenField ID="hfBookPrice" runat="server" />

                    </div>

                    <!-- Modal Footer -->
                    <div class="modal-footer">
                        <button type="button"
                            class="btn btn-outline-secondary"
                            data-bs-dismiss="modal">
                            Cancel
                       
                        </button>

                        <asp:Button ID="btnCollectFine"
                            runat="server"
                            Text="Collect Fine"
                            CssClass="btn btn-danger"
                            OnClick="btnCollectFine_Click"
                            OnClientClick="return validateFineAmount();" />
                    </div>

                </div>
            </div>
        </div>
    </form>
    <uc1:footer runat="server" id="Footer" />
    <script type="text/javascript">

        document.addEventListener("DOMContentLoaded", function () {
            restoreClearIcons();
        });

        /* Clear individual textbox */
        function clearSearch(el) {
            var wrapper = el.closest('.input-clear-wrapper');
            if (!wrapper) return;

            var input = wrapper.querySelector('input');
            if (!input) return;

            input.value = '';
            el.style.display = 'none';
            input.focus();

            toggleClearFilterIcon();
            __doPostBack('<%=btnFilterPostBack.ClientID%>', '');
        }

        /* Allow only numbers */
        function allowOnlyNumbers(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;
            if (charCode === 8 || charCode === 46 || charCode === 37 || charCode === 39)
                return true;

            if (charCode < 48 || charCode > 57) {
                evt.preventDefault();
                return false;
            }
            return true;
        }

        /* Allow alphanumeric */
        function allowAlphaNumeric(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;

            if (charCode === 8 || charCode === 32 || charCode === 46)
                return true;

            if ((charCode >= 48 && charCode <= 57) ||
                (charCode >= 65 && charCode <= 90) ||
                (charCode >= 97 && charCode <= 122))
                return true;

            evt.preventDefault();
            return false;
        }

        /* Toggle ❌ inside textbox */
        function toggleClearIcon(input) {
            var wrapper = input.closest('.input-clear-wrapper');
            if (!wrapper) return;

            var clearBtn = wrapper.querySelector('.clear-btn-inside');
            if (!clearBtn) return;

            clearBtn.style.display = input.value.trim() !== '' ? 'block' : 'none';

            toggleClearFilterIcon();
        }

        /* Toggle global clear filter (S.No ❌) */
        function toggleClearFilterIcon() {

            var grid = document.getElementById('gvBooks');
            if (!grid) return;

            var hasValue = false;

            var inputs = grid.querySelectorAll(
                'input[id*="txtFilterISBN"],' +
                'input[id*="txtFilterBookTitle"],' +
                'input[id*="txtFilterMemberID"],' +
                'input[id*="txtFilterMemberType"]'
            );

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].value.trim() !== '') {
                    hasValue = true;
                    break;
                }
            }

            var clearFilterBtn = grid.querySelector('.clear-filter-btn');
            if (!clearFilterBtn) return;

            clearFilterBtn.style.display = hasValue ? 'inline-block' : 'none';
        }


        function restoreClearIcons() {

            var grid = document.getElementById('gvBooks');
            if (!grid) return;

            var inputs = grid.querySelectorAll('input[type="text"], input[type="search"]');

            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                if (input.value.trim() !== '') {
                    var wrapper = input.closest('.input-clear-wrapper');
                    if (wrapper) {
                        var clearBtn = wrapper.querySelector('.clear-btn-inside');
                        if (clearBtn) clearBtn.style.display = 'block';
                    }
                }
            }

            toggleClearFilterIcon();
        }

        function validateFineAmount() {

            var fineAmount = parseFloat(document.getElementById('<%= txtFineAmount.ClientID %>').value);
            var bookPrice = parseFloat(document.getElementById('<%= hfBookPrice.ClientID %>').value);

            if (isNaN(fineAmount)) {
                AlertMessage("Please enter fine amount.", "warning");
                return false;
            }

            if (fineAmount > bookPrice) {
                AlertMessage("Fine amount cannot be greater than Book Price.", "error");
                return false;
            }

            return true;
        }

    </script>
</body>
</html>
