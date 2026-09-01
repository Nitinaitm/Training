<%@ Page Language="C#"
MasterPageFile="~/EmpMaster.Master"
AutoEventWireup="true"
CodeBehind="FeedbackTrainingRelatedEmployee.aspx.cs"
Inherits="Training.Employee.FeedbackTrainingRelatedEmployee" %>


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

Training Related Feedback

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



<div class="col-md-4">

<label>
Aspect
</label>

<asp:DropDownList
ID="ddlAspect"
runat="server"
CssClass="form-control">

</asp:DropDownList>

</div>

</div>


<br/>


<div class="row">

<div class="col-md-6">

<label>
Organized By
</label>

<asp:TextBox
ID="txtOrganizedBy"
runat="server"
CssClass="form-control">

</asp:TextBox>

</div>



<div class="col-md-6">

<label>
Grading
</label>

<asp:DropDownList
ID="ddlGrade"
runat="server"
CssClass="form-control">

<asp:ListItem Value="">
--Select--
</asp:ListItem>

<asp:ListItem>1</asp:ListItem>
<asp:ListItem>2</asp:ListItem>
<asp:ListItem>3</asp:ListItem>
<asp:ListItem>4</asp:ListItem>
<asp:ListItem>5</asp:ListItem>

</asp:DropDownList>

</div>

</div>


<br/>


<label>
Remarks
</label>

<asp:TextBox
ID="txtRemarks"
runat="server"
TextMode="MultiLine"
Rows="4"
CssClass="form-control">

</asp:TextBox>


<br/>


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

Submitted Feedback

</h4>


<div style="overflow:auto">

<asp:GridView
ID="gvFeedback"
runat="server"
AutoGenerateColumns="false"
CssClass="table table-bordered table-striped">

<Columns>

<asp:BoundField
DataField="TrainingRelatedAspects"
HeaderText="Aspect"/>

<asp:BoundField
DataField="OrganizedBy"
HeaderText="Organized By"/>

<asp:BoundField
DataField="Remarks"
HeaderText="Remarks"/>

<asp:BoundField
DataField="Grading"
HeaderText="Grading"/>

<asp:BoundField
DataField="CreatedOn"
HeaderText="Created On"
DataFormatString="{0:dd-MM-yyyy}" />

</Columns>

</asp:GridView>

</div>

</div>

</div>

</asp:Content>