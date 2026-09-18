<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Forget_Password.aspx.cs" Inherits="Training.Forget_Password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script.js" type="text/javascript"></script>
    <script type="text/javascript">
        function DisableBackButton() {
            window.history.forward()
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
        window.onunload = function () { void (0) }
    </script>
    <script>
        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
    <%--<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />--%>
    <script>
        function validatePassword() {
            var password = document.getElementById('<%= txtNewPassword.ClientID %>').value;
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;
            var message = "";

            // Password strength rules
            if (password.length < 8) {
                message = "Password must be at least 8 characters long.";
            } else if (!/[A-Z]/.test(password)) {
                message = "Password must contain at least one uppercase letter.";
            } else if (!/[a-z]/.test(password)) {
                message = "Password must contain at least one lowercase letter.";
            } else if (!/[0-9]/.test(password)) {
                message = "Password must contain at least one number.";
            } else if (!/[!@#$%^&*_]/.test(password)) {
                message = "Password must contain at least one special character (!@#$%^&*_).";
            }
            else if (password !== confirmPassword) {
                message = "Passwords do not match.";
            }

            // Display message
            document.getElementById("passwordMessage").innerText = message;

            // Prevent form submission if validation fails
            return message === "";
        }
    </script>
    <style>
        .btn-custom {
            display: inline-block;
            font-size: 16px;
            font-weight: 600;
            color: #FF5733;
            text-decoration: none;
            padding: 10px 20px;
            border: 2px solid #FF5733;
            border-radius: 5px;
            transition: 0.3s;
        }

            .btn-custom:hover {
                color: #FFFFFF;
                background-color: #3366FF;
                border-color: #3366FF;
            }
    </style>
    <style>
        .CSSTableGenerator {
            margin: 0px;
            padding-top: 15px;
            padding-bottom: 5px;
            text-align: center;
            width: 100%;
            height: auto;
            box-shadow: 0px 1px 3px 3px #CCCCCC;
            border: 1px solid #006699;
            -moz-border-radius-bottomleft: 14px;
            -webkit-border-bottom-left-radius: 14px;
            border-bottom-left-radius: 14px;
            -moz-border-radius-bottomright: 14px;
            -webkit-border-bottom-right-radius: 14px;
            border-bottom-right-radius: 14px;
            -moz-border-radius-topright: 14px;
            -webkit-border-top-right-radius: 14px;
            border-top-right-radius: 14px;
            -moz-border-radius-topleft: 14px;
            -webkit-border-top-left-radius: 14px;
            border-top-left-radius: 14px;
        }
    </style>
   

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    <br />
    <div class="container-fluid pt-5">
        <div class="container">
            <div class="row">
                <div class="col-md-3">
                </div>
                <div class="col-md-12 text-center center">
                    <div class="text-center pb-2">

                        <div>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="PNLLOGIN" runat="server" CssClass="CSSTableGenerator">
                                        <div class="col-md-6 offset-md-3">
                                            <h3 class="text-center" style="color: green; font-weight: bolder">Reset Password</h3>
                                            <br />
                                            <div id="emp_show" runat="server">


                                                <div class="mb-3">
                                                    <%--<label for="txtEmpID" class="form-label">Enter Username/ Emp ID</label>--%>
                                                    <asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" required="required" MaxLength="20" Placeholder="Enter Username/ Emp ID"></asp:TextBox>




                                                </div>

                                                <div class="mb-3">
                                                    <label for="CAPTCHA" class="form-label">Captcha</label>
                                                    <img src="CaptchaImage.aspx" alt="CAPTCHA" /><br />
                                                    <%--<label for="txtCaptcha"><b>Enter Captcha</b></label>--%>
                                                    <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control" Placeholder="Enter CAPTCHA" required="required" MaxLength="10"></asp:TextBox>

                                                </div>
                                                <div class="text-center">
                                                    <asp:Label ID="lblMessage2" runat="server" CssClass="text-danger" Font-Bold="True" ForeColor="Green"></asp:Label>

                                                </div>

                                                <div class="mb-3 text-center">
                                                    <asp:Button ID="btnSendOTP" runat="server" Text="Send OTP" CssClass="btn btn-primary" OnClick="btnSendOTP_Click" />
                                                </div>

                                            </div>
                                            <div id="enter_otp" runat="server" visible="false">


                                                

                                                <div class="mb-3 ">
                                                    <%--<label for="txtOTPMobile" class="form-label">Enter OTP (Mobile)</label>--%>
                                                    <asp:TextBox ID="txtOTPMobile" runat="server" CssClass="form-control" required="required" Placeholder="Enter OTP (Mobile)" MaxLength="4"></asp:TextBox>
                                                </div>
                                                <!-- New Password -->
                                                <div class="mb-3 ">
                                                    <%--<label for="txtNewPassword" class="form-label">New Password</label>--%>
                                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" Placeholder="Enter New Password" TextMode="Password" MaxLength="49" required="required"></asp:TextBox>
                                                </div>

                                                <!-- Confirm Password -->
                                                <div class="mb-3 ">
                                                    <%--<label for="txtConfirmPassword" class="form-label">Confirm Password</label>--%>
                                                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Confirm Password" MaxLength="49" required="required"></asp:TextBox>
                                                </div>

                                                <div class="text-center">
                                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </div>
                                                <div class="text-center">
                                                    <asp:Label ID="lblMessage1" runat="server" CssClass="text-danger" Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </div>
                                                <!-- Reset Password Button -->
                                                <div class="mb-3 text-center">
                                                    <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="btn btn-success" OnClientClick="return validatePassword();" OnClick="btnResetPassword_Click" />
                                                </div>
                                                <div class="text-center">
                                                    <asp:Label ID="lblMessage3" runat="server" CssClass="text-danger" Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </div>
                                            </div>
                                            <div id="home" runat="server" visible="false" class="text-center">
                                                <!-- Message Label -->
                                                <asp:Label ID="Label1" runat="server" CssClass="text-danger" Font-Bold="True" ForeColor="Green"></asp:Label>
                                                <br />
                                                <br />
                                                <div>
                                                    <a href="Default.aspx" class="btn-custom">Go To Login</a>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnSendOTP" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
