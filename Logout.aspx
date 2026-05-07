<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="LMS.Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Logout | LMS</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
<!-- Favicon icon-->
<link rel="icon" href="assets/images/ficon.png" type="image/x-icon"/>
<link rel="shortcut icon" href="assets/images/ficon.png" type="image/x-icon"/>
<!-- Google font-->
<link rel="preconnect" href="https://fonts.googleapis.com"/>
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin=""/>
<link href="https://fonts.googleapis.com/css2?family=Nunito+Sans:opsz,wght@6..12,200;6..12,300;6..12,400;6..12,500;6..12,600;6..12,700;6..12,800;6..12,900;6..12,1000&amp;display=swap" rel="stylesheet"/>
<!-- Flag icon css -->
<link rel="stylesheet" href="assets/css/vendors/flag-icon.css"/>
<!-- iconly-icon-->
<link rel="stylesheet" href="assets/css/iconly-icon.css"/>
<link rel="stylesheet" href="assets/css/bulk-style.css"/>
<!-- iconly-icon-->
<link rel="stylesheet" href="assets/css/themify.css"/>
<!--fontawesome-->
<link rel="stylesheet" href="assets/css/fontawesome-min.css"/>


<!-- App css -->
<link rel="stylesheet" href="assets/css/style.css" />
<link id="color" rel="stylesheet" href="assets/css/color-1.css" media="screen"/>



    
   
      <style>
        body {
            background: linear-gradient(135deg, #198754, #157347);
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', sans-serif;
        }

        .logout-card {
            border-radius: 20px;
            padding: 40px;
            max-width: 420px;
            width: 100%;
            animation: fadeInUp 0.8s ease;
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
        }

        .icon-circle {
            width: 80px;
            height: 80px;
            background-color: #198754;
            color: white;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            margin: 0 auto 20px;
            animation: popIn 0.5s ease;
        }
          
.icon {
    width: 50px;
    height: 50px;
    display: flex;
    justify-content: center;
    align-items: center;
}

.icon img {
    max-width: 100%;
    max-height: 100%;
    object-fit: contain;
}

        .btn-login {
            border-radius: 50px;
            padding: 10px 25px;
            transition: 0.3s;
        }

        .btn-login:hover {
            transform: scale(1.05);
        }

        /* Animations */
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        @keyframes popIn {
            0% {
                transform: scale(0.5);
                opacity: 0;
            }
            100% {
                transform: scale(1);
                opacity: 1;
            }
        }

      
    </style>
    
</head>
<body class="d-flex justify-content-center align-items-center vh-100 bg-light">
    <form id="form1" runat="server">


    <div class="card logout-card text-center">

        <!-- Icon -->
        <div class="icon-circle">
            <img src="assets/images/logout.png" class="icon light"/>
        </div>

        <!-- Title -->
        <h3 class="fw-bold text-success">Logout Successful</h3>

        <!-- Message -->
        <p class="text-muted mt-3">
            You have been successfully logged out of your account.
        </p>

         <p class="text-muted small pt-1">Thank you. We hope to see you again soon!
</p>
      
        <!-- Button -->
        <a href="LMSLogin.aspx" class="btn btn-success btn-login mt-3">
            <i class="bi bi-box-arrow-in-right me-2"></i> Login Again
        </a>

    </div>
       
    </form>

      <script src="assets/js/vendors/bootstrap/dist/js/bootstrap.bundle.min.js" defer=""></script>
  <script src="assets/js/vendors/bootstrap/dist/js/popper.min.js" defer=""></script>
  <!--fontawesome-->
  <script src="assets/js/vendors/font-awesome/fontawesome-min.js"></script>
</body>
</html>
