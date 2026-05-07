<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Admin.ChangePassword" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Password</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <style>
        body {
            background-color: #f5f7fa;
            font-family: Arial, sans-serif;
        }

        .page-body {
            min-height: calc(100vh - 120px);
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }

        .card-box {
            width: 100%;
            max-width: 480px;
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0px 8px 25px rgba(0,0,0,0.1);
            padding: 0 24px 30px 24px;
        }

        .custom-header {
            background-color: #0b6f63;
            padding: 16px;
            border-radius: 16px 16px 0 0;
            text-align: center;
            margin: 0 -24px 20px -24px;
        }

        .custom-header h2 {
            color: #ffffff;
            margin: 0;
            font-size: 22px;
            font-weight: bold;
        }

        label {
            font-size: 15px;
            margin-bottom: 6px;
            display: block;
        }

        .input-box {
            width: 100%;
            height: 44px;
            padding: 10px 14px;
            border-radius: 10px;
            border: 1px solid #cfd8dc;
            background-color: #f1f8ff;
            margin-bottom: 18px;
            font-size: 14px;
        }

        .password-wrapper {
            position: relative;
        }

        .eye-icon {
            display: none;   
            position: absolute;
            right: 14px;
            top: 35%;
            transform: translateY(-50%);
            cursor: pointer;
            font-size: 12px;
            color: #555;
            user-select: none;
        }

        .btn-submit {
            width: 100%;
            padding: 12px;
            border-radius: 30px;
            border: none;
            font-size: 16px;
            font-weight: bold;
            background-color: #0b6f63;
            color: white;
            cursor: pointer;
            margin-top: 10px;
        }

        .btn-submit:hover {
            background-color: #09564d;
        }
            
        @media (min-width: 768px) {
            .card-box {
                max-width: 520px;
            }
        }
    </style>

    <script>
        function togglePassword(id, icon) {
            var textbox = document.getElementById(id);
            if (textbox.type === "password") {
                textbox.type = "text";
                icon.textContent = "Hide";
            } else {
                textbox.type = "password";
                icon.textContent = "Show";
            }
        }
    </script>
</head> 

<body>

    <uc:header id="Header" runat="server" />

    <form id="form" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div class="page-body">

            <div class="card-box">

                <div class="custom-header">
                    <h2>Reset Password</h2>
                </div>

                <!-- Current Password -->
                <label>Current Password</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtOldPassword" runat="server"
                        TextMode="Password"
                        CssClass="input-box"
                        placeholder="Enter Current Password"
                        MaxLength="20" />
                    <span class="eye-icon"
                        onclick="togglePassword('<%= txtOldPassword.ClientID %>', this)">Show</span>
                </div>

                <!-- New Password -->
                <label>New Password</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtNewPassword" runat="server"
                        TextMode="Password"
                        CssClass="input-box"
                        placeholder="Enter New Password"
                        MaxLength="20" />
                    <span class="eye-icon"
                        onclick="togglePassword('<%= txtNewPassword.ClientID %>', this)">Show</span>
                </div>

                <!-- Confirm Password -->
                <label>Confirm New Password</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtConfirmPassword" runat="server"
                        TextMode="Password"
                        CssClass="input-box"
                        placeholder="Enter New Password Again"
                        MaxLength="20" />
                    <span class="eye-icon"
                        onclick="togglePassword('<%= txtConfirmPassword.ClientID %>', this)">Show</span>
                </div>

                <asp:Button ID="btnChange" runat="server"
                    Text="Update Password"
                    CssClass="btn-submit"
                    OnClick="btnChange_Click" />

            </div>

        </div>

    </form>

    <uc:footer id="Footer" runat="server" />
    <script>
        document.addEventListener("DOMContentLoaded", function () {

            document.querySelectorAll(".password-wrapper input").forEach(function (input) {

                input.addEventListener("input", function () {
                    this.nextElementSibling.style.display = this.value ? "inline" : "none";
                });

                input.addEventListener("blur", function () {
                    if (this.value === "") {
                        this.nextElementSibling.style.display = "none";
                    }
                });
            });

        });
    </script>

</body>
</html>
