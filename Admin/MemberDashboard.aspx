<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberDashboard.aspx.cs" Inherits="Admin.MemberDashboard" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MemberDashboard</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
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
            transition: transform 0.3s ease; /* keeps black icons clean */
        }
        /* Icon animation on hover */


        /* Background colors */
        .bg-borrowed {
            background: linear-gradient(135deg, #4a90e2, #d6e9ff);
        }

        .bg-returned {
            background: linear-gradient(135deg, #43a047, #dcedc8);
        }

        .bg-due {
            background: linear-gradient(135deg, #ef6c00, #ffe0b2);
        }

        /* authors column */
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

        .modal-dialog {
            max-width: 600px !important;
        }



        /* Ensure rows auto-expand */
    </style>
</head>
<body>
    <uc:header id="Header" runat="server" />
    <form id="form1" runat="server">
        <div class="page-body">
            <div class="container-fluid pt-2">
                <asp:HiddenField ID="hdnBookID" runat="server" />
                <asp:HiddenField ID="hdnBookIssueID" runat="server" />


                <asp:ScriptManager ID="ScriptManager1" runat="server" />
                <div class="alert border-warning d-flex align-items-center gap-2 flex-wrap p-2 p-sm-3"
                    id="divOverDueAlert" runat="server" visible="false" role="alert">


                    <!-- Message -->
                    <p class="mb-0 flex-grow-1">
                        <i class="fa-regular fa-clock fs-4 text-warning"></i>
                        You have <b>
                            <asp:Label ID="lblOverDueCount" runat="server"></asp:Label></b> overdue books that have not been returned.
                                     Please return or renew them immediately.
                    </p>
                </div>

                <div id="divSummary" runat="server" class="row g-4">
                    <!-- Borrowed Books -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="Borrowed_Click">
                            <div class="card summary-card bg-borrowed shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div class="text-start">
                                        <h4 class="mb-1">Borrowed Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblBorrowedCount" runat="server" Text="0"></asp:Label>
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/library.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <!-- Returned Books -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="Returned_Click">
                            <div class="card summary-card bg-returned shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div class="text-start">
                                        <h4 class="mb-1">Returned Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblReturnedCount" runat="server" Text="0"></asp:Label></h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/book.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <!-- Due Date -->
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="Due_Click">
                            <div class="card summary-card bg-due shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div class="text-start">
                                        <h4 class="mb-1">Due Books</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblDueCount" runat="server" Text="0"></asp:Label>
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/deadline.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card" id="divGrid" runat="server" visible="true">
                    <div class="card-header bg-primary p-2 d-flex align-items-center justify-content-between flex-wrap">
                        <h4 class="card-title mb-0" id="lblGridTitle" runat="server"></h4>
                        <div class="d-flex align-items-center text-white" id="divPageSize" runat="server">
                            <span class="me-2">Show</span>
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                                CssClass="form-select form-select-sm w-auto me-2">
                            </asp:DropDownList>
                            <span>entries</span>
                        </div>
                    </div>
                    <div class="card">
                        <div class="card-body pt-0">
                            <div class="table-responsive pt-2" style="overflow-x: auto; white-space: nowrap;">
                                <%-- <div class="d-flex justify-content-end">
                                        <asp:Label ID="lblRecordCount"
                                            runat="server" CssClass="fw-bold text-primary"></asp:Label>
                                    </div>--%>
                                <asp:GridView
                                    ID="gvBooks" runat="server"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-striped text-center"
                                    GridLines="None" OnPageIndexChanging="gvBooks_PageIndexChanging" OnRowCommand="gvBooks_RowCommand"
                                    OnRowDataBound="gvBooks_RowDataBound"
                                    EmptyDataText="No records found" AllowPaging="True" PagerSettings-Visible="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="S.No.">
                                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="60px" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="BookIssueID" Visible="false" />
                                        <asp:BoundField DataField="BookID" Visible="false" />

                                        <asp:BoundField DataField="ISBN" HeaderText="ISBN" />
                                        <asp:TemplateField HeaderText="Book Title">
                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("BookTitle") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                        <asp:TemplateField HeaderText="Authors">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("AuthorNames") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Edition" HeaderText="Edition" />

                                        <asp:BoundField DataField="IssueDate" HeaderText="Borrowed Date"
                                            DataFormatString="{0:dd-MMM-yyyy}" />

                                        <asp:BoundField DataField="DueDate" HeaderText="Due Date"
                                            DataFormatString="{0:dd-MMM-yyyy}" />

                                        <asp:BoundField DataField="ReturnDate" HeaderText="Returned Date"
                                            DataFormatString="{0:dd-MMM-yyyy}" />

                                        <asp:TemplateField HeaderText="Pay Fine">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkPayFine"
                                                    runat="server"
                                                    CommandName="PayFine"
                                                    CommandArgument='<%# Eval("BookIssueID") %>'
                                                    CssClass="btn btn-sm btn-danger"
                                                    ToolTip="Pay Fine">
                                                    Pay Fine
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Renewal Count">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate><%# Eval("RenewalCount") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Renewal Request">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkRenewal"
                                                    runat="server"
                                                    CommandName="RenewalRequest"
                                                    CommandArgument='<%# Eval("BookIssueID") %>'
                                                    CssClass="btn btn-sm btn-secondary me-2"
                                                    ToolTip="TIME TO RENEW">
                                                    <span>Renew</span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Last Renewal Status">

                                            <ItemTemplate>
                                                <span class='badge
                                            <%# Eval("Last Renewal Status").ToString() == "Approved" ? "bg-success" :
                                                Eval("Last Renewal Status").ToString() == "Rejected" ? "bg-danger" :
                                                Eval("Last Renewal Status").ToString() == "Pending" ? "bg-warning" :
                                                Eval("Last Renewal Status").ToString() == "Returned" ? "bg-primary" :

                                                "bg-secondary" %>'>
                                                    <%# Eval("Last Renewal Status") %>
                                                </span>
                                            </ItemTemplate>
                                            <HeaderStyle CssClass="wrap-header" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rejected Reason">

                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate>
                                                <%# Eval("RejectReason") %>
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

                <div class="modal fade" id="renewalModal" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">

                            <!-- Modal Header -->
                            <div class="modal-header bg-primary">
                                <h5 class="modal-title">Renewal Request</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>

                            <!-- Modal Body -->
                            <div class="modal-body">
                                <!--  Book Information Card -->
                                <div class="card mb-0">
                                    <div class="card-body">
                                        <div class="row small">
                                            <div class="col-12 mb-2">
                                                <div class="d-flex justify-content-between align-items-start">

                                                    <!-- Title Section -->
                                                    <div style="max-width: 75%;">
                                                        <h6 class="card-title fs-5 mb-1 fw-bold">
                                                            <asp:Label ID="lblBookTitle" runat="server" />
                                                        </h6>

                                                        <p class="text-muted mb-3">
                                                            by
                                                            <asp:Label ID="lblAuthors" runat="server" />
                                                        </p>
                                                    </div>

                                                    <!-- Due Date Fixed Right -->
                                                    <div class="ms-3 flex-shrink-0">
                                                        <span class="badge bg-warning text-dark p-2">Due on
                                                            <asp:Label ID="lblDueDate" runat="server" />
                                                        </span>
                                                    </div>

                                                </div>
                                            </div>

                                        </div>


                                        <div class="row small">
                                            <div class="col-4 mb-2">
                                                <strong>ISBN</strong><br />
                                                <asp:Label ID="lblISBN" runat="server" />
                                            </div>
                                            <div class="col-4 mb-2">
                                                <strong>Category</strong><br />
                                                <asp:Label ID="lblCategory" runat="server" />
                                            </div>
                                            <div class="col-4 mb-2">
                                                <strong>Edition</strong><br />
                                                <asp:Label ID="lblEdition" runat="server" />
                                            </div>

                                        </div>
                                        <div class="row align-items-center">
                                            <div class="col-auto">
                                                <label class="form-label mb-0">
                                                    Extend due date by
                                                </label>
                                            </div>

                                            <div class="col-auto">
                                                <div class="input-group" style="width: 250px;">
                                                    <asp:TextBox ID="txtRenewDays"
                                                        runat="server"
                                                        CssClass="form-control"
                                                        placeholder="e.g. 7"
                                                        EnableViewState="True" />
                                                    <span class="input-group-text">days</span>
                                                </div>
                                            </div>
                                        </div>

                                        <small class="text-muted d-block mt-2">Maximum extension allowed: 14 days
                                        </small>


                                    </div>
                                </div>

                                <!-- 🔁 Renewal Action Card -->


                            </div>

                            <!-- Modal Footer -->
                            <div class="modal-footer">
                                <button type="button" class="btn btn-outline-secondary"
                                    data-bs-dismiss="modal">
                                    Cancel
                                </button>
                                <asp:Button ID="btnSubmitRenewal"
                                    runat="server"
                                    Text="Send Request"
                                    CssClass="btn btn-primary"
                                    OnClick="btnSubmitRenewal_Click"
                                    OnClientClick="return validateRenewalInput();" />
                            </div>

                        </div>
                    </div>
                </div>
                <!-- Renewal Expired Warning Modal -->
                <div class="modal fade" id="renewalExpiredModal" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content shadow">

                            <div class="modal-header bg-warning text-dark">
                                <h5 class="modal-title">
                                    <i class="bi bi-exclamation-triangle-fill me-2"></i>
                                    Renewal Time Expired
                                </h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>

                            <div class="modal-body text-center p-4">
                                <i class="bi bi-exclamation-circle text-warning fs-1 mb-3"></i>

                                <p class="fw-bold mb-2">
                                    Renewal period has expired.
                                </p>

                                <p class="mb-0">
                                    Books can be renewed only within
                    <strong>
                        <asp:Label ID="lblMaxRenewDays" runat="server"></asp:Label>
                        days after the due date
                    </strong>.
                    <br />
                                    Please return the book to the library.
                                </p>

                                <p class="text-danger mt-2">
                                    Overdue fine will be applicable.
                                </p>
                            </div>

                            <div class="modal-footer justify-content-center">
                                <button type="button"
                                    class="btn btn-warning"
                                    data-bs-dismiss="modal">
                                    OK
                                </button>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </div>
    </form>
    <uc:footer id="Footer1" runat="server" />

    <script>
        function validateRenewalInput() {

            var daysPattern = /^[0-9]+$/;
            var RenewDays = document.getElementById('<%= txtRenewDays.ClientID %>').value.trim();

            if (!RenewDays) {
                AlertMessage("Please enter number of days.", "error");
                document.getElementById('<%= txtRenewDays.ClientID %>').focus();
                return false;
            }

            if (!daysPattern.test(RenewDays)) {
                AlertMessage("Number of days must be numeric.", "error");
                document.getElementById('<%= txtRenewDays.ClientID %>').focus();
                return false;
            }

            if (parseInt(RenewDays) <= 0) {
                AlertMessage("Number of days must be greater than zero.", "error");
                document.getElementById('<%= txtRenewDays.ClientID %>').focus();
                return false;
            }

            if (parseInt(RenewDays) > 14) {
                AlertMessage("Maximum renewal allowed is 14 days.", "warning");
                document.getElementById('<%= txtRenewDays.ClientID %>').focus();
                return false;
            }

            return true;
        }
    </script>


</body>
</html>
