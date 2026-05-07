<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequestDetails.aspx.cs" Inherits="Admin.RequestDetials" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Renewal Requests</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../assets/css/vendors/flatpickr/flatpickr.min.css" />

    <style>
        .summary-card {
            border-radius: 12px;
            color: #333;
            min-height: 120px;
            cursor: pointer;
            transition: transform 0.25s ease, box-shadow 0.25s ease;
            border-radius: 12px;
        }

            .summary-card:hover {
                transform: scale(1.04); /* very light increase */
                box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
            }

            .summary-card h6 {
                color: #555;
            }

            .summary-card h2 {
                color: #222;
            }

        .icon {
            width: 75px;
            height: 70px;
            filter: brightness(0);
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            /* keeps black icons clean */
        }
        /*        .summary-card {
    transition: all 0.3s ease;
    cursor: pointer;
}*/
        .summary-card.selected-card {
            opacity: 1 !important;
            transform: none !important;
            box-shadow: none !important;
        }




        /* Background colors */
        /* 🔄 Renewal Requests (Blue – action/info) */
        .bg-renewals {
            background: linear-gradient(135deg, #3f87ff, #c9dcff);
        }

        /* 📥 Pending Requests (Amber – attention) */
        .bg-requests {
            background: linear-gradient(135deg, #f9a825, #fff3cd);
        }

        /* ✅ Approved (Green – success) */
        .bg-approve {
            background: linear-gradient(135deg, #2e7d32, #c8e6c9);
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

        .search-wrapper {
            position: relative;
        }

            .search-wrapper input {
                padding-right: 30px;
            }

        .clear-btn {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: red;
            display: none;
        }

            .clear-btn:hover {
                color: #a71d2a;
            }


        .search-wrapper input:not(:placeholder-shown) ~ .clear-btn {
            display: block;
        }

        .hide-clear-filter {
            display: none !important;
        }

        .input-clear-wrapper {
            position: relative;
        }

        .clear-input {
            padding-right: 28px;
        }

        .clear-btn-inside {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            display: none;
            color: #dc3545;
            font-size: 14px;
        }
    </style>

</head>

<body>
    <uc:header id="Header" runat="server" />
    <form id="form1" runat="server">
        <div class="page-body">
            <div class="container-fluid pt-2">
                <asp:HiddenField ID="hdnRejectRequestID" runat="server" />
                <asp:HiddenField runat="server" ID="hfRemoveColumnsCSV" />

                <div id="divSummary" runat="server" class="row g-4">
                    <!-- Borrowed Books -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardAllRenewals_Click">
                            <div class="card summary-card bg-renewals shadow-sm" id="cardAll" runat="server">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div class="text-start">
                                        <h4 class="mb-1">Total Renewal Requests</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblRenewalsCount" runat="server" Text="0"></asp:Label>
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/AllRequest.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <!-- Returned Books -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardNewRequests_Click">
                            <div class="card summary-card bg-requests shadow-sm" id="cardNew" runat="server">
                                <div class="card-body d-flex align-items-center justify-content-between ">
                                    <div class="text-start">
                                        <h4 class="mb-1">Yet To Be Process</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblRequestCount" runat="server" Text="0"></asp:Label></h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/Renewal.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <!-- Due Date -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardApproved_Click">
                            <div class="card summary-card bg-approve shadow-sm" id="cardProcessed" runat="server">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div class="text-start">
                                        <h4 class="mb-1">Processed</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblApproveCount" runat="server" Text="0"></asp:Label>
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/approve.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                </div>
                <asp:LinkButton
                    ID="btnFilterPostBack"
                    runat="server"
                    OnClick="btnSearch_Click"
                    Style="display: none" />


                <div class="card" id="divGrid" runat="server" visible="true">
                    <div class="card-header bg-primary p-2">
                        <div class="row align-items-center justify-content-between">
                            <div class="col-auto">
                                <h4 class="card-title mb-0" id="lblGridTitle" runat="server">All Renewals</h4>
                            </div>
                            <div class="col-auto text-end">
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
                                        Width="35" Height="35" />
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="card">
                        <div class="card-body pt-0">
                            <div class="col-12 col-lg-auto ms-lg-auto text-lg-end p-1">
                                <asp:Label ID="lblRecordCount" runat="server"
                                    CssClass="fw-bold text-primary"></asp:Label>
                            </div>
                            <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                                <asp:GridView
                                    ID="gvRenewals" runat="server" AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-striped text-center"
                                    EmptyDataText="No records found" AllowPaging="True" PageSize="5"
                                    PagerSettings-Visible="false" DataKeyNames="RenewalRequestID"
                                    OnRowCommand="gvRenewals_RowCommand" OnPageIndexChanging="gvRenewals_PageIndexChanging">
                                    <EmptyDataTemplate>
                                        <div class="text-center py-4">
                                            <i class="bi bi-search fs-2 text-muted"></i>

                                            <div class="mt-2 fw-semibold text-muted">
                                                No results found
                                           
                                            </div>

                                            <div class="mt-3">
                                                <asp:LinkButton
                                                    ID="btnEmptyRefresh"
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
                                                <div class="fw-bold text-center">S.No</div>

                                                <div class="d-flex justify-content-center gap-2 mt-1">
                                                    <asp:LinkButton ID="btnApplyFilter"
                                                        runat="server"
                                                        CssClass="text-primary"
                                                        ToolTip="Filter"
                                                        OnClientClick="toggleClearFilterIcon();"
                                                        OnClick="btnSearch_Click">
                                                    <i class="fa-solid fa-filter"></i>
                                                    </asp:LinkButton>


                                                    <asp:LinkButton ID="btnClearFilter"
                                                        runat="server"
                                                        ToolTip="Clear Filter"
                                                        CssClass="text-danger clear-filter-btn"
                                                        Style="display: none;"
                                                        OnClick="btnRefresh_Click">
                                                        <i class="fa-solid fa-circle-xmark"></i>
                                                    </asp:LinkButton>

                                                </div>
                                            </HeaderTemplate>

                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:BoundField DataField="BookIssueID" Visible="false" />


                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="fw-bold">Requested Type</div>

                                                <div class="input-clear-wrapper">
                                                    <asp:TextBox ID="txtFilterRequestedType"
                                                        runat="server"
                                                        CssClass="form-control form-control-sm clear-input"
                                                        placeholder="Search Type"
                                                        onkeyup="toggleClearIcon(this);" />

                                                    <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                                </div>
                                            </HeaderTemplate>
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Requested Type") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="fw-bold">Requested Member</div>

                                                <div class="input-clear-wrapper">
                                                    <asp:TextBox ID="txtFilterRequestedMember"
                                                        runat="server"
                                                        CssClass="form-control form-control-sm clear-input"
                                                        placeholder="Search Member"
                                                        onkeyup="toggleClearIcon(this);" />

                                                    <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                                </div>
                                            </HeaderTemplate>
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Requested Member") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <div class="fw-bold">ISBN</div>

                                                <div class="input-clear-wrapper">
                                                    <asp:TextBox ID="txtFilterISBN"
                                                        runat="server"
                                                        CssClass="form-control form-control-sm clear-input"
                                                        placeholder="Search ISBN"
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

                                                <div class="input-clear-wrapper">
                                                    <asp:TextBox ID="txtFilterBookTitle"
                                                        runat="server"
                                                        CssClass="form-control form-control-sm clear-input"
                                                        placeholder="Search Title"
                                                        onkeyup="toggleClearIcon(this);" />

                                                    <span class="clear-btn-inside" onclick="clearSearch(this)">✖</span>
                                                </div>
                                            </HeaderTemplate>
                                             <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("Book Title") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Authors">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("AuthorNames") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Edition" HeaderText="Edition" />


                                        <asp:TemplateField HeaderText="Current Due Date">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Current Due Date", "{0:dd-MMM-yyyy}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Requested Due Date">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Requested Due Date", "{0:dd-MMM-yyyy}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Approved Due Date">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Approved Due Date", "{0:dd-MMM-yyyy}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Requested Days">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Requested Days Count") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Last Renewal Status">

                                            <ItemTemplate>
                                                <span class='badge
            <%# Eval("Last Renewal Status").ToString() == "Approved" ? "bg-primary" :
                Eval("Last Renewal Status").ToString() == "Rejected" ? "bg-danger" :
                Eval("Last Renewal Status").ToString() == "Pending" ? "bg-warning" :
                "bg-secondary" %>'>
                                                    <%# Eval("Last Renewal Status") %>
                                                </span>
                                            </ItemTemplate>
                                            <HeaderStyle CssClass="wrap-header" />
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Requested On">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <%# Eval("Requested On", "{0:dd-MMM-yyyy}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Approve Till">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtApprovedDueDate"
                                                    runat="server"
                                                    CssClass="form-control form-control-sm flatpickr-date wrap-header"
                                                    Text='<%# Eval("Requested Due Date", "{0:dd-MMM-yyyy}") %>'
                                                    Visible='<%# (int)Eval("RequestStatus") == 1 %>'
                                                    onkeydown="return false;"
                                                    onpaste="return false;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Action">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnApprove"
                                                    runat="server"
                                                    CssClass="btn btn-success btn-sm me-1"
                                                    CommandName="Approve"
                                                    CommandArgument='<%# Eval("RenewalRequestID") %>'
                                                    ToolTip="Approve"
                                                    CausesValidation="false"
                                                    Visible='<%# (int)Eval("RequestStatus") == 1 %>'>
                                                   <i class="fa-solid fa-check"></i>
                                                </asp:LinkButton>

                                                <asp:LinkButton
                                                    ID="btnReject"
                                                    runat="server"
                                                    CssClass="btn btn-danger btn-sm"
                                                    CommandName="Reject"
                                                    CommandArgument='<%# Eval("RenewalRequestID") %>'
                                                    OnClientClick='<%# "openRejectModal(" + Eval("RenewalRequestID") + "); return false;" %>'
                                                    Visible='<%# (int)Eval("RequestStatus") == 1 %>'>
                                                    <i class="fa-solid fa-xmark"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Rejected Reason">

                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("Rejected Reason") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>


                            </div>
                            <div class="pager-fixed d-flex justify-content-end">
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

                <div class="modal fade" id="rejectModal" tabindex="-1">
                    <div class="modal-dialog">
                        <div class="modal-content">

                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title">Reject Renewal Request</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>

                            <div class="modal-body">
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Reject Reason</label>
                                    <%--<asp:TextBox
                                        ID="txtRejectReason"
                                        runat="server"
                                        CssClass="form-control"
                                        TextMode="MultiLine"
                                        Rows="4"
                                        placeholder="Enter reject reason...">
                                    </asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlRejectReason" runat="server" CssClass="form-select" AppendDataBoundItems="true">

                                        <asp:ListItem Text="-- Select Reject Reason --" Value="" />
                                        <asp:ListItem Text="Book is reserved by another member" Value="Book is reserved by another member" />
                                        <asp:ListItem Text="Renewal limit exceeded" Value="Renewal limit exceeded" />
                                        <asp:ListItem Text="Book is overdue" Value="Book is overdue" />
                                        <asp:ListItem Text="Outstanding fine pending" Value="Outstanding fine pending" />
                                        <asp:ListItem Text="Book is non-renewable" Value="Book is non-renewable" />
                                        <asp:ListItem Text="Membership inactive or expired" Value="Membership inactive or expired" />
                                        <asp:ListItem Text="Book already returned" Value="Book already returned" />
                                        <asp:ListItem Text="Book requested by faculty/staff" Value="Book requested by faculty/staff" />
                                        <asp:ListItem Text="Special collection – renewal not allowed" Value="Special collection – renewal not allowed" />
                                        <asp:ListItem Text="Library policy does not allow renewal" Value="Library policy does not allow renewal" />
                                        <asp:ListItem Text="Renewal request submitted too late" Value="Renewal request submitted too late" />
                                        <asp:ListItem Text="Maximum issue period reached" Value="Maximum issue period reached" />
                                        <asp:ListItem Text="Item required for inventory check" Value="Item required for inventory check" />
                                        <asp:ListItem Text="Administrative decision" Value="Administrative decision" />
                                        <asp:ListItem Text="Technical or system issue" Value="Technical or system issue" />
                                        <asp:ListItem Text="Incorrect request details" Value="Incorrect request details" />
                                        <asp:ListItem Text="Manual override by librarian" Value="Manual override by librarian" />
                                        <asp:ListItem Text="Other (see remarks)" Value="Other (see remarks)" />

                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                    Cancel
                               
                                </button>

                                <asp:Button
                                    ID="btnConfirmReject"
                                    runat="server"
                                    CssClass="btn btn-danger"
                                    Text="Reject"
                                    OnClick="btnConfirmReject_Click" OnClientClick="return validateRejectReason();" />
                            </div>

                        </div>
                    </div>
                </div>


            </div>
        </div>
    </form>
    <script src="../assets/js/flat-pickr/flatpickr.js"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            flatpickr(".flatpickr-date", {
                dateFormat: "d-M-Y",
                allowInput: false,
                onChange: function (selectedDates, dateStr, instance) {
                    instance.input.value = dateStr.toUpperCase();
                }
            });
        });

        function validateRejectReason() {
            var reason = document.getElementById('<%= ddlRejectReason.ClientID %>').value.trim();

            if (reason === "") {
                AlertMessage('Please enter reject reason.', 'error');
                return false; // stop postback
            }
            return true; // allow postback
        }

        function clearSearch(el) {
            const wrapper = el.closest('.input-clear-wrapper');
            if (!wrapper) return;

            const input = wrapper.querySelector('input');
            if (!input) return;

            input.value = '';
            el.style.display = 'none';
            input.focus();

            toggleClearFilterIcon();
            __doPostBack('<%= btnFilterPostBack.ClientID %>', '');
        }

        function allowOnlyNumbers(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;

            // Allow backspace, delete, arrows
            if (charCode === 8 || charCode === 46 || charCode === 37 || charCode === 39) {
                return true;
            }

            // Allow only digits
            if (charCode < 48 || charCode > 57) {
                evt.preventDefault();
                return false;
            }

            return true;
        }

        function allowAlphaNumeric(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;

            // Allow backspace, space, delete
            if (charCode === 8 || charCode === 32 || charCode === 46) {
                return true;
            }

            // 0-9 A-Z a-z
            if (
                (charCode >= 48 && charCode <= 57) ||
                (charCode >= 65 && charCode <= 90) ||
                (charCode >= 97 && charCode <= 122)
            ) {
                return true;
            }

            evt.preventDefault();
            return false;
        }

        function toggleClearIcon(input) {
            const wrapper = input.closest('.input-clear-wrapper');
            if (!wrapper) return;

            const clearBtn = wrapper.querySelector('.clear-btn-inside');
            if (!clearBtn) return;

            clearBtn.style.display = input.value.trim() ? 'block' : 'none';

            toggleClearFilterIcon();
        }

        function toggleClearFilterIcon() {

            const grid = $('#<%= gvRenewals.ClientID %>');

            const hasValue =
                grid.find('input[id*="txtFilterRequestedType"]').val()?.trim() ||
                grid.find('input[id*="txtFilterISBN"]').val()?.trim() ||
                grid.find('input[id*="txtFilterBookTitle"]').val()?.trim() ||
                grid.find('input[id*="txtFilterRequestedMember"]').val()?.trim();

            const clearBtn = grid.find('.clear-filter-btn');

            hasValue ? clearBtn.show() : clearBtn.hide();
        }
        document.addEventListener("DOMContentLoaded", function () {
            restoreClearIcons();
        });

        function restoreClearIcons() {

            const grid = document.getElementById('<%= gvRenewals.ClientID %>');
            if (!grid) return;

            const inputs = grid.querySelectorAll('input[type="text"], input[type="search"]');

            inputs.forEach(input => {

                // show/hide textbox clear icon
                if (input.value.trim() !== '') {
                    const wrapper = input.closest('.input-clear-wrapper');
                    if (wrapper) {
                        const clearBtn = wrapper.querySelector('.clear-btn-inside');
                        if (clearBtn) clearBtn.style.display = 'block';
                    }
                }
            });

            // show/hide overall clear filter icon
            toggleClearFilterIcon();
        }

    </script>
    <uc:footer id="Footer1" runat="server" />
</body>
</html>
