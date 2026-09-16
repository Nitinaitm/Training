<%@ Page Title="Training Portal"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Training.Default" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <script src="../script.js" type="text/javascript"></script>

    <style type="text/css">

        /* =====================================================
           PAGE
        ===================================================== */

        .login-page {
            background: #f5f8fc;
            min-height: 520px;
        }

        .portal-login-section {
            padding: 24px 15px 30px 15px;
        }

        .login-container {
            width: 100%;
            max-width: 1180px;
            margin: 0 auto;
        }

        .portal-main-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 55px;
        }


        /* =====================================================
           LEFT - PORTAL INFORMATION
        ===================================================== */

        .portal-description {
            flex: 1;
            min-width: 0;
            padding: 20px 10px;
        }

        .portal-description-icon {
            width: 58px;
            height: 58px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 17px;
            border-radius: 12px;
            background: #173b67;
            color: #ffd54f;
            font-size: 25px;
            box-shadow: 0 5px 15px rgba(23, 59, 103, .16);
        }

        .portal-description h1 {
            margin: 0 0 12px 0;
            color: #173b67;
            font-size: 32px;
            line-height: 40px;
            font-weight: 800;
        }

        .hero-subtitle {
            max-width: 590px;
            margin: 0;
            color: #5d6c7b;
            font-size: 15px;
            line-height: 25px;
            text-align: left;
        }

        #more1 {
            display: none;
        }

        .read-more-btn {
            display: inline;
            padding: 0;
            margin: 0 0 0 4px;
            border: 0;
            background: transparent;
            color: #198754;
            font-family: inherit;
            font-size: 14px;
            font-weight: 800;
            cursor: pointer;
            outline: none !important;
        }

        .read-more-btn:hover {
            color: #173b67;
            text-decoration: underline;
        }


        /* =====================================================
           FEATURE LIST
        ===================================================== */

        .portal-feature-list {
            margin-top: 22px;
        }

        .portal-feature {
            display: flex;
            align-items: center;
            margin-bottom: 11px;
            color: #465667;
            font-size: 14px;
            font-weight: 700;
        }

        .portal-feature i {
            width: 28px;
            min-width: 28px;
            color: #198754;
            font-size: 15px;
        }


        /* =====================================================
           LOGIN COLUMN
        ===================================================== */

        .portal-login-column {
            flex: 0 0 475px;
            width: 475px;
            max-width: 475px;
        }


        /* =====================================================
           LOGIN CARD
        ===================================================== */

        .login-card {
            width: 100%;
            margin: 0;
            overflow: hidden;
            background: #ffffff;
            border: 1px solid #e3eaf2;
            border-radius: 12px;
            box-shadow: 0 7px 28px rgba(23, 59, 103, .13);
        }

        .login-card-header {
            padding: 14px 20px 13px 20px;
            background: #173b67;
            color: #ffffff;
            text-align: center;
        }

        .login-card-header-icon {
            width: 40px;
            height: 40px;
            margin: 0 auto 6px auto;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: rgba(255,255,255,.13);
            color: #ffd54f;
            font-size: 17px;
        }

        .login-card-header h3 {
            margin: 0 0 2px 0;
            color: #ffffff;
            font-size: 19px;
            font-weight: 800;
        }

        .login-card-header p {
            margin: 0;
            color: #dce7f3;
            font-size: 12px;
        }

        .login-card-body {
            padding: 18px 27px 16px 27px;
        }


        /* =====================================================
           FORM
        ===================================================== */

        .form-group-custom {
            margin-bottom: 12px;
        }

        .form-label-custom {
            display: block;
            margin: 0 0 5px 0;
            color: #34495e;
            font-size: 13px;
            font-weight: 700;
        }

        .form-label-custom i {
            width: 19px;
            color: #173b67;
            margin-right: 3px;
        }

        .login-input {
            display: block;
            width: 100%;
            height: 39px;
            padding: 7px 11px;
            border: 1px solid #ced7e2;
            border-radius: 6px;
            background: #ffffff;
            color: #333333;
            font-size: 13px;
            outline: none;
            transition: border-color .2s ease, box-shadow .2s ease;
        }

        .login-input:focus {
            border-color: #3f78b5;
            box-shadow: 0 0 0 3px rgba(23,59,103,.08);
        }


        /* =====================================================
           CAPTCHA
        ===================================================== */

        .captcha-wrapper {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 6px 9px;
            margin-bottom: 12px;
            border: 1px solid #e0e6ed;
            border-radius: 7px;
            background: #f8fafc;
        }

        .captcha-label {
            color: #34495e;
            font-size: 13px;
            font-weight: 700;
            white-space: nowrap;
        }

        .captcha-label i {
            color: #173b67;
            margin-right: 4px;
        }

        .captcha-image-box {
            margin-left: auto;
            padding: 2px;
            overflow: hidden;
            background: #ffffff;
            border: 1px solid #d8dee6;
            border-radius: 4px;
        }

        .captcha-image-box img {
            display: block;
            max-width: 100%;
            height: auto;
        }


        /* =====================================================
           OTP
        ===================================================== */

        .otp-box {
            padding: 10px 12px;
            margin-bottom: 12px;
            border: 1px dashed #9bb7d5;
            border-radius: 7px;
            background: #f4f8fc;
        }

        .otp-note {
            margin: 0 0 7px 0;
            color: #617184;
            font-size: 11px;
            line-height: 17px;
        }


        /* =====================================================
           BUTTONS
        ===================================================== */

        .login-actions {
            display: flex;
            justify-content: center;
            align-items: center;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 3px;
        }

        .portal-btn {
            min-width: 120px;
            min-height: 38px;
            padding: 7px 16px;
            border: 0;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            transition: all .2s ease;
        }

        .portal-btn-primary {
            background: #173b67;
            color: #ffffff;
        }

        .portal-btn-primary:hover {
            background: #102d50;
            color: #ffffff;
        }

        .portal-btn-success {
            background: #198754;
            color: #ffffff;
        }

        .portal-btn-success:hover {
            background: #146c43;
            color: #ffffff;
        }

        .portal-btn-secondary {
            background: #347c8c;
            color: #ffffff;
        }

        .portal-btn-secondary:hover {
            background: #286675;
            color: #ffffff;
        }


        /* =====================================================
           STATUS
        ===================================================== */

        .login-status {
            margin-top: 8px;
            text-align: center;
        }

        .message-label {
            display: block;
            font-size: 12px;
            font-weight: 700;
            line-height: 18px;
        }

        .timer-label {
            display: block;
            margin-top: 3px;
            color: #b02a37;
            font-size: 12px;
            font-weight: 700;
        }


        /* =====================================================
           FORGOT PASSWORD
        ===================================================== */

        .forgot-password {
            padding: 11px 15px;
            text-align: center;
            border-top: 1px solid #edf0f4;
            background: #fafbfd;
        }

        .forgot-password a {
            color: #173b67;
            font-size: 12px;
            font-weight: 700;
            text-decoration: none;
        }

        .forgot-password a i {
            margin-right: 5px;
            color: #198754;
        }

        .forgot-password a:hover {
            color: #198754;
            text-decoration: none;
        }


        /* =====================================================
           SECURITY INFO
        ===================================================== */

        .security-info {
            width: 100%;
            margin: 9px 0 0 0;
            display: flex;
            align-items: flex-start;
            gap: 8px;
            padding: 8px 11px;
            background: #edf7f2;
            border-left: 4px solid #198754;
            border-radius: 5px;
            color: #53635b;
            font-size: 11px;
            line-height: 17px;
        }

        .security-info i {
            color: #198754;
            font-size: 14px;
            margin-top: 2px;
        }


        /* =====================================================
           TABLET
        ===================================================== */

        @media (max-width: 991.98px) {

            .portal-login-section {
                padding: 22px 15px 28px 15px;
            }

            .portal-main-row {
                gap: 25px;
            }

            .portal-description {
                padding: 15px 5px;
            }

            .portal-description h1 {
                font-size: 26px;
                line-height: 33px;
            }

            .hero-subtitle {
                font-size: 14px;
                line-height: 23px;
            }

            .portal-login-column {
                flex: 0 0 440px;
                width: 440px;
                max-width: 440px;
            }

        }


        /* =====================================================
           MOBILE
        ===================================================== */

        @media (max-width: 767.98px) {

            .login-page {
                min-height: auto;
            }

            .portal-login-section {
                padding: 14px 10px 25px 10px;
            }

            .portal-main-row {
                flex-direction: column;
                gap: 22px;
            }

            /* LOGIN FIRST ON MOBILE */

            .portal-login-column {
                order: 1;
                flex: none;
                width: 100%;
                max-width: 520px;
            }

            /* DESCRIPTION AFTER LOGIN */

            .portal-description {
                order: 2;
                width: 100%;
                max-width: 520px;
                padding: 10px 5px;
            }

            .login-card {
                width: 100%;
            }

            .login-card-header {
                padding: 13px 15px;
            }

            .login-card-header-icon {
                width: 37px;
                height: 37px;
                font-size: 16px;
                margin-bottom: 5px;
            }

            .login-card-header h3 {
                font-size: 18px;
            }

            .login-card-body {
                padding: 17px 18px 15px 18px;
            }

            .portal-description-icon {
                width: 48px;
                height: 48px;
                margin-bottom: 12px;
                font-size: 20px;
            }

            .portal-description h1 {
                font-size: 23px;
                line-height: 29px;
            }

            .hero-subtitle {
                max-width: 100%;
                font-size: 14px;
                line-height: 22px;
            }

            .portal-feature-list {
                margin-top: 17px;
            }

            .captcha-wrapper {
                flex-wrap: wrap;
            }

            .security-info {
                width: 100%;
            }

        }


        /* =====================================================
           SMALL MOBILE
        ===================================================== */

        @media (max-width: 480px) {

            .portal-login-section {
                padding-left: 7px;
                padding-right: 7px;
            }

            .login-card-body {
                padding: 16px 14px 14px 14px;
            }

            .captcha-wrapper {
                align-items: flex-start;
                flex-direction: column;
            }

            .captcha-image-box {
                width: 100%;
                margin-left: 0;
                text-align: center;
            }

            .captcha-image-box img {
                margin: 0 auto;
            }

            .login-actions {
                flex-direction: column;
            }

            .portal-btn {
                width: 100%;
            }

            .portal-description h1 {
                font-size: 21px;
                line-height: 27px;
            }

        }
        /* =====================================================
   SMALL HEIGHT LAPTOP
   Example: 1366 x 768 / 1280 x 720
===================================================== */

@media (min-width: 768px) and (max-height: 800px) {

    .login-page {
        min-height: auto;
    }

    .portal-login-section {
        padding-top: 10px;
        padding-bottom: 12px;
    }

    .portal-main-row {
        align-items: center;
        gap: 35px;
    }

    .portal-description {
        padding-top: 5px;
        padding-bottom: 5px;
    }

    .portal-description-icon {
        width: 46px;
        height: 46px;
        margin-bottom: 10px;
        font-size: 20px;
    }

    .portal-description h1 {
        margin-bottom: 7px;
        font-size: 27px;
        line-height: 32px;
    }

    .hero-subtitle {
        font-size: 13px;
        line-height: 20px;
    }

    .portal-feature-list {
        margin-top: 13px;
    }

    .portal-feature {
        margin-bottom: 7px;
        font-size: 13px;
    }

    .login-card-header {
        padding: 9px 18px 8px 18px;
    }

    .login-card-header-icon {
        width: 32px;
        height: 32px;
        margin-bottom: 3px;
        font-size: 14px;
    }

    .login-card-header h3 {
        font-size: 17px;
    }

    .login-card-header p {
        font-size: 11px;
    }

    .login-card-body {
        padding: 11px 22px 10px 22px;
    }

    .form-group-custom {
        margin-bottom: 8px;
    }

    .form-label-custom {
        margin-bottom: 3px;
        font-size: 12px;
    }

    .login-input {
        height: 34px;
        padding: 5px 10px;
        font-size: 12px;
    }

    .captcha-wrapper {
        padding: 4px 8px;
        margin-bottom: 8px;
    }

    .captcha-label {
        font-size: 12px;
    }

    .otp-box {
        padding: 7px 10px;
        margin-bottom: 8px;
    }

    .otp-note {
        margin-bottom: 4px;
        line-height: 14px;
    }

    .portal-btn {
        min-height: 34px;
        padding: 5px 14px;
        font-size: 12px;
    }

    .login-status {
        margin-top: 5px;
    }

    .forgot-password {
        padding: 7px 12px;
    }

    .security-info {
        margin-top: 6px;
        padding: 6px 9px;
        line-height: 15px;
    }
}


/* Very short laptop screen */

@media (min-width: 768px) and (max-height: 700px) {

    .portal-login-section {
        padding-top: 6px;
        padding-bottom: 8px;
    }

    .portal-description-icon {
        display: none;
    }

    .portal-feature-list {
        margin-top: 10px;
    }

    .portal-feature {
        margin-bottom: 5px;
    }

    .login-card-header-icon {
        display: none;
    }

    .login-card-header {
        padding: 8px 15px;
    }

    .login-card-body {
        padding-top: 8px;
        padding-bottom: 8px;
    }

    .security-info {
        font-size: 10px;
    }
}
    </style>


    <script type="text/javascript">

        /* =====================================================
           READ MORE
        ===================================================== */

        function myFunction1() {

            var dots =
                document.getElementById("dots1");

            var moreText =
                document.getElementById("more1");

            var btnText =
                document.getElementById("myBtn1");

            if (
                moreText.style.display
                ===
                "inline"
            ) {

                dots.style.display =
                    "inline";

                moreText.style.display =
                    "none";

                btnText.innerHTML =
                    "Read more";

            }
            else {

                dots.style.display =
                    "none";

                moreText.style.display =
                    "inline";

                btnText.innerHTML =
                    "Show less";

            }

        }


        /* =====================================================
           OTP TIMER
        ===================================================== */

        var timer;

        var seconds =
            200;


        function startTimer() {

            clearInterval(timer);

            seconds =
                200;

            timer =
                setInterval(
                    function () {

                        var timerLabel =
                            document.getElementById(
                                '<%= lblTimer.ClientID %>');

                        if (timerLabel) {

                            timerLabel.innerText =
                                Math.max(
                                    seconds,
                                    0)
                                +
                                " seconds remaining";

                        }

                        seconds--;


                        if (
                            seconds
                            <
                            0
                        ) {

                            clearInterval(timer);


                            var resendButton =
                                document.getElementById(
                                    '<%= btnResendOTP.ClientID %>');

                            var loginButton =
                                document.getElementById(
                                    '<%= btnLogin.ClientID %>');

                            var otpContainer =
                                document.getElementById(
                                    '<%= otpVisible.ClientID %>');


                            if (resendButton) {

                                resendButton.style.display =
                                    "inline-block";

                            }


                            if (loginButton) {

                                loginButton.style.display =
                                    "none";

                            }


                            if (otpContainer) {

                                otpContainer.style.display =
                                    "none";

                            }


                            seconds =
                                0;

                        }

                    },
                    1000);

        }


        function stopTimer() {

            clearInterval(timer);


            var timerLabel =
                document.getElementById(
                    '<%= lblTimer.ClientID %>');

            var loginButton =
                document.getElementById(
                    '<%= btnLogin.ClientID %>');


            if (timerLabel) {

                timerLabel.innerText =
                    "";

            }


            if (loginButton) {

                loginButton.style.display =
                    "inline-block";

            }

        }


        function startTimerAfterDelay() {

            setTimeout(
                function () {

                    startTimer();

                },
                1000);

        }


        /* =====================================================
           ENTER KEY
        ===================================================== */

        function handleKeyPress(e) {

            var key =
                e.keyCode
                ||
                e.which;


            if (
                key
                !==
                13
            ) {

                return;

            }


            var focusedElement =
                document.activeElement;


            if (!focusedElement) {

                return;

            }


            e.preventDefault();


            if (
                focusedElement.id
                ===
                '<%= enterOTP.ClientID %>'
            ) {

                var loginButton =
                    document.getElementById(
                        '<%= btnLogin.ClientID %>');


                if (loginButton) {

                    loginButton.click();

                }

            }
            else {

                var otpButton =
                    document.getElementById(
                        '<%= btnOTP.ClientID %>');


                if (otpButton) {

                    otpButton.click();

                    startTimerAfterDelay();

                }

            }

        }


        document.addEventListener(
            "keydown",
            handleKeyPress);

    </script>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">


    <asp:ScriptManager
        ID="ScriptManager2"
        runat="server"
        EnableCdn="true">
    </asp:ScriptManager>


    <div class="login-page">

        <section class="portal-login-section">

            <div class="login-container">

                <div class="portal-main-row">


                    <!-- ================================================= -->
                    <!-- LEFT SIDE : PORTAL INFORMATION                    -->
                    <!-- ================================================= -->

                    <div class="portal-description">

                        <div class="portal-description-icon">

                            <i class="fas fa-graduation-cap"></i>

                        </div>


                        <h1>
                            Training Management Portal
                        </h1>


                        <p class="hero-subtitle">

                            A centralized platform for managing employee
                            training activities, schedules, examinations,
                            feedback and certification.

                            <span id="dots1">...</span>

                            <span id="more1">

                                The portal helps administrators, trainers and
                                trainees manage the complete training lifecycle
                                efficiently. Training records, participation,
                                assessments and related information are available
                                through a single integrated system.

                            </span>

                            <button
                                type="button"
                                id="myBtn1"
                                class="read-more-btn"
                                onclick="myFunction1();">

                                Read more

                            </button>

                        </p>


                        <div class="portal-feature-list">

                            <div class="portal-feature">

                                <i class="fas fa-check-circle"></i>

                                Training &amp; Session Management

                            </div>


                            <div class="portal-feature">

                                <i class="fas fa-check-circle"></i>

                                Online Pre &amp; Post Training Examination

                            </div>


                            <div class="portal-feature">

                                <i class="fas fa-check-circle"></i>

                                Attendance &amp; Feedback Management

                            </div>


                            <div class="portal-feature">

                                <i class="fas fa-check-circle"></i>

                                Online Training Certificate

                            </div>

                        </div>

                    </div>


                    <!-- ================================================= -->
                    <!-- RIGHT SIDE : LOGIN                               -->
                    <!-- ================================================= -->

                    <div class="portal-login-column">


                        <asp:UpdatePanel
                            ID="UpdatePanel2"
                            runat="server"
                            UpdateMode="Conditional">

                            <ContentTemplate>


                                <asp:UpdatePanel
                                    ID="UpdatePanel1"
                                    runat="server"
                                    UpdateMode="Conditional">

                                    <ContentTemplate>


                                        <asp:Panel
                                            ID="PNLLOGIN"
                                            runat="server"
                                            CssClass="login-card">


                                            <!-- ================================= -->
                                            <!-- LOGIN HEADER                      -->
                                            <!-- ================================= -->

                                            <div class="login-card-header">

                                                <div class="login-card-header-icon">

                                                    <i class="fas fa-user-lock"></i>

                                                </div>

                                                <h3>
                                                    User Login
                                                </h3>

                                                <p>
                                                    Secure access using OTP verification
                                                </p>

                                            </div>


                                            <!-- ================================= -->
                                            <!-- LOGIN BODY                        -->
                                            <!-- ================================= -->

                                            <div class="login-card-body">

                                                <div id="enab">


                                                    <!-- USER ID -->

                                                    <div class="form-group-custom">

                                                        <label
                                                            class="form-label-custom"
                                                            for="<%= txtUserId.ClientID %>">

                                                            <i class="fas fa-user"></i>

                                                            User ID

                                                        </label>


                                                        <asp:TextBox
                                                            ID="txtUserId"
                                                            runat="server"
                                                            CssClass="login-input"
                                                            MaxLength="20"
                                                            required="required"
                                                            autocomplete="username"
                                                            placeholder="Enter User ID">
                                                        </asp:TextBox>

                                                    </div>


                                                    <!-- PASSWORD -->

                                                    <div class="form-group-custom">

                                                        <label
                                                            class="form-label-custom"
                                                            for="<%= txtPassword.ClientID %>">

                                                            <i class="fas fa-lock"></i>

                                                            Password

                                                        </label>


                                                        <asp:TextBox
                                                            ID="txtPassword"
                                                            runat="server"
                                                            CssClass="login-input"
                                                            TextMode="Password"
                                                            MaxLength="20"
                                                            autocomplete="current-password"
                                                            placeholder="Enter Password">
                                                        </asp:TextBox>

                                                    </div>


                                                    <!-- CAPTCHA -->

                                                    <div class="captcha-wrapper">

                                                        <div class="captcha-label">

                                                            <i class="fas fa-shield-alt"></i>

                                                            Security Code

                                                        </div>


                                                        <div class="captcha-image-box">

                                                            <img
                                                                src="CaptchaImage.aspx"
                                                                alt="CAPTCHA Security Code" />

                                                        </div>

                                                    </div>


                                                    <!-- ENTER CAPTCHA -->

                                                    <div class="form-group-custom">

                                                        <label
                                                            class="form-label-custom"
                                                            for="<%= txtCaptcha.ClientID %>">

                                                            <i class="fas fa-keyboard"></i>

                                                            Enter Captcha

                                                        </label>


                                                        <asp:TextBox
                                                            ID="txtCaptcha"
                                                            runat="server"
                                                            CssClass="login-input"
                                                            Placeholder="Enter security code"
                                                            required="required"
                                                            MaxLength="10"
                                                            autocomplete="off">
                                                        </asp:TextBox>

                                                    </div>


                                                    <!-- OTP -->

                                                    <div
                                                        id="otpVisible"
                                                        runat="server"
                                                        visible="false"
                                                        class="otp-box">


                                                        <label
                                                            class="form-label-custom"
                                                            for="<%= enterOTP.ClientID %>">

                                                            <i class="fas fa-mobile-alt"></i>

                                                            Enter OTP

                                                        </label>


                                                        <p class="otp-note">

                                                            Enter the 4-digit OTP sent to your
                                                            registered mobile number.

                                                        </p>


                                                        <asp:TextBox
                                                            ID="enterOTP"
                                                            runat="server"
                                                            CssClass="login-input"
                                                            required="required"
                                                            MaxLength="4"
                                                            Enabled="false"
                                                            autocomplete="one-time-code"
                                                            placeholder="Enter OTP">
                                                        </asp:TextBox>

                                                    </div>


                                                    <!-- BUTTONS -->

                                                    <div class="login-actions">


                                                        <asp:Button
                                                            ID="btnOTP"
                                                            runat="server"
                                                            Text="Send OTP"
                                                            CssClass="portal-btn portal-btn-primary"
                                                            ValidationGroup="v"
                                                            OnClick="btnOTP_Click"
                                                            OnClientClick="startTimer();" />


                                                        <asp:Button
                                                            ID="btnResendOTP"
                                                            runat="server"
                                                            Text="Resend OTP"
                                                            CssClass="portal-btn portal-btn-secondary"
                                                            ValidationGroup="v"
                                                            OnClick="btnResendOTP_Click"
                                                            Style="display:none;" />


                                                        <asp:Button
                                                            ID="btnLogin"
                                                            runat="server"
                                                            Text="Log In"
                                                            CssClass="portal-btn portal-btn-success"
                                                            ValidationGroup="v"
                                                            OnClick="btnLogin_Click"
                                                            Visible="false"
                                                            Enabled="false" />

                                                    </div>


                                                    <!-- STATUS -->

                                                    <div class="login-status">


                                                        <asp:Label
                                                            ID="lblMsg"
                                                            runat="server"
                                                            CssClass="message-label"
                                                            Font-Bold="true"
                                                            ForeColor="Green">
                                                        </asp:Label>


                                                        <asp:Label
                                                            ID="lblTimer"
                                                            runat="server"
                                                            CssClass="timer-label"
                                                            Visible="false">
                                                        </asp:Label>


                                                    </div>

                                                </div>

                                            </div>


                                            <!-- ================================= -->
                                            <!-- FORGOT PASSWORD                   -->
                                            <!-- ================================= -->

                                            <div class="forgot-password">

                                                <a href="Forget_Password.aspx">

                                                    <i class="fas fa-unlock-alt"></i>

                                                    Forgot Password? Reset your password

                                                </a>

                                            </div>


                                        </asp:Panel>


                                    </ContentTemplate>

                                </asp:UpdatePanel>


                                <!-- ============================================= -->
                                <!-- SECURITY MESSAGE                              -->
                                <!-- ============================================= -->

                                <div class="security-info">

                                    <i class="fas fa-shield-alt"></i>

                                    <div>

                                        For your security, do not share your
                                        password or OTP with anyone.

                                    </div>

                                </div>


                            </ContentTemplate>

                        </asp:UpdatePanel>


                    </div>

                </div>

            </div>

        </section>

    </div>

</asp:Content>