<%@ Page Title=""
Language="C#"
MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="TopicEntry.aspx.cs"
Inherits="Training.Admin.TopicEntry" %>

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

.validation{
    color:red;
    font-size:13px;
}

.gridview th{
    background:#0d6efd;
    color:white;
    text-align:center;
}

</style>

</asp:Content>

<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">
Topic Master Entry
</div>

<div class="row">

<div class="col-md-4 mb-3">

<label>Topic Name *</label>

<asp:TextBox
ID="txtTopicName"
runat="server"
CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
ID="rfvTopic"
runat="server"
ControlToValidate="txtTopicName"
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage="Enter Topic Name">
</asp:RequiredFieldValidator>

</div>

<div class="col-md-4 mb-3">

<label>Category *</label>

<asp:DropDownList
ID="ddlCategory"
runat="server"
CssClass="form-select">

<asp:ListItem Text="Select Category" Value=""></asp:ListItem>
<asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
<asp:ListItem Text="Law" Value="Law"></asp:ListItem>
<asp:ListItem Text="Management" Value="Management"></asp:ListItem>
<asp:ListItem Text="IT" Value="IT"></asp:ListItem>
<asp:ListItem Text="Other" Value="Other"></asp:ListItem>

</asp:DropDownList>

<asp:RequiredFieldValidator
ID="rfvCategory"
runat="server"
ControlToValidate="ddlCategory"
InitialValue=""
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage="Select Category">
</asp:RequiredFieldValidator>

</div>

<div class="col-md-12 mb-3">

<label>Description</label>

<asp:TextBox
ID="txtDescription"
runat="server"
CssClass="form-control"
TextMode="MultiLine"
Rows="4">
</asp:TextBox>

</div>

<div class="col-md-12">

<asp:Button
ID="btnSave"
runat="server"
Text="Save Topic"
CssClass="btn btn-primary"
ValidationGroup="SaveGroup"
OnClick="btnSave_Click" />

</div>

<div class="col-md-12 mt-3">

<asp:Label
ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

<hr />

<asp:GridView
ID="gvTopic"
runat="server"
AutoGenerateColumns="False"
CssClass="table table-bordered table-striped gridview">

<Columns>

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