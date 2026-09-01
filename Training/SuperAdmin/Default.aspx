<%@ Page Title="" Language="C#"
MasterPageFile="~/SuperAdminMaster.Master"
AutoEventWireup="true"
CodeBehind="Default.aspx.cs"
Inherits="Training.SuperAdmin.Default" %>


<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<style>

*{
box-sizing:border-box;
}

body{
overflow-x:hidden;
}

.main-container{
width:100%;
padding:20px;
}

.search-card,
.grid-card{

background:#fff;
border-radius:12px;
padding:25px;
margin-bottom:25px;
box-shadow:0 2px 12px rgba(0,0,0,.08);

}

.page-title{

font-size:28px;
font-weight:600;
margin-bottom:25px;

}

.search-grid{

display:grid;
grid-template-columns:
repeat(7,1fr);

gap:15px;

}

.form-group{

display:flex;
flex-direction:column;

}

.form-group label{

font-weight:600;
margin-bottom:8px;

}

.textbox{

padding:12px;
border:1px solid #cbd5e1;
border-radius:8px;

}

.button-container{

margin-top:30px;
display:flex;
gap:15px;

}

.btn{

padding:10px 25px;
border:none;
border-radius:8px;
color:white;
cursor:pointer;

}

.btn-search{

background:#2563eb;

}

.btn-reset{

background:#64748b;

}

.grid-card{

overflow:auto;

}

.gridview{

width:100%;
border-collapse:collapse;
min-width:1200px;

}

.gridview th{

background:#2563eb;
color:white;
padding:12px;

}

.gridview td{

padding:10px;
border-bottom:1px solid #ddd;

}

.gridview tr:nth-child(even){

background:#f8fafc;

}

.multiselect-container{
position:relative;
}

.multiselect-content{

display:none;
position:absolute;
background:white;
border:1px solid #ddd;
z-index:99999;
max-height:250px;
overflow:auto;
padding:10px;
width:100%;

}

.multiselect-container.active
.multiselect-content{

display:block;

}

.select2-container{
width:220px!important;
}

.select2-selection--single{
height:35px!important;
padding-top:3px;
}

</style>

<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
rel="stylesheet"/>

</asp:Content>



<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="main-container">


<div class="search-card">

<div class="page-title">

Employee Search

</div>


<div class="search-grid">


<div class="form-group">

<label>Employee ID</label>

<asp:TextBox
ID="txtEmpID"
runat="server"
CssClass="textbox"/>

</div>


<div class="form-group">

<label>Employee Name</label>

<asp:TextBox
ID="txtEmpName"
runat="server"
CssClass="textbox"/>

</div>


<div class="form-group">

<label>Mobile No</label>

<asp:TextBox
ID="txtMobile"
runat="server"
CssClass="textbox"/>

</div>



<div class="form-group">

<label>Email ID</label>

<asp:TextBox
ID="txtEmail"
runat="server"
CssClass="textbox"/>

</div>



<div class="form-group">

<label>Designation</label>

<div class="multiselect-container"
id="designationBox">

<div class="textbox"
onclick="toggleMultiSelect('designationBox')">

Select Designation

</div>


<div class="multiselect-content">

<asp:CheckBoxList
ID="chkDesignation"
runat="server"/>

</div>

</div>

</div>



<div class="form-group">

<label>Company</label>

<div class="multiselect-container"
id="companyBox">

<div class="textbox"
onclick="toggleMultiSelect('companyBox')">

Select Company

</div>


<div class="multiselect-content">

<asp:CheckBoxList
ID="chkCompany"
runat="server"/>

</div>

</div>

</div>



<div class="form-group">

<label>Posting Place</label>

<div class="multiselect-container"
id="postingBox">

<div class="textbox"
onclick="toggleMultiSelect('postingBox')">

Select Posting Place

</div>


<div class="multiselect-content">

<asp:CheckBoxList
ID="chkPostingPlace"
runat="server"/>

</div>

</div>

</div>


</div>



<div class="button-container">

<asp:Button
ID="btnSearch"
runat="server"
Text="Search"
CssClass="btn btn-search"
OnClick="btnSearch_Click"/>


<asp:Button
ID="btnReset"
runat="server"
Text="Reset"
CssClass="btn btn-reset"
OnClick="btnReset_Click"/>

</div>

</div>




<div class="grid-card">

<asp:GridView
ID="gvEmployee"
runat="server"
AutoGenerateColumns="false"
CssClass="gridview"
DataKeyNames="ID"
OnRowEditing="gvEmployee_RowEditing"
OnRowCancelingEdit="gvEmployee_RowCancelingEdit"
OnRowUpdating="gvEmployee_RowUpdating"
OnRowDataBound="gvEmployee_RowDataBound">

<Columns>

<asp:TemplateField HeaderText="Sl No">

<ItemTemplate>

<%# Container.DataItemIndex+1 %>

</ItemTemplate>

</asp:TemplateField>


<asp:BoundField
DataField="EmpID"
HeaderText="Emp ID"
ReadOnly="true"/>


<asp:BoundField
DataField="EmpName"
HeaderText="Employee Name"
ReadOnly="true"/>


<asp:BoundField
DataField="MobileNo"
HeaderText="Mobile"
ReadOnly="true"/>


<asp:BoundField
DataField="EmailId"
HeaderText="Email"
ReadOnly="true"/>



<asp:TemplateField HeaderText="Company">

<ItemTemplate>

<%# Eval("EmpCompany") %>

</ItemTemplate>

<EditItemTemplate>

<asp:DropDownList
ID="ddlCompanyEdit"
runat="server"
CssClass="searchddl">
</asp:DropDownList>

</EditItemTemplate>

</asp:TemplateField>




<asp:TemplateField HeaderText="Designation">

<ItemTemplate>

<%# Eval("EmpDesignation") %>

</ItemTemplate>

<EditItemTemplate>

<asp:DropDownList
ID="ddlDesignationEdit"
runat="server"
CssClass="searchddl">
</asp:DropDownList>

</EditItemTemplate>

</asp:TemplateField>




<asp:TemplateField HeaderText="Posting Place">

<ItemTemplate>

<%# Eval("EmpPostingPlace") %>

</ItemTemplate>

<EditItemTemplate>

<asp:DropDownList
ID="ddlPostingEdit"
runat="server"
CssClass="searchddl">
</asp:DropDownList>

</EditItemTemplate>

</asp:TemplateField>



<asp:CommandField
ShowEditButton="true"
ButtonType="Button"
ControlStyle-CssClass=
"btn btn-search"/>

</Columns>

</asp:GridView>

</div>

</div>


<script>

function toggleMultiSelect(id){

document
.getElementById(id)
.classList
.toggle("active");

}


function pageLoad(){

setTimeout(function(){

$('.searchddl')
.select2({

width:'220px'

});

},300);

}


document
.addEventListener(
"click",

function(e){

var x=
document
.getElementsByClassName(
"multiselect-container");

for(
var i=0;
i<x.length;
i++
){

if(
!x[i]
.contains(
e.target))
{

x[i]
.classList
.remove(
"active");

}

}

});

</script>

</asp:Content>