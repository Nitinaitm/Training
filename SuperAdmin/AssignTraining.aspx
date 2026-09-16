
<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="AssignTraining.aspx.cs" Inherits="Training.SuperAdmin.AssignTraining" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>

        body { background-color: #f5f5f5; }

        .main-card {
            background: #ffffff;
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

        .btn-save {
            background-color: darkcyan;
            color: white;
            border: none;
        }

        .btn-save:hover {
            background-color: teal;
            color: white;
        }

        .select2-container { width: 100% !important; }

        .select2-container--default .select2-selection--multiple {
            min-height: 38px !important;
            border: 1px solid #ced4da !important;
        }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Assign Training
            </div>

            <ul class="nav nav-tabs mb-4" id="myTab" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="single-tab" data-bs-toggle="tab" data-bs-target="#single" type="button">Single / Multiple Assignment</button>
                </li>

                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="bulk-tab" data-bs-toggle="tab" data-bs-target="#bulk" type="button">Bulk Assignment</button>
                </li>
            </ul>

            <div class="tab-content">

                <!-- SINGLE MULTIPLE ASSIGN -->

                <div class="tab-pane fade show active" id="single">

                    <div class="row">

                        <div class="col-12 col-md-6 col-lg-4 mb-3">

                            <label class="form-label">Training *</label>

                            <asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select"></asp:DropDownList>

                            <asp:RequiredFieldValidator ID="rfvTraining" runat="server" ControlToValidate="ddlTraining" InitialValue="" ValidationGroup="Assign" ErrorMessage="Select Training" CssClass="validation"></asp:RequiredFieldValidator>

                        </div>

                        <div class="col-12 col-md-6 col-lg-8 mb-3">

                            <label class="form-label">Employee *</label>

                            <asp:ListBox ID="lstEmployee" runat="server" CssClass="form-control" SelectionMode="Multiple"></asp:ListBox>

                        </div>

                        <div class="col-12 mt-2">

                            <asp:Button ID="btnAssign" runat="server" Text="Assign Training" CssClass="btn btn-save" ValidationGroup="Assign" OnClick="btnAssign_Click" />

                        </div>

                        <div class="col-12 mt-3">

                            <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>

                        </div>

                    </div>

                </div>

                <!-- BULK ASSIGN -->

                <div class="tab-pane fade" id="bulk">

                    <div class="row">

                        <div class="col-12 col-md-6 col-lg-4 mb-3">

                            <label class="form-label">Training *</label>

                            <asp:DropDownList ID="ddlBulkTraining" runat="server" CssClass="form-select"></asp:DropDownList>

                        </div>

                        <div class="col-12 col-md-6 col-lg-4 mb-3">

                            <label class="form-label">Excel File *</label>

                            <asp:FileUpload ID="fuExcel" runat="server" CssClass="form-control" />

                        </div>

                        <div class="col-12 col-md-6 col-lg-4 mb-3 d-flex align-items-end">

                            <asp:Button ID="btnBulkUpload" runat="server" Text="Upload & Assign" CssClass="btn btn-save w-100" OnClick="btnBulkUpload_Click" />

                        </div>

                        <div class="col-12 mt-2">

                            <div class="alert alert-info">
                                Excel should contain only one column:<br />
                                <b>EmpID</b>
                            </div>
                        </div>

                        <div class="col-12 mt-3">

                            <asp:Label ID="lblBulkMessage" runat="server" Font-Bold="true"></asp:Label>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <script>

        $(document).ready(function () {

            $('#ddlTraining').select2({ width: '100%' });

            $('#ddlBulkTraining').select2({ width: '100%' });

            $('#lstEmployee').select2({
                placeholder: 'Select Employee(s)',
                width: '100%'
            });

        });

    </script>

</asp:Content>
