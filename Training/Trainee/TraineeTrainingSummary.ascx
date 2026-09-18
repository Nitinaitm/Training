<%@ Control Language="C#"
    AutoEventWireup="true"
    CodeBehind="TraineeTrainingSummary.ascx.cs"
    Inherits="Training.Trainee.TraineeTrainingSummary" %>

<style>
    .summary-card {
    background: #f8f9fa;
    border: 1px solid #dee2e6;
    border-radius: 10px;
    padding: 15px;
    margin-bottom: 20px;
    font-weight: bold;
    width: 100%;
    overflow: hidden;
}

.summary-card .row {
    margin-bottom: 0;
}

.summary-card [class*="col-"] {
    min-width: 0;
}

.summary-label {
    font-weight: bold;
    color: #0d6efd;
    display: block;
    white-space: normal;
    line-height: 1.5;
}

.summary-value {
    color: #212529;
    font-size: 15px;
    font-weight: 600;
    word-break: break-word;
    overflow-wrap: anywhere;
    white-space: normal;
}

.status-badge {
    display: inline-block;
    max-width: 100%;
    font-size: 12px;
    font-weight: 600;
    padding: 5px 10px;
    border-radius: 15px;
    line-height: 1.3;
    white-space: normal;
    word-break: normal;
    overflow-wrap: break-word;
    vertical-align: middle;
    text-align: center;
    margin-top: 3px;
}

.status-success {
    background-color: #198754 !important;
    color: #ffffff !important;
}

.status-warning {
    background-color: #ffc107 !important;
    color: #212529 !important;
}

.status-danger {
    background-color: #dc3545 !important;
    color: #ffffff !important;
}

.status-info {
    background-color: #0dcaf0 !important;
    color: #212529 !important;
}

.status-secondary {
    background-color: #6c757d !important;
    color: #ffffff !important;
}

.exam-type {
    display: block;
    font-weight: bold;
    color: #198754;
    white-space: normal;
}

.summary-heading {
    font-size: 18px;
    font-weight: bold;
    color: #198754;
    margin-bottom: 18px;
    padding-bottom: 8px;
    border-bottom: 1px solid #dee2e6;
}

.summary-section-title,
.exam-heading {
    font-size: 16px;
    font-weight: bold;
    color: #198754;
    margin-bottom: 15px;
}

.summary-divider {
    margin-top: 0;
    margin-bottom: 17px;
    border-top: 1px solid #dee2e6;
}

@media (max-width: 991px) {

    .summary-label {
        font-size: 13px;
    }

    .summary-value {
        font-size: 14px;
    }

    .status-badge {
        font-size: 11px;
        padding: 5px 8px;
    }
}

@media (max-width: 767px) {

    .summary-card {
        padding: 12px;
    }

    .summary-heading {
        font-size: 16px;
    }

    .summary-label {
        font-size: 12px;
    }

    .summary-value {
        font-size: 14px;
    }

    .status-badge {
        display: inline-block;
        width: auto;
        max-width: 100%;
    }
}
</style>


<div class="summary-card">


    <!-- =====================================================
         HEADING
         ===================================================== -->

    <div class="summary-heading">

        <i class="fa fa-graduation-cap"></i>

        Training Summary

    </div>


    <!-- =====================================================
         ROW 1
         ===================================================== -->

    <div class="row">


        <div class="col-lg-4 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">

                Training ID :

                <asp:Label
                    ID="lblTrainingID"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Course :

                <asp:Label
                    ID="lblCourse"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Training Type :

                <asp:Label
                    ID="lblTrainingType"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>
          <div class="col-lg-2 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Organizer :

                <asp:Label
                    ID="lblOrganizer"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-2 col-md-2 col-sm-12 mb-4" runat="server" visible="false">

            <span class="summary-label">

                Status :

                <asp:Label
    ID="lblTrainingStatus"
    runat="server"
    CssClass="status-badge status-secondary" />

            </span>

        </div>


    </div>


    <!-- =====================================================
         ROW 2
         ===================================================== -->

    <div class="row">


        <div class="col-lg-4 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">

                Location :

                <asp:Label
                    ID="lblLocation"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


      <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Total Sessions :

                <asp:Label
                    ID="lblTotalSessions"
                    runat="server"
                    CssClass="summary-value"
                    Text="0" />

            </span>

        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Batch :

                <asp:Label
                    ID="lblBatch"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-2 col-md-2 col-sm-6 mb-4">

            <span class="summary-label">

                Duration :

                <asp:Label
                    ID="lblDuration"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


    </div>


    <!-- =====================================================
         ROW 3
         ===================================================== -->

    <div class="row">


        <div class="col-lg-4 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">

                Date From :

                <asp:Label
                    ID="lblDateFrom"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Date To :

                <asp:Label
                    ID="lblDateTo"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        


    </div>


    <!-- =====================================================
         TRAINER DETAILS
         ===================================================== -->

    <hr class="summary-divider" />


    <div class="summary-section-title">

        <i class="fa fa-user"></i>

        Trainer Details

    </div>


    <div class="row">


        <div class="col-lg-4 col-md-8 col-sm-12 mb-4">

            <span class="summary-label">

                Trainer(s) :

                <asp:Label
                    ID="lblTrainer"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">

                Trainer Type :

                <asp:Label
                    ID="lblTrainerType"
                    runat="server"
                    CssClass="summary-value" />

            </span>

        </div>


    </div>


    <!-- =====================================================
         ATTENDANCE
         ===================================================== -->

    <hr class="summary-divider" />


    <div class="summary-section-title">

        <i class="fa fa-check-square-o"></i>

        My Attendance

    </div>


    <div class="row">


        <div class="col-lg-4 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Present :

                <asp:Label
                    ID="lblPresent"
                    runat="server"
                    CssClass="summary-value"
                    Text="0" />

            </span>

        </div>


        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Total Sessions :

                <asp:Label
                    ID="lblTotalAttendanceSessions"
                    runat="server"
                    CssClass="summary-value"
                    Text="0" />

            </span>

        </div>


        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Attendance % :

                <asp:Label
                    ID="lblAttendancePercent"
                    runat="server"
                    CssClass="summary-value"
                    Text="0%" />

            </span>

        </div>


        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">

                Status :

              <asp:Label
    ID="lblAttendanceStatus"
    runat="server"
    CssClass="status-badge status-secondary"
    Text="Pending" />

            </span>

        </div>


    </div>


    <!-- =====================================================
         COMPLETION STATUS
         ===================================================== -->

    <hr class="summary-divider" />


    <div class="summary-section-title">

        <i class="fa fa-tasks"></i>

        Completion Status

    </div>


    <div class="row">


        <div class="col-lg-4 col-md-4 col-sm-12 mb-3">

            <span class="summary-label">

                Training Feedback :

               <asp:Label
    ID="lblFeedbackStatus"
    runat="server"
    CssClass="status-badge status-secondary"
    Text="Pending" />

            </span>

        </div>


        <div class="col-lg-3 col-md-4 col-sm-12 mb-3">

            <span class="summary-label">

                Certificate :

             <asp:Label
    ID="lblCertificateStatus"
    runat="server"
    CssClass="status-badge status-secondary"
    Text="Not Generated" />

            </span>

        </div>


        <div class="col-lg-5 col-md-4 col-sm-12 mb-3">

            <span class="summary-label">

                Overall Status :

              <asp:Label
    ID="lblOverallStatus"
    runat="server"
    CssClass="status-badge status-secondary"
    Text="In Progress" />

            </span>

        </div>


    </div>


</div>