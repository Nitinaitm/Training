<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Training.Trainer.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
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

        .profile-photo {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            object-fit: cover;
            border: 3px solid #198754
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

        .select2-container {
            width: 100% !important
        }

        .btn-save {
            min-width: 150px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">My Profile</div>
        <div class="row">
            <div class="col-md-4">
                <div class="dashboard-card text-center">
                    <div class="mb-3">
                        <asp:Image ID="imgPhoto" runat="server" CssClass="profile-photo" ImageUrl="~/Images/default-user.png" AlternateText="Profile Photo" /></div>
                    <h4>
                        <asp:Label ID="lblTrainerName" runat="server" /></h4>
                    <p><span class="badge bg-primary">
                        <asp:Label ID="lblTrainerID" runat="server" /></span></p>
                    <p>
                        <asp:Label ID="lblTrainerType" runat="server" CssClass="badge bg-info" /></p>
                    <div class="mt-3">
                        <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" /><asp:Button ID="btnUploadPhoto" runat="server" Text="Upload Photo" CssClass="btn btn-primary btn-sm mt-2" OnClick="btnUploadPhoto_Click" /></div>
                    <div class="mt-2">
                        <asp:Label ID="lblPhotoMsg" runat="server" Font-Bold="true" /></div>
                </div>
            </div>
            <div class="col-md-8">
                <div class="dashboard-card">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fa fa-user-edit"></i>Edit Profile</h5>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <label>Employee ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#f8f9fa" /></div>
                            <div class="col-md-6">
                                <label>Trainer ID</label><asp:TextBox ID="txtTrainerID" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#f8f9fa" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <label>Full Name *</label><asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" /></div>
                            <div class="col-md-6">
                                <label>Email *</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <label>Mobile Number *</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" MaxLength="10" /></div>
                            <div class="col-md-6">
                                <label>Designation</label><asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <label>Organization</label><asp:TextBox ID="txtOrganization" runat="server" CssClass="form-control" /></div>
                            <div class="col-md-6">
                                <label>Area of Expertise</label><asp:DropDownList ID="ddlExpertise" runat="server" CssClass="form-select" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <label>Highest Qualification</label><asp:DropDownList ID="ddlQualification" runat="server" CssClass="form-select" /></div>
                            <div class="col-md-6">
                                <label>Experience (Years)</label><asp:TextBox ID="txtExperience" runat="server" CssClass="form-control" TextMode="Number" Step="0.5" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <label>Availability</label><asp:DropDownList ID="ddlAvailability" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Available" Value="Available" />
                                    <asp:ListItem Text="Busy" Value="Busy" />
                                    <asp:ListItem Text="On Leave" Value="On Leave" />
                                    <asp:ListItem Text="Inactive" Value="Inactive" />
                                </asp:DropDownList></div>
                            <div class="col-md-6">
                                <label>Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Active" Value="Active" />
                                    <asp:ListItem Text="Inactive" Value="Inactive" />
                                </asp:DropDownList></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-12">
                                <label>Certifications</label><asp:TextBox ID="txtCertifications" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-12">
                                <label>Profile / Bio</label><asp:TextBox ID="txtProfile" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" /></div>
                        </div>
                        <div class="mt-3">
                            <asp:Button ID="btnSave" runat="server" Text="Update Profile" CssClass="btn btn-success btn-save" OnClick="btnSave_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary btn-save ms-2" OnClick="btnReset_Click" /><asp:Label ID="lblMessage" runat="server" Font-Bold="true" CssClass="ms-3" /></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>$(document).ready(function () { $('#ddlExpertise,#ddlQualification').select2({ width: '100%', placeholder: 'Select' }); }); if (typeof (Sys) !== 'undefined') { Sys.Application.add_load(function () { $('#ddlExpertise,#ddlQualification').select2({ width: '100%', placeholder: 'Select' }); }); }</script>
</asp:Content>
