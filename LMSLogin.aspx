<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LMSLogin.aspx.cs" Inherits="LMS.LMSLogin" UnobtrusiveValidationMode="None" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="Admiro admin is super flexible, powerful, clean &amp; modern responsive bootstrap 5 admin template with unlimited possibilities.">
    <meta name="keywords" content="admin template, Admiro admin template, best javascript admin, dashboard template, bootstrap admin template, responsive admin template, web app">
    <meta name="author" content="pixelstrap">
    <title>Lms-Login</title>
    <!-- Favicon icon-->
    <link rel="icon" href="assets/images/ficon.png" type="image/x-icon">
    <link rel="shortcut icon" href="assets/images/ficon.png" type="image/x-icon">
    <!-- Google font-->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="">
    <link href="https://fonts.googleapis.com/css2?family=Nunito+Sans:opsz,wght@6..12,200;6..12,300;6..12,400;6..12,500;6..12,600;6..12,700;6..12,800;6..12,900;6..12,1000&amp;display=swap" rel="stylesheet">
    <!-- Flag icon css -->
    <link rel="stylesheet" href="assets/css/vendors/flag-icon.css">
    <!-- iconly-icon-->
    <link rel="stylesheet" href="assets/css/iconly-icon.css">
    <link rel="stylesheet" href="assets/css/bulk-style.css">
    <!-- iconly-icon-->
    <link rel="stylesheet" href="assets/css/themify.css">
    <!--fontawesome-->
    <link rel="stylesheet" href="assets/css/fontawesome-min.css">


    <!-- App css -->
    <link rel="stylesheet" href="assets/css/style.css">
    <link id="color" rel="stylesheet" href="assets/css/color-1.css" media="screen">
    <link href="assets/js/toastr/toastr.min.css" rel="stylesheet" />
    <script src="assets/js/vendors/jquery/jquery.min.js"></script>

    <style>
        .card {
            width: 430px;
            box-shadow: 0 15px 30px rgba(0,0,0,0.2);
        }

        .form-area {
            animation: pageMove 0.4s ease;
        }

        .nav-tabs {
            border-bottom: none;
            margin-bottom: 25px;
        }

            .nav-tabs .nav-link {
                color: #178E87;
                font-weight: 600;
                border: none;
                padding: 10px 22px;
                position: relative;
            }

                .nav-tabs .nav-link.active {
                    background: #178E87;
                    color: #fff;
                }

        /* Form highlight */
        .tab-pane {
            border: 1px solid transparent;
            /*  border-radius: 18px;*/
            padding: 20px;
            transition: all 0.4s ease;
        }

            .tab-pane.active {
                border-color: #178E87;
                box-shadow: 0 0 25px rgba(23,142,135,0.4);
            }

        @keyframes pageMove {
            from {
                transform: translateX(50px);
                opacity: 0;
            }

            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        .btn-group .btn {
            font-weight: 600;
        }

        .password-wrapper {
            position: relative;
        }

        .eye-icon {
            display: none;
            position: absolute;
            right: 14px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            font-size: 12px;
            color: #555;
        }
    </style>

</head>
<body class="bg-primary">
    <!-- tap on top starts-->
    <div class="tap-top"><i class="iconly-Arrow-Up icli"></i></div>
    <!-- tap on tap ends-->
    <!-- loader-->
    <div class="loader-wrapper">
        <div class="loader"><span></span><span></span><span></span><span></span><span></span></div>
    </div>
    <!-- login page start-->

    <div class="container-fluid p-0">
        <form class="theme-form" runat="server">
            <asp:HiddenField ID="hdnActiveTab" runat="server" Value="student" />

            <div class="container d-flex justify-content-center align-items-center min-vh-100">
                <div class="card p-5 rounded-5">
                    <h2 class="text-center fw-bold text-primary pb-3">Library Management System  </h2>
                    <!-- Tabs -->
                    <ul class="nav nav-tabs nav-primary mb-0 justify-content-flexstart" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="pills-student-tab" data-bs-toggle="pill" href="#pills-student" role="tab">Student</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="pills-admin-tab" data-bs-toggle="pill" href="#pills-admin" role="tab">Admin</a>
                        </li>
                    </ul>

                    <!-- Tab Content -->
                    <div class="tab-content">

                        <!-- Student Login -->
                        <div class="tab-pane fade show active" id="pills-student" role="tabpanel">
                            <div class="form-group">
                                <label class="fw-semibold">Student ID</label>
                                <asp:TextBox ID="txtStudentID" runat="server"
                                    CssClass="form-control"
                                    placeholder="Enter Student ID" />
                            </div>

                            <div class="form-group mt-3">
                                <label class="fw-semibold">Password</label>

                                <div class="password-wrapper">
                                    <asp:TextBox ID="txtStudentPwd" runat="server"
                                        CssClass="form-control"
                                        TextMode="Password"
                                        placeholder="Enter Password" />

                                    <span class="eye-icon"
                                        onclick="togglePassword('<%= txtStudentPwd.ClientID %>', this)">Show
                                    </span>
                                </div>
                            </div>

                            <div class="my-2">
                                <a class="link" href="forget-password.html">Forgot password?</a>
                            </div>
                            <asp:Button ID="btnStudentLogin" runat="server"
                                CssClass="btn btn-primary w-100 fw-bold rounded-pill p-2"
                                Text="Login" OnClick="btnStudentLogin_Click" />
                        </div>

                        <!-- Admin Login -->
                        <div class="tab-pane fade show " id="pills-admin" role="tabpanel">
                            <div class="form-group">
                                <label class="fw-semibold">Admin ID</label>
                                <asp:TextBox ID="txtAdminID" runat="server"
                                    CssClass="form-control"
                                    placeholder="Enter Admin ID" />
                            </div>

                            <div class="form-group mt-3">
                                <label class="fw-semibold">Password</label>

                                <div class="password-wrapper">
                                    <asp:TextBox ID="txtAdminPwd" runat="server"
                                        CssClass="form-control"
                                        TextMode="Password"
                                        placeholder="Enter Password" />

                                    <span class="eye-icon"
                                        onclick="togglePassword('<%= txtAdminPwd.ClientID %>', this)">Show
                                    </span>
                                </div>
                            </div>

                            <div class="my-2">
                                <a class="link" href="forget-password.html">Forgot password?</a>
                            </div>
                            <asp:Button ID="btnAdminLogin" runat="server"
                                CssClass="btn btn-primary w-100 fw-bold  rounded-pill p-2"
                                Text="Login" OnClick="btnAdminLogin_Click" />
                        </div>

                    </div>
                </div>
            </div>
        </form>

        <script src="assets/js/toastr/toastr.min.js" type="text/javascript"></script>
        <!-- bootstrap js-->
        <script src="assets/js/vendors/bootstrap/dist/js/bootstrap.bundle.min.js" defer=""></script>
        <script src="assets/js/vendors/bootstrap/dist/js/popper.min.js" defer=""></script>
        <!--fontawesome-->
        <script src="assets/js/vendors/font-awesome/fontawesome-min.js"></script>


        <!-- password_show-->
        <script src="assets/js/password.js"></script>
        <!-- custom script -->
        <script src="assets/js/script.js"></script>
        <script src="assets/js/Custom-Toasts.js"></script>



        <script>

            document.addEventListener("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();

                    var activeTab = document.getElementById('<%= hdnActiveTab.ClientID %>').value;

                    if (activeTab === "admin") {
                        document.getElementById('<%= btnAdminLogin.ClientID %>').click();
                    } else {
                        document.getElementById('<%= btnStudentLogin.ClientID %>').click();
                    }
                }
            });


            $(document).ready(function () {
                $('.password-wrapper input').on('input', function () {
                    if ($(this).val()) {
                        $(this).next('.eye-icon').show();
                    } else {
                        $(this).next('.eye-icon').hide();
                    }
                });

                $('a[data-bs-toggle="pill"]').on('shown.bs.tab', function (e) {
                    let tabId = $(e.target).attr('id');

                    if (tabId === 'pills-admin-tab') {
                        $('#<%= hdnActiveTab.ClientID %>').val('admin');
                    } else {
                        $('#<%= hdnActiveTab.ClientID %>').val('student');
                    }

                    $('#<%= txtStudentID.ClientID %>, #<%= txtStudentPwd.ClientID %>, ' +
              '#<%= txtAdminID.ClientID %>, #<%= txtAdminPwd.ClientID %>').val('');
                });

                // Restore tab after postback
                let activeTab = $('#<%= hdnActiveTab.ClientID %>').val();
                let tabToShow = activeTab === 'admin'
                    ? 'pills-admin-tab'
                    : 'pills-student-tab';

                new bootstrap.Tab(document.getElementById(tabToShow)).show();
            });

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




    </div>


</body>
</html>
