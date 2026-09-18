<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="Training.Trainer.Notifications" %>

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

        .notification-unread {
            font-weight: bold;
            background: #f8f9fa
        }

        .notification-read {
            background: #fff
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Notifications</div>
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-12 text-end">
                    <asp:Button ID="btnMarkAllRead" runat="server" Text="Mark All as Read" CssClass="btn btn-success" OnClick="btnMarkAllRead_Click" /></div>
            </div>
            <asp:GridView ID="gvNotifications" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover" EmptyDataText="No Notifications">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Message">
                        <ItemTemplate><%# Eval("Message") %><%# Eval("IsRead").ToString()=="False" ? " <span class='badge bg-danger'>New</span>" : "" %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedOn" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkMarkRead" runat="server" Text="Mark Read" CssClass="btn btn-sm btn-primary" CommandName="MarkRead" CommandArgument='<%# Eval("NotificationID") %>' Visible='<%# Eval("IsRead").ToString()=="False" %>' /></ItemTemplate>
                        <ItemStyle Width="120px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
