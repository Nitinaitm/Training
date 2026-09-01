<%@ Page Title=""
Language="C#"
MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="TopicManagement.aspx.cs"
Inherits="Training.Admin.TopicManagement" %>

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
Topic Management
</div>

<div class="search-panel">

<div class="row">

<div class="col-md-4">

<label>
Topic Name
</label>

<asp:TextBox
ID="txtSearchTopic"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-3">

<label>
Category
</label>

<asp:DropDownList
ID="ddlSearchCategory"
runat="server"
CssClass="form-select">

<asp:ListItem Text="All" Value=""></asp:ListItem>
<asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
<asp:ListItem Text="Law" Value="Law"></asp:ListItem>
<asp:ListItem Text="Management" Value="Management"></asp:ListItem>
<asp:ListItem Text="IT" Value="IT"></asp:ListItem>
<asp:ListItem Text="Other" Value="Other"></asp:ListItem>

</asp:DropDownList>

</div>

<div class="col-md-5">

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
ID="gvTopic"
runat="server"
AutoGenerateColumns="False"
CssClass="table table-bordered table-striped gridview"
OnRowCommand="gvTopic_RowCommand">

<Columns>

<asp:TemplateField HeaderText="Sl No">
<ItemTemplate>
<%# Container.DataItemIndex + 1 %>
</ItemTemplate>
</asp:TemplateField>

<asp:BoundField
DataField="TopicID"
HeaderText="Topic ID" />

<asp:BoundField
DataField="TopicName"
HeaderText="Topic Name" />

<asp:BoundField
DataField="Category"
HeaderText="Category" />

<asp:BoundField
DataField="Description"
HeaderText="Description" />

<asp:TemplateField HeaderText="Action">

<ItemTemplate>

<asp:LinkButton
ID="lnkEdit"
runat="server"
Text="Edit"
CssClass="btn btn-warning btn-sm"
CommandName="EditTopic"
CommandArgument='<%# Eval("ID") %>' />

&nbsp;

<asp:LinkButton
ID="lnkDelete"
runat="server"
Text="Delete"
CssClass="btn btn-danger btn-sm"
CommandName="DeleteTopic"
CommandArgument='<%# Eval("ID") %>'
OnClientClick="return confirm('Delete Topic?');" />

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
Edit Topic
</div>

<div class="edit-body">

<asp:HiddenField
ID="hfID"
runat="server" />

<div class="row">

<div class="col-md-6 mb-3">

<label>
Topic Name *
</label>

<asp:TextBox
ID="txtEditTopicName"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-6 mb-3">

<label>
Category *
</label>

<asp:DropDownList
ID="ddlEditCategory"
runat="server"
CssClass="form-select">

<asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
<asp:ListItem Text="Law" Value="Law"></asp:ListItem>
<asp:ListItem Text="Management" Value="Management"></asp:ListItem>
<asp:ListItem Text="IT" Value="IT"></asp:ListItem>
<asp:ListItem Text="Other" Value="Other"></asp:ListItem>

</asp:DropDownList>

</div>

</div>

<div class="row">

<div class="col-md-12">

<label>
Description
</label>

<asp:TextBox
ID="txtEditDescription"
runat="server"
CssClass="form-control"
TextMode="MultiLine"
Rows="4">
</asp:TextBox>

</div>

</div>

<div class="mt-4">

<asp:Button
ID="btnUpdate"
runat="server"
Text="Update Topic"
CssClass="btn btn-success"
OnClick="btnUpdate_Click" />

&nbsp;

<asp:Button
ID="btnCancel"
runat="server"
Text="Cancel"
CssClass="btn btn-secondary"
CausesValidation="false"
OnClick="btnCancel_Click" />

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