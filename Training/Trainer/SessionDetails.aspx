<%@ Page Title="Session Details"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="SessionDetails.aspx.cs"
    Inherits="Training.Trainer.SessionDetails" %>

<%@ Register Src="~/Trainer/TrainerSummary.ascx"
    TagPrefix="uc"
    TagName="TrainerSummary" %>

<%@ Register Src="~/Trainer/SessionSummary.ascx"
    TagPrefix="uc"
    TagName="SessionSummary" %>

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
                Session Details

            </div>

            <uc:TrainerSummary
                ID="TrainerSummary1"
                runat="server" />

            <uc:SessionSummary
                ID="SessionSummary1"
                runat="server" />

            <div class="action-card">

                <div class="action-title">
                    Trainer Actions

                </div>
                 <asp:Button
                    ID="btnDashboard"
                    runat="server"
                    Text="Back"
                    CssClass="btn btn-primary btn-action"
                    OnClick="btnDashboard_Click" />
                <asp:Button
                    ID="btnAttendance"
                    runat="server"
                    Text="Attendance"
                    CssClass="btn btn-primary btn-action"
                    OnClick="btnAttendance_Click" />

                <asp:Button
                    ID="btnMaterial"
                    runat="server"
                    Text="Training Material"
                    CssClass="btn btn-success btn-action"
                    OnClick="btnMaterial_Click" />

                <asp:Button
                    ID="btnQuestionBank"
                    runat="server"
                    Text="Question Bank"
                    CssClass="btn btn-info btn-action"
                    OnClick="btnQuestionBank_Click" />

                <asp:Button
                    ID="btnPreTest"
                    runat="server"
                    Text="Pre Training Test"
                    CssClass="btn btn-warning btn-action"
                    OnClick="btnPreTest_Click" />

                <asp:Button
                    ID="btnPostTest"
                    runat="server"
                    Text="Post Training Test"
                    CssClass="btn btn-dark btn-action"
                    OnClick="btnPostTest_Click" />

            </div>

            <div class="workflow-box" runat="server" visible="false">

                <div class="workflow-title">
                    Workflow Status

                </div>

                <div class="row">

                    <div class="col-md-3">
                        Training Workflow

                        <br />

                        <asp:Label
                            ID="lblWorkflow"
                            runat="server"
                            CssClass="badge bg-primary workflow-status" />

                    </div>

                    <div class="col-md-3">
                        Training Status

                        <br />

                        <asp:Label
                            ID="lblTrainingStatus"
                            runat="server"
                            CssClass="badge bg-success workflow-status" />

                    </div>

                    <div class="col-md-3">
                        Attendance

                        <br />

                        <asp:Label
                            ID="lblAttendanceStatus"
                            runat="server"
                            CssClass="badge bg-secondary workflow-status" />

                    </div>

                    <div class="col-md-3">
                        Session Status

                        <br />

                        <asp:Label
                            ID="lblSessionStatus"
                            runat="server"
                            CssClass="badge bg-info workflow-status" />

                    </div>

                </div>

            </div>
        </div>
    </div>
</asp:Content>
