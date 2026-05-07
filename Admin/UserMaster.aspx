<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserMaster.aspx.cs" Inherits="Admin.UserMaster" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>User Master</title>
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
    <style>
        #rblGender input[type="radio"] {
            margin-right: 6px;
            margin-left: 10px;
            margin-top: 10px;
            margin-bottom: 10px;
            accent-color: #308e87; /* Bootstrap primary color */
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
    </style>

</head>
<body>
    <uc:header id="Header" runat="server" />

    <form id="form1" runat="server">
        <div class="page-body ">

            <!-- Container-fluid starts-->
            <div class="container-fluid pt-2">

                <div class="row ">
                    <div class="text-end mb-2">
                        <asp:Button ID="btnAddUser" runat="server" Text="Add User" CssClass="btn btn-success mt-2" OnClick="btnAddUser_Click" />
                    </div>
                </div>
                <!-- USER GRID SECTION -->
                <div class="card" id="divGrid" runat="server">
                    <div class="card-header bg-primary p-3">
                        <h3 class="card-title mb-0 ">User Details</h3>
                    </div>
                    <div class="card">
                        <div class="card-body">
                            <div class="row mb-3 align-items-end g-2">
                                <div class="col-12 col-lg d-flex flex-wrap gap-2 align-items-end">
                                    <div class="col-12 col-sm-6 col-md-3">
                                        <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="form-select">
                                            <asp:ListItem Value="">-------- Search By --------</asp:ListItem>
                                            <asp:ListItem Value="UserName">User Name</asp:ListItem>
                                            <asp:ListItem Value="Email">Email</asp:ListItem>
                                            <asp:ListItem Value="RoleType">Role Type</asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-6 col-md-3">
                                        <asp:TextBox ID="txtSearchValue" runat="server" CssClass="form-control"
                                            Placeholder="Enter search text" MaxLength="50"></asp:TextBox>
                                    </div>
                                    <div class="col-auto">
                                        <asp:Button ID="btnSearch" runat="server" Text="Search"
                                            CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                                    </div>
                                    <div class="col-auto">
                                        <asp:Button ID="btnClearSearch" runat="server" Text="Clear"
                                            CssClass="btn btn-secondary" OnClick="btnClearSearch_Click" />
                                    </div>
                                </div>
                                <div class="col-12 col-lg-auto ms-lg-auto text-lg-end">
                                    <asp:Label ID="lblRecordCount" runat="server"
                                        CssClass="fw-bold text-primary"></asp:Label>
                                </div>
                            </div>
                            <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                                <asp:GridView ID="gvUser" runat="server"
                                    CssClass="table table-bordered table-striped text-center"
                                    AutoGenerateColumns="False" OnRowCommand="gvUser_RowCommand"
                                    AllowPaging="True"
                                    PageSize="5"
                                    OnPageIndexChanging="gvUser_PageIndexChanging"
                                    PagerSettings-Visible="false" EmptyDataText="No records found">

                                    <EmptyDataTemplate>
                                        <div class="text-center py-4">
                                            <i class="bi bi-search fs-2 text-muted"></i>
                                            <div class="mt-2 fw-semibold text-muted">
                                                No results found
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                                                    <Columns>
                                        <asp:BoundField DataField="Sno" HeaderText="S.No." />
                                        <asp:BoundField DataField="UserID" Visible="false" />
                                        <asp:BoundField DataField="LoginID" HeaderText="Login ID" />

                                        <asp:TemplateField HeaderText="Employee Code">
                                            <HeaderStyle CssClass="wrap-header" />
                                            <ItemTemplate><%# Eval("EmpCode") %> </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Gender" HeaderText="Gender" />
                                        <asp:BoundField DataField="UserName" HeaderText="User Name" />
                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                        <asp:BoundField DataField="AltEmail" HeaderText="Alternate Email" Visible="false" />
                                        <asp:BoundField DataField="Phone" HeaderText="Phone" />
                                        <asp:BoundField DataField="AltPhone" HeaderText="Alternate Phone Number" Visible="false" />
                                        <asp:BoundField DataField="Department" HeaderText="Department" />
                                        <asp:BoundField DataField="Designation" HeaderText="Designation" />
                                        <asp:BoundField DataField="RoleName" HeaderText="Role Type" />
                                        <asp:BoundField DataField="Password" HeaderText="Password" />
                                        <asp:BoundField DataField="ActiveStatus" HeaderText="Status" />


                                        <asp:TemplateField HeaderText="Edit">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server"
                                                    CommandName="EditUser"
                                                    CommandArgument='<%# Eval("UserID") %>'
                                                    CssClass="btn btn-sm btn-primary me-2"
                                                    ToolTip="Edit User">
                            <i class="iconly-Edit icli"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delete">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server"
                                                    CommandName="DeleteUser"
                                                    CommandArgument='<%# Eval("UserID") %>'
                                                    CssClass='<%# Convert.ToBoolean(Eval("Active")) 
                                                    ? "btn btn-sm btn-danger" 
                                                    : "btn btn-sm btn-secondary disabled" %>'
                                                    ToolTip='<%# Convert.ToBoolean(Eval("Active")) 
                                                    ? "Click to deactivate" 
                                                    : "Already inactive" %>'
                                                    OnClientClick='<%# Convert.ToBoolean(Eval("Active")) 
                                                          ? "return confirm(\"Are you sure you want to deactivate this user?\");" 
                                                          : "return false;" %>'>
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
                <div id="divForm" runat="server" visible="false">
                    <div class="edit-profile">
                        <div class="row">
                            <asp:HiddenField ID="hdnUserID" runat="server" />
                            <div class="col">
                                <div class="card">
                                    <div class="card-header bg-primary">
                                        <h3 class="card-title mb-0">User Profile</h3>
                                    </div>
                                    <div class="card">
                                        <div class="card-body">
                                            <div class="row">

                                                <!-- Login ID -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Login ID <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtLoginId" runat="server" CssClass="form-control" placeholder="Enter Login ID" MaxLength="15"></asp:TextBox>
                                                </div>

                                                <!-- Employee Code -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Employee Code <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtEmpCode" runat="server" CssClass="form-control" placeholder="Enter Employee Code" MaxLength="15"></asp:TextBox>
                                                </div>

                                                <!-- User Name -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">User Name <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter User Name" MaxLength="50"></asp:TextBox>
                                                </div>
                                                <!-- Gender -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label d-block">Gender <span style="color: red">*</span></label>

                                                    <asp:RadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" CssClass="form-check-inline">
                                                        <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                                                        <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                                        <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                                <!-- Email -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Email <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email" MaxLength="50"></asp:TextBox>
                                                </div>

                                                <!-- Alternate Email -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Alternate Email</label>
                                                    <asp:TextBox ID="txtAlternateEmail" runat="server" CssClass="form-control" placeholder="Enter Alternate Email" MaxLength="50"></asp:TextBox>
                                                </div>

                                                <!-- Phone Number -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Phone Number <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="TxtPhone" runat="server" CssClass="form-control" onkeypress="return allowOnlyNumbers(event)"
                                                        oninput="removeNonNumeric(this)" placeholder="Enter Phone Number" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                                </div>

                                                <!-- Alternate Phone -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Alternate Number</label>
                                                    <asp:TextBox ID="txtAlternatePhone" runat="server" CssClass="form-control" placeholder="Enter Alternate Number" onkeypress="return allowOnlyNumbers(event)"
                                                        oninput="removeNonNumeric(this)" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                                </div>

                                                <!-- Department -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Department <span style="color: red">*</span></label>
                                                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select">
                                                        <asp:ListItem Text="Select Department" Value="" Selected="True" Disabled="True"></asp:ListItem>
                                                        <asp:ListItem Text="IT" Value="IT"></asp:ListItem>
                                                        <asp:ListItem Text="HR" Value="HR"></asp:ListItem>
                                                        <asp:ListItem Text="Finance" Value="Finance"></asp:ListItem>
                                                        <asp:ListItem Text="Marketing" Value="Marketing"></asp:ListItem>
                                                        <asp:ListItem Text="Operations" Value="Operations"></asp:ListItem>
                                                        <asp:ListItem Text="Library" Value="Library"></asp:ListItem>
                                                        <asp:ListItem Text="Others" Value="Others"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Designation -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Designation <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" placeholder="Enter Designation" MaxLength="50"></asp:TextBox>
                                                </div>

                                                <!-- Role Type -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Role Type <span style="color: red">*</span></label>
                                                    <asp:DropDownList ID="ddlRoleType" runat="server" CssClass="form-select">
                                                    </asp:DropDownList>
                                                </div>

                                                <!-- Password -->
                                                <div class="col-md-4 mb-3">
                                                    <label class="form-label">Password <span style="color: red">*</span></label>
                                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter Password" MaxLength="20"></asp:TextBox>
                                                </div>

                                                <!-- Active Checkbox -->
                                                <div class="col-md-3">
                                                    <label class="form-label d-block">Status <span style="color: red">*</span></label>
                                                    <div class="form check form-check-inline">
                                                        <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                                                        <label class="form-check-label ms-2" for="chkActive">Active</label>
                                                    </div>
                                                </div>
                                            </div>
                                            <!-- End Row -->
                                        </div>

                                        <div class="card-footer text-end">
                                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary me-2" OnClick="btnSave_Click" OnClientClick="return validateUserMaster()" />
                                            <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-success me-2" Visible="false" OnClick="btnUpdate_Click" />

                                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning me-2" OnClick="btnClear_Click" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                        </div>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>


            </div>


        </div>
        <script type="text/javascript">
            function allowOnlyNumbers(evt) {
                var charCode = evt.which ? evt.which : evt.keyCode;

                // Allow Backspace, Delete, Arrow keys, Tab
                if (charCode === 8 || charCode === 9 || charCode === 37 || charCode === 39 || charCode === 46) {
                    return true;
                }

                // Allow only numbers (0–9)
                if (charCode < 48 || charCode > 57) {
                    evt.preventDefault();
                    return false;
                }

                return true;
            }

            function removeNonNumeric(input) {
                input.value = input.value.replace(/[^0-9]/g, '');
            }

            function showUserForm() {
                document.getElementById('<%= divGrid.ClientID %>').style.display = 'none';
                document.getElementById('<%= divForm.ClientID %>').style.display = 'block';
                return false; // prevent postback
            }

            function showGrid() {
                document.getElementById('<%= divGrid.ClientID %>').style.display = 'block';
                document.getElementById('<%= divForm.ClientID %>').style.display = 'none';
                return false;
            }

            function confirmDelete() {
                return confirm("Are you sure you want to delete this user?");
            }




            function validateUserMaster() {

                // Get values
                var loginId = document.getElementById('<%= txtLoginId.ClientID %>').value.trim();
                var empCode = document.getElementById('<%= txtEmpCode.ClientID %>').value.trim();
                var userName = document.getElementById('<%= txtUsername.ClientID %>').value.trim();
                var email = document.getElementById('<%= txtEmail.ClientID %>').value.trim();
                var altEmail = document.getElementById('<%= txtAlternateEmail.ClientID %>').value.trim();
                var phone = document.getElementById('<%= TxtPhone.ClientID %>').value.trim();
                var altPhone = document.getElementById('<%= txtAlternatePhone.ClientID %>').value.trim();
                var department = document.getElementById('<%= ddlDepartment.ClientID %>').value.trim();
                var designation = document.getElementById('<%= txtDesignation.ClientID %>').value.trim();
                var roleType = document.getElementById('<%= ddlRoleType.ClientID %>').value.trim();

                var password = document.getElementById('<%= txtPassword.ClientID %>').value.trim();
                var active = document.getElementById('<%= chkActive.ClientID %>').checked;

                // Gender check
                var gender = document.getElementById('<%= rblGender.ClientID %>').value.trim();

                // Regex patterns
                var alphanumeric = /^[A-Za-z0-9 ]+$/;
                var namePattern = /^[A-Za-z. ]+$/;
                var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                var phonePattern = /^[0-9]{10}$/;
                var passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
                var namePattern2 = /^[A-Za-z' -]+$/;


                // --- Login ID ---
                if (!loginId) {
                    AlertMessage("Please enter Login ID.", "error");
                    document.getElementById('<%= txtLoginId.ClientID %>').focus();
                    return false;
                }
                if (!alphanumeric.test(loginId)) {
                    AlertMessage("Login ID must contain only letters and numbers.", "error");
                    document.getElementById('<%= txtLoginId.ClientID %>').focus();
                    return false;
                }
                if (loginId.length < 5) {
                    AlertMessage("Login ID must be at least 5 characters long.", "error");
                    document.getElementById('<%= txtLoginId.ClientID %>').focus();
                    return false;
                }

                // --- Employee Code ---
                if (!empCode) {
                    AlertMessage("Please enter Employee Code.", "error");
                    document.getElementById('<%= txtEmpCode.ClientID %>').focus();
                    return false;
                }
                if (!alphanumeric.test(empCode)) {
                    AlertMessage("Employee Code must contain only letters and numbers.", "error");
                    document.getElementById('<%= txtEmpCode.ClientID %>').focus();
                    return false;
                }

                // --- User Name ---
                if (!userName) {
                    AlertMessage("Please enter User Name.", "error");
                    document.getElementById('<%= txtUsername.ClientID %>').focus();
                    return false;
                }
                if (!namePattern.test(userName)) {
                    AlertMessage("User Name can only contain alphabets and spaces.", "error");
                    document.getElementById('<%= txtUsername.ClientID %>').focus();
                    return false;
                }

                // --- Gender ---
                //if (!rbMale && !rbFemale && !rbOther) {
                //    AlertMessage("Please select Gender.", "error");
                //    return false;
                //}
                function isGenderSelected() {
                    var radios = document.querySelectorAll(
        '#<%= rblGender.ClientID %> input[type=radio]'
                    );

                    for (var i = 0; i < radios.length; i++) {
                        if (radios[i].checked) {
                            return true;
                        }
                    }
                    return false;
                }

                // --- Email ---
                if (!email) {
                    AlertMessage("Please enter Email.", "error");
                    document.getElementById('<%= txtEmail.ClientID %>').focus();
                    return false;
                }
                if (!emailPattern.test(email)) {
                    AlertMessage("Please enter a valid Email address (e.g., example@domain.com).", "error");
                    document.getElementById('<%= txtEmail.ClientID %>').focus();
                    return false;
                }

                // --- Alternate Email ---
                if (altEmail && !emailPattern.test(altEmail)) {
                    AlertMessage("Please enter a valid Alternate Email address.", "error");
                    document.getElementById('<%= txtAlternateEmail.ClientID %>').focus();
                    return false;
                }

                // --- Phone ---
                if (!phone) {
                    AlertMessage("Please enter Phone Number.", "error");
                    document.getElementById('<%= TxtPhone.ClientID %>').focus();
                    return false;
                }
                if (!phonePattern.test(phone)) {
                    AlertMessage("Phone Number must be 10 digits only.", "error");
                    document.getElementById('<%= TxtPhone.ClientID %>').focus();
                    return false;
                }

                // --- Alternate Phone ---
                if (altPhone && !phonePattern.test(altPhone)) {
                    AlertMessage("Alternate Phone Number must be 10 digits only.", "error");
                    document.getElementById('<%= txtAlternatePhone.ClientID %>').focus();
                    return false;
                }

                // --- Department ---
                if (department === "") {
                    AlertMessage("Please select a Department.", "error");
                    document.getElementById('<%= ddlDepartment.ClientID %>').focus();
                    return false;
                }

                // --- Designation ---
                if (designation === "") {
                    AlertMessage("Please Enter a Designation.", "error");
                    document.getElementById('<%= txtDesignation.ClientID %>').focus();
                    return false;
                }
                if (!namePattern2.test(designation)) {
                    AlertMessage("Designation can only contain alphabets and spaces.", "error");
                    document.getElementById('<%= txtDesignation.ClientID %>').focus();
                    return false;
                }

                // --- Role Type ---
                if (roleType === "") {
                    AlertMessage("Please select a Role Type.", "error");
                    document.getElementById('<%= ddlRoleType.ClientID %>').focus();
                    return false;
                }

                // --- Password ---
                if (!password) {
                    AlertMessage("Please enter Password.", "error");
                    document.getElementById('<%= txtPassword.ClientID %>').focus();
                    return false;
                }
                if (!passwordPattern.test(password)) {
                    AlertMessage("Password must contain at least one uppercase, one lowercase, one number, and one special character.", "error");
                    document.getElementById('<%= txtPassword.ClientID %>').focus();
                    return false;
                }

                // --- Active Checkbox ---
                if (active === undefined) {
                    AlertMessage("Please select Active status.", "error");
                    return false;
                }

                return true;
            }

            function handleCheckboxClick(checkbox) {
                // 'checkbox' refers to the HTML input element for the ASP.NET CheckBox

                if (checkbox.checked) {
                    // Checkbox is checked
                    AlertMessage("Are you sure the user is active", "error");

                } else {
                    // Checkbox is unchecked
                    AlertMessage("Are you sure the user is decactive.", "error");

                }
            }

        </script>
    </form>

    <uc:footer id="Footer1" runat="server" />

</body>
</html>
