<%@ Page Language="C#"
MasterPageFile="~/EmpMaster.Master"
AutoEventWireup="true"
CodeBehind="FeedbackReportTopic.aspx.cs"
Inherits="Training.Employee.FeedbackReportTopic" %>


<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
rel="stylesheet"/>

<style>

.card-box{
background:#fff;
padding:25px;
border-radius:10px;
box-shadow:0px 0px 10px #ddd;
margin-top:20px;
margin-bottom:20px;
}

.heading{
font-size:28px;
font-weight:bold;
margin-bottom:20px;
color:darkcyan;
}

</style>

</asp:Content>



<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">


<div class="container">

<div class="card-box">

<div class="heading">

Feedback Report Entry

</div>

<hr/>


<div class="row">

<div class="col-md-4">

<label>
Training
</label>

<asp:DropDownList
ID="ddlTraining"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnSelectedIndexChanged=
"ddlTraining_SelectedIndexChanged">

</asp:DropDownList>

</div>

</div>


<br/>


<div style="overflow:auto">

<asp:GridView
ID="gvTopic"
runat="server"
AutoGenerateColumns="false"
CssClass="table table-bordered"
OnRowDeleting="gvTopic_RowDeleting">

<Columns>

<asp:TemplateField
HeaderText="Topic">

<ItemTemplate>

<asp:TextBox
ID="txtTopic"
runat="server"
Text='<%# Eval("Topic") %>'
CssClass="form-control">

</asp:TextBox>

</ItemTemplate>

</asp:TemplateField>



<asp:TemplateField
HeaderText="Report">

<ItemTemplate>

<asp:TextBox
ID="txtReport"
runat="server"
Text='<%# Eval("Report") %>'
TextMode="MultiLine"
Rows="2"
CssClass="form-control">

</asp:TextBox>

</ItemTemplate>

</asp:TemplateField>


<asp:CommandField
ShowDeleteButton="true"/>

</Columns>

</asp:GridView>

</div>


<br/>


<asp:Button
ID="btnAdd"
runat="server"
Text="Add More"
CssClass="btn btn-info"
OnClick="btnAdd_Click"/>


<asp:Button
ID="btnSave"
runat="server"
Text="Save"
CssClass="btn btn-success"
OnClick="btnSave_Click"/>


<br/><br/>


<asp:Label
ID="lblMsg"
runat="server">

</asp:Label>


<hr/>


<h4>

Submitted Topic Feedback

</h4>


<div style="overflow:auto">

<asp:GridView
ID="gvSubmitted"
runat="server"
AutoGenerateColumns="false"
CssClass="table table-bordered table-striped">

<Columns>

<asp:BoundField
DataField="Topic"
HeaderText="Topic"/>

<asp:BoundField
DataField="Report"
HeaderText="Report"/>

<asp:BoundField
DataField="CreatedOn"
HeaderText="Submitted On"
DataFormatString="{0:dd-MM-yyyy}" />

</Columns>

</asp:GridView>

</div>

</div>

</div>

</asp:Content>