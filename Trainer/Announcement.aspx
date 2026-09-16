<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="Announcement.aspx.cs" Inherits="Training.Trainer.Announcement" %>

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
        <div class="page-heading">Announcements</div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="fa fa-bullhorn"></i>New Announcement</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-8">
                        <label>Title *</label><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" /></div>
                    <div class="col-md-4">
                        <label>Target Audience</label><asp:DropDownList ID="ddlAudience" runat="server" CssClass="form-select">
                            <asp:ListItem Value="All">All Trainees</asp:ListItem>
                            <asp:ListItem Value="Batch">Specific Batch</asp:ListItem>
                            <asp:ListItem Value="Training">Specific Training</asp:ListItem>
                        </asp:DropDownList></div>
                </div>
                <div class="row mt-2">
                    <div class="col-md-12">
                        <label>Message *</label><asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" /></div>
                </div>
                <div class="mt-3">
                    <asp:Button ID="btnSend" runat="server" Text="Send Announcement" CssClass="btn btn-success" OnClick="btnSend_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary ms-2" OnClick="btnClear_Click" /><asp:Label ID="lblMessage" runat="server" Font-Bold="true" CssClass="ms-3" /></div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="fa fa-history"></i>Past Announcements</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvAnnouncements" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Announcements">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:BoundField DataField="Message" HeaderText="Message" />
                        <asp:BoundField DataField="Audience" HeaderText="Audience" />
                        <asp:BoundField DataField="CreatedOn" HeaderText="Sent On" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
