<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="MySessions.aspx.cs" Inherits="Training.Trainer.MySessions" %>

<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>
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

        .btn-action {
            min-width: 100px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">My Sessions</div>
        <uc:TrainingSummary ID="TrainingSummary1" runat="server" OnTrainingSelected="TrainingSummary1_TrainingSelected" />
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" OnRowCommand="gvSessions_RowCommand" OnRowDataBound="gvSessions_RowDataBound" DataKeyNames="SessionID">
                        <Columns>
                            <asp:TemplateField HeaderText="Sl No">
                                <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="TrainingID" HeaderText="Training" />
                            <asp:BoundField DataField="Batch" HeaderText="Batch" />
                            <asp:BoundField DataField="SessionNo" HeaderText="Session No" />
                            <asp:BoundField DataField="SessionName" HeaderText="Session" />
                            <asp:BoundField DataField="TopicName" HeaderText="Topic" />
                            <asp:BoundField DataField="SessionDate" HeaderText="Date" />
                            <asp:BoundField DataField="SessionTime" HeaderText="Time" />
                            <asp:TemplateField HeaderText="Attendance">
                                <ItemTemplate>
                                    <asp:Label ID="lblAttendance" runat="server" Text='<%# Eval("AttendanceStatus") %>' CssClass='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "badge bg-success" : "badge bg-warning text-dark" %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnAction" runat="server" Text='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "View" : "Take Attendance" %>' CssClass='<%# Eval("AttendanceStatus").ToString()=="Completed" ? "btn btn-info btn-sm btn-action" : "btn btn-success btn-sm btn-action" %>' CommandName="ViewSession" CommandArgument='<%# Eval("SessionID") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="140px" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
