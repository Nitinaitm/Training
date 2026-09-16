<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="AttendanceReport.aspx.cs" Inherits="Training.Trainer.AttendanceReport" %>

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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Attendance Report</div>
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-3">
                    <label>Training ID</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged" /></div>
                <div class="col-md-3">
                    <label>From Date</label><asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="form-control" /></div>
                <div class="col-md-3">
                    <label>To Date</label><asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="form-control" /></div>
                <div class="col-md-3">
                    <br />
                    <asp:Button ID="btnGenerate" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnGenerate_Click" /></div>
            </div>
            <div class="mt-3">
                <asp:Button ID="btnExportPDF" runat="server" Text="Export PDF" CssClass="btn btn-danger" OnClick="btnExportPDF_Click" /><asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success ms-2" OnClick="btnExportExcel_Click" /></div>
            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Data Found">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                    <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                    <asp:BoundField DataField="TotalSessions" HeaderText="Total Sessions" />
                    <asp:BoundField DataField="Present" HeaderText="Present" />
                    <asp:BoundField DataField="Absent" HeaderText="Absent" />
                    <asp:BoundField DataField="Percentage" HeaderText="Attendance %" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
