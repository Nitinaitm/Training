<%@ Page Title="Manager Dashboard" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Training.Manager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .dashboard-card { background:#fff; border-radius:12px; padding:20px; margin-bottom:20px; box-shadow:0 2px 12px rgba(0,0,0,.08); }
        .page-title { font-size:28px; font-weight:600; color:#1e293b; margin-bottom:20px; }
        .info-label { font-size:13px; color:#64748b; font-weight:600; }
        .info-value { font-size:16px; color:#1e293b; font-weight:500; }
        .gridview th { background:#2563eb; color:#fff; padding:10px; white-space:nowrap; }
        .gridview td { padding:10px; white-space:nowrap; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <div class="page-title">Manager Dashboard</div>

        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-3 mb-3"><div class="info-label">Manager ID</div><div class="info-value"><asp:Label ID="lblManagerID" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Employee ID</div><div class="info-value"><asp:Label ID="lblEmpID" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Manager Name</div><div class="info-value"><asp:Label ID="lblName" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Designation</div><div class="info-value"><asp:Label ID="lblDesignation" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Posting</div><div class="info-value"><asp:Label ID="lblPosting" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Mapped Location</div><div class="info-value"><asp:Label ID="lblMapForLocation" runat="server"></asp:Label></div></div>
                <div class="col-md-3 mb-3"><div class="info-label">Training Location</div><div class="info-value"><asp:Label ID="lblTrainingLocation" runat="server"></asp:Label></div></div>
            </div>
        </div>

        <div class="dashboard-card">
            <h5 class="mb-3">Trainings for My Location</h5>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
            <div class="table-responsive">
                <asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Training Found" DataKeyNames="TrainingID">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                        <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                        <asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
                        <asp:BoundField DataField="TrainingOrganizer" HeaderText="Organizer" />
                        <asp:BoundField DataField="TrainingLocation" HeaderText="Location" />
                        <asp:BoundField DataField="Batch" HeaderText="Batch" />
                        <asp:BoundField DataField="DateFrom" HeaderText="From" />
                        <asp:BoundField DataField="DateTo" HeaderText="To" />
                        <asp:BoundField DataField="TrainingStatus" HeaderText="Status" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>