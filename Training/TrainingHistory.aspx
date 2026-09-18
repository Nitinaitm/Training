<%@ Page Title="Training History" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainingHistory.aspx.cs" Inherits="Training.TrainingHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .history-page { padding: 25px 15px 40px; }
        .history-card { background:#fff; border:1px solid #e6ebf0; border-radius:12px; box-shadow:0 3px 12px rgba(0,0,0,.06); margin-bottom:20px; overflow:hidden; }
        .history-header { padding:16px 20px; border-bottom:1px solid #e9ecef; }
        .history-title { font-size:19px; font-weight:700; color:#173b67; margin:0; }
        .history-body { padding:18px 20px; }
        .info-label { color:#6c757d; font-size:12px; font-weight:600; }
        .info-value { color:#212529; font-size:14px; font-weight:700; }
        .table th { white-space:nowrap; }
        .question-table td { vertical-align:top; }
        @media(max-width:767px) { .history-page{padding:15px 8px 30px;} .history-body{padding:14px;} }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid history-page">
        <div class="history-card">
            <div class="history-header"><div class="history-title"><i class="fa fa-history"></i>&nbsp; Closed Training History</div></div>
            <div class="history-body">
                <div class="row">
                    <div class="col-md-2 col-sm-6 mb-3"><div class="info-label">Training ID</div><div class="info-value"><asp:Label ID="lblTrainingID" runat="server" /></div></div>
                    <div class="col-md-3 col-sm-6 mb-3"><div class="info-label">Training Type</div><div class="info-value"><asp:Label ID="lblTrainingType" runat="server" /></div></div>
                    <div class="col-md-3 col-sm-6 mb-3"><div class="info-label">Organizer</div><div class="info-value"><asp:Label ID="lblOrganizer" runat="server" /></div></div>
                    <div class="col-md-2 col-sm-6 mb-3"><div class="info-label">Batch</div><div class="info-value"><asp:Label ID="lblBatch" runat="server" /></div></div>
                    <div class="col-md-2 col-sm-6 mb-3"><div class="info-label">Status</div><div class="info-value"><asp:Label ID="lblStatus" runat="server" CssClass="badge bg-secondary" /></div></div>
                </div>
                <div class="row">
                    <div class="col-md-3 mb-2"><div class="info-label">From</div><div class="info-value"><asp:Label ID="lblDateFrom" runat="server" /></div></div>
                    <div class="col-md-3 mb-2"><div class="info-label">To</div><div class="info-value"><asp:Label ID="lblDateTo" runat="server" /></div></div>
                    <div class="col-md-6 mb-2 text-md-end"><asp:Button ID="btnCertificate" runat="server" Text="Download Certificate" CssClass="btn btn-success" CausesValidation="false" OnClick="btnCertificate_Click" /><asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" CssClass="btn btn-outline-secondary ms-2" CausesValidation="false" OnClick="btnBack_Click" /></div>
                </div>
            </div>
        </div>

        <div class="history-card">
            <div class="history-header"><div class="history-title"><i class="fa fa-calendar-check-o"></i>&nbsp; Attendance</div></div>
            <div class="history-body"><asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" EmptyDataText="No attendance history available.">
                <Columns>
                    <asp:BoundField DataField="SessionNo" HeaderText="Session" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session Name" />
                    <asp:BoundField DataField="SessionDate" HeaderText="Date" />
                    <asp:BoundField DataField="AttendanceStatus" HeaderText="Attendance" />
                    <asp:BoundField DataField="TrainerName" HeaderText="Trainer" />
                </Columns>
            </asp:GridView></div>
        </div>

        <div class="history-card">
            <div class="history-header"><div class="history-title"><i class="fa fa-pencil-square-o"></i>&nbsp; Pre-Test / Post-Test Results</div></div>
            <div class="history-body"><asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" EmptyDataText="No test history available.">
                <Columns>
                    <asp:BoundField DataField="TestType" HeaderText="Test" />
                    <asp:BoundField DataField="TestTitle" HeaderText="Test Title" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session" />
                    <asp:BoundField DataField="TestDate" HeaderText="Date" />
                    <asp:BoundField DataField="AttemptNo" HeaderText="Attempt" />
                    <asp:BoundField DataField="TotalQuestions" HeaderText="Questions" />
                    <asp:BoundField DataField="CorrectAnswers" HeaderText="Correct" />
                    <asp:BoundField DataField="Score" HeaderText="Score" />
                    <asp:BoundField DataField="Status" HeaderText="Result" />
                    <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted On" />
                </Columns>
            </asp:GridView></div>
        </div>

        <div class="history-card">
            <div class="history-header"><div class="history-title"><i class="fa fa-list-ol"></i>&nbsp; Attempted Questions</div></div>
            <div class="history-body"><asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped question-table" EmptyDataText="No attempted questions available.">
                <Columns>
                    <asp:BoundField DataField="TestType" HeaderText="Test" />
                    <asp:BoundField DataField="TestTitle" HeaderText="Test Title" />
                    <asp:BoundField DataField="EmpID" HeaderText="Trainee ID" />
                    <asp:BoundField DataField="AttemptNo" HeaderText="Attempt" />
                    <asp:BoundField DataField="QuestionNo" HeaderText="#" />
                    <asp:BoundField DataField="Question" HeaderText="Question" />
                    <asp:BoundField DataField="SelectedAnswer" HeaderText="Selected Answer" />
                    <asp:BoundField DataField="CorrectAnswer" HeaderText="Correct Answer" />
                    <asp:BoundField DataField="IsCorrect" HeaderText="Result" />
                </Columns>
            </asp:GridView></div>
        </div>

        <div class="history-card">
            <div class="history-header"><div class="history-title"><i class="fa fa-comments"></i>&nbsp; Feedback</div></div>
            <div class="history-body"><asp:GridView ID="gvFeedback" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" EmptyDataText="No feedback history available.">
                <Columns>
                    <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted On" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                    <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                    <asp:BoundField DataField="AnswerType" HeaderText="Type" />
                    <asp:BoundField DataField="Rating" HeaderText="Rating" />
                    <asp:BoundField DataField="Answer" HeaderText="Answer" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session" />
                    <asp:BoundField DataField="TrainerName" HeaderText="Trainer" />
                    <asp:BoundField DataField="EmpID" HeaderText="Trainee ID" />
                </Columns>
            </asp:GridView></div>
        </div>
    </div>
</asp:Content>
