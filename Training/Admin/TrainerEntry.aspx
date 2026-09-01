<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainerEntry.aspx.cs"
    Inherits="Training.Admin.TrainerEntry" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px;
        }

        .section-header {
            background: #198754;
            color: white;
            padding: 10px 15px;
            border-radius: 5px;
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 15px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
        }

        .gridview td {
            vertical-align: middle;
        }

        .btn-save {
            background: #198754;
            color: white;
            border: none;
        }

            .btn-save:hover {
                background: #157347;
                color: white;
            }

        .panel-box {
            border: 1px solid #dee2e6;
            padding: 20px;
            border-radius: 8px;
            background: #fafafa;
            margin-top: 15px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Trainer Master Entry
            </div>

            <div class="row">

                <div class="col-md-4">

                    <label class="form-label">
                        Trainer Type *
                    </label>

                    <asp:DropDownList
                        ID="ddlTrainerType"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlTrainerType_SelectedIndexChanged">

                        <asp:ListItem Text="Select" Value=""></asp:ListItem>
                        <asp:ListItem Text="Internal" Value="Internal"></asp:ListItem>
                        <asp:ListItem Text="External" Value="External"></asp:ListItem>

                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvTrainerType"
                        runat="server"
                        ControlToValidate="ddlTrainerType"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Trainer Type">
                    </asp:RequiredFieldValidator>

                </div>

            </div>

            <!-- INTERNAL TRAINER PANEL -->

            <asp:Panel
                ID="pnlInternal"
                runat="server"
                Visible="false"
                CssClass="panel-box">

                <div class="section-header">
                    Internal Trainer Details
                </div>

                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Employee ID *
                        </label>

                        <asp:TextBox
                            ID="txtEmpID"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnTextChanged="txtEmpID_TextChanged">
                        </asp:TextBox>

                    </div>
                    <div class="row mt-3">

                        <div class="col-md-12">

                            <div class="card border-success">

                                <div class="card-header bg-success text-white">
                                    Employee Details

                                </div>

                                <div class="card-body">

                                    <div class="row">

                                        <div class="col-md-4">

                                            <b>Name :</b>

                                            <asp:Label
                                                ID="lblEmpName"
                                                runat="server" />

                                        </div>

                                        <div class="col-md-4">

                                            <b>Designation :</b>

                                            <asp:Label
                                                ID="lblEmpDesignation"
                                                runat="server" />

                                        </div>

                                        <div class="col-md-4">

                                            <b>Company :</b>

                                            <asp:Label
                                                ID="lblEmpCompany"
                                                runat="server" />

                                        </div>

                                    </div>

                                    <br />

                                    <div class="row">

                                        <div class="col-md-4">

                                            <b>Mobile :</b>

                                            <asp:Label
                                                ID="lblEmpMobile"
                                                runat="server" />

                                        </div>

                                        <div class="col-md-4">

                                            <b>Email :</b>

                                            <asp:Label
                                                ID="lblEmpEmail"
                                                runat="server" />

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>
                    <div class="col-md-8 mb-3">

                        <label class="form-label">
                            Remarks
                        </label>

                        <asp:TextBox
                            ID="txtRemarksInternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Area of Expertise *
                        </label>

                        <asp:DropDownList
                            ID="ddlExpertiseInternal"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Highest Qualification
                        </label>

                        <asp:DropDownList
                            ID="ddlQualificationInternal"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Experience (Years)
                        </label>

                        <asp:TextBox
                            ID="txtExperienceInternal"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Number">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Certifications
                        </label>

                        <asp:TextBox
                            ID="txtCertificationInternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="form-label">
                            Available From
                        </label>

                        <asp:TextBox
                            ID="txtAvailableFromInternal"
                            runat="server"
                            CssClass="form-control flatpickr" placeholder="dd-mm-yyyy"
                            autocomplete="off"
                            onkeydown="return false;">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="form-label">
                            Available To
                        </label>

                        <asp:TextBox
                            ID="txtAvailableToInternal"
                            runat="server"
                            CssClass="form-control flatpickr" placeholder="dd-mm-yyyy"
                            autocomplete="off"
                            onkeydown="return false;">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Availability
                        </label>

                        <asp:DropDownList
                            ID="ddlAvailabilityInternal"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem Text="Available" Value="Available"></asp:ListItem>

                            <asp:ListItem Text="Busy" Value="Busy"></asp:ListItem>

                            <asp:ListItem Text="On Leave" Value="On Leave"></asp:ListItem>

                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>

                        </asp:DropDownList>

                    </div>



                </div>
            </asp:Panel>


            <!-- EXTERNAL TRAINER PANEL -->

            <asp:Panel
                ID="pnlExternal"
                runat="server"
                Visible="false"
                CssClass="panel-box">

                <div class="section-header">
                    External Trainer Details
                </div>

                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Employee ID
                        </label>

                        <asp:TextBox
                            ID="txtEmpIDExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Name *
                        </label>

                        <asp:TextBox
                            ID="txtNameExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Designation
                        </label>

                        <asp:TextBox
                            ID="txtDesignationExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Area of Expertise *
                        </label>

                        <asp:DropDownList
                            ID="ddlExpertiseExternal"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Highest Qualification
                        </label>

                        <asp:DropDownList
                            ID="ddlQualificationExternal"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Experience (Years)
                        </label>

                        <asp:TextBox
                            ID="txtExperienceExternal"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Number">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Mobile No
                        </label>

                        <asp:TextBox ID="txtMobileExternal" runat="server" CssClass="form-control"></asp:TextBox>

                    </div>

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Email ID
                        </label>

                        <asp:TextBox ID="txtEmailExternal" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Certifications
                        </label>

                        <asp:TextBox
                            ID="txtCertificationExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="form-label">
                            Available From
                        </label>

                        <asp:TextBox
                            ID="txtAvailableFromExternal"
                            runat="server"
                            CssClass="form-control flatpickr">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="form-label">
                            Available To
                        </label>

                        <asp:TextBox
                            ID="txtAvailableToExternal"
                            runat="server"
                            CssClass="form-control flatpickr">
                        </asp:TextBox>

                    </div>

                </div>
                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Availability
                        </label>

                        <asp:DropDownList
                            ID="ddlAvailabilityExternal"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem Text="Available" Value="Available"></asp:ListItem>

                            <asp:ListItem Text="Busy" Value="Busy"></asp:ListItem>

                            <asp:ListItem Text="On Leave" Value="On Leave"></asp:ListItem>

                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>

                        </asp:DropDownList>

                    </div>



                </div>
                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Organization
                        </label>

                        <asp:TextBox
                            ID="txtOrganizationExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Remarks
                        </label>

                        <asp:TextBox
                            ID="txtRemarksExternal"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>



            </asp:Panel>

            <div class="modal fade"
                id="trainerModal"
                tabindex="-1">

                <div class="modal-dialog modal-lg">

                    <div class="modal-content">

                        <div class="modal-header bg-success text-white">

                            <h5 class="modal-title">Trainer Profile

                            </h5>

                            <button
                                type="button"
                                class="btn-close"
                                data-bs-dismiss="modal">
                            </button>

                        </div>

                        <div class="modal-body">

                            <table class="table table-bordered">

                                <tr>

                                    <th width="30%">Trainer ID</th>

                                    <td>
                                        <asp:Label ID="lblTrainerID" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Name</th>

                                    <td>
                                        <asp:Label ID="lblTrainerName" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Type</th>

                                    <td>
                                        <asp:Label ID="lblTrainerType" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Organization</th>

                                    <td>
                                        <asp:Label ID="lblOrganization" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Designation</th>

                                    <td>
                                        <asp:Label ID="lblDesignation" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Qualification</th>

                                    <td>
                                        <asp:Label ID="lblQualification" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Expertise</th>

                                    <td>
                                        <asp:Label ID="lblExpertise" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Experience</th>

                                    <td>
                                        <asp:Label ID="lblExperience" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Certification</th>

                                    <td>
                                        <asp:Label ID="lblCertification" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Availability</th>

                                    <td>
                                        <asp:Label ID="lblAvailability" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Available Time</th>

                                    <td>
                                        <asp:Label ID="lblTime" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Mobile</th>

                                    <td>
                                        <asp:Label ID="lblMobile" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Email</th>

                                    <td>
                                        <asp:Label ID="lblEmail" runat="server" />
                                    </td>

                                </tr>

                                <tr>

                                    <th>Remarks</th>

                                    <td>
                                        <asp:Label ID="lblRemarks" runat="server" />
                                    </td>

                                </tr>

                            </table>

                        </div>

                    </div>

                </div>

            </div>
            <div class="mt-3">

                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save Trainer"
                    CssClass="btn btn-save"
                    ValidationGroup="SaveGroup"
                    OnClick="btnSave_Click" />

            </div>

            <div class="mt-3">

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    Font-Bold="true">
                </asp:Label>

            </div>

            <hr />
            <div class="search-panel">

                <div class="row">

                    <div class="col-md-2">

                        <label>Trainer Type</label>

                        <asp:DropDownList
                            ID="ddlSearchTrainerType"
                            runat="server"
                            CssClass="form-select"   AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSearchTrainerType_SelectedIndexChanged">

                            <asp:ListItem Text="All" Value=""></asp:ListItem>
                            <asp:ListItem Text="Internal" Value="Internal"></asp:ListItem>
                            <asp:ListItem Text="External" Value="External"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-2">

                        <label>Employee ID</label>

                        <asp:TextBox
                            ID="txtSearchEmpID"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnTextChanged="SearchChanged">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>Trainer Name</label>

                        <asp:TextBox
                            ID="txtSearchTrainerName"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnTextChanged="SearchChanged">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>Organization</label>

                        <asp:TextBox
                            ID="txtSearchOrganization"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnTextChanged="SearchChanged">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2">

                        <br />



                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-secondary"
                            OnClick="btnReset_Click" />

                    </div>

                </div>

            </div>

            <div class="mb-2">

                <asp:Label
                    ID="lblCount"
                    runat="server"
                    Font-Bold="true">
                </asp:Label>

            </div>

            <hr />

            <div class="section-header">
                Trainers
            </div>

            <asp:GridView
                ID="gvTrainer" DataKeyNames="TrainerID"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped gridview" OnRowCommand="gvTrainer_RowCommand">

                <Columns>

                    <asp:BoundField DataField="DisplayTrainerID" HeaderText="Trainer ID" />

                    <asp:BoundField DataField="TrainerName" HeaderText="Trainer Name" />

                    <asp:BoundField DataField="TrainerType" HeaderText="Type" />

                    <asp:BoundField
                        DataField="Designation"
                        HeaderText="Designation" />

                    <asp:BoundField
                        DataField="Organization"
                        HeaderText="Organization" />


                    <asp:BoundField DataField="ExpertiseName" HeaderText="Area Of Expertise" />

                    <asp:BoundField DataField="ExperienceYears" HeaderText="Experience" />

                    <asp:BoundField DataField="TrainerAvailability" HeaderText="Availability" />

                    <asp:BoundField DataField="ActiveStatus" HeaderText="Status" />

                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkView"
                                runat="server"
                                Text="View"
                                CssClass="btn btn-info btn-sm"
                                CommandName="ViewRecord"
                                CommandArgument='<%# Eval("TrainerID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkEdit"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-warning btn-sm"
                                CommandName="EditRecord"
                                CommandArgument='<%# Eval("TrainerID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkDelete"
                                runat="server"
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm"
                                CommandName="DeleteRecord"
                                CommandArgument='<%# Eval("TrainerID") %>'
                                OnClientClick="return confirm('Are you sure you want to delete this trainer?');" />

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

            <br />



        </div>

    </div>

    <script>

        function initControls() {
            document.querySelectorAll(".flatpickr").forEach(function (el) {
                if (el._flatpickr) {
                    el._flatpickr.destroy();
                }
            });

            flatpickr(".flatpickr",
                {
                    dateFormat: "d-m-Y",
                    allowInput: false,
                    clickOpens: true
                });
        }

        $(document).ready(function () {
            initControls();
        });

        if (typeof (Sys) !== "undefined") {
            Sys.Application.add_load(function () {
                initControls();
            });
        }

    </script>

</asp:Content>
