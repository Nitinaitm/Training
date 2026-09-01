<%@ Page Title=""
Language="C#"
MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="AllTopic.aspx.cs"
Inherits="Training.Admin.AllTopic" %>

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

.gridview th{
    background:#0d6efd;
    color:white;
    text-align:center;
}

.search-panel{
    background:#f8f9fa;
    padding:15px;
    border-radius:8px;
    margin-bottom:20px;
}

</style>

</asp:Content>

<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">
All Topics
</div>

<div class="search-panel">

<div class="row">

<div class="col-md-4">

<label>Topic Name</label>

<asp:TextBox
ID="txtTopicName"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<div class="col-md-4">

<label>Category</label>

<asp:DropDownList
ID="ddlCategory"
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

<div class="col-md-4">

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

<div class="mb-2">

<asp:Label
ID="lblCount"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

<asp:GridView
ID="gvTopic"
runat="server"
AutoGenerateColumns="False"
CssClass="table table-bordered table-striped gridview">

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

<asp:BoundField
DataField="CreatedOn"
HeaderText="Created On"
DataFormatString="{0:dd-MM-yyyy}" />

</Columns>

</asp:GridView>

</div>

</div>

</asp:Content>