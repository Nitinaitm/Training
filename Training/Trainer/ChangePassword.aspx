<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Training.Trainer.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .info-box {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 15px
        }

        .info-label {
            font-weight: bold;
            color: #0d6efd
        }

        .btn-save {
            min-width: 150px
        }

        .password-hint {
            color: #6c757d;
            font-size: 13px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Change Password</div>
        <div class="row">
            <div class="col-md-6 offset-md-3">
                <div class="dashboard-card">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-key"></i>Change Your Password</h5>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <label>Current Password *</label><asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="form-control" /><asp:RequiredFieldValidator ID="rfvCurrent" runat="server" ControlToValidate="txtCurrentPassword" CssClass="text-danger" ErrorMessage="Required" ValidationGroup="ChangePwd" /></div>
                        <div class="mb-3">
                            <label>New Password *</label><asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control" /><asp:RequiredFieldValidator ID="rfvNew" runat="server" ControlToValidate="txtNewPassword" CssClass="text-danger" ErrorMessage="Required" ValidationGroup="ChangePwd" /><div class="password-hint">Password must be at least 6 characters long.</div>
                        </div>
                        <div class="mb-3">
                            <label>Confirm New Password *</label><asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" /><asp:RequiredFieldValidator ID="rfvConfirm" runat="server" ControlToValidate="txtConfirmPassword" CssClass="text-danger" ErrorMessage="Required" ValidationGroup="ChangePwd" /><asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtNewPassword" Operator="Equal" Type="String" ErrorMessage="Passwords do not match!" CssClass="text-danger" ValidationGroup="ChangePwd" /></div>
                        <div class="mt-3">
                            <asp:Button ID="btnChange" runat="server" Text="Change Password" CssClass="btn btn-success btn-save" OnClick="btnChange_Click" ValidationGroup="ChangePwd" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary btn-save ms-2" OnClick="btnClear_Click" CausesValidation="false" /><asp:Label ID="lblMessage" runat="server" Font-Bold="true" CssClass="ms-3" /></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
