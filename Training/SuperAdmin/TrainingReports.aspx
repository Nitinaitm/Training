<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingReports.aspx.cs" Inherits="Training.SuperAdmin.TrainingReports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid mt-4">
<div class="card p-4 shadow">
<h3 class="mb-4 text-primary">Training Reports</h3>

<div class="row">

<div class="col-12 col-md-4 mb-3">
<label>Report Type</label>
<asp:DropDownList ID="ddlReport" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged">
<asp:ListItem Value="1">Training Master Report</asp:ListItem>
<asp:ListItem Value="2">Employee Training History</asp:ListItem>
<asp:ListItem Value="3">Attendance Summary</asp:ListItem>
<asp:ListItem Value="4">Pending Attendance</asp:ListItem>
<asp:ListItem Value="5">Never Assigned Employee</asp:ListItem>
</asp:DropDownList>
</div>

<div class="col-12 col-md-3 mb-3 d-flex align-items-end">
<asp:Button ID="btnLoad" runat="server" Text="Load Report" CssClass="btn btn-primary w-100" OnClick="btnLoad_Click"/>
</div>

<div class="col-12 col-md-3 mb-3 d-flex align-items-end">
<asp:Button ID="btnExcel" runat="server" Text="Export Excel" CssClass="btn btn-success w-100" OnClick="btnExcel_Click"/>
</div>

</div>

<div class="table-responsive mt-4">
<asp:GridView ID="gvReport" runat="server" CssClass="table table-bordered table-hover" AllowPaging="true" PageSize="20" OnPageIndexChanging="gvReport_PageIndexChanging"></asp:GridView>
</div>

<asp:Label ID="lblMsg" runat="server"></asp:Label>

</div>
</div>
</asp:Content>

