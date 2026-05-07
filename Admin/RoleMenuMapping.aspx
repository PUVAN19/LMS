<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleMenuMapping.aspx.cs"
    Inherits="Admin.RoleMenuMapping" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Role Menu Mapping</title>
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
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div class="page-body">
            <div class="container-fluid pt-2">
                <div class="card">
                    <div class="card-header bg-primary p-3">
                        <h3>Role Menu Mapping</h3>
                    </div>

                    <div class="card-body basic-form">

                        <div class="d-flex align-items-center gap-2 flex-nowrap">

                            <label class="form-label mb-0 text-nowrap fw-bold">
                                Role Type
                           
                            </label>

                            <asp:DropDownList ID="ddlRoleType" runat="server"
                                CssClass="form-control w-auto"
                                AutoPostBack="True"
                                OnSelectedIndexChanged="ddlRoleType_SelectedIndexChanged">
                            </asp:DropDownList>

                            <asp:Button ID="btnClear" runat="server"
                                Text="Clear"
                                CssClass="btn btn-secondary"
                                OnClick="btnClear_Click" />

                            <div class="col-12 col-lg-auto ms-lg-auto text-lg-end" id="divPageSize" runat="server">
                                Show 
                             <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                                 OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                                 CssClass="form-select d-inline w-auto">
                             </asp:DropDownList>
                                entries
   
                            </div>

                        </div>

                        <asp:Panel ID="pnlMenuList" runat="server" class="pt-0" Visible="false">
                            
                            <div class="table-responsive pt-2">
                                <asp:GridView ID="gvRoleMenu" runat="server"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-striped table-bordered text-center pt-1"
                                    DataKeyNames="MenuID"
                                    AllowPaging="true"
                                    AutoPostBack="true"
                                    PagerSettings-Visible="false"
                                    OnPageIndexChanging="gvMenu_PageIndexChanging"
                                    EmptyDataText="No records found">

                                    <Columns>


                                        <asp:TemplateField HeaderText="S.No.">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="MenuName" HeaderText="Menu Name" />


                                        <asp:BoundField DataField="SequenceNo" HeaderText="Sequence No" />


                                        <asp:BoundField DataField="ParentMenu" HeaderText="Parent Menu" />


                                        <asp:BoundField DataField="PageName" HeaderText="Page Name" />


                                        <asp:TemplateField HeaderText="Active">
                                            <ItemTemplate>
                                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Yes" : "No" %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Visible">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAllow" runat="server"
                                                    Checked='<%# Convert.ToBoolean(Eval("IsChecked")) %>'
                                                    AutoPostBack="true"
                                                    OnCheckedChanged="chkAllow_CheckedChanged" />
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
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>


    </form>

    <uc:footer id="Footer" runat="server" />

</body>

</html>
