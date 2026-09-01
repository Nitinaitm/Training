<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="TestResult.aspx.cs" Inherits="Training.Trainer.TestResult" %>

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

        .info-box {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 15px
        }

        .info-label {
            font-weight: bold;
            color: #0d6efd
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

        .status-badge {
            font-size: 14px;
            padding: 6px 12px
        }

        .btn-back {
            min-width: 120px
        }

        .summary-value {
            font-size: 28px;
            font-weight: bold
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Test Results</div>
        <div class="row">
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Total Trainees</div>
                    <div class="summary-value">
                        <asp:Label ID="lblTotal" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Passed</div>
                    <div class="summary-value">
                        <asp:Label ID="lblPassed" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Failed</div>
                    <div class="summary-value">
                        <asp:Label ID="lblFailed" runat="server" Text="0" /></div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-box">
                    <div class="summary-title">Avg Score</div>
                    <div class="summary-value">
                        <asp:Label ID="lblAvgScore" runat="server" Text="0%" /></div>
                </div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-info text-white">
                <h5 class="mb-0"><i class="fa fa-file-lines"></i>Test Information</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Test ID</div>
                            <asp:Label ID="lblTestID" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Title</div>
                            <asp:Label ID="lblTitle" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Passing %</div>
                            <asp:Label ID="lblPassing" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Total Questions</div>
                            <asp:Label ID="lblQuestions" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="fa fa-users"></i>Trainee Results</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <label>Search</label><asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name or EmpID..." /></div>
                    <div class="col-md-3">
                        <label>Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem Value="Pass">Pass</asp:ListItem>
                            <asp:ListItem Value="Fail">Fail</asp:ListItem>
                        </asp:DropDownList></div>
                    <div class="col-md-5">
                        <br />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /><asp:Button ID="btnExport" runat="server" Text="Export Excel" CssClass="btn btn-success ms-1" OnClick="btnExport_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary ms-1" OnClick="btnReset_Click" /></div>
                </div>
                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Results Found" ShowHeaderWhenEmpty="true" OnRowDataBound="gvResults_RowDataBound"  OnRowCommand="gvResults_RowCommand"> 
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                        <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                        <asp:BoundField DataField="EmpDesignation" HeaderText="Designation" />
                        <asp:BoundField DataField="TotalQuestions" HeaderText="Total Questions" />
                        <asp:BoundField DataField="CorrectAnswers" HeaderText="Correct" />
                        <asp:BoundField DataField="Score" HeaderText="Score %" DataFormatString="{0:F2}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>' CssClass='<%# Eval("Status").ToString()=="Pass" ? "badge bg-success status-badge" : "badge bg-danger status-badge" %>'></asp:Label></ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted On" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" runat="server" Text="View Answers" CssClass="btn btn-info btn-sm" CommandName="View" CommandArgument='<%# Eval("ResultID") %>' /></ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="120px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="text-center">
            <asp:Button ID="btnBack" runat="server" Text="Back to Tests" CssClass="btn btn-secondary btn-lg btn-back" OnClick="btnBack_Click" /></div>
    </div>
</asp:Content>
