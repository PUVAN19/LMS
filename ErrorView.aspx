<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorView.aspx.cs" Inherits="LMS.ErrorView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<meta name="description" content="Library Management System" />
<meta name="keywords" content="Library Management System, Error View, ISHInfo" />
    <title>Error Log Viewer</title>
</head>
    <%-- <link rel="icon" href="assets/images/favicon/favicon.ico" type="image/x-icon" />
    <link rel="shortcut icon" href="assets/images/favicon/favicon.ico" type="image/x-icon" />--%>
     <style>
                 body {
            background-color: #308e87;
        }

        .error-container {
            padding: 30px;
            background-color: #ffffff;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .error-title {
            font-size: 1.5rem;
            font-weight: bold;
            color: #dc3545;
            margin-bottom: 15px;
        }

        .file-content {
            white-space: pre-wrap;
            word-wrap: break-word;
            background-color: #f1f1f1;
            padding: 20px;
            border-radius: 5px;
            font-family: monospace;
        }
    </style>

<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="error-container">
                <div class="error-title mb-3">Error File Viewer</div>
                <div class="file-content">
                    <asp:Literal ID="litFileContent" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
      <%--  body {
            background-color: #308e87;
            font-family: Arial;
        }

        .container {
            width: 90%;
            margin: 40px auto;
        }

        .error-container {
            padding: 20px;
            background-color: #fff;
            border-radius: 8px;
        }

        .error-title {
            font-size: 20px;
            font-weight: bold;
            color: #dc3545;
            margin-bottom: 10px;
        }

        .file-content {
            background-color: #f5f5f5;
            padding: 15px;
            border-radius: 5px;
            font-family: Consolas;
            max-height: 500px;
            overflow-y: auto;
            white-space: pre-wrap;
        }
    </style>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="error-container">
                <div class="error-title">Error File Viewer</div>

                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>

                <div class="file-content">
                    <asp:Literal ID="litFileContent" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>--%>
</html>
