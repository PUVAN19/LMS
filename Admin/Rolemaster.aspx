<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleMaster.aspx.cs" Inherits="Admin.RoleMaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Role Master</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/customPagination.css" rel="stylesheet" />

    <style>
        .required {
            color: red;
            font-weight: bold;
        }

        #gvRoleMaster th,
        #gvRoleMaster td {
            text-align: center;
            vertical-align: middle;
        }
    </style>

</head>

<body>
    <uc:header id="Header" runat="server" />
    <form id="form2" runat="server">



        <div class="page-body">
            <div class="container-fluid pt-2">

                <div class="card mb-1">
                    <div class="card-header bg-primary p-3">
                        <h3>Role Master</h3>
                    </div>

                    <div class="card-body">
                        <div class="row mb-3 align-items-end g-2">
                            <div class="col-12 col-lg d-flex flex-wrap gap-2 align-items-end">

                                <div class="col-12 col-sm-6 col-md-3">
                                    <label class="form-label">User Role <span class="required">*</span></label>
                                    <asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                                <div class="col-12 col-sm-6 col-md-3">
                                    <label class="form-label">Default Page<span style="color: red">*</span></label>
                                    <asp:DropDownList ID="ddlDefaultPage" runat="server" CssClass="form-select" AutoPostBack="true">
                                          <asp:ListItem Text="Select..." Value="" Disabled="True" Selected="True" runat="server" />
                                    </asp:DropDownList>
                                </div>
                               
                                <div class="form-check col-12 col-sm-6 col-md-2">
                                    <asp:CheckBox ID="chkActive" runat="server"  Checked="true"/>
                                    <label class="form-check-label ms-1" for="IsDefault">
                                       Active <span class="required">*</span>
                                    </label>
                                </div>

                                <!-- ✅ Hidden field -->
                                <asp:HiddenField ID="hfRoleID" runat="server" />
                                <div class="col-3 col-md-4 col-sm-4 ">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Save"
                                        CssClass="btn btn-primary me-2"
                                        OnClientClick="return validateRoleMaster();"
                                        OnClick="btnSubmit_Click" />
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update"
                                        CssClass="btn btn-warning me-2 "
                                        OnClientClick="return validateRoleMaster();"
                                        OnClick="btnUpdate_Click"
                                        Visible="false" />
                                    <asp:Button ID="btnClear" runat="server" Text="Clear"
                                        CssClass="btn btn-secondary "
                                        OnClick="btnClear_Click" />
                                </div>
                            </div>
                            <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="card" id="divGrid" runat="server">
                    <div class="card-header bg-primary p-3">
                        <h4>Role Details</h4>
                    </div>

                    <div class="card-body pt-1">

                        <div class="d-flex justify-content-center justify-content-md-end mb-2">
                            <asp:Label ID="lblRecordCount" runat="server"
                                CssClass="fw-bold text-primary"></asp:Label>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView
                                ID="gvRoleMaster" runat="server"  AutoGenerateColumns="False" AllowPaging="True"

                                DataKeyNames="RoleID" OnRowCommand="gvRoleMaster_RowCommand"  OnRowDeleting="gvRoleMaster_RowDeleting" PageSize="5"
                                CssClass="table table-bordered table-striped" EmptyDataText="No records found">
                            <Columns>
                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate>  <%# Container.DataItemIndex + 1 %>  </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserRole" HeaderText="User Role" />
                                    <asp:BoundField DataField="MenuName" HeaderText="Default Page" />
                                    <asp:TemplateField HeaderText="Active">
                                        <ItemTemplate> <%# Convert.ToBoolean(Eval("Active")) ? "Yes" : "No" %> </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="btnEdit" runat="server" CommandName="EditRole" CommandArgument='<%# Eval("RoleID") %>'
                                                CssClass="btn btn-sm btn-primary me-2" ToolTip="Edit"><i class="iconly-Edit icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="btnDelete"
                                                runat="server"
                                                CommandName="Delete"
                                                CommandArgument='<%# Eval("RoleID") %>'
                                                CssClass="btn btn-sm btn-danger "
                                                ToolTip="Delete"
                                                OnClientClick="return confirm('Are you sure you want to delete this role?');">
                                            <i class="iconly-Delete icli"></i>
                                            </asp:LinkButton>
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


        </div>
    </form>
    <uc:footer id="Footer2" runat="server" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
 

    <script type="text/javascript">
        function validateRoleMaster() {
            var userRole = document.getElementById('<%= txtRoleName.ClientID %>').value.trim();
                var isActiveChecked = document.getElementById('<%= chkActive.ClientID %>').checked;

                if (!userRole) {
                    AlertMessage("Please enter the User Role.", "error");
                    document.getElementById('<%= txtRoleName.ClientID %>').focus();
                    return false;
                }

                if (!/^[A-Za-z' -]+$/.test(userRole)) {
                    AlertMessage("User Role must contain only alphabets and spaces.", "error");
                    document.getElementById('<%= txtRoleName.ClientID %>').focus();
                return false;
            }

          
            return true;
        }
    </script>

</body>
</html>
