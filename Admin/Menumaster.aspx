<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menumaster.aspx.cs" Inherits="Admin.Menumaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="Admiro admin is super flexible, powerful, clean &amp; modern responsive bootstrap 5 admin template with unlimited possibilities." />
    <meta name="keywords" content="admin template, Admiro admin template, best javascript admin, dashboard template, bootstrap admin template, responsive admin template, web app" />
    <meta name="author" content="pixelstrap" />

    <title>Menu Master</title>
    <link href="../assets/css/customPagination.css" rel="stylesheet" />
    <style>
        .required {
            color: red;
            font-weight: bold;
        }
    </style>
</head>

<body>
    <uc:header id="Header" runat="server" />

    <form id="form1" runat="server">

        <div class="page-body">
            <div class="container-fluid pt-2">
                <div class="card mb-1">
                    <div class="card-header bg-primary p-3">
                        <h3>Menu Master</h3>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <!-- Menu Name -->
                            <div class="mb-3 col-md-6 col-12 col-sm-12">
                                <label class="form-label">Menu Name <span class="required">*</span></label>
                                <asp:TextBox ID="txtMenuName" runat="server"
                                    CssClass="form-control"
                                    MaxLength="40"
                                    placeholder="Ex: Reports and Analytics"></asp:TextBox>
                            </div>


                            <!-- Active -->
                            <div class="mb-3 col-md-6 col-12 col-sm-12">
                                <label class="form-label d-block">Status </label>
                                <div class="form check form-check-inline">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" AutoPostBack="true"
                                        OnCheckedChanged="chkIsActive_CheckedChanged" />
                                    <label class="form-check-label ms-2 mb-1" for="chkIsActive">Active</label>
                                </div>
                            </div>
                            <!-- Page Name -->
                            <div class="mb-2 col-md-6 col-12 col-sm-12">
                                <label class="form-label">Page Name</label>
                                <asp:TextBox ID="txtPageName" runat="server"
                                    CssClass="form-control"
                                    placeholder="Ex: ReportsAndAnalytics.aspx"
                                    MaxLength="50">
                                </asp:TextBox>

                                <!-- Checkboxes BELOW Page Name -->
                                <div class="d-flex gap-4 mt-3">
                                    <div class="form-check ps-0">
                                        <asp:CheckBox ID="IsDefault" runat="server" />
                                        <label class="form-check-label ms-1" for="IsDefault">
                                            Is Default Page
                                       
                                        </label>
                                    </div>

                                    <div class="form-check ps-0">
                                        <asp:CheckBox ID="chkIsChild" runat="server"
                                            AutoPostBack="true"
                                            OnCheckedChanged="chkIsChild_CheckedChanged" />
                                        <label class="form-check-label ms-1" for="chkIsChild">
                                            Is Child Menu
                                       
                                        </label>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <!-- Parent Menu DropDown -->
                        <div class="mb-3 col-md-6 col-12 col-sm-12" id="parentMenuDiv" runat="server">
                            <label class="form-label">Parent Menu<span class="required">*</span></label>
                            <asp:DropDownList ID="ddlParentMenu" runat="server"
                                CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <asp:HiddenField ID="hfMenuID" runat="server" />

                        <div class="mt-0">
                            <asp:Button ID="btnSubmit" runat="server" Text="Save"
                                CssClass="btn btn-primary me-2"
                                OnClick="btnSubmit_Click" />

                            <asp:Button ID="btnUpdate" runat="server" Text="Update"
                                Visible="false"
                                CssClass="btn btn-warning me-2"
                                OnClick="btnUpdate_Click" />

                            <asp:Button ID="btnClear" runat="server" Text="Clear"
                                CssClass="btn btn-secondary"
                                OnClick="btnClear_Click" />
                        </div>
                    </div>
                </div>
                <!-- GRID -->
                <div class="card" id="divGrid" runat="server">
                    <div class="card-header bg-primary p-3">
                        <h4 class="mb-0 fs-6 fs-md-4">Menu List</h4>
                    </div>
                    <div class="card-body p-3">
                        <div class="row mb-3 align-items-end g-2">
                            <div class="col-12 col-lg d-flex flex-wrap gap-2 align-items-end">
                                <div class="col-12 col-sm-6 col-md-3">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="">---------- Search By ---------</asp:ListItem>
                                        <asp:ListItem Value="MenuName">Menu Name</asp:ListItem>
                                        <asp:ListItem Value="PageName">Page Name</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-12 col-sm-6 col-md-3">
                                    <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"
                                        Placeholder="Enter search text" MaxLength="50"></asp:TextBox>
                                </div>

                                <div class="col-auto">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search"
                                        CssClass="btn btn-primary"
                                        OnClick="btnSearch_Click" />
                                </div>

                                <div class="col-auto">
                                    <asp:Button ID="btnClearSearch" runat="server" Text="Clear"
                                        CssClass="btn btn-secondary"
                                        OnClick="btnClearSearch_Click" />
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
                        </div>
                        <div class="table-responsive">
                            <asp:GridView ID="gvMenu" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="table table-striped table-bordered text-center"
                                DataKeyNames="MenuID"
                                AllowPaging="true"
                                PagerSettings-Visible="false"
                                OnPageIndexChanging="gvMenu_PageIndexChanging"
                                OnRowCommand="gvMenu_RowCommand"
                                OnRowDeleting="gvMenu_RowDeleting"
                                OnRowDataBound="gvMenu_RowDataBound"
                                EmptyDataText="No records found">
                                <Columns>

                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="MenuName" HeaderText="Menu Name" />
                                    <asp:BoundField DataField="PageName" HeaderText="Page Name" />

                                    <asp:TemplateField HeaderText="Active">
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(Eval("IsActive")) ? "Yes" : "No" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Is Child">
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(Eval("IsChildMenu")) ? "Yes" : "No" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Is Default">
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(Eval("IsDefaultPage")) ? "Yes" : "No" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>

                                            <asp:LinkButton ID="btnEdit" runat="server"
                                                CommandName="EditMenu"
                                                CommandArgument='<%# Eval("MenuID") %>'
                                                CssClass="btn btn-sm btn-primary me-2"
                                                ToolTip="Edit">
                                                    <i class="iconly-Edit icli"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnDelete" runat="server"
                                                CommandName="Delete"
                                                CommandArgument='<%# Eval("MenuID") %>'
                                                CssClass="btn btn-sm btn-danger"
                                                Enabled='<%# Convert.ToBoolean(Eval("IsActive")) %>'
                                                ToolTip='<%# Convert.ToBoolean(Eval("IsActive")) ? "Delete" : "Inactive - Cannot Delete" %>'
                                                OnClientClick='<%# Convert.ToBoolean(Eval("IsActive")) ? "return confirm(\"Are you sure you want to delete this menu?\");" : "return false;" %>'>
                                                    <i class="iconly-Delete icli"></i>
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

        </div>



    </form>

    <uc:footer id="Footer" runat="server" />

</body>
</html>
