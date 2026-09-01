<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="OrganizerManagement.aspx.cs" Inherits="Training.Admin.OrganizerManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


    <style>
        body {
            background: #f5f5f5;
        }

        .main-card {
            background: #fff;
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
            background: darkcyan;
            color: white;
            border: none;
        }

            .btn-save:hover {
                background: teal;
                color: white;
            }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--multiple {
            min-height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .form-select {
            height: 38px !important;
        }

        .select2-container
        .select2-selection--single {
            height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .select2-selection__rendered {
            line-height: 36px !important;
        }

        .select2-selection__arrow {
            height: 36px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">

    <div class="card shadow">

        <div class="card-header bg-primary text-white">
            <h5 class="mb-0">Training Organizer Management</h5>
        </div>

        <div class="card-body">

            <div class="row mb-3">

                <div class="col-md-4">

                    <asp:TextBox ID="txtSearch"
                        runat="server"
                        CssClass="form-control"
                        placeholder="Search Training Organizer">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <asp:Button ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />

                </div>

            </div>

            <asp:GridView ID="gvTrainingOrganizer"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                DataKeyNames="ID"
                OnRowEditing="gvTrainingOrganizer_RowEditing"
                OnRowUpdating="gvTrainingOrganizer_RowUpdating"
                OnRowCancelingEdit="gvTrainingOrganizer_RowCancelingEdit"
                OnRowDeleting="gvTrainingOrganizer_RowDeleting">

                <Columns>

                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="TrainingOrganizerID"
                        HeaderText="Training Organizer ID"
                        ReadOnly="true" />

                    <asp:BoundField DataField="TrainingOrganizer"
                        HeaderText="Training Organizer" />

                    <asp:BoundField DataField="CreatedOn"
                        HeaderText="Created On"
                        ReadOnly="true"
                        DataFormatString="{0:dd-MM-yyyy}" />

                    <asp:CommandField
                        ShowEditButton="true"
                        ButtonType="Button"
                        ControlStyle-CssClass="btn btn-warning btn-sm" />

                    <asp:CommandField
                        ShowDeleteButton="true"
                        ButtonType="Button"
                        ControlStyle-CssClass="btn btn-danger btn-sm" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>

</asp:Content>