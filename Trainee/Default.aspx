<%@ Page Title="Trainee Dashboard"
    Language="C#"
    MasterPageFile="~/TraineeMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Training.Trainee.Default" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style type="text/css">

        .trainee-dashboard {
            padding-bottom: 25px;
        }

        /* =====================================================
           WELCOME
           ===================================================== */

        .welcome-card {
            border: 0;
            border-radius: 14px;
            background: linear-gradient(135deg, #0d6efd, #084298);
            color: #ffffff;
            box-shadow: 0 5px 18px rgba(13,110,253,.18);
            margin-bottom: 22px;
            overflow: hidden;
        }

        .welcome-body {
            padding: 22px 25px;
        }

        .welcome-title {
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 5px;
        }

        .welcome-subtitle {
            font-size: 14px;
            opacity: .90;
        }

        .trainee-name {
            font-weight: 700;
        }

        .trainee-id {
            display: inline-block;
            margin-top: 10px;
            padding: 5px 12px;
            background: rgba(255,255,255,.15);
            border-radius: 20px;
            font-size: 13px;
        }


        /* =====================================================
           SECTION
           ===================================================== */

        .dashboard-section {
            margin-bottom: 22px;
        }

        .section-heading {
            font-size: 18px;
            font-weight: 700;
            color: #343a40;
            margin-bottom: 14px;
        }

        .section-heading i {
            margin-right: 6px;
            color: #0d6efd;
        }


        /* =====================================================
           SUMMARY CARDS
           ===================================================== */

        .summary-card {
            position: relative;
            background: #ffffff;
            border: 1px solid #e8edf3;
            border-radius: 12px;
            min-height: 135px;
            padding: 18px;
            margin-bottom: 20px;
            box-shadow: 0 3px 12px rgba(0,0,0,.06);
            overflow: hidden;
            transition: all .20s ease-in-out;
        }

        .summary-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 7px 18px rgba(0,0,0,.10);
        }

        .summary-icon {
            width: 48px;
            height: 48px;
            border-radius: 12px;
            text-align: center;
            line-height: 48px;
            font-size: 21px;
            margin-bottom: 12px;
        }

        .icon-training {
            background: #e7f1ff;
            color: #0d6efd;
        }

        .icon-attendance {
            background: #e8f7ee;
            color: #198754;
        }

        .icon-test {
            background: #fff3cd;
            color: #997404;
        }

        .icon-feedback {
            background: #f3e8ff;
            color: #6f42c1;
        }

        .icon-certificate {
            background: #fff0e5;
            color: #fd7e14;
        }

        .summary-value {
            font-size: 27px;
            line-height: 30px;
            font-weight: 700;
            color: #212529;
        }

        .summary-label {
            margin-top: 4px;
            color: #6c757d;
            font-size: 14px;
            font-weight: 600;
        }

        .summary-small {
            display: block;
            margin-top: 5px;
            color: #8a929a;
            font-size: 12px;
        }


        /* =====================================================
           CLICKABLE CARDS
           ===================================================== */

        .dashboard-link {
            display: block;
            color: inherit;
            text-decoration: none !important;
        }

        .dashboard-link:hover,
        .dashboard-link:focus {
            color: inherit;
            text-decoration: none !important;
        }


        /* =====================================================
           PROGRESS
           ===================================================== */

        .progress-card {
            background: #ffffff;
            border: 1px solid #e8edf3;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 3px 12px rgba(0,0,0,.06);
            height: calc(100% - 20px);
        }

        .progress-card-title {
            font-size: 17px;
            font-weight: 700;
            color: #343a40;
            margin-bottom: 18px;
        }

        .progress-row {
            margin-bottom: 16px;
        }

        .progress-row:last-child {
            margin-bottom: 0;
        }

        .progress-label {
            font-size: 13px;
            font-weight: 600;
            color: #495057;
            margin-bottom: 6px;
        }

        .progress-value {
            float: right;
            color: #6c757d;
            font-weight: 600;
        }

        .progress {
            height: 8px;
            border-radius: 10px;
            background: #edf0f3;
        }

        .progress-bar {
            border-radius: 10px;
        }


        /* =====================================================
           CURRENT STATUS
           ===================================================== */

        .status-card {
            background: #ffffff;
            border: 1px solid #e8edf3;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 3px 12px rgba(0,0,0,.06);
            height: calc(100% - 20px);
        }

        .status-title {
            font-size: 17px;
            font-weight: 700;
            color: #343a40;
            margin-bottom: 15px;
        }

        .status-row {
            padding: 10px 0;
            border-bottom: 1px solid #eeeeee;
        }

        .status-row:last-child {
            border-bottom: 0;
        }

        .status-label {
            color: #6c757d;
            font-size: 13px;
        }

        .status-value {
            float: right;
            font-weight: 600;
            color: #343a40;
        }


        /* =====================================================
           QUICK ACTION
           ===================================================== */

        .quick-card {
            background: #ffffff;
            border: 1px solid #e8edf3;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 3px 12px rgba(0,0,0,.06);
        }

        .quick-title {
            font-size: 17px;
            font-weight: 700;
            color: #343a40;
            margin-bottom: 15px;
        }

        .quick-btn {
            margin-right: 7px;
            margin-bottom: 8px;
            border-radius: 6px;
        }


        /* =====================================================
           INFORMATION
           ===================================================== */

        .info-card {
            background: #ffffff;
            border: 1px solid #e8edf3;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 3px 12px rgba(0,0,0,.06);
        }

        .info-title {
            font-size: 17px;
            font-weight: 700;
            color: #343a40;
            margin-bottom: 13px;
        }

        .instruction-list {
            padding-left: 20px;
            margin-bottom: 0;
            color: #555f68;
        }

        .instruction-list li {
            padding: 4px 0;
            line-height: 1.5;
        }


        /* =====================================================
           MOBILE
           ===================================================== */

        @media (max-width: 767px) {

            .welcome-body {
                padding: 18px;
            }

            .welcome-title {
                font-size: 20px;
            }

            .summary-card {
                min-height: 120px;
            }

            .summary-value {
                font-size: 24px;
            }

            .status-value {
                float: none;
                display: block;
                margin-top: 3px;
            }

        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid trainee-dashboard">


        <!-- ===================================================
             WELCOME
             =================================================== -->

        <div class="welcome-card">

            <div class="welcome-body">

                <div class="welcome-title">

                    Welcome,
                    
                    <asp:Label
                        ID="lblTraineeName"
                        runat="server"
                        CssClass="trainee-name"
                        Text="Trainee">
                    </asp:Label>

                </div>

                <div class="welcome-subtitle">

                    View your assigned trainings, tests, feedback
                    and certificates from your dashboard.

                </div>

                <div class="trainee-id">

                    Trainee ID :

                    <asp:Label
                        ID="lblTraineeID"
                        runat="server">
                    </asp:Label>

                    &nbsp; | &nbsp;

                    <asp:Label
                        ID="lblTraineeType"
                        runat="server">
                    </asp:Label>

                </div>

            </div>

        </div>


        <!-- ===================================================
             TRAINING SUMMARY
             =================================================== -->

        <div class="dashboard-section">

            <div class="section-heading">

                <i class="fa fa-bar-chart"></i>
                Training Summary

            </div>


            <div class="row">


                <!-- MY TRAININGS -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <asp:LinkButton
                        ID="lnkMyTraining"
                        runat="server"
                        CssClass="dashboard-link"
                        CausesValidation="false"
                        OnClick="lnkMyTraining_Click">

                        <div class="summary-card">

                            <div class="summary-icon icon-training">

                                <i class="fa fa-book"></i>

                            </div>

                            <div class="summary-value">

                                <asp:Label
                                    ID="lblTrainingCount"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </div>

                            <div class="summary-label">

                                Assigned Trainings

                            </div>

                            <span class="summary-small">

                                View training details and sessions

                            </span>

                        </div>

                    </asp:LinkButton>

                </div>


                <!-- ATTENDANCE -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <asp:LinkButton
                        ID="lnkAttendance"
                        runat="server"
                        CssClass="dashboard-link"
                        CausesValidation="false"
                        OnClick="lnkAttendance_Click">

                        <div class="summary-card">

                            <div class="summary-icon icon-attendance">

                                <i class="fa fa-check-circle"></i>

                            </div>

                            <div class="summary-value">

                                <asp:Label
                                    ID="lblAttendance"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </div>

                            <div class="summary-label">

                                Attendance Completed

                            </div>

                            <span class="summary-small">

                                Trainings with completed attendance

                            </span>

                        </div>

                    </asp:LinkButton>

                </div>


                <!-- PUBLISHED TESTS -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <div class="summary-card">

                        <div class="summary-icon icon-test">

                            <i class="fa fa-pencil-square-o"></i>

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblCompletedTests"
                                runat="server"
                                Text="0">
                            </asp:Label>

                            <span style="font-size:18px;color:#adb5bd;">

                                /

                            </span>

                            <asp:Label
                                ID="lblPublishedTests"
                                runat="server"
                                Text="0">
                            </asp:Label>

                        </div>

                        <div class="summary-label">

                            Published Tests Completed

                        </div>

                        <span class="summary-small">

                            Pre/Post tests currently published

                        </span>

                    </div>

                </div>


                <!-- BATCH FEEDBACK -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <asp:LinkButton
                        ID="lnkBatchFeedback"
                        runat="server"
                        CssClass="dashboard-link"
                        CausesValidation="false"
                        OnClick="lnkBatchFeedback_Click">

                        <div class="summary-card">

                            <div class="summary-icon icon-feedback">

                                <i class="fa fa-comments"></i>

                            </div>

                            <div class="summary-value">

                                <asp:Label
                                    ID="lblBatchFeedback"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </div>

                            <div class="summary-label">

                                Training Feedback

                            </div>

                            <span class="summary-small">

                                Submitted batch feedback

                            </span>

                        </div>

                    </asp:LinkButton>

                </div>


                <!-- CERTIFICATE -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <asp:LinkButton
                        ID="lnkCertificate"
                        runat="server"
                        CssClass="dashboard-link"
                        CausesValidation="false"
                        OnClick="lnkCertificate_Click">

                        <div class="summary-card">

                            <div class="summary-icon icon-certificate">

                                <i class="fa fa-certificate"></i>

                            </div>

                            <div class="summary-value">

                                <asp:Label
                                    ID="lblCertificate"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </div>

                            <div class="summary-label">

                                Certificates

                            </div>

                            <span class="summary-small">

                                Generated certificates

                            </span>

                        </div>

                    </asp:LinkButton>

                </div>


                <!-- PENDING TESTS -->

                <div class="col-lg-3 col-md-6 col-sm-6">

                    <asp:LinkButton
                        ID="lnkPendingTests"
                        runat="server"
                        CssClass="dashboard-link"
                        CausesValidation="false"
                        OnClick="lnkPendingTests_Click">

                        <div class="summary-card">

                            <div class="summary-icon icon-test">

                                <i class="fa fa-clock-o"></i>

                            </div>

                            <div class="summary-value">

                                <asp:Label
                                    ID="lblPendingTests"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </div>

                            <div class="summary-label">

                                Pending Tests

                            </div>

                            <span class="summary-small">

                                Published tests awaiting completion

                            </span>

                        </div>

                    </asp:LinkButton>

                </div>

            </div>

        </div>


        <!-- ===================================================
             PROGRESS + CURRENT STATUS
             =================================================== -->

        <div class="row">


            <!-- OVERALL PROGRESS -->

            <div class="col-lg-7 col-md-12">

                <div class="progress-card">

                    <div class="progress-card-title">

                        <i class="fa fa-line-chart"></i>
                        My Progress

                    </div>


                    <div class="progress-row">

                        <div class="progress-label">

                            Attendance

                            <asp:Label
                                ID="lblProgressAttendance"
                                runat="server"
                                CssClass="progress-value"
                                Text="0/0">
                            </asp:Label>

                        </div>

                        <div class="progress">

                            <asp:Panel
                                ID="barAttendance"
                                runat="server"
                                CssClass="progress-bar bg-success"
                                Style="width:0%">
                            </asp:Panel>

                        </div>

                    </div>


                    <div class="progress-row">

                        <div class="progress-label">

                            Published Tests

                            <asp:Label
                                ID="lblProgressTests"
                                runat="server"
                                CssClass="progress-value"
                                Text="0/0">
                            </asp:Label>

                        </div>

                        <div class="progress">

                            <asp:Panel
                                ID="barTests"
                                runat="server"
                                CssClass="progress-bar bg-warning"
                                Style="width:0%">
                            </asp:Panel>

                        </div>

                    </div>


                    <div class="progress-row">

                        <div class="progress-label">

                            Training Feedback

                            <asp:Label
                                ID="lblProgressFeedback"
                                runat="server"
                                CssClass="progress-value"
                                Text="0/0">
                            </asp:Label>

                        </div>

                        <div class="progress">

                            <asp:Panel
                                ID="barFeedback"
                                runat="server"
                                CssClass="progress-bar bg-info"
                                Style="width:0%">
                            </asp:Panel>

                        </div>

                    </div>


                    <div class="progress-row">

                        <div class="progress-label">

                            Certificates

                            <asp:Label
                                ID="lblProgressCertificate"
                                runat="server"
                                CssClass="progress-value"
                                Text="0/0">
                            </asp:Label>

                        </div>

                        <div class="progress">

                            <asp:Panel
                                ID="barCertificate"
                                runat="server"
                                CssClass="progress-bar bg-primary"
                                Style="width:0%">
                            </asp:Panel>

                        </div>

                    </div>

                </div>

            </div>


            <!-- CURRENT STATUS -->

            <div class="col-lg-5 col-md-12">

                <div class="status-card">

                    <div class="status-title">

                        <i class="fa fa-info-circle"></i>
                        Current Status

                    </div>


                    <div class="status-row">

                        <span class="status-label">

                            Assigned Trainings

                        </span>

                        <asp:Label
                            ID="lblStatusTraining"
                            runat="server"
                            CssClass="status-value"
                            Text="0">
                        </asp:Label>

                    </div>


                    <div class="status-row">

                        <span class="status-label">

                            Published Tests

                        </span>

                        <asp:Label
                            ID="lblStatusTests"
                            runat="server"
                            CssClass="status-value"
                            Text="0">
                        </asp:Label>

                    </div>


                    <div class="status-row">

                        <span class="status-label">

                            Pending Tests

                        </span>

                        <asp:Label
                            ID="lblStatusPendingTests"
                            runat="server"
                            CssClass="status-value"
                            Text="0">
                        </asp:Label>

                    </div>


                    <div class="status-row">

                        <span class="status-label">

                            Feedback Pending

                        </span>

                        <asp:Label
                            ID="lblStatusFeedback"
                            runat="server"
                            CssClass="status-value"
                            Text="0">
                        </asp:Label>

                    </div>


                    <div class="status-row">

                        <span class="status-label">

                            Certificates Generated

                        </span>

                        <asp:Label
                            ID="lblStatusCertificate"
                            runat="server"
                            CssClass="status-value"
                            Text="0">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </div>


        <!-- ===================================================
             QUICK ACTIONS
             =================================================== -->

        <div class="quick-card">

            <div class="quick-title">

                <i class="fa fa-bolt"></i>
                Quick Actions

            </div>


            <asp:LinkButton
                ID="btnMyTrainings"
                runat="server"
                CssClass="btn btn-primary quick-btn"
                CausesValidation="false"
                OnClick="lnkMyTraining_Click">

                <i class="fa fa-book"></i>
                My Trainings

            </asp:LinkButton>


            <asp:LinkButton
                ID="btnAttendance"
                runat="server"
                CssClass="btn btn-success quick-btn"
                CausesValidation="false"
                OnClick="lnkAttendance_Click">

                <i class="fa fa-check-circle"></i>
                Attendance

            </asp:LinkButton>


            <asp:LinkButton
                ID="btnCertificates"
                runat="server"
                CssClass="btn btn-warning quick-btn"
                CausesValidation="false"
                OnClick="lnkCertificate_Click">

                <i class="fa fa-certificate"></i>
                My Certificates

            </asp:LinkButton>

        </div>


        <!-- ===================================================
             INSTRUCTIONS
             =================================================== -->

        <div class="info-card">

            <div class="info-title">

                <i class="fa fa-info-circle"></i>
                Training Instructions

            </div>

            <ul class="instruction-list">

                <li>
                    Open <b>My Trainings</b> to view your assigned
                    training and session details.
                </li>

                <li>
                    Attend the scheduled sessions and ensure that
                    your attendance is recorded.
                </li>

                <li>
                    Published Pre/Post Tests will become available
                    from the respective session.
                </li>

                <li>
                    Complete all required published tests before
                    submitting Training Feedback.
                </li>

                <li>
                    Submit Training Feedback after completing the
                    required training activities.
                </li>

                <li>
                    Your certificate becomes available after all
                    mandatory activities are completed.
                </li>

            </ul>

        </div>

    </div>

</asp:Content>