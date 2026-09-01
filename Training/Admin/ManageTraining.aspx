<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="ManageTraining.aspx.cs"
    Inherits="Training.Admin.ManageTraining" %>

<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px;
        }

        .summary-card {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
        }

        .summary-label {
            font-weight: bold;
            color: #0d6efd;
        }

        .info-box {
            margin-bottom: 12px;
        }

        .action-card {
            margin-top: 20px;
            background: #fff;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
        }

        .btn-action {
            min-width: 180px;
            margin-right: 10px;
            margin-bottom: 10px;
        }

        .status-badge {
            font-size: 16px;
            padding: 8px 15px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Training Management
            </div>

            <uc:trainingsummary id="TrainingSummary1" runat="server" />
            <div class="col-md-3 info-box" runat="server" visible="false">
                <span class="summary-label">Status :
                </span>
                <br />

                <asp:Label
                    ID="lblStatus"
                    runat="server"
                    CssClass="badge bg-primary status-badge" />
            </div>
            <%-- <asp:Label
                    ID="Label1"
                    runat="server"
                    Font-Bold="true" />--%>
            <%--<div class="action-card text-center" runat="server" id="action">--%>
            <div class="action-card text-center">

                <h5>Actions
                </h5>

                <hr />

                <asp:Button
                    ID="btnUpdateTraining"
                    runat="server"
                    Text="Update Batch"
                    CssClass="btn btn-warning btn-action" BackColor="Gray" BorderColor="Gray"
                    OnClick="btnUpdateTraining_Click" />
                <asp:Button
                    ID="btnAssignSession"
                    runat="server"
                    Text="Assign Sessions & Trainers"
                    CssClass="btn btn-warning btn-action"
                    OnClick="btnAssignSession_Click" />


                <asp:Button
                    ID="btnAssignTrainee"
                    runat="server"
                    Text="Assign Trainee"
                    CssClass="btn btn-primary btn-action"
                    OnClick="btnAssignTrainee_Click" />

                <asp:Button
                    ID="btnCertificateTemplate"
                    runat="server"
                    Text="Certificate Template"
                    CssClass="btn btn-primary btn-action"
                    Enabled="false"
                    OnClick="btnCertificateTemplate_Click" />


            <%--    <asp:Button
                    ID="btnCertificate"
                    runat="server"
                    Text="Configure Certificate Template"
                    CssClass="btn btn-dark btn-action"
                    Visible="false" OnClick="btnCertificate_Click" />--%>

                <asp:Button
                    ID="btnAssignHostel"
                    runat="server"
                    Text="Assign Hostel"
                    CssClass="btn btn-info btn-action"
                    Visible="false" OnClick="btnAssignHostel_Click" />

                <asp:Button
                    ID="btnStartTraining"
                    runat="server"
                    Text="Start Training"
                    CssClass="btn btn-success btn-action"
                    Visible="false"
                    OnClick="btnStartTraining_Click" />
                <asp:Button
                    ID="btnAttendance"
                    runat="server"
                    Text="Attendance"
                    CssClass="btn btn-info btn-action"
                    Visible="false" OnClick="btnAttendance_Click" />


            </div>

            <asp:Panel
                ID="pnlHostelConfirmation"
                runat="server"
                Visible="false"
                CssClass="card mt-3">

                <div class="card-header bg-warning text-dark">

                    <b>Hostel Requirement
                    </b>

                </div>

                <div class="card-body">

                    <p>
                        Is hostel accommodation required for trainees?
                    </p>

                    <asp:Button
                        ID="btnHostelYes"
                        runat="server"
                        Text="Yes"
                        CssClass="btn btn-primary"
                        CausesValidation="false"
                        OnClick="btnHostelYes_Click" />

                    &nbsp;

        <asp:Button
            ID="btnHostelNo"
            runat="server"
            Text="No"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnHostelNo_Click" />

                </div>

            </asp:Panel>

            <div class="mt-3">

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    Font-Bold="true" />

            </div>

        </div>

    </div>

</asp:Content>
