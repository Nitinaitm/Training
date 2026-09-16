<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="SessionSummary.ascx.cs"
    Inherits="Training.Trainer.SessionSummary" %>

<style>
    .session-card {
        background: #fff;
        border-radius: 12px;
        box-shadow: 0 0 10px #d9d9d9;
        padding: 20px;
        margin-bottom: 20px;
    }

    .session-heading {
        font-size: 24px;
        font-weight: bold;
        color: #0d6efd;
        margin-bottom: 20px;
    }

    .summary-label {
        font-weight: 600;
        color: #0d6efd;
        display: block;
        margin-bottom: 3px;
    }

    .summary-value {
        color: #212529;
        word-break: break-word;
        font-weight: 500;
    }

    .info-row {
        margin-bottom: 15px;
    }

    .status-badge {
        font-size: 15px;
        padding: 7px 16px;
    }

    .summary-strip {
        margin-top: 20px;
        border-top: 1px solid #dee2e6;
        padding-top: 15px;
    }

    .summary-box {
        text-align: center;
        border-right: 1px solid #e6e6e6;
    }

        .summary-box:last-child {
            border-right: none;
        }

    .summary-count {
        font-size: 26px;
        font-weight: bold;
        color: #198754;
    }

    .summary-text {
        font-size: 14px;
        color: #666;
    }
</style>

<div class="session-card">

    <div class="session-heading">
        Session Summary

    </div>

    <div class="row">

        <div class="col-lg-4 col-md-4 col-sm-12 mb-4">
            <span class="summary-label">Session ID

         

            <asp:Label
                ID="lblSessionID"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">Session No

           

            <asp:Label
                ID="lblSessionNo"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">Session Name

         

            <asp:Label
                ID="lblSessionName"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-2 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">Topic

          

            <asp:Label
                ID="lblTopic"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-4 col-md-4 col-sm-12 mb-4">
            <span class="summary-label">Trainer

          

            <asp:Label
                ID="lblTrainer"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">
            <span class="summary-label">Trainer Type

            

            <asp:Label
                ID="lblTrainerType"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">
            <span class="summary-label">Session Date

            

            <asp:Label
                ID="lblSessionDate"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>
        <div class="col-lg-2 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">Total Hours

           

            <asp:Label
                ID="lblHours"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

    </div>
    <div class="row">
        <div class="col-lg-4 col-md-4 col-sm-12 mb-4">
            <span class="summary-label">Start Time

          

            <asp:Label
                ID="lblStartTime"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>
        <div class="col-lg-3 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">End Time

           

            <asp:Label
                ID="lblEndTime"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>



        <div class="col-lg-3 col-md-4 col-sm-12 mb-4" runat="server" visible="false">

            <span class="summary-label">Session Status

          

            <asp:Label
                ID="lblSessionStatus"
                runat="server"
                CssClass="badge bg-primary status-badge" />
            </span>
        </div>

        <div class="col-lg-2 col-md-4 col-sm-12 mb-4">

            <span class="summary-label">Attendance

          

            <asp:Label
                ID="lblAttendanceStatus"
                runat="server"
                CssClass="badge bg-success status-badge" />
            </span>
        </div>



    </div>

    <div class="summary-strip">

        <div class="row">

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblTotalTrainee"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Total Trainees

                </div>

            </div>

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblPresent"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Present

                </div>

            </div>

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblAbsent"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Absent

                </div>

            </div>

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblMaterial"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Materials

                </div>

            </div>

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblPreTest"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Pre Test

                </div>

            </div>

            <div class="col-md-2 summary-box">

                <div class="summary-count">

                    <asp:Label
                        ID="lblPostTest"
                        runat="server"
                        Text="0" />

                </div>

                <div class="summary-text">
                    Post Test

                </div>

            </div>

        </div>
        <hr />



    </div>

</div>
