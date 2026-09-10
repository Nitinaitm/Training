<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="TestResult.aspx.cs" Inherits="Training.Trainer.TestResult" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.dashboard-card{background:#fff;border-radius:10px;box-shadow:0 0 10px #d9d9d9;padding:20px;margin-bottom:20px}.info-box{background:#f8f9fa;padding:15px;border-radius:8px;margin-bottom:15px}.info-label{font-weight:bold;color:#0d6efd}.gridview th{background:#198754;color:white;text-align:center;vertical-align:middle}.gridview td{vertical-align:middle}.status-badge{font-size:14px;padding:6px 12px}.summary-box{background:#f8f9fa;border-left:5px solid #198754;padding:15px;border-radius:8px;margin-bottom:15px;text-align:center}.summary-title{color:#666;font-size:15px}.summary-value{font-size:28px;font-weight:bold;color:#198754}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Test Results</div>
        <div class="row">
            <div class="col-md-3"><div class="summary-box"><div class="summary-title">Total Trainees</div><div class="summary-value"><asp:Label ID="lblTotal" runat="server" Text="0" /></div></div></div>
            <div class="col-md-3"><div class="summary-box"><div class="summary-title">Passed</div><div class="summary-value"><asp:Label ID="lblPassed" runat="server" Text="0" /></div></div></div>
            <div class="col-md-3"><div class="summary-box"><div class="summary-title">Failed</div><div class="summary-value"><asp:Label ID="lblFailed" runat="server" Text="0" /></div></div></div>
            <div class="col-md-3"><div class="summary-box"><div class="summary-title">Avg Score</div><div class="summary-value"><asp:Label ID="lblAvgScore" runat="server" Text="0%" /></div></div></div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-info text-white"><h5 class="mb-0"><i class="fa fa-file-lines"></i> Session Information</h5></div>
            <div class="card-body"><div class="row">
                <div class="col-md-3"><div class="info-box"><div class="info-label">Training ID</div><asp:Label ID="lblTrainingID" runat="server" CssClass="fs-5 fw-bold" /></div></div>
                <div class="col-md-3"><div class="info-box"><div class="info-label">Session ID</div><asp:Label ID="lblSessionID" runat="server" CssClass="fs-5 fw-bold" /></div></div>
                <div class="col-md-3"><div class="info-box"><div class="info-label">Topic</div><asp:Label ID="lblTitle" runat="server" CssClass="fs-5 fw-bold" /></div></div>
                <div class="col-md-3"><div class="info-box"><div class="info-label">Session Date</div><asp:Label ID="lblPassing" runat="server" CssClass="fs-5 fw-bold" /></div></div>
            </div></div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white"><h5 class="mb-0"><i class="fa fa-users"></i> Trainee-wise Pre &amp; Post Test Results</h5></div>
            <div class="card-body">
                <div class="row mb-3"><div class="col-md-5"><label>Search</label><asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name or EmpID..." /></div><div class="col-md-7"><br /><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /><asp:Button ID="btnExport" runat="server" Text="Export Excel" CssClass="btn btn-success ms-1" OnClick="btnExport_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary ms-1" OnClick="btnReset_Click" /></div></div>
                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Results Found" ShowHeaderWhenEmpty="true" OnRowDataBound="gvResults_RowDataBound" OnRowCommand="gvResults_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><ItemStyle Width="50px" HorizontalAlign="Center" /></asp:TemplateField>
                        <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                        <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                        <asp:BoundField DataField="EmpDesignation" HeaderText="Designation" />
                        <asp:TemplateField HeaderText="Pre-Test"><ItemTemplate><asp:Label ID="lblPreStatus" runat="server" Text='<%# Eval("PreStatus") %>' CssClass="badge status-badge"></asp:Label><br /><asp:Label ID="lblPreScore" runat="server" Text='<%# Eval("PreScore", "{0:F2}%") %>'></asp:Label><br /><asp:LinkButton ID="lnkPre" runat="server" Text="View" CssClass="btn btn-outline-primary btn-sm mt-1" CommandName="View" CommandArgument='<%# Eval("PreResultID") %>' Visible='<%# Eval("PreResultID") != DBNull.Value %>' /></ItemTemplate><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
                        <asp:TemplateField HeaderText="Post-Test"><ItemTemplate><asp:Label ID="lblPostStatus" runat="server" Text='<%# Eval("PostStatus") %>' CssClass="badge status-badge"></asp:Label><br /><asp:Label ID="lblPostScore" runat="server" Text='<%# Eval("PostScore", "{0:F2}%") %>'></asp:Label><br /><asp:LinkButton ID="lnkPost" runat="server" Text="View" CssClass="btn btn-outline-success btn-sm mt-1" CommandName="View" CommandArgument='<%# Eval("PostResultID") %>' Visible='<%# Eval("PostResultID") != DBNull.Value %>' /></ItemTemplate><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="text-center"><asp:Button ID="btnBack" runat="server" Text="Back to Session Details" CssClass="btn btn-secondary btn-lg" OnClick="btnBack_Click" /></div>
    </div>
</asp:Content>
