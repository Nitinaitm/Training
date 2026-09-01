<%@ page title="TrainingDetails" language="C#" masterpagefile="~/TraineeMaster.Master" autoeventwireup="true" codebehind="TrainingDetails.aspx.cs" maintainscrollpositiononpostback="true" inherits="Training.Trainee.TrainingDetails" %>

<%@ register
    src="~/Trainee/TraineeTrainingSummary.ascx"
    tagprefix="uc1"
    tagname="TraineeTrainingSummary" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
      <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>



    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        .card-box {
            border: 0;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
            margin-bottom: 20px;
        }

        .page-title {
            font-size: 26px;
            font-weight: 600;
        }

        .title {
            font-weight: 600;
            color: #555;
        }

        .value {
            font-size: 16px;
            font-weight: 500;
        }

        .status-box {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 4px;
            color: #fff;
            font-size: 13px;
        }

        .completed {
            background: #28a745;
        }

        .pending {
            background: #dc3545;
        }

        .progress {
            height: 24px;
        }

        .workflow-btn {
            width: 100%;
            margin-bottom: 10px;
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle
        }

        .gridview td {
            vertical-align: middle
        }
    </style>

</asp:Content>

<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        

        <uc1:TraineeTrainingSummary
    ID="TraineeTrainingSummary1"
    runat="server" />

      

        <div class="card card-box" runat="server" visible="false">

            <div class="card-header">

                <b>Training Progress
                </b>

            </div>

            <div class="card-body">

                <div class="progress">

                    <div
                        id="progressBar"
                        runat="server"
                        class="progress-bar progress-bar-striped progress-bar-animated bg-success"
                        role="progressbar"
                        style="width: 0%;">

                        <asp:Label
                            ID="lblProgress"
                            runat="server"
                            ForeColor="White">
                        </asp:Label>

                    </div>

                </div>

                <br />

                <div>
                    Next Activity :

                    <b>

                        <asp:Label
                            ID="lblNextActivity"
                            runat="server">
                        </asp:Label>

                    </b>

                </div>

            </div>

        </div>

      <div class="card card-box">

    <div class="card-header">

        <b>Training Sessions</b>

    </div>

    <div class="card-body table-responsive">

        <asp:GridView
            ID="gvSession"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover gridview"
            EmptyDataText="No Session Available."
            DataKeyNames="SessionID"
            OnRowCommand="gvSession_RowCommand" OnRowDataBound="gvSession_RowDataBound">

            <Columns>

                <asp:BoundField
                    HeaderText="No"
                    DataField="SessionNo" />

                <asp:BoundField
                    HeaderText="Session"
                    DataField="SessionName" />

                <asp:BoundField
                    HeaderText="Topic"
                    DataField="TopicName" />

               <asp:BoundField
    HeaderText="Trainer"
    DataField="TrainerName" />

                <asp:BoundField
                    HeaderText="Date"
                    DataField="SessionDate"
                    DataFormatString="{0:dd-MMM-yyyy}" />

                <asp:TemplateField HeaderText="Time">

                    <ItemTemplate>

                        <%# Eval("StartTime") %>
                        -
                        <%# Eval("EndTime") %>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:BoundField
                    HeaderText="Attendance"
                    DataField="AttendanceStatus" />

                <asp:TemplateField HeaderText="Pre Test">
    <ItemTemplate>
        <asp:Label
            ID="lblPre"
            runat="server"
            ForeColor="Black"
            Text='<%# Eval("PreStatus") %>'>

        </asp:Label>
    </ItemTemplate>
</asp:TemplateField>

               <asp:TemplateField HeaderText="Post Test">
    <ItemTemplate>
        <asp:Label
            ID="lblPost"
            runat="server"
            ForeColor="Black"
            Text='<%# Eval("PostStatus") %>'>
        </asp:Label>
    </ItemTemplate>
</asp:TemplateField>
               
                <asp:TemplateField HeaderText="Action">

                    <ItemTemplate>

                        <asp:LinkButton
                            ID="lnkView"
                            runat="server"
                            CssClass="btn btn-primary btn-sm"
                            CommandName="ViewSession"
                            CommandArgument='<%# Eval("SessionID") %>'>

                            View

                        </asp:LinkButton>

                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

</div>
        <div class="row">

    <div class="col-md-12 text-center">
        <asp:Button
            ID="btnBack"
            runat="server"
            Text="Back"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnBack_Click" />

        <asp:Button
            ID="btnBatchFeedback"
            runat="server"
            Text="Batch Feedback"
            CssClass="btn btn-warning btn-block"
            OnClick="btnBatchFeedback_Click" />

   

        <asp:Button
            ID="btnCertificate"
            runat="server"
            Text="Download Certificate"
            CssClass="btn btn-success btn-block"
            OnClick="btnCertificate_Click" />
      

        

    </div>

</div>

    </div>

</asp:content>
