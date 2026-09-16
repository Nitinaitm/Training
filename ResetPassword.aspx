<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Training.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function validateForm() {
            var newPassword = document.getElementById('<%= txtNewPassword.ClientID %>').value.trim();
            var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value.trim();
            var errorLabel = document.getElementById('<%= lblMessage.ClientID %>');

            errorLabel.innerHTML = ""; // Clear previous errors
            errorLabel.style.display = "none"; // Hide label initially

            // Check if passwords are empty
            if (newPassword === "" || confirmPassword === "") {
                errorLabel.innerHTML = "⚠ Please enter and confirm your new password.";
                errorLabel.style.display = "block";
                return false;
            }

            // Password strength check (Minimum 8 characters, 1 uppercase, 1 number, 1 special character)
            //var passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            var passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[A-Za-z\d@$!%*?&#^()_+\-=<>.,]{8,}$/;
            if (!passwordPattern.test(newPassword)) {
                errorLabel.innerHTML = "⚠ Password must be at least 8 characters long, contain at least one uppercase letter, one number, and one special character.";
                errorLabel.style.display = "block";
                return false;
            }

            // Check if passwords match
            if (newPassword !== confirmPassword) {
                errorLabel.innerHTML = "⚠ Passwords do not match!";
                errorLabel.style.display = "block";
                return false;
            }

            return true;
        }
    </script>

    <style>
        body {
            background-color: #f8f9fa;
        }

        .reset-container {
            max-width: 450px;
            background: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
            margin: auto;
            margin-top: 50px;
        }

        .reset-container h2 {
            font-size: 22px;
            font-weight: bold;
            color: #333;
        }

        .reset-container input {
            width: 100%;
            padding: 10px;
            margin: 10px 0;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
        }

        .reset-container input:focus {
            border-color: #007bff;
            outline: none;
        }

        .reset-container .btn-reset {
            background-color: #007bff;
            color: white;
            font-size: 18px;
            padding: 10px;
            border: none;
            width: 100%;
            border-radius: 5px;
            transition: 0.3s;
        }

        .reset-container .btn-reset:hover {
            background-color: #0056b3;
        }

        .error {
            color: red;
            font-size: 14px;
            margin-top: 10px;
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="reset-container text-center">
            <h2>🔐 Reset Your Password For First Time</h2>
            <hr>

            <asp:Panel ID="PNLLOGIN" runat="server">
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
                
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Confirm new password"></asp:TextBox>
                
                <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>

                <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password"
                    CssClass="btn-reset" OnClientClick="return validateForm();" OnClick="btnResetPassword_Click" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
