<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CategoryMaster.aspx.cs" Inherits="Admin.CategoryMaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>CategoryMenu</title>
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
</head>

<style>
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
</style>
<body>

    <uc:header id="Header" runat="server" />
    <form id="form1" runat="server">
        <div class="page-body">
            <div class="container-fluid pt-2">

                <div id="divForm" class="card mb-3" runat="server" visible="true">
                    <div class="card-header bg-primary p-3">
                        <h3 class="card-title mb-0">Category Details</h3>
                    </div>

                    <asp:HiddenField ID="hdnCategoryID" runat="server" />

                    <div class="card-body mt-0">
                        <!-- Responsive grid: stack on small screens -->
                        <div class="row g-3 needs-validation  validation-forms" novalidate>

                            <!-- Left column: name + status -->
                            <div class="col-12 col-md-6 col-lg-5 position-relative">
                                <div>
                                    <label class="form-label" for="<%= txtCategoryName.ClientID %>">
                                        Category Name <span class="text-danger">*</span>
                                    </label>
                                    <asp:TextBox ID="txtCategoryName"
                                        runat="server"
                                        CssClass="form-control"
                                        MaxLength="50"
                                        placeholder="Enter Category Name"></asp:TextBox>

                                </div>

                                <div class="mt-3">
                                    <label class="form-label d-block">
                                        Status <span class="text-danger">*</span>
                                    </label>
                                    <div class="form check form-check-inline">
                                        <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                                        <label class="form-check-label ms-2" for="chkActive">Active</label>
                                    </div>
                                </div>
                            </div>

                            <!-- Right column: description -->
                            <div class="col-12 col-md-6 col-lg-7">
                                <label class="form-label" for="<%= txtDescription.ClientID %>">Description <span class="text-danger">*</span></label>
                                <!-- HtmlTextArea (server-side) -->
                                <textarea id="txtDescription"
                                    runat="server"
                                    class="form-control"
                                    rows="4" maxlength="300"></textarea>
                            </div>
                        </div>

                        <!-- Action Buttons -->
                        <div class="col-12 mt-3 mb-0">
                            <!-- Stack on xs, inline on sm+ -->
                            <div class="d-grid gap-2 d-sm-flex">
                                <asp:Button ID="btnAdd" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="Submit_Click" OnClientClick="return validateInput()" />
                                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success" OnClick="Update_Click" OnClientClick="return validateInput()" Visible="false" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="Clear_Click" />
                            </div>
                        </div>

                    </div>
                </div>
                <div class="card" id="divGrid" runat="server">
                    <div class="card-header bg-primary p-2 d-flex align-items-center justify-content-between flex-wrap">
                        <h3 class="card-title mb-0 ps-1 text-white">Categories
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
                    <div class="card">
                        <div class="card-body pt-0">
                           
                            <div class="table-responsive pt-2" style="overflow-x: auto; white-space: nowrap;">
                                <asp:GridView ID="gvCategory" runat="server"
                                    CssClass="table table-bordered table-striped text-center"
                                    AutoGenerateColumns="False"
                                    OnRowCommand="gvCategory_RowCommand"
                                    AllowPaging="True"

                                    OnPageIndexChanging="gvCategory_PageIndexChanging"
                                    PagerSettings-Visible="false"
                                    EmptyDataText="No records found">
                                    <Columns>
                                        <asp:BoundField DataField="Sno" HeaderText="S.No." />
                                        <asp:TemplateField HeaderText="CategoryID" Visible="false">
                                            <ItemTemplate>
                                                <%# Eval("CategoryID") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CategoryName" HeaderText="Category Name" />
                                        <asp:TemplateField HeaderText="Description">
                                            <ItemStyle CssClass="wraping-cell" />
                                            <ItemTemplate><%# Eval("Description") %> </ItemTemplate>
                                        </asp:TemplateField>
                                      
                                        <asp:BoundField DataField="ActiveStatus" HeaderText="Status" />
                                        <asp:TemplateField HeaderText="Edit">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server"
                                                    CommandName="EditCategory"
                                                    CommandArgument='<%# Eval("CategoryID") %>'
                                                    CssClass="btn btn-sm btn-primary me-2"
                                                    ToolTip="Edit Category">
                    <i class="iconly-Edit icli"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server"
                                                    CommandName="DeleteCategory"
                                                    CommandArgument='<%# Eval("CategoryID") %>'
                                                    CssClass='<%# Convert.ToBoolean(Eval("Active")) 
                                ? "btn btn-sm btn-danger" 
                                : "btn btn-sm btn-secondary disabled" %>'
                                                    ToolTip='<%# Convert.ToBoolean(Eval("Active")) 
                                ? "Click to deactivate" 
                                : "Already inactive" %>'
                                                    OnClientClick='<%# Convert.ToBoolean(Eval("Active")) 
                                      ? "return confirm(\"Are you sure you want to deactivate this Category?\");" 
                                      : "return false;" %>'>
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


            <script>
                function validateInput() {
                    var namePattern2 = /^[A-Za-z0-9 &\-]+$/;
                    var descPattern = /^[A-Za-z0-9 .,()'":;!?-]+$/;
                    var CategoryName = document.getElementById('<%= txtCategoryName.ClientID %>').value.trim();
                var Description = document.getElementById('<%= txtDescription.ClientID %>').value.trim();

                // 1️⃣ Required check
                if (!CategoryName) {
                    AlertMessage("Please enter Category Name.", "error");
                    document.getElementById('<%= txtCategoryName.ClientID %>').focus();
                    return false;
                }

                if (CategoryName.length < 3) {
                    AlertMessage("Category Name must be at least 3 characters.", "error");
                    document.getElementById('<%= txtCategoryName.ClientID %>').focus();
                    return false;
                }
                if (!namePattern2.test(CategoryName)) {
                    AlertMessage("Category Name can contain only letters, numbers, space, '&' and '-' symbols.", "error");
                    document.getElementById('<%= txtCategoryName.ClientID %>').focus();
                    return false;
                }
                // 1️⃣ Required
                if (description === "") {
                    AlertMessage("Please enter a Description.", "error");
                    document.getElementById('<%= txtDescription.ClientID %>').focus();
                    return false;
                }
                // 2️⃣ Minimum length
                if (description.length < 5) {
                    AlertMessage("Description must be at least 5 characters long.", "error");
                    document.getElementById('<%= txtDescription.ClientID %>').focus();
                    return false;
                }
                // 3️⃣ Maximum length
                if (description.length > 500) {
                    AlertMessage("Description cannot exceed 500 characters.", "error");
                    document.getElementById('<%= txtDescription.ClientID %>').focus();
                    return false;
                }
                // 4️⃣ Pattern check
                if (!descPattern.test(description)) {
                    AlertMessage("Description contains invalid characters.", "error");
                    document.getElementById('<%= txtDescription.ClientID %>').focus();
                        return false;
                    }
                    return true;
                }
        </script>
    </form>

    <uc:footer id="Footer1" runat="server" />


</body>
</html>
