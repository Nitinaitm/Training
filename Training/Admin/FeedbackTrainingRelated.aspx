<%@ Page Title=""
Language="C#"
MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="FeedbackTrainingRelated.aspx.cs"
Inherits="Training.Admin.FeedbackTrainingRelated" %>

<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">
      <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
      rel="stylesheet" />
<style>

.main-card{
background:#fff;
padding:25px;
border-radius:10px;
box-shadow:0 0 10px #dcdcdc;
margin-top:20px;
margin-bottom:20px;
}

.page-heading{
font-size:28px;
font-weight:bold;
color:darkcyan;
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
Training Related Feedback
</div>


<ul class="nav nav-tabs"
id="myTab"
role="tablist">

<li class="nav-item">

<a
class="nav-link active"
id="single-tab"
data-bs-toggle="tab"
href="#singleentry"
role="tab">

Single Entry

</a>

</li>


<li class="nav-item">

<a
class="nav-link"
id="bulk-tab"
data-bs-toggle="tab"
href="#bulkentry"
role="tab">

Bulk Upload

</a>

</li>

</ul>



<div class="tab-content mt-4">


<!-- SINGLE -->

<div class="tab-pane fade show active"
id="singleentry"
role="tabpanel">

<div class="row">


<div class="col-lg-4 mb-3">

<label>Training</label>

<asp:DropDownList
ID="ddlTrainingID"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnSelectedIndexChanged="ddlTrainingID_SelectedIndexChanged">

</asp:DropDownList>

</div>




<div class="col-lg-4 mb-3">

<label>Employee</label>

<asp:DropDownList
ID="ddlEmpID"
runat="server"
CssClass="form-control"
AutoPostBack="true"
OnSelectedIndexChanged="ddlEmpID_SelectedIndexChanged">

</asp:DropDownList>

</div>




<div class="col-lg-4 mb-3">

<label>
Training Related Aspect
</label>

<asp:DropDownList
ID="ddlAspect"
runat="server"
CssClass="form-control">

</asp:DropDownList>

</div>




<div class="col-lg-4 mb-3">

<label>
Organized By
</label>

<asp:TextBox
ID="txtOrganizedBy"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>




<div class="col-lg-4 mb-3">

<label>
Grading
</label>

<asp:DropDownList
ID="ddlGrade"
runat="server"
CssClass="form-control">

<asp:ListItem>
--Select--
</asp:ListItem>

<asp:ListItem>1</asp:ListItem>
<asp:ListItem>2</asp:ListItem>
<asp:ListItem>3</asp:ListItem>
<asp:ListItem>4</asp:ListItem>
<asp:ListItem>5</asp:ListItem>

</asp:DropDownList>

</div>




<div class="col-lg-12 mb-3">

<label>Remarks</label>

<asp:TextBox
ID="txtRemarks"
runat="server"
TextMode="MultiLine"
Rows="4"
CssClass="form-control">

</asp:TextBox>

</div>




<div class="col-lg-12">

<asp:Button
ID="btnSave"
runat="server"
Text="Save Aspect Feedback"
CssClass="btn btn-success"
OnClick="btnSave_Click"/>

</div>




<div class="col-lg-12 mt-2">

<asp:Label
ID="lblSingleMessage"
runat="server">

</asp:Label>

</div>


<hr class="mt-4 mb-4"/>


<h4>

Overall Response and Suggestions

</h4>


<div class="col-lg-12 mb-3">

<asp:TextBox
ID="txtOverall"
runat="server"
TextMode="MultiLine"
Rows="6"
CssClass="form-control">

</asp:TextBox>

</div>



<div class="col-lg-12">

<asp:Button
ID="btnOverall"
runat="server"
Text="Save Overall Response"
CssClass="btn btn-dark"
OnClick="btnOverall_Click"/>

</div>




<div class="col-lg-12 mt-2">

<asp:Label
ID="lblOverall"
runat="server">

</asp:Label>

</div>


</div>

</div>




<!-- BULK -->


<div class="tab-pane fade"
id="bulkentry"
role="tabpanel">

<div class="row">


<h5>

Training Related Feedback Upload

</h5>


<div class="col-lg-6">

<label>
Upload Aspect Excel
</label>

<asp:FileUpload
ID="fuExcel"
runat="server"
CssClass="form-control"/>

</div>



<div class="col-lg-6 d-flex align-items-end">

<asp:Button
ID="btnUpload"
runat="server"
Text="Upload Aspect Feedback"
CssClass="btn btn-primary"
OnClick="btnUpload_Click"/>

</div>




<div class="col-12 mt-3">

<div class="alert alert-info">

Excel Format:

<br/><br/>

EmpID |
TrainingID |
TrainingRelatedAspects |
OrganizedBy |
Remarks |
Grading

</div>

</div>



<hr class="mt-4"/>


<h5>

Overall Response Upload

</h5>



<div class="col-lg-6">

<label>
Upload Overall Excel
</label>

<asp:FileUpload
ID="fuOverall"
runat="server"
CssClass="form-control"/>

</div>



<div class="col-lg-6 d-flex align-items-end">

<asp:Button
ID="btnOverallUpload"
runat="server"
Text="Upload Overall Response"
CssClass="btn btn-dark"
OnClick="btnOverallUpload_Click"/>

</div>




<div class="col-12 mt-3">

<div class="alert alert-info">

Excel Format:

<br/><br/>

EmpID |
TrainingID |
OverallResponse

</div>

</div>



<div class="col-lg-12">

<asp:Label
ID="lblBulkMessage"
runat="server">

</asp:Label>

</div>

</div>

</div>


</div>

</div>

</div>



<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

<script>

document.addEventListener(
'DOMContentLoaded',
function()
{
var triggerTabList=
[].slice.call(
document.querySelectorAll(
'#myTab a'));

triggerTabList.forEach(
function(triggerEl)
{
new bootstrap.Tab(
triggerEl);
});
});

</script>

</asp:Content>