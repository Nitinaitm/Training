<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="TrainingSummary.ascx.cs"
    Inherits="Training.Admin.TrainingSummary" %>
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
 .description-text {
        display: block;
        min-height: 60px;
        text-align: justify;
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
            <span class="summary-label">Training ID :
            </span>
            <asp:Label ID="lblTrainingID"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Course :
            </span>
            <asp:Label ID="lblCourse"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Course Category :
            </span>
            <asp:Label ID="lblCourseCategory"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-2 col-md-2 col-sm-12 mb-4">
            <span class="summary-label">Status :
            </span>


            <asp:Label
                ID="lblStatus"
                runat="server"
                CssClass="badge bg-primary status-badge" />


        </div>

    </div>



    <!-- ================= Row-2 ================= -->
    <div class="row">
         <div class="col-lg-4 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Location :
            </span>
            <asp:Label ID="lblLocation"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Training Category :
            </span>
            <asp:Label ID="lblCategory"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Training Type :
            </span>
            <asp:Label ID="lblTrainingType"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Organizer :
            </span>
            <asp:Label ID="lblOrganizer"
                runat="server"
                CssClass="summary-value" />
        </div>



    </div>



    <!-- ================= Row-3 ================= -->
    <div class="row">
       
        <div class="col-lg-4 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Training Duration :
            </span>
            <asp:Label ID="lblTrainingDuration"
                runat="server"
                CssClass="summary-value" />
        </div>
        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">No Of Days :
            </span>
            <asp:Label ID="lblNoOfDays"
                runat="server"
                CssClass="summary-value" />
        </div>
        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Batch :
            </span>
            <asp:Label ID="lblBatch"
                runat="server"
                CssClass="summary-value" />
        </div>
        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Planned Hours :
            </span>
            <asp:Label ID="lblPlannedHours"
                runat="server"
                CssClass="summary-value" />
        </div>
       




    </div>



    <!-- ================= Row-4 ================= -->
    <div class="row">
         <div class="col-lg-4 col-md-3 col-sm-6 mb-4" >
            <span class="summary-label">Hostel Required :
            </span>
            <asp:Label ID="lblHostelRequired"
                runat="server"
                CssClass="summary-value" />
        </div>
          <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Batch Strength :
            </span>
            <asp:Label ID="lblBatchStrength"
                runat="server"
                CssClass="summary-value" />
        </div>

       

        <div class="col-lg-3 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Assigned :
            </span>
            <asp:Label ID="lblAssigned"
                runat="server"
                CssClass="summary-value" />
        </div>

        <div class="col-lg-2 col-md-3 col-sm-6 mb-4">
            <span class="summary-label">Remaining :
            </span>
            <asp:Label ID="lblRemaining"
                runat="server"
                CssClass="summary-value" />
        </div>


    </div>



    <!-- ================= Row-5 ================= -->
    <div class="row">
        <div class="col-12">

            <div class="d-flex flex-wrap align-items-start">
                <span class="summary-label me-2">Course Description :
                </span>

                <asp:Label
                    ID="lblCourseDescription"
                    runat="server"
                    CssClass="summary-value flex-grow-1" />
            </div>

        </div>
       
    </div>

</div>
