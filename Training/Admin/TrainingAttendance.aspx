<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingAttendance.aspx.cs"
    Inherits="Training.Admin.TrainingAttendance" %>

<%@ Register Src="~/Admin/TrainingSummary.ascx"
    TagPrefix="uc"
    TagName="TrainingSummary" %>

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

        .summary-box {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            text-align: center;
            margin-bottom: 15px;
        }

        .summary-title {
            font-size: 13px;
            color: #666;
        }

        .summary-value {
            font-size: 24px;
            font-weight: bold;
            color: #198754;
        }

        .table th {
            background: #198754;
            color: white;
            vertical-align: middle;
        }

        .table td {
            vertical-align: middle;
        }

        .status-pending {
            color: #dc3545;
            font-weight: bold;
        }

        .status-complete {
            color: #198754;
            font-weight: bold;
        }

        .action-btn {
            min-width: 130px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Session Attendance

            </div>

            <!---------------------------------------------->
            <!-- Training Summary -->
            <!---------------------------------------------->

            <uc:trainingsummary
                id="TrainingSummary1"
                runat="server" />

            <hr />

            <!---------------------------------------------->
            <!-- Attendance Summary -->
            <!---------------------------------------------->

            <div class="row">

                <div class="col-md-3">

                    <div class="summary-box">

                        <div class="summary-title">
                            Total Sessions
                        </div>

                        <div class="summary-value">

                            <asp:label
                                id="lblTotalSession"
                                runat="server" />

                        </div>

                    </div>

                </div>

                <div class="col-md-3">

                    <div class="summary-box">

                        <div class="summary-title">
                            Attendance Completed
                        </div>

                        <div class="summary-value text-success">

                            <asp:label
                                id="lblCompleted"
                                runat="server" />

                        </div>

                    </div>

                </div>

                <div class="col-md-3">

                    <div class="summary-box">

                        <div class="summary-title">
                            Pending
                        </div>

                        <div class="summary-value text-danger">

                            <asp:label
                                id="lblPending"
                                runat="server" />

                        </div>

                    </div>

                </div>

                <div class="col-md-3">

                    <div class="summary-box">

                        <div class="summary-title">
                            Progress
                        </div>

                        <div class="summary-value text-primary">

                            <asp:label
                                id="lblProgress"
                                runat="server" />

                        </div>

                    </div>

                </div>

            </div>

            <hr />

            <!---------------------------------------------->
            <!-- Session Grid -->
            <!---------------------------------------------->

            <asp:gridview
                id="gvSession"
                runat="server"
                cssclass="table table-bordered table-hover"
                autogeneratecolumns="false"
                datakeynames="SessionID"
                onrowcommand="gvSession_RowCommand">

                <Columns>

                    <asp:BoundField
                        DataField="SessionNo"
                        HeaderText="Session No" />

                    <asp:BoundField
                        DataField="SessionDate"
                        HeaderText="Date" />

                    <asp:BoundField
                        DataField="SessionName"
                        HeaderText="Session Name" />

                    <asp:BoundField
                        DataField="TopicName"
                        HeaderText="Topic" />

                    <asp:BoundField
                        DataField="TrainerName"
                        HeaderText="Trainer" />

                    <asp:BoundField
                        DataField="StartTime"
                        HeaderText="Start" />

                    <asp:BoundField
                        DataField="EndTime"
                        HeaderText="End" />

                    <asp:BoundField
                        DataField="TotalHours"
                        HeaderText="Hours" />

                    <asp:TemplateField
                        HeaderText="Attendance">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblAttendance"
                                runat="server"
                                Text='<%# Eval("AttendanceStatus") %>'
                                CssClass='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "status-complete" : "status-pending" %>'>
                            </asp:Label>

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField                        HeaderText="Action">

                        <ItemTemplate>

                            <asp:Button
                                ID="btnAttendance"
                                runat="server"
                                CssClass="btn btn-primary btn-sm action-btn"
                                Text='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "View Attendance" : "Mark Attendance" %>'
                                CommandName="Attendance"
                                CommandArgument='<%# Eval("SessionID") %>' />

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:gridview>

            <br />

            <!---------------------------------------------->
            <!-- Final Attendance -->
            <!---------------------------------------------->

            <div class="text-center">

                <asp:button
                    id="btnFinalizeAttendance"
                    runat="server"
                    text="Finalize Attendance"
                    cssclass="btn btn-success btn-lg"
                    visible="false"
                    onclick="btnFinalizeAttendance_Click" />

            </div>

            <br />

            <asp:label
                id="lblMessage"
                runat="server"
                font-bold="true">
            </asp:label>

        </div>

    </div>

</asp:Content>
