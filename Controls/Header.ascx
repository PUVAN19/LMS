<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="Controls.Header" %>


<!-- Favicon icon-->
<link rel="icon" href="../assets/images/favicon2.jpg" type="image/x-icon" />
<link rel="shortcut icon" href="../assets/images/favicon2.jpg" type="image/x-icon" />
<!-- Google font-->
<link rel="preconnect" href="https://fonts.googleapis.com" />
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="" />
<link href="https://fonts.googleapis.com/css2?family=Nunito+Sans:opsz,wght@6..12,200;6..12,300;6..12,400;6..12,500;6..12,600;6..12,700;6..12,800;6..12,900;6..12,1000&amp;display=swap" rel="stylesheet" />
<!-- Flag icon css -->
<link rel="stylesheet" href="../assets/css/vendors/flag-icon.css" />
<!-- iconly-icon-->
<link rel="stylesheet" href="../assets/css/iconly-icon.css" />
<link rel="stylesheet" href="../assets/css/bulk-style.css" />
<!-- iconly-icon-->
<%--<link rel="stylesheet" href="../assets/css/themify.css"/>--%>
<!--fontawesome-->
<link rel="stylesheet" href="../assets/css/fontawesome-min.css" />
<!-- Whether Icon css-->
<link rel="stylesheet" type="text/css" href="../assets/css/vendors/scrollbar.css" />
<link rel="stylesheet" type="text/css" href="../assets/css/vendors/slick.css" />
<link rel="stylesheet" type="text/css" href="../assets/css/vendors/slick-theme.css" />
<!-- App css -->
<link rel="stylesheet" href="../assets/css/style.css" />
<link id="color" rel="stylesheet" href="../assets/css/color-1.css" media="screen" />

<link href="../assets/js/toastr/toastr.min.css" rel="stylesheet" />
<style>
    @media (max-width: 1192px) {
        .lms-title {
            color: #000 !important;
        }
    }
</style>
<style>
    /* Parent menu green */
    .sidebar-menu .sidebar-list > a,
    .sidebar-menu .sidebar-title,
    .sidebar-menu .sidebar-list > a span,
    .sidebar-menu .sidebar-list > a h6 {
        color: #1E8A75 !important;
        font-weight: 600;
    }

    /* Parent menu icon green */
    .sidebar-menu svg,
    .sidebar-menu svg use {
        stroke: #1E8A75 !important;
        fill: #1E8A75 !important;
    }

    /* Child menu — restore original color */
    .sidebar-submenu li a {
        color: #2A3547 !important; /* ORIGINAL TEXT COLOR */
        font-weight: normal !important;
    }

    /* Hover effects */
    .sidebar-menu .sidebar-list > a:hover {
        color: #136754 !important;
    }

    .sidebar-submenu li a:hover {
        color: #000 !important; /* Dark on hover */
    }
    .toggle-sidebar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  transition: background-color 0.3s ease;
}

/* background on hover */
.toggle-sidebar:hover {
  background-color: #ffffff;
}

/* ✅ icon color on hover */
.toggle-sidebar:hover i {
  color: #198754 /* change to needed color */
}
@media (max-width: 768px) {
    .header-right {
        display: flex !important;
        align-items: center;
    }

    .profile-nav {
        display: block !important;
    }

    .user-img {
        display: block !important;
    }

    .user-img img {
        width: 35px;
        height: 35px;
        object-fit: cover;
    }

    .user-content {
        display: none; /* hide username on mobile, show only icon */
    }

    .custom-menu {
        right: 0;
        left: auto;
        min-width: 180px;
    }
}
</style>

<!-- jquery-->
<script src="../assets/js/vendors/jquery/jquery.min.js"></script>
<script src="../assets/js/toastr/toastr.min.js"></script>
<script src="../assets/js/Custom-Toasts.js"></script>


<!-- page-wrapper Start-->
<!-- tap on top starts-->
<div class="tap-top"><i class="iconly-Arrow-Up icli"></i></div>
<!-- tap on tap ends-->
<!-- loader-->
<div class="loader-wrapper">
    <div class="loader"><span></span><span></span><span></span><span></span><span></span></div>
</div>
<div class="page-wrapper compact-wrapper" id="pageWrapper">
    <header class="page-header row">
        <div class="logo-wrapper d-flex align-items-center justify-content-between px-3 py-2 col-auto">

            <h6 class="lms-title text-white fw-bold fs-6 mb-0 lh-sm">Library<br>
                <span class="fw-bold">Management System</span>
            </h6>
          




            <a class="close-btn toggle-sidebar text-white ms-2" href="javascript:void(0)" aria-label="Toggle Sidebar">
                <i class="fa-solid fa-bars"></i>
            </a>
        </div>
        <div class="page-main-header col">
            <div class="header-left">
            </div>
            <div class="nav-right">
                <ul class="header-right">

                    <li class="profile-nav custom-dropdown">
                        <div class="user-wrap">
                            <div class="user-img">
                                <img src="../assets/images/user_img.png" alt="user" class="img-fluid" />
                            </div>

                            <div class="user-content">
                                <h6>
                                    <asp:Literal ID="ltrUserName" runat="server"></asp:Literal></h6>

                            </div>
                        </div>
                        <div class="custom-menu overflow-hidden">
                            <ul class="profile-body">
                                <li class="d-flex">
                                    <svg class="svg-color">
                                        <use href="../assets/svg/iconly-sprite.svg#Profile"></use>
                                    </svg><a class="ms-2" href="/LMS/Admin/ChangePassword.aspx">Change Password</a>
                                </li>
                                <li class="d-flex">
                                    <svg class="svg-color">
                                        <use href="../assets/svg/iconly-sprite.svg#Login"></use>
                                    </svg>
                                    <a class="ms-2" href="/LMS/Logout.aspx" onclick="return confirmLogout();">Log Out</a>
                                </li>

                                <script type="text/javascript">
                                    function confirmLogout() {
                                        // Show confirmation popup
                                        return confirm("Are you sure you want to log out?");
                                        // If user clicks "Cancel", link won't navigate
                                    }
                                </script>

                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>


    </header>
    <!-- Page Body Start-->
    <div class="page-body-wrapper">
        <!-- Page sidebar start-->
        <aside class="page-sidebar">
            <div class="left-arrow" id="left-arrow"><i data-feather="arrow-left"></i></div>
            <div class="main-sidebar" id="main-sidebar">
                <ul class="sidebar-menu" id="simple-bar">
                    <li class="pin-title sidebar-main-title">
                        <div>
                            <h5 class="sidebar-title f-w-700">Pinned</h5>
                        </div>
                    </li>

                    <asp:Literal ID="ltrMenu" runat="server"></asp:Literal>
                </ul>



            </div>
            <div class="right-arrow" id="right-arrow"><i data-feather="arrow-right"></i></div>
        </aside>
