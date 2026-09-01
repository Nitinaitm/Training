<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ExternalEmpEntry.aspx.cs" Inherits="Training.Admin.ExternalEmpEntry" %>
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="tab-pane fade show active"
                    id="singleentry">

                    <div class="row">

                        <!-- EmpID -->

                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Registration ID 
                            </label>

                            <asp:TextBox ID="txtEmpID"
                                runat="server" Enabled="false"
                                CssClass="form-control">
                            </asp:TextBox>

                           

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
                        <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Designation*
                            </label>

                            <asp:TextBox ID="txtDesignation"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvDesignation"
                                runat="server"
                                ControlToValidate="txtDesignation"
                                ErrorMessage=" Designation required"
                                CssClass="validation">
                            </asp:RequiredFieldValidator>

                        </div>
                         <div class="col-lg-4 col-md-6 mb-3">

                            <label class="form-label">
                                Organization*
                            </label>

                            <asp:TextBox ID="txtOrganization"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator ID="rfvOrganization"
                                runat="server"
                                ControlToValidate="txtOrganization"
                                ErrorMessage=" Organization required"
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
</asp:Content>
