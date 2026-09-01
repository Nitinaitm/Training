<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="TestDetails.aspx.cs" Inherits="Training.Trainer.TestDetails" %>

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

        .badge-answer {
            font-size: 14px;
            padding: 8px 14px
        }

        .btn-back {
            min-width: 120px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Test Details</div>
        <div class="dashboard-card">
            <div class="card-header bg-info text-white">
                <h5 class="mb-0"><i class="fa fa-file-lines"></i>Test Information</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="info-box">
                            <div class="info-label">Test ID</div>
                            <asp:Label ID="lblTestID" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-4">
                        <div class="info-box">
                            <div class="info-label">Title</div>
                            <asp:Label ID="lblTitle" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-4">
                        <div class="info-box">
                            <div class="info-label">Training ID</div>
                            <asp:Label ID="lblTrainingID" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Duration (Minutes)</div>
                            <asp:Label ID="lblDuration" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Total Questions</div>
                            <asp:Label ID="lblTotalQuestions" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Passing %</div>
                            <asp:Label ID="lblPassing" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Status</div>
                            <asp:Label ID="lblStatus" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="fa fa-list"></i>Questions List</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Questions Found" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField HeaderText="#">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Question" HeaderText="Question" />
                        <asp:BoundField DataField="Type" HeaderText="Type" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:TemplateField HeaderText="Options">
                            <ItemTemplate><%# Eval("OptionA") + (string.IsNullOrEmpty(Eval("OptionA").ToString()) ? "" : "<br/>") + Eval("OptionB") + (string.IsNullOrEmpty(Eval("OptionB").ToString()) ? "" : "<br/>") + Eval("OptionC") + (string.IsNullOrEmpty(Eval("OptionC").ToString()) ? "" : "<br/>") + Eval("OptionD") %></ItemTemplate>
                            <ItemStyle Width="250px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Correct Answer">
                            <ItemTemplate><span class='<%# Eval("Answer").ToString() == "True" ? "badge bg-success" : Eval("Answer").ToString() == "False" ? "badge bg-danger" : "badge bg-primary" %> badge-answer'><%# Eval("Answer") %></span></ItemTemplate>
                            <ItemStyle Width="120px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Marks" HeaderText="Marks" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="text-center">
            <asp:Button ID="btnBack" runat="server" Text="Back to Tests" CssClass="btn btn-secondary btn-lg btn-back" OnClick="btnBack_Click" /></div>
    </div>
</asp:Content>
