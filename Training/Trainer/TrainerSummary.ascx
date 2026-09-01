<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="TrainerSummary.ascx.cs"
    Inherits="Training.Trainer.TrainerSummary" %>

<style>
   .summary-card {
    background: #f8f9fa;
    border: 1px solid #dee2e6;
    border-radius: 10px;
    padding: 15px;
    margin-bottom: 20px;
    overflow: hidden;
}

.summary-label {
    font-weight: bold;
    color: #0d6efd;
    display: block;
    margin-bottom: 3px;
}

.summary-value {
    color: #212529;
    font-size: 15px;
    display: block;
    white-space: normal;
    word-wrap: break-word;
    overflow-wrap: anywhere;
}

.status-badge {
    font-size: 16px;
    padding: 8px 15px;
    display: inline-block;
    white-space: nowrap;
}

.row > div {
    min-width: 0;
}

@media (max-width:767px) {

    .summary-card {
        padding: 12px;
    }

    .summary-label {
        font-size: 13px;
    }

    .summary-value {
        font-size: 14px;
    }
}
</style>

<div class="summary-card">

    <!-- ================= Row-1 ================= -->

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
                Course Category :
            

            <asp:Label
                ID="lblCategory"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-2 col-md-2 col-sm-12 mb-4">

            <span class="summary-label">
                Training Status :
           

            <asp:Label
                ID="lblStatus"
                runat="server"
                CssClass="badge bg-success status-badge" />
             </span>
        </div>

    </div>

    <!-- ================= Row-2 ================= -->

    <div class="row">

        <div class="col-lg-4 col-md-3 col-sm-6 mb-4">

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
                Training Type :
            

            <asp:Label
                ID="lblTrainingType"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                Organizer :

            <asp:Label
                ID="lblOrganizer"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                Batch :
            

            <asp:Label
                ID="lblBatch"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

    </div>

    <!-- ================= Row-3 ================= -->

    <div class="row">

        <div class="col-lg-4 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                Training Duration :
           

            <asp:Label
                ID="lblDuration"
                runat="server"
                CssClass="summary-value" />
             </span>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                No. Of Days :
            

            <asp:Label
                ID="lblDays"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                Planned Hours :
            

            <asp:Label
                ID="lblHours"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">

            <span class="summary-label">
                Batch Strength :
           

            <asp:Label
                ID="lblBatchStrength"
                runat="server"
                CssClass="summary-value" />
             </span>
        </div>

    </div>

    <!-- ================= Row-4 ================= -->

    <div class="row">

        <div class="col-lg-4 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">
                Total Sessions :
            

            <asp:Label
                ID="lblTotalSession"
                runat="server"
                CssClass="summary-value" />
            </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">
                Completed :
          

            <asp:Label
                ID="lblCompleted"
                runat="server"
                CssClass="summary-value" />
              </span>
        </div>

        <div class="col-lg-3 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">
                Pending :
            

            <asp:Label
                ID="lblPending"
                runat="server"
                CssClass="summary-value" />
                </span>
        </div>

        <div class="col-lg-2 col-md-4 col-sm-6 mb-4">

            <span class="summary-label">
                Today :
           

            <asp:Label
                ID="lblToday"
                runat="server"
                CssClass="summary-value" />
             </span>
        </div>

       

    </div>

    <!-- ================= Row-5 ================= -->

    <div class="row">
         <div class="col-lg-4 col-md-8 col-sm-12 mb-4">

            <span class="summary-label">
                Attendance Completion :
            
            <asp:Label
                ID="lblAttendance"
                runat="server"
                CssClass="summary-value" />
                </span>

        </div>
        <div class="col-lg-8 col-md-8 col-sm-12 mb-4">

           

                <span class="summary-label me-2">
                    Course Description :
                

                <asp:Label
                    ID="lblDescription"
                    runat="server"
                    CssClass="summary-value flex-grow-1" />
                </span>
            </div>

      
    </div>

</div>