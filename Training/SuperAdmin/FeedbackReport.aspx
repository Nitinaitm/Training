<%@ Page Title=""
Language="C#"
MasterPageFile="~/SuperAdminMaster.Master"
AutoEventWireup="true"
CodeBehind="FeedbackReport.aspx.cs"
Inherits="Training.SuperAdmin.FeedbackReport" %>

<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<style>

.main-card{
background:white;
padding:25px;
border-radius:10px;
box-shadow:0px 0px 10px #d9d9d9;
margin-top:20px;
margin-bottom:20px;
}

.page-heading{
font-size:28px;
font-weight:bold;
color:darkcyan;
margin-bottom:20px;
}

.validation{
color:red;
font-size:13px;
}

.btn-save{
background:darkcyan;
color:white;
border:none;
}

.btn-save:hover{
background:teal;
color:white;
}

</style>

</asp:Content>



<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">
Feedback Report Entry
</div>


<ul class="nav nav-tabs">

<li class="nav-item">

<button
class="nav-link active"
data-bs-toggle="tab"
data-bs-target="#singleentry"
type="button">

Single Entry

</button>

</li>


<li class="nav-item">

<button
class="nav-link"
data-bs-toggle="tab"
data-bs-target="#bulkentry"
type="button">

Bulk Upload

</button>

</li>

</ul>



<div class="tab-content mt-4">


<!-- SINGLE ENTRY -->

<div class="tab-pane fade show active"
id="singleentry">

<div class="row">


<div class="col-lg-4 mb-3">

<label>
Training ID *
</label>

<asp:DropDownList
ID="ddlTrainingID"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnSelectedIndexChanged=
"ddlTrainingID_SelectedIndexChanged">

</asp:DropDownList>

</div>




<div class="col-lg-4 mb-3">

<label>
Employee *
</label>

<asp:DropDownList
ID="ddlEmpID"
runat="server"
CssClass="form-control">

</asp:DropDownList>

</div>




<div class="col-lg-4 mb-3">

<label>
Topic *
</label>

<asp:TextBox
ID="txtTopic"
runat="server"
CssClass="form-control">

</asp:TextBox>

</div>




<div class="col-lg-12 mb-3">

<label>
Report *
</label>

<asp:TextBox
ID="txtReport"
runat="server"
Rows="4"
TextMode="MultiLine"
CssClass="form-control">

</asp:TextBox>

</div>




<div class="col-12">

<asp:Button
ID="btnSave"
runat="server"
Text="Save"
CssClass="btn btn-success"
OnClick="btnSave_Click"/>

</div>



<div class="col-12 mt-3">

<asp:Label
ID="lblSingleMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</div>




<!-- BULK -->

<div class="tab-pane fade"
id="bulkentry">

<div class="row">


<div class="col-lg-6">

<label>

Upload Excel File

</label>

<asp:FileUpload
ID="fuExcel"
runat="server"
CssClass="form-control"/>

</div>



<div class="col-lg-6
d-flex
align-items-end">

<asp:Button
ID="btnUpload"
runat="server"
Text="Upload Excel"
CssClass="btn btn-primary"
OnClick="btnUpload_Click"
CausesValidation="false"/>

</div>



<div class="col-12 mt-4">

<div class="alert alert-info">

<b>
Excel Format:
</b>

<br/><br/>

EmpID |
TrainingID |
Topic |
Report

<br/><br/>

First row must contain headers

</div>

</div>



<div class="col-12">

<asp:Label
ID="lblBulkMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</div>


</div>

</div>

</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</asp:Content>