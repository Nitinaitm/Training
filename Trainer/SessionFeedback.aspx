<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="SessionFeedback.aspx.cs" Inherits="Training.Trainer.SessionFeedback" %>

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

        .rating-star {
            color: #ffc107;
            font-size: 20px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Session Feedback</div>
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-4">
                    <label>Select Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged" /></div>
                <div class="col-md-3">
                    <label>From Date</label><asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="form-control" /></div>
                <div class="col-md-3">
                    <label>To Date</label><asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="form-control" /></div>
                <div class="col-md-2">
                    <br />
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></div>
            </div>
            <asp:GridView ID="gvFeedback" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Feedback Found">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                    <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                    <asp:BoundField DataField="Feedback" HeaderText="Feedback" />
                    <asp:TemplateField HeaderText="Rating">
                        <ItemTemplate><span class="rating-star"><%# GetStars(Eval("Rating")) %></span></ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedOn" HeaderText="Submitted On" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <script>function GetStars(rating) { var stars = ''; for (var i = 1; i <= 5; i++) { stars += (i <= rating) ? '★' : '☆'; } return stars; }</script>
</asp:Content>
