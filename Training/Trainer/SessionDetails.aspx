<%@ Page Title="Session Details"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="SessionDetails.aspx.cs"
    Inherits="Training.Trainer.SessionDetails" %>

<%@ Register Src="~/Trainer/TrainerSummary.ascx" TagPrefix="uc" TagName="TrainerSummary" %>
<%@ Register Src="~/Trainer/SessionSummary.ascx" TagPrefix="uc" TagName="SessionSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px}.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.summary-card{background:#f8f9fa;border:1px solid #dee2e6;border-radius:10px;padding:20px}.summary-label{font-weight:bold;color:#0d6efd}.info-box{margin-bottom:12px}.action-card{margin-top:20px;background:#fff;border:1px solid #dee2e6;border-radius:10px;padding:20px}.btn-action{min-width:180px;margin-right:10px;margin-bottom:10px}.status-badge{font-size:16px;padding:8px 15px}.skip-card{margin-top:20px;background:#fff;border:1px solid #dee2e6;border-radius:10px;padding:20px}.skip-row{border-bottom:1px solid #eee;padding:12px 0}.skip-row:last-child{border-bottom:0}.reason{max-width:520px}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">Session Details</div>
            <uc:TrainerSummary ID="TrainerSummary1" runat="server" />
            <uc:SessionSummary ID="SessionSummary1" runat="server" />

            <div class="action-card">
                <div class="action-title">Trainer Actions</div>
                <asp:Button ID="btnDashboard" runat="server" Text="Back" CssClass="btn btn-primary btn-action" OnClick="btnDashboard_Click" />
                <asp:Button ID="btnAttendance" runat="server" Text="Attendance" CssClass="btn btn-primary btn-action" OnClick="btnAttendance_Click" />
                <asp:Button ID="btnMaterial" runat="server" Text="Training Material" CssClass="btn btn-success btn-action" OnClick="btnMaterial_Click" />
                <asp:Button ID="btnQuestionBank" runat="server" Text="Question Bank" CssClass="btn btn-info btn-action" OnClick="btnQuestionBank_Click" />
                <asp:Button ID="btnPreTest" runat="server" Text="Pre Training Test" CssClass="btn btn-warning btn-action" OnClick="btnPreTest_Click" />
                <asp:Button ID="btnPostTest" runat="server" Text="Post Training Test" CssClass="btn btn-dark btn-action" OnClick="btnPostTest_Click" />
                <asp:Button ID="btnTestResult" runat="server" Text="Test Result" CssClass="btn btn-success btn-action" OnClick="btnTestResult_Click" />
            </div>

            <asp:Panel ID="pnlSkip" runat="server" CssClass="skip-card">
                <h5>Session Requirement Skip</h5>
                <p class="text-muted mb-2">Trainer can skip Attendance, Pre-Test or Post-Test for this session. Skip reason is mandatory. Trainer cannot unskip.</p>
                <div class="skip-row">
                    <div class="fw-bold">Attendance</div>
                    <asp:Label ID="lblAttendanceSkip" runat="server" CssClass="badge bg-secondary" />
                    <asp:TextBox ID="txtAttendanceSkipReason" runat="server" CssClass="form-control reason mt-2" TextMode="MultiLine" Rows="2" placeholder="Reason for skipping Attendance" />
                    <asp:Button ID="btnSkipAttendance" runat="server" Text="Skip Attendance" CssClass="btn btn-outline-danger mt-2" OnClick="btnSkipAttendance_Click" />
                </div>
                <div class="skip-row">
                    <div class="fw-bold">Pre-Test</div>
                    <asp:Label ID="lblPreSkip" runat="server" CssClass="badge bg-secondary" />
                    <asp:TextBox ID="txtPreSkipReason" runat="server" CssClass="form-control reason mt-2" TextMode="MultiLine" Rows="2" placeholder="Reason for skipping Pre-Test" />
                    <asp:Button ID="btnSkipPre" runat="server" Text="Skip Pre-Test" CssClass="btn btn-outline-danger mt-2" OnClick="btnSkipPre_Click" />
                </div>
                <div class="skip-row">
                    <div class="fw-bold">Post-Test</div>
                    <asp:Label ID="lblPostSkip" runat="server" CssClass="badge bg-secondary" />
                    <asp:TextBox ID="txtPostSkipReason" runat="server" CssClass="form-control reason mt-2" TextMode="MultiLine" Rows="2" placeholder="Reason for skipping Post-Test" />
                    <asp:Button ID="btnSkipPost" runat="server" Text="Skip Post-Test" CssClass="btn btn-outline-danger mt-2" OnClick="btnSkipPost_Click" />
                </div>
                <asp:Label ID="lblSkipMessage" runat="server" Font-Bold="true" />
            </asp:Panel>

            <div class="workflow-box" runat="server" visible="false">
                <div class="workflow-title">Workflow Status</div>
                <div class="row">
                    <div class="col-md-3">Training Workflow<br /><asp:Label ID="lblWorkflow" runat="server" CssClass="badge bg-primary workflow-status" /></div>
                    <div class="col-md-3">Training Status<br /><asp:Label ID="lblTrainingStatus" runat="server" CssClass="badge bg-success workflow-status" /></div>
                    <div class="col-md-3">Attendance<br /><asp:Label ID="lblAttendanceStatus" runat="server" CssClass="badge bg-secondary workflow-status" /></div>
                    <div class="col-md-3">Session Status<br /><asp:Label ID="lblSessionStatus" runat="server" CssClass="badge bg-info workflow-status" /></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>