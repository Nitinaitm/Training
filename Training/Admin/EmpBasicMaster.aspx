<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="EmpBasicMaster.aspx.cs"
    Inherits="Training.Admin.EmpBasicMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- Bootstrap -->

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <!-- Select2 -->

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <!-- jQuery -->

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <!-- Select2 -->

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <script>

        function LoadSearchableDropdown() {

            $('[id*=ddlDesignation]').select2({
                placeholder: 'Search Designation',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlPostingPlace]').select2({
                placeholder: 'Search Posting Place',
                allowClear: true,
                width: '100%'
            });

        }

        function ClearSearchableDropdown() {

            $('[id*=ddlDesignation]').val('').trigger('change');

            $('[id*=ddlPostingPlace]').val('').trigger('change');

        }

        $(document).ready(function () {

            LoadSearchableDropdown();

        });

    </script>

    <style>

        body {
            background-color: #f5f5f5;
        }

        .main-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0px 0px 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .nav-tabs .nav-link.active {
            background-color: darkcyan;
            color: white !important;
        }

        .btn-save {
            background-color: darkcyan;
            color: white;
            border: none;
        }

        .btn-save:hover {
            background-color: teal;
            color: white;
        }

        .select2-container {
            width: 100% !important;
        }

        .select2-selection--single {
            height: 38px !important;
            padding-top: 4px;
            border: 1px solid #ced4da !important;
        }

    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Employee Master
            </div>

            <!-- Tabs -->

            <ul class="nav nav-tabs">

                <li class="nav-item">

                    <button class="nav-link active"
                        data-bs-toggle="tab"
                        data-bs-target="#singleentry"
                        type="button">

                        Single Entry

                    </button>

                </li>

                <li class="nav-item">

                    <button class="nav-link"
                        data-bs-toggle="tab"
                        data-bs-target="#bulkentry"
                        type="button">

                        Bulk Upload

                    </button>

                </li>

            </ul>

            <!-- Tab Content -->

            <div class="tab-content mt-4">

                <!-- SINGLE ENTRY -->

                <div class="tab-pane fade show active"
                    id="singleentry">

                    <div class="row">

                        <!-- EmpID -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Employee ID *
                            </label>

                            <asp:TextBox ID="txtEmpID"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvEmpID"
                                runat="server"
                                ControlToValidate="txtEmpID"
                                ErrorMessage="Employee ID required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- EmpName -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Employee Name *
                            </label>

                            <asp:TextBox ID="txtEmpName"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvEmpName"
                                runat="server"
                                ControlToValidate="txtEmpName"
                                ErrorMessage="Employee Name required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- DOB -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                DOB *
                            </label>

                            <asp:TextBox ID="txtDOB"
                                runat="server"
                                TextMode="Date"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvDOB"
                                runat="server"
                                ControlToValidate="txtDOB"
                                ErrorMessage="DOB required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- DOJ -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                DOJ *
                            </label>

                            <asp:TextBox ID="txtDOJ"
                                runat="server"
                                TextMode="Date"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvDOJ"
                                runat="server"
                                ControlToValidate="txtDOJ"
                                ErrorMessage="DOJ required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- Mobile -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Mobile Number *
                            </label>

                            <asp:TextBox ID="txtMobileNo"
                                runat="server"
                                CssClass="form-control"
                                MaxLength="10">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvMobile"
                                runat="server"
                                ControlToValidate="txtMobileNo"
                                ErrorMessage="Mobile Number required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                            <br />

                            <asp:RegularExpressionValidator ID="revMobile"
                                runat="server"
                                ControlToValidate="txtMobileNo"
                                ValidationExpression="^[0-9]{10}$"
                                ErrorMessage="Enter valid 10 digit mobile number"
                                CssClass="validation">
                            </asp:RegularExpressionValidator>

                        </div>

                        <!-- Email -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Email ID
                            </label>

                            <asp:TextBox ID="txtEmailId"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                        </div>

                        <!-- Company -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Company *
                            </label>

                            <asp:DropDownList ID="ddlCompany"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                            <asp:RequiredFieldValidator ID="rfvCompany"
                                runat="server"
                                ControlToValidate="ddlCompany"
                                InitialValue=""
                                ErrorMessage="Select Company"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- Designation -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Designation *
                            </label>

                            <asp:DropDownList ID="ddlDesignation"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                            <asp:RequiredFieldValidator ID="rfvDesignation"
                                runat="server"
                                ControlToValidate="ddlDesignation"
                                InitialValue=""
                                ErrorMessage="Select Designation"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- Posting Place -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Posting Place
                            </label>

                            <asp:DropDownList ID="ddlPostingPlace"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                        </div>

                        <!-- Save -->

                        <div class="col-12 mt-3">

                            <asp:Button ID="btnSave"
                                runat="server"
                                Text="Save Employee"
                                CssClass="btn btn-save"
                                UseSubmitBehavior="false"
                                OnClick="btnSave_Click" />

                        </div>

                        <!-- Message -->

                        <div class="col-12 mt-3">

                            <asp:Label ID="lblSingleMessage"
                                runat="server"
                                Font-Bold="true">
                            </asp:Label>

                        </div>

                    </div>

                </div>

                <!-- BULK UPLOAD -->

                <div class="tab-pane fade"
                    id="bulkentry">

                    <div class="row">

                        <!-- Upload -->

                        <div class="col-lg-6 mb-3">

                            <label class="form-label">
                                Upload Excel File
                            </label>

                            <asp:FileUpload ID="fuExcel"
                                runat="server"
                                CssClass="form-control" />

                        </div>

                        <!-- Upload Button -->

                        <div class="col-lg-6 mb-3 d-flex align-items-end">

                            <asp:Button ID="btnUpload"
                                runat="server"
                                Text="Upload Excel"
                                CssClass="btn btn-success"
                                CausesValidation="false"
                                UseSubmitBehavior="false"
                                OnClick="btnUpload_Click" />

                        </div>

                        <!-- Excel Format -->

                        <div class="col-12">

                            <div class="alert alert-info">

                                <strong>
                                    Excel Format:
                                </strong>

                                <br />

                                EmpID |
                                EmpName |
                                DOB |
                                DOJ |
                                MobileNo |
                                EmailId |
                                EmpCompany |
                                EmpDesignation |
                                EmpPostingPlace

                                <br /><br />

                                First row must contain headers.

                            </div>

                        </div>

                        <!-- Bulk Message -->

                        <div class="col-12 mt-3">

                            <asp:Label ID="lblBulkMessage"
                                runat="server"
                                Font-Bold="true">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

    <!-- Bootstrap -->

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</asp:Content>