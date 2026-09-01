<%@ Page Title=""
Language="C#"
MasterPageFile="~/SuperAdminMaster.Master"
AutoEventWireup="true"
CodeBehind="CreateTraining.aspx.cs"
Inherits="Training.SuperAdmin.CreateTraining"
ClientIDMode="Static" %>

<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
rel="stylesheet"/>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

<link rel="stylesheet"
href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css"/>

<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
rel="stylesheet"/>

<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


<style>

body{
background:#f5f5f5;
}

.main-card{

background:#fff;
padding:25px;
border-radius:12px;
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

.select2-container{
width:100%!important;
}

.select2-container--default
.select2-selection--multiple{

min-height:38px!important;
border:1px solid #ced4da!important;

}

.form-select{

height:38px!important;

}

.select2-container
.select2-selection--single{

height:38px!important;
border:1px solid #ced4da!important;

}

.select2-selection__rendered{

line-height:36px!important;

}

.select2-selection__arrow{

height:36px!important;

}

</style>

</asp:Content>




<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">

Create Training

</div>

<div class="row">


<!-- Training ID -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Training ID

</label>

<asp:TextBox
ID="txtTrainingID"
runat="server"
CssClass="form-control"
ReadOnly="true">
</asp:TextBox>

</div>




<!-- Type -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Training Type *

</label>

<asp:DropDownList
ID="ddlTrainingType"
runat="server"
CssClass="form-select"
AutoPostBack="true"
OnSelectedIndexChanged=
"ddlTrainingType_SelectedIndexChanged">

</asp:DropDownList>


<asp:RequiredFieldValidator
ID="rfvType"
runat="server"
ControlToValidate=
"ddlTrainingType"
InitialValue=""
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Select Training Type">

</asp:RequiredFieldValidator>

</div>





<!-- Organizer -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Training Organizer *

</label>

<asp:DropDownList
ID="ddlTrainingOrganizer"
runat="server"
CssClass="form-select"
AutoPostBack="true"
OnSelectedIndexChanged=
"ddlTrainingOrganizer_SelectedIndexChanged">

</asp:DropDownList>


<asp:RequiredFieldValidator
ID="rfvOrganizer"
runat="server"
ControlToValidate=
"ddlTrainingOrganizer"
InitialValue=""
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Select Organizer">

</asp:RequiredFieldValidator>

</div>





<!-- Location -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Training Location *

</label>

<asp:DropDownList
ID="ddlTrainingLocation"
runat="server"
CssClass="form-select">

</asp:DropDownList>


<asp:RequiredFieldValidator
ID="rfvLocation"
runat="server"
ControlToValidate=
"ddlTrainingLocation"
InitialValue=""
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Select Location">

</asp:RequiredFieldValidator>

</div>





<!-- Designation -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Employee Designation *

</label>

<asp:ListBox
ID="lstDesignation"
runat="server"
CssClass="form-control"
SelectionMode="Multiple">

</asp:ListBox>

</div>





<!-- Batch -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Batch *

</label>

<asp:TextBox
ID="txtBatch"
runat="server"
CssClass="form-control">

</asp:TextBox>


<asp:RequiredFieldValidator
ID="rfvBatch"
runat="server"
ControlToValidate=
"txtBatch"
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Batch required">

</asp:RequiredFieldValidator>

</div>






<!-- Date From -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Date From *

</label>

<asp:TextBox
ID="txtDateFrom"
runat="server"
CssClass="form-control flatpickr"
placeholder="dd-mm-yyyy"
autocomplete="off"
onkeydown="return false;">
</asp:TextBox>


<asp:RequiredFieldValidator
ID="rfvDateFrom"
runat="server"
ControlToValidate=
"txtDateFrom"
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Date required">

</asp:RequiredFieldValidator>

</div>





<!-- Date To -->

<div class="col-lg-4 mb-3">

<label class="form-label">

Date To *

</label>

<asp:TextBox
ID="txtDateTo"
runat="server"
CssClass="form-control flatpickr"
placeholder="dd-mm-yyyy"
autocomplete="off"
onkeydown="return false;">
</asp:TextBox>


<asp:RequiredFieldValidator
ID="rfvDateTo"
runat="server"
ControlToValidate=
"txtDateTo"
ValidationGroup="SaveGroup"
CssClass="validation"
ErrorMessage=
"Date required">

</asp:RequiredFieldValidator>

</div>





<div class="col-12 mt-3">

<asp:Button
ID="btnSave"
runat="server"
Text="Create Training"
CssClass=
"btn btn-save"
ValidationGroup=
"SaveGroup"
OnClick=
"btnSave_Click"/>

</div>



<div class="col-12 mt-3">

<asp:Label
ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</div>

</div>



<script>

function initControls()
{
    // Flatpickr destroy + recreate

    document
    .querySelectorAll(".flatpickr")
    .forEach(function(el){

        if(el._flatpickr)
        {
            el._flatpickr.destroy();
        }

    });


    flatpickr(".flatpickr",{

        dateFormat:"d-m-Y",
        allowInput:false,
        clickOpens:true

    });



    if($('#ddlTrainingType').length)
    {
        if($('#ddlTrainingType')
        .hasClass(
        'select2-hidden-accessible'))
        {
            $('#ddlTrainingType')
            .select2('destroy');
        }

        $('#ddlTrainingType')
        .select2({
            width:'100%'
        });
    }



    if($('#ddlTrainingOrganizer').length)
    {
        if($('#ddlTrainingOrganizer')
        .hasClass(
        'select2-hidden-accessible'))
        {
            $('#ddlTrainingOrganizer')
            .select2('destroy');
        }

        $('#ddlTrainingOrganizer')
        .select2({
            width:'100%'
        });
    }



    if($('#ddlTrainingLocation').length)
    {
        if($('#ddlTrainingLocation')
        .hasClass(
        'select2-hidden-accessible'))
        {
            $('#ddlTrainingLocation')
            .select2('destroy');
        }

        $('#ddlTrainingLocation')
        .select2({
            width:'100%'
        });
    }



    if($('#lstDesignation').length)
    {
        if($('#lstDesignation')
        .hasClass(
        'select2-hidden-accessible'))
        {
            $('#lstDesignation')
            .select2('destroy');
        }

        $('#lstDesignation')
        .select2({

            placeholder:
            'Select Designation',

            width:'100%'

        });
    }

}



$(document)
.ready(function(){

initControls();

});


if(typeof(Sys)!=="undefined")
{
    Sys.Application.add_load(
    function(){

        initControls();

    });
}

</script>

</asp:Content>