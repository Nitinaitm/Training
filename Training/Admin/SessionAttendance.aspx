<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="SessionAttendance.aspx.cs"
    Inherits="Training.Admin.SessionAttendance" %>

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

        .summary-card {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
        }

        .summary-label {
            font-weight: bold;
            color: #0d6efd;
        }

        .table th {
            background: #198754;
            color: white;
            vertical-align: middle;
        }

        .table td {
            vertical-align: middle;
        }

        .btn-space {
            margin-right: 8px;
            margin-bottom: 8px;
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

            <!---------------------------------------------------------->
            <!-- Training Summary -->
            <!---------------------------------------------------------->

            <uc:TrainingSummary
                ID="TrainingSummary1"
                runat="server" />

            <hr />

            <!---------------------------------------------------------->
            <!-- Session Summary -->
            <!---------------------------------------------------------->

            <div class="summary-card">

                <div class="row">

                    <div class="col-md-2">

                        <span class="summary-label">Session No

                        </span>

                        <br />

                        <asp:Label
                            ID="lblSessionNo"
                            runat="server" />

                    </div>

                    <div class="col-md-3">

                        <span class="summary-label">Session Name

                        </span>

                        <br />

                        <asp:Label
                            ID="lblSessionName"
                            runat="server" />

                    </div>

                    <div class="col-md-2">

                        <span class="summary-label">Session Date

                        </span>

                        <br />

                        <asp:Label
                            ID="lblSessionDate"
                            runat="server" />

                    </div>

                    <div class="col-md-2">

                        <span class="summary-label">Topic

                        </span>

                        <br />

                        <asp:Label
                            ID="lblTopic"
                            runat="server" />

                    </div>

                    <div class="col-md-3">

                        <span class="summary-label">Trainer

                        </span>

                        <br />

                        <asp:Label
                            ID="lblTrainer"
                            runat="server" />

                    </div>

                </div>

                <br />

                <div class="row">

                    <div class="col-md-2">

                        <span class="summary-label">Start Time

                        </span>

                        <br />

                        <asp:Label
                            ID="lblStartTime"
                            runat="server" />

                    </div>

                    <div class="col-md-3">

                        <span class="summary-label">End Time

                        </span>

                        <br />

                        <asp:Label
                            ID="lblEndTime"
                            runat="server" />

                    </div>

                    <div class="col-md-2">

                        <span class="summary-label">Hours

                        </span>

                        <br />

                        <asp:Label
                            ID="lblHours"
                            runat="server" />

                    </div>

                    <div class="col-md-2">

                        <span class="summary-label">Attendance Status

                        </span>

                        <br />

                        <asp:Label
                            ID="lblAttendanceStatus"
                            runat="server"
                            Font-Bold="true" />

                    </div>

                </div>

            </div>


            <div class="row">

                <div class="col-md-2">

                    <div class="summary-box">

                        <div class="summary-title">
                            Total Trainees

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblTotalTrainee"
                                runat="server" />

                        </div>

                    </div>

                </div>
                <div class="col-md-2">

    <div class="summary-box">

        <div class="summary-title">
            Pending
        </div>

        <div class="summary-value text-warning">

            <asp:Label
                ID="lblPending"
                runat="server" />

        </div>

    </div>

</div>
                <div class="col-md-2">

                    <div class="summary-box">

                        <div class="summary-title">
                            Present

                        </div>

                        <div class="summary-value text-success">

                            <asp:Label
                                ID="lblPresent"
                                runat="server" />

                        </div>

                    </div>

                </div>

                <div class="col-md-2">

                    <div class="summary-box">

                        <div class="summary-title">
                            Absent

                        </div>

                        <div class="summary-value text-danger">

                            <asp:Label
                                ID="lblAbsent"
                                runat="server" />

                        </div>

                    </div>

                </div>

                <div class="col-md-2">

                    <div class="summary-box">

                        <div class="summary-title">
                            Attendance %

                        </div>

                        <div class="summary-value text-primary">

                            <asp:Label
                                ID="lblAttendancePercent"
                                runat="server" />

                        </div>

                    </div>

                </div>

            </div>
            <hr />
            <!---------------------------------------------------------->
            <!-- Action Buttons -->
            <!---------------------------------------------------------->
            <div class="mb-3">
                <asp:Button
                    ID="btnManualAttendance"
                    runat="server"
                    Text="Manual Attendance"
                    CssClass="btn btn-primary"
                    OnClick="btnManualAttendance_Click" />

                <asp:Button
                    ID="btnBulkAttendance"
                    runat="server"
                    Text="Bulk Attendance"
                    CssClass="btn btn-success"
                    OnClick="btnBulkAttendance_Click" />
            </div>
            <hr />
            <asp:Panel
                ID="pnlManual"
                runat="server">
                <asp:Button
                    ID="btnPresentAll"
                    runat="server"
                    Text="Mark All Present"
                    CssClass="btn btn-success btn-space"
                    OnClick="btnPresentAll_Click" />
                <asp:GridView
                    ID="gvAttendance"
                    runat="server"
                    CssClass="table table-bordered table-hover"
                    AutoGenerateColumns="false">

                    <Columns>

                        <asp:BoundField
                            DataField="EmpID"
                            HeaderText="Employee ID" />

                        <asp:BoundField
                            DataField="EmpName"
                            HeaderText="Employee Name" />

                        <asp:BoundField
                            DataField="EmpDesignation"
                            HeaderText="Designation" />

                        <asp:BoundField
                            DataField="EmpCompany"
                            HeaderText="Organization" />

                        <asp:TemplateField
                            HeaderText="Attendance">

                            <ItemTemplate>

                                <asp:DropDownList
                                    ID="ddlAttendance"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Text="Present"
                                        Value="Present" />

                                    <asp:ListItem
                                        Text="Absent"
                                        Value="Absent" />

                                </asp:DropDownList>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField
                            HeaderText="Remarks">

                            <ItemTemplate>

                                <asp:TextBox
                                    ID="txtRemarks"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="200" />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </asp:Panel>

            <asp:Panel
                ID="pnlBulk"
                runat="server"
                Visible="false">

                <asp:Button
                    ID="btnDownloadExcel"
                    runat="server"
                    Text="Download Excel Format"
                    CssClass="btn btn-primary btn-space"
                    OnClick="btnDownloadExcel_Click" />

                <asp:FileUpload
                    ID="fuExcel"
                    runat="server" />

                <asp:Button
                    ID="btnUploadExcel"
                    runat="server"
                    Text="Upload Excel"
                    CssClass="btn btn-warning btn-space"
                    OnClick="btnUploadExcel_Click" />

            </asp:Panel>

<hr />

            <div class="mb-3">

                <asp:FileUpload
                    ID="fuAttendanceSheet"
                    runat="server" />

                <asp:Button
                    ID="btnUploadAttendanceSheet"
                    runat="server"
                    Text="Upload Attendance Sheet (PDF)"
                    CssClass="btn btn-secondary btn-space"
                    OnClick="btnUploadAttendanceSheet_Click" />

            </div>

            <hr />

            <!---------------------------------------------------------->
            <!-- Employee Grid -->
            <!---------------------------------------------------------->



            <br />

            <div class="text-center">

                <asp:Button
                    ID="btnSaveAttendance"
                    runat="server"
                    Text="Mark Attendance"
                    CssClass="btn btn-success btn-lg"
                    OnClick="btnSaveAttendance_Click" />

                &nbsp;

                <asp:Button
                    ID="btnBack"
                    runat="server"
                    Text="Back"
                    CssClass="btn btn-secondary btn-lg"
                    OnClick="btnBack_Click" />

            </div>

            <br />

            <asp:Label
                ID="lblMessage"
                runat="server"
                Font-Bold="true" />

        </div>

    </div>

</asp:Content>
