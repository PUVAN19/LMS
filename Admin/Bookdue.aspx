<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bookdue.aspx.cs" Inherits="Admin.Bookdue" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Book Dues</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/customPagination.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../assets/css/vendors/flatpickr/flatpickr.min.css" />

    <style>
        #rblMemberType input[type="radio"] {
            margin-right: 6px;
            margin-left: 10px;
            margin-top: 10px;
            margin-bottom: 10px;
            accent-color: #308e87;
        }

        .nowrap {
            white-space: nowrap;
        }

        .required {
            color: red;
            margin-left: 5px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <uc:header id="Header" runat="server" />
        <div class="page-body">
            <div class="container-fluid pt-2">
                <div class="card">
                    <div class="card-header bg-primary p-3">
                        <h3>Book Dues</h3>
                    </div>

                    <div class="card-body basic-form">
                        <div class="row">
                            <!-- MEMBER SELECTION SECTION -->
                            <div class="col-md-3">
                                <label class="fw-bold mb-2 fs-7">Taken By<span class="required"> * </span></label>
                                <asp:RadioButtonList ID="rblMemberType" runat="server"
                                    RepeatDirection="Horizontal"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="rblMemberType_SelectedIndexChanged">
                                    <asp:ListItem Text="Student &nbsp;&nbsp;" Value="Student"></asp:ListItem>
                                    <asp:ListItem Text="Staff" Value="Staff"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>

                            <!-- SEARCH SECTION -->
                            <div class="col-md-9 mt-3" id="divInputSection" runat="server" visible="false">
                                <asp:Label ID="lblEnterId" runat="server" CssClass="fw-bold me-2"><span class="required"> * </span></asp:Label>
                                <asp:TextBox ID="txtMemberID" runat="server"
                                    CssClass="form-control d-inline-block w-50 me-2"></asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" Text="Search"
                                    CssClass="btn btn-primary"
                                    OnClick="btnSearch_Click" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear"
                                    CssClass="btn btn-warning" OnClick="btnClear_Click" />
                            </div>
                        </div>

                        <!-- GRID -->
                        <div class="table-responsive mt-4">
                            <asp:GridView ID="gvBookDues" runat="server"
                                AutoGenerateColumns="False"
                                DataKeyNames="IssueID"
                                PageSize="5"
                                AllowPaging="true"
                                PagerSettings-Visible="false"
                                CssClass="table table-bordered table-striped text-center">
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Select">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="ISBN" HeaderText="ISBN" />
                                    <asp:BoundField DataField="BookTitle" HeaderText="Book Name" />
                                    <asp:BoundField DataField="AuthorName" HeaderText="Author Name" />
                                     <asp:BoundField DataField="Edition" HeaderText="Edition" />
                                    <asp:BoundField DataField="IssueDate"
                                        HeaderText="Issue Date"
                                        DataFormatString="{0:dd-MMM-yyyy}"
                                        HtmlEncode="false" ItemStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-CssClass="nowrap" HeaderStyle-CssClass="nowrap" />


                                    <asp:BoundField DataField="DueDate"
                                        HeaderText="Due Date"
                                        DataFormatString="{0:dd-MMM-yyyy}"
                                        HtmlEncode="false" ItemStyle-Width="110px" HeaderStyle-Width="110px" ItemStyle-CssClass="nowrap" HeaderStyle-CssClass="nowrap" />


                                    <asp:BoundField DataField="RenewalCount" HeaderText="Renewals" />

                                    <asp:BoundField DataField="LastRenewalDate"
                                        HeaderText="Last Renewal Date"
                                        DataFormatString="{0:dd-MMM-yyyy}" />

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


                        <!-- DATE PICKER -->
                        <div class="row mt-4" id="divDateSection" runat="server" visible="false">
                            <div class="col-md-6 d-flex align-items-center">
                                <label class="form-label fw-bold me-2 mb-0">Select Date<span class="required">*</span></label>

                                <div style="width: 180px;">
                                    <%-- <asp:TextBox ID="txtDate" runat="server" TextMode="Date"
                                        CssClass="form-control" onkeydown="return false;"  onpaste="return false;"></asp:TextBox>
                                </div>--%>
                                    <asp:TextBox ID="txtDate" runat="server"
                                        CssClass="form-control flatpickr-date"
                                        onkeydown="return false;" onpaste="return false;">
                                    </asp:TextBox>

                                </div>
                            </div>


                            <!-- ACTION BUTTONS -->
                            <div class="mt-4 mb-3" id="divActionButtons" runat="server" visible="false">
                                <asp:Button ID="btnMarkReturned" runat="server" Text="Mark as Returned"
                                    CssClass="btn btn-primary"
                                    OnClick="btnMarkReturned_Click" />

                                <asp:Button ID="btnMarkRenewed" runat="server" Text="Mark as Renewed"
                                    CssClass="btn btn-warning"
                                    OnClick="btnMarkRenewed_Click" />

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <uc:footer id="Footer" runat="server" />
    </form>
    <script src="../assets/js/flat-pickr/flatpickr.js"></script>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            flatpickr(".flatpickr-date", {
                dateFormat: "d-M-Y",
                defaultDate: "today",
                allowInput: false,
                onChange: function (selectedDates, dateStr, instance) {
                    instance.input.value = dateStr.toUpperCase();
                }
            });
        });

    </script>

</body>
</html>
