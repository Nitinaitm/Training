<%@ Page Title="Session Attendance"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="SessionAttendance.aspx.cs"
    Inherits="Training.Trainer.SessionAttendance" %>

<%@ Register Src="~/Trainer/SessionSummary.ascx"
    TagPrefix="uc"
    TagName="SessionSummary" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

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
            border-radius: 10px;
            padding: 20px;
        }

        .summary-label {
            font-weight: bold;
            color: #0d6efd;
        }

        

        .mode-card {
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 0 8px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px;
        }

        .mode-title {
            font-size: 22px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 15px;
        }

        .panel-card {
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 0 8px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px;
        }

        .panel-title {
            font-size: 20px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 15px;
        }

        .btn-mode {
            min-width: 220px;
            margin-right: 10px;
        }

        .gridview th {
            background: #198754;
            color: #ffffff;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
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

            <uc:SessionSummary
                ID="SessionSummary1"
                runat="server" />

            <hr />

            <div class="row">

                <div class="col-lg-2 col-md-4">

                    <div class="summary-box">

                        <div class="summary-title">
                            Total Trainees

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblTotal"
                                runat="server"
                                Text="0" />

                        </div>

                    </div>

                </div>

                <div class="col-lg-2 col-md-4">

                    <div class="summary-box">

                        <div class="summary-title">
                            Present

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblPresent"
                                runat="server"
                                Text="0" />

                        </div>

                    </div>

                </div>

                <div class="col-lg-2 col-md-4">

                    <div class="summary-box">

                        <div class="summary-title">
                            Absent

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblAbsent"
                                runat="server"
                                Text="0" />

                        </div>

                    </div>

                </div>

                <div class="col-lg-3 col-md-6">

                    <div class="summary-box">

                        <div class="summary-title">
                            Pending

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblPending"
                                runat="server"
                                Text="0" />

                        </div>

                    </div>

                </div>

                <div class="col-lg-3 col-md-6">

                    <div class="summary-box">

                        <div class="summary-title">
                            Attendance %

                        </div>

                        <div class="summary-value">

                            <asp:Label
                                ID="lblAttendancePercent"
                                runat="server"
                                Text="0.00 %" />

                        </div>

                    </div>

                </div>

            </div>
            <hr />
            <div class="mode-card">

                <div class="mode-title">
                    Attendance Mode

                </div>

                <asp:Button
                    ID="btnNormalAttendance"
                    runat="server"
                    Text="Normal Attendance"
                    CssClass="btn btn-success btn-mode"
                    OnClick="btnNormalAttendance_Click" />

                <asp:Button
                    ID="btnBulkAttendance"
                    runat="server"
                    Text="Bulk Attendance"
                    CssClass="btn btn-primary btn-mode"
                    OnClick="btnBulkAttendance_Click" />

            </div>

            <asp:Panel
                ID="pnlNormalAttendance"
                runat="server"
                CssClass="panel-card">

                <div class="panel-title">
                    Normal Attendance

                </div>
                <asp:GridView
                    ID="gvAttendance"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-hover gridview"
                    Width="100%"
                    EmptyDataText="No Trainee Assigned"
                    ShowHeaderWhenEmpty="true"
                    DataKeyNames="AssignmentID,EmpID" OnRowDataBound="gvAttendance_RowDataBound">

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate>
                                <asp:Label
                                    ID="lblSlNo"
                                    runat="server"
                                    Text='<%# Container.DataItemIndex + 1 %>' />
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:BoundField
                            DataField="EmpID"
                            HeaderText="Employee ID">

                            <ItemStyle Width="120px" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="EmpName"
                            HeaderText="Employee Name" />

                        <asp:BoundField
                            DataField="EmpDesignation"
                            HeaderText="Designation">

                            <ItemStyle Width="180px" />

                        </asp:BoundField>

                        <asp:TemplateField HeaderText="Attendance">

                            <ItemTemplate>

                                <asp:DropDownList
                                    ID="ddlAttendance"
                                    runat="server"
                                    CssClass="form-select">

                                    <asp:ListItem
                                        Value=""
                                        Text="--Select--" />

                                    <asp:ListItem
                                        Value="Present"
                                        Text="Present" />

                                    <asp:ListItem
                                        Value="Absent"
                                        Text="Absent" />

                                </asp:DropDownList>

                            </ItemTemplate>

                            <ItemStyle
                                Width="160px"
                                HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Remarks">

                            <ItemTemplate>

                                <asp:TextBox
                                    ID="txtRemarks"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="250"
                                    Text='<%# Eval("Remarks") %>' />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

                <div class="row mt-3">

                    <div class="col-md-12 text-center">

                        <asp:Button
                            ID="btnSaveAttendance"
                            runat="server"
                            Text="Save Attendance"
                            CssClass="btn btn-success btn-lg"
                            OnClick="btnSaveAttendance_Click" />

                    </div>

                </div>

            </asp:Panel>

            <asp:Panel
                ID="pnlBulkAttendance"
                runat="server"
                CssClass="panel-card"
                Visible="false">

                <div class="panel-title">
                    Bulk Attendance

                </div>

                <div class="row">

                    <div class="col-md-3">

                        <asp:Button
                            ID="btnDownloadSample"
                            runat="server"
                            Text="Download Sample"
                            CssClass="btn btn-success w-100"
                            OnClick="btnDownloadSample_Click" />

                    </div>

                    <div class="col-md-6">

                        <asp:FileUpload
                            ID="fuAttendanceExcel"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-3">

                        <asp:Button
                            ID="btnUploadExcel"
                            runat="server"
                            Text="Upload Excel"
                            CssClass="btn btn-primary w-100"
                            OnClick="btnUploadExcel_Click" />

                    </div>

                </div>

            </asp:Panel>
            <div class="panel-card">

                <div class="panel-title">
                    Attendance Sheet

                </div>

                <div class="row">

                    <div class="col-md-8">

                        <label>
                            Upload Attendance Sheet (PDF)

                        </label>

                        <asp:FileUpload
                            ID="fuAttendanceSheet"
                            runat="server"
                            CssClass="form-control" />

                        <small class="text-muted">Only PDF file is allowed.

                        </small>

                    </div>

                    <div class="col-md-4">

                        <label>
                            &nbsp;

                        </label>

                        <br />

                        <asp:Button
                            ID="btnUploadAttendanceSheet"
                            runat="server"
                            Text="Upload PDF"
                            CssClass="btn btn-info w-100"
                            OnClick="btnUploadAttendanceSheet_Click" />

                    </div>

                </div>

            </div>

            <div class="panel-card">

                <div class="row">

                    <div class="col-md-12 text-center">
                         <asp:Button
                            ID="btnSessionDetails"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-danger btn-lg"
                            Width="260px"
                            OnClick="btnSessionDetails_Click"
                            />
                        <asp:Button
                            ID="btnCompleteAttendance"
                            runat="server"
                            Text="Complete Attendance"
                            CssClass="btn btn-danger btn-lg"
                            Width="260px"
                            OnClick="btnCompleteAttendance_Click"
                            OnClientClick="return confirm('Attendance will be locked after completion. Do you want to continue?');" />
                       
                    </div>

                </div>

                <div class="row mt-3">

                    <div class="col-md-12 text-center">

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Font-Bold="true"
                            CssClass="text-danger" />

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
