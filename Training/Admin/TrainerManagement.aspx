<%@ Page Title=""
Language="C#"
MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="TrainerManagement.aspx.cs"
Inherits="Training.Admin.TrainerManagement" %>

<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
rel="stylesheet" />

<style>

.main-card{
    background:#fff;
    padding:25px;
    border-radius:12px;
    box-shadow:0 0 10px #d9d9d9;
    margin-top:20px;
}

.page-heading{
    font-size:28px;
    font-weight:bold;
    color:#0d6efd;
    margin-bottom:20px;
}

.search-panel{
    background:#f8f9fa;
    padding:15px;
    border-radius:8px;
    margin-bottom:20px;
}

.gridview th{
    background:#0d6efd;
    color:white;
    text-align:center;
}

.gridview td{
    vertical-align:middle;
}

.edit-panel{
    margin-top:25px;
    border:1px solid #dee2e6;
    border-radius:10px;
    overflow:hidden;
}

.edit-header{
    background:#ffc107;
    color:#000;
    padding:12px 20px;
    font-weight:bold;
    font-size:18px;
}

.edit-body{
    padding:20px;
    background:#fff;
}

</style>

</asp:Content>

<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">
Trainer Management
</div>

<div class="search-panel">

<div class="row">

<div class="col-md-3">

<label>
Trainer Type
</label>

<asp:DropDownList
ID="ddlSearchTrainerType"
runat="server"
CssClass="form-select">

<asp:ListItem Text="All" Value=""></asp:ListItem>
<asp:ListItem Text="Internal" Value="Internal"></asp:ListItem>
<asp:ListItem Text="External" Value="External"></asp:ListItem>

</asp:DropDownList>

</div>

<div class="col-md-3">

<label>
Employee ID
</label>

<asp:TextBox
ID="txtSearchEmpID"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<label>
Trainer Name
</label>

<asp:TextBox
ID="txtSearchName"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<br />

<asp:Button
ID="btnSearch"
runat="server"
Text="Search"
CssClass="btn btn-primary"
OnClick="btnSearch_Click" />

<asp:Button
ID="btnReset"
runat="server"
Text="Reset"
CssClass="btn btn-secondary"
OnClick="btnReset_Click" />

</div>

</div>

</div>

<asp:GridView
ID="gvTrainer"
runat="server"
AutoGenerateColumns="False"
CssClass="table table-bordered table-striped gridview"
OnRowCommand="gvTrainer_RowCommand">

<Columns>

<asp:TemplateField HeaderText="Sl No">
<ItemTemplate>
<%# Container.DataItemIndex + 1 %>
</ItemTemplate>
</asp:TemplateField>

<asp:BoundField
DataField="TrainerID"
HeaderText="Trainer ID" />

<asp:BoundField
DataField="TrainerType"
HeaderText="Trainer Type" />

<asp:BoundField
DataField="EmpID"
HeaderText="Employee ID" />

<asp:BoundField
DataField="TrainerName"
HeaderText="Trainer Name" />

<asp:BoundField
DataField="Designation"
HeaderText="Designation" />

<asp:BoundField
DataField="Organization"
HeaderText="Organization" />

<asp:BoundField
DataField="Remarks"
HeaderText="Remarks" />

<asp:TemplateField HeaderText="Action">

<ItemTemplate>

<asp:LinkButton
ID="lnkEdit"
runat="server"
Text="Edit"
CssClass="btn btn-warning btn-sm"
CommandName="EditTrainer"
CommandArgument='<%# Eval("ID") %>' />

&nbsp;

<asp:LinkButton
ID="lnkDelete"
runat="server"
Text="Delete"
CssClass="btn btn-danger btn-sm"
CommandName="DeleteTrainer"
CommandArgument='<%# Eval("ID") %>'
OnClientClick="return confirm('Delete Trainer?');" />

</ItemTemplate>

</asp:TemplateField>

</Columns>

</asp:GridView>

    <asp:Panel
ID="pnlEdit"
runat="server"
Visible="false"
CssClass="edit-panel">

<div class="edit-header">
Edit Trainer
</div>

<div class="edit-body">

<asp:HiddenField
ID="hfID"
runat="server" />

<asp:HiddenField
ID="hfTrainerType"
runat="server" />

<!-- INTERNAL EDIT PANEL -->

<asp:Panel
ID="pnlEditInternal"
runat="server"
Visible="false">

<div class="row">

<div class="col-md-4">

<label>
Employee ID
</label>

<asp:TextBox
ID="txtEditEmpID"
runat="server"
CssClass="form-control"
ReadOnly="true">
</asp:TextBox>

</div>

<div class="col-md-8">

<label>
Remarks
</label>

<asp:TextBox
ID="txtEditRemarksInternal"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

</div>

</asp:Panel>

<!-- EXTERNAL EDIT PANEL -->

<asp:Panel
ID="pnlEditExternal"
runat="server"
Visible="false">

<div class="row">

<div class="col-md-3">

<label>
Employee ID
</label>

<asp:TextBox
ID="txtEditEmpIDExternal"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<label>
Name
</label>

<asp:TextBox
ID="txtEditName"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<label>
Designation
</label>

<asp:TextBox
ID="txtEditDesignation"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<label>
Organization
</label>

<asp:TextBox
ID="txtEditOrganization"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

</div>

<div class="row mt-3">

<div class="col-md-12">

<label>
Remarks
</label>

<asp:TextBox
ID="txtEditRemarksExternal"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

</div>

</asp:Panel>

<div class="mt-4">

<asp:Button
ID="btnUpdate"
runat="server"
Text="Update Trainer"
CssClass="btn btn-success"
OnClick="btnUpdate_Click" />

&nbsp;

<asp:Button
ID="btnCancel"
runat="server"
Text="Cancel"
CssClass="btn btn-secondary"
OnClick="btnCancel_Click"
CausesValidation="false" />

</div>

<div class="mt-3">

<asp:Label
ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</asp:Panel>

</div>

</div>

</asp:Content>