<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="Training.Trainer.Attendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .summary-box {
            background: #f8f9fa;
            border-left: 5px solid #198754;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 15px;
            text-align: center
        }

        .summary-title {
            color: #666;
            font-size: 15px
        }

        .summary-value {
            font-size: 28px;
            font-weight: bold;
            color: #198754
        }

        .search-panel {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px
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

        .btn-action {
            min-width: 100px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Attendance Management</div>
        <div class="row">
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Total Sessions</div>
                    <div class="summary-value">
                        <asp:Label ID="lblTotal" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Attendance Completed</div>
                    <div class="summary-value">
                        <asp:Label ID="lblCompleted" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Pending Attendance</div>
                    <div class="summary-value">
                        <asp:Label ID="lblPending" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Attendance %</div>
                    <div class="summary-value">
                        <asp:Label ID="lblPercent" runat="server" Text="0%" /></div>
                </div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="search-panel">
                <div class="row">
                    <div class="col-md-3">
                        <label>Training ID</label><asp:TextBox ID="txtTrainingID" runat="server" CssClass="form-control" /></div>
                    <div class="col-md-2">
                        <label>From Date</label><asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="form-control" /></div>
                    <div class="col-md-2">
                        <label>To Date</label><asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="form-control" /></div>
                    <div class="col-md-2">
                        <label>Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="All" Value="" />
                            <asp:ListItem Text="Pending" />
                            <asp:ListItem Text="Completed" />
                        </asp:DropDownList></div>
                    <div class="col-md-3">
                        <br />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" /></div>
                </div>
            </div>
            <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" OnRowCommand="gvAttendance_RowCommand" DataKeyNames="SessionID">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="SessionNo" HeaderText="Session No" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session Name" />
                    <asp:BoundField DataField="SessionDate" HeaderText="Date" />
                    <asp:BoundField DataField="StartTime" HeaderText="Start" />
                    <asp:BoundField DataField="EndTime" HeaderText="End" />
                    <asp:BoundField DataField="TotalTrainees" HeaderText="Total Trainees" />
                    <asp:BoundField DataField="Present" HeaderText="Present" />
                    <asp:BoundField DataField="Absent" HeaderText="Absent" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("AttendanceStatus") %>' CssClass='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "badge bg-success" : "badge bg-warning text-dark" %>'></asp:Label></ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button ID="btnAction" runat="server" Text='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "View" : "Take Attendance" %>' CssClass='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "btn btn-info btn-sm" : "btn btn-success btn-sm" %>' CommandName="TakeAttendance" CommandArgument='<%# Eval("SessionID") %>' /></ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="140px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
