<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="AnswerDetails.aspx.cs" Inherits="Training.Trainer.AnswerDetails" %>

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

        .answer-correct {
            background: #d4edda !important
        }

        .answer-wrong {
            background: #f8d7da !important
        }

        .answer-selected {
            font-weight: bold
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Trainee Answer Details</div>
        <div class="dashboard-card">
            <div class="card-header bg-info text-white">
                <h5 class="mb-0"><i class="fa fa-user-graduate"></i>Trainee Information</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Employee ID</div>
                            <asp:Label ID="lblEmpID" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Employee Name</div>
                            <asp:Label ID="lblEmpName" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Designation</div>
                            <asp:Label ID="lblDesignation" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Status</div>
                            <asp:Label ID="lblStatus" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Test ID</div>
                            <asp:Label ID="lblTestID" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Total Questions</div>
                            <asp:Label ID="lblTotalQ" runat="server" CssClass="fs-5 fw-bold" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Correct Answers</div>
                            <asp:Label ID="lblCorrect" runat="server" CssClass="fs-5 fw-bold text-success" /></div>
                    </div>
                    <div class="col-md-3">
                        <div class="info-box">
                            <div class="info-label">Score</div>
                            <asp:Label ID="lblScore" runat="server" CssClass="fs-5 fw-bold text-primary" /></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="fa fa-list"></i>Question-wise Answers</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvAnswers" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Answers Found" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField HeaderText="#">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Question" HeaderText="Question" />
                        <asp:BoundField DataField="Type" HeaderText="Type" />
                        <%--<asp:TemplateField HeaderText="Options">
                            <ItemTemplate><%# GetOptions(Eval("OptionA"), Eval("OptionB"), Eval("OptionC"), Eval("OptionD"), Eval("SelectedAnswer"), Eval("CorrectAnswer")) %></ItemTemplate>
                            <ItemStyle Width="300px" />
                        </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="Selected Answer">
                            <ItemTemplate><span class='<%# Eval("IsCorrect").ToString() == "True" ? "badge bg-success" : "badge bg-danger" %> status-badge'><%# Eval("SelectedAnswer") %></span></ItemTemplate>
                            <ItemStyle Width="150px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Correct Answer">
                            <ItemTemplate><span class="badge bg-primary status-badge"><%# Eval("CorrectAnswer") %></span></ItemTemplate>
                            <ItemStyle Width="150px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Result">
                            <ItemTemplate><span class='<%# Eval("IsCorrect").ToString() == "True" ? "badge bg-success" : "badge bg-danger" %> status-badge'><%# Eval("IsCorrect").ToString() == "True" ? "Correct" : "Wrong" %></span></ItemTemplate>
                            <ItemStyle Width="100px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="text-center">
            <asp:Button ID="btnBack" runat="server" Text="Back to Results" CssClass="btn btn-secondary btn-lg btn-back" OnClick="btnBack_Click" /></div>
    </div>
    <script>function toggleAnswer(element) { var row = element.closest('tr'); var details = row.nextElementSibling; if (details && details.classList.contains('detail-row')) { details.style.display = details.style.display == 'none' ? 'table-row' : 'none'; } }</script>
</asp:Content>
