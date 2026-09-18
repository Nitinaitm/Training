<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Training.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .dashboard-page { padding: 5px 0 30px; }
        .dashboard-title { font-size: 28px; font-weight: 800; color: #173b67; margin: 10px 0 22px; }
        .dashboard-subtitle { color: #6c757d; font-size: 14px; margin-top: -14px; margin-bottom: 22px; }
        .stat-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 14px; padding: 20px; min-height: 145px; box-shadow: 0 3px 12px rgba(0,0,0,.06); margin-bottom: 20px; transition: transform .2s ease, box-shadow .2s ease; }
        .stat-card:hover { transform: translateY(-2px); box-shadow: 0 7px 18px rgba(0,0,0,.09); }
        .stat-icon { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; background: #edf5ff; color: #173b67; font-size: 21px; margin-bottom: 13px; }
        .stat-value { font-size: 30px; line-height: 34px; font-weight: 800; color: #173b67; }
        .stat-label { color: #667085; font-size: 14px; font-weight: 700; margin-top: 4px; }
        .quick-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 14px; padding: 20px; box-shadow: 0 3px 12px rgba(0,0,0,.05); height: 100%; }
        .section-title { font-size: 18px; font-weight: 800; color: #173b67; margin-bottom: 15px; }
        .quick-link { display: flex; align-items: center; justify-content: space-between; padding: 11px 13px; margin-bottom: 9px; border-radius: 8px; background: #f8fafc; color: #34495e; text-decoration: none; font-weight: 700; }
        .quick-link:hover { background: #edf5ff; color: #173b67; text-decoration: none; }
        .quick-link i { margin-right: 8px; color: #198754; }
        @media(max-width:767px) { .dashboard-title { font-size: 23px; } .stat-card { min-height: 125px; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid dashboard-page">
        <div class="dashboard-title"><i class="fas fa-tachometer-alt"></i>&nbsp; Dashboard</div>
        <div class="dashboard-subtitle">Training Management overview and pending actions</div>

        <div class="row">
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-chalkboard-teacher"></i></div><div class="stat-value"><asp:Label ID="lblActiveTrainings" runat="server" Text="0" /></div><div class="stat-label">Active Trainings</div></div>
            </div>
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-calendar-alt"></i></div><div class="stat-value"><asp:Label ID="lblStarting7Days" runat="server" Text="0" /></div><div class="stat-label">Starting in 7 Days</div></div>
            </div>
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-hourglass-half"></i></div><div class="stat-value"><asp:Label ID="lblConfirmationPending" runat="server" Text="0" /></div><div class="stat-label">Confirmation Pending</div></div>
            </div>
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-question-circle"></i></div><div class="stat-value"><asp:Label ID="lblQuestionsPending" runat="server" Text="0" /></div><div class="stat-label">Questions Pending Approval</div></div>
            </div>
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-certificate"></i></div><div class="stat-value"><asp:Label ID="lblCertificatesPending" runat="server" Text="0" /></div><div class="stat-label">Certificates Pending</div></div>
            </div>
            <div class="col-xl-2 col-lg-4 col-md-4 col-sm-6">
                <div class="stat-card"><div class="stat-icon"><i class="fas fa-calendar-check"></i></div><div class="stat-value"><asp:Label ID="lblTrainingsThisMonth" runat="server" Text="0" /></div><div class="stat-label">Trainings This Month</div></div>
            </div>
        </div>

        <div class="row mt-2">
            <div class="col-lg-7 mb-3">
                <div class="quick-card">
                    <div class="section-title"><i class="fas fa-bolt"></i>&nbsp; Quick Actions</div>
                    <a class="quick-link" href="CreateBatch.aspx"><span><i class="fas fa-plus-circle"></i>Create Batch</span><i class="fas fa-chevron-right"></i></a>
                    <a class="quick-link" href="TrainingList.aspx"><span><i class="fas fa-list"></i>Training List</span><i class="fas fa-chevron-right"></i></a>
                    <a class="quick-link" href="QuestionApproval.aspx"><span><i class="fas fa-check-circle"></i>Question Approval</span><i class="fas fa-chevron-right"></i></a>
                    <a class="quick-link" href="GeneratedCertificateList.aspx"><span><i class="fas fa-certificate"></i>Generated Certificates</span><i class="fas fa-chevron-right"></i></a>
                </div>
            </div>
            <div class="col-lg-5 mb-3">
                <div class="quick-card">
                    <div class="section-title"><i class="fas fa-chart-line"></i>&nbsp; Reports</div>
                    <a class="quick-link" href="TrainingDetailsReport.aspx"><span><i class="fas fa-file-alt"></i>Training Details Report</span><i class="fas fa-chevron-right"></i></a>
                    <a class="quick-link" href="TrainingCompletionSummaryReport.aspx"><span><i class="fas fa-chart-bar"></i>Completion Summary</span><i class="fas fa-chevron-right"></i></a>
                    <a class="quick-link" href="TrainingSearch.aspx"><span><i class="fas fa-search"></i>Employee Training Search</span><i class="fas fa-chevron-right"></i></a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>