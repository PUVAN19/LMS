<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorMaster.aspx.cs" Inherits="Admin.AuthorMaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>AuthorMenu</title>
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />

</head>
<body>
    <uc:header id="Header" runat="server" />
    <form id="form1" runat="server">
        <div class="page-body">
            <div class="container-fluid pt-2">
                <div id="divForm" class="card mb-3" runat="server" visible="true">
                    <div class="card-header bg-primary p-3 ">
                        <h3 class="card-title mb-0">Author Details</h3>
                    </div>
                    <asp:HiddenField ID="hdnAuthorID" runat="server" />
                    <div class="card-body">
                        <div class="row g-3 needs-validation  validation-forms">
                            <!-- Author Name -->
                            <div class="col-12 col-md-6 col-lg-4 position-relative">
                                <label class="form-label" for="<%= txtAuthorName.ClientID %>">
                                    Author Name <span class="text-danger">*</span>
                                </label>
                                <asp:TextBox ID="txtAuthorName" runat="server" CssClass="form-control" MaxLength="50" placeholder="Enter Author Name"></asp:TextBox>
                            </div>

                            <!-- Author Type -->
                            <div class="col-12 col-md-6 col-lg-4">
                                <label class="form-label" for="<%= ddlAuthorType.ClientID %>">
                                    Type of Author <span class="text-danger">*</span>
                                </label>
                                <asp:DropDownList ID="ddlAuthorType" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Choose..." Value="" Disabled="True" Selected="True" runat="server" />
                                    <asp:ListItem Text="Main Author" Value="Main" runat="server" />
                                    <asp:ListItem Text="Co-Author" Value="Co-Author" runat="server" />
                                </asp:DropDownList>
                            </div>
                            <!-- Status -->
                            <div class="col-12 col-lg-4">
                                <label class="form-label d-block">Status <span class="text-danger">*</span></label>
                                <div class="form check form-check-inline">
                                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                                    <label class="form-check-label ms-2" for="chkActive">Active</label>
                                </div>
                            </div>

                            <!-- Action Buttons -->
                            <div class="col-12">
                                <div class="d-grid gap-2 d-sm-flex">
                                    <asp:Button ID="btnAdd" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="Submit_Click" OnClientClick="return validateInput()" />
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success" OnClick="Update_Click" OnClientClick="return validateInput()" Visible="false" />
                                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="Clear_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card" id="divGrid" runat="server">
                    <div class="card-header bg-primary p-2 d-flex align-items-center justify-content-between flex-wrap">
    <h3 class="card-title mb-0 ps-1 text-white">
        List Of Authors
    </h3>

    <div class="d-flex align-items-center text-white" id="divPageSize" runat="server">
        <span class="me-2">Show</span>

        <asp:DropDownList 
            ID="ddlPageSize" 
            runat="server" 
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
            CssClass="form-select form-select-sm w-auto me-2">
        </asp:DropDownList>

        <span>entries</span>
    </div>
</div>

                    <div class="card-body pt-2">
                        <%--<div class="col-12 col-lg-auto ms-lg-auto text-lg-end p-1">
                            <asp:Label ID="lblRecordCount" runat="server"
                                CssClass="fw-bold text-primary"></asp:Label>
                        </div>--%>

                        <!-- Table wrapper ensures horizontal scroll on small screens -->
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvAuthor" runat="server" CssClass="table table-bordered table-striped align-middle mb-0 text-center"
                                AutoGenerateColumns="False" OnRowCommand="gvAuthor_RowCommand" AllowPaging="True"
                                OnPageIndexChanging="gvAuthor_PageIndexChanging"
                                PagerSettings-Visible="false"
     EmptyDataText="No records found">
                                <Columns>
                                    <asp:BoundField DataField="Sno" HeaderText="S.No." />
                                    <asp:TemplateField HeaderText="AuthorID" Visible="false">
                                        <ItemTemplate><%# Eval("AuthorID") %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="AuthorName" HeaderText="Author Name" />
                                    <asp:BoundField DataField="AuthorType" HeaderText="Author Type" />
                                    <asp:BoundField DataField="ActiveStatus" HeaderText="Status" />
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditAuthor" CommandArgument='<%# Eval("AuthorID") %>'
                                                CssClass="btn btn-sm btn-primary me-2" ToolTip="Edit Author"> <i class="iconly-Edit icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteAuthor"
                                                CommandArgument='<%# Eval("AuthorID") %>'
                                                CssClass='<%# Convert.ToBoolean(Eval("Active")) 
                                                          ? "btn btn-sm btn-danger" 
                                                          : "btn btn-sm btn-secondary disabled" %>'
                                                ToolTip='<%# Convert.ToBoolean(Eval("Active")) 
                                                         ? "Click to deactivate" 
                                                         : "Already inactive" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("Active")) 
                                                               ? "return confirm(\"Are you sure you want to deactivate this Author?\");" 
                                                               : "return false;" %>'><i class="iconly-Delete icli"></i>
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

                            <!-- ✅ Page Info -->


                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <uc:footer id="Footer1" runat="server" />
    <script>
        function validateInput() {
            var namePattern2 = /^[A-Za-z'. -]+$/;
            var AuthorName = document.getElementById('<%= txtAuthorName.ClientID %>').value.trim();
            var AuthorType = document.getElementById('<%= ddlAuthorType.ClientID %>').value.trim();
            if (!AuthorName) {
                AlertMessage("Please enter Author Name.", "error");
                document.getElementById('<%= txtAuthorName.ClientID %>').focus();
                return false;
            }
            if (!namePattern2.test(AuthorName)) {
                AlertMessage("Author Name can only contain alphabets and spaces.", "error");
                document.getElementById('<%= txtAuthorName.ClientID %>').focus();
                return false;
            }
            if (AuthorType === "") {
                AlertMessage("Please select a Author Type.", "error");
                document.getElementById('<%= ddlAuthorType.ClientID %>').focus();
                return false;
            }
            return true;
        }
     </script>
</body>
</html>
