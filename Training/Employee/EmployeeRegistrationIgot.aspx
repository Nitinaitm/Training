<%@ Page Language="C#"
MasterPageFile="~/EmpMaster.Master"
AutoEventWireup="true"
CodeBehind="EmployeeRegistrationIgot.aspx.cs"
Inherits="Training.Employee.EmployeeRegistrationIgot" %>


<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<style>

.card-box{
background:#fff;
padding:25px;
margin-top:20px;
border-radius:10px;
box-shadow:0px 0px 10px #ddd;
}

.red{
color:red;
font-weight:bold;
}

.validation{
color:red;
font-size:12px;
display:block;
}

.note{
margin-top:20px;
padding:10px;
background:#fff8dc;
border-left:5px solid orange;
}

.select2-container{
width:100%!important;
}

.select2-selection--single{
height:34px!important;
border:1px solid #ced4da!important;
padding-top:3px!important;
}

</style>
<script>
window.onload = function () {

    alert(
        "📢 IMPORTANT NOTICE\n\n" +
        "If you have already completed your iGOT registration, no further action is required.\n\n" +
        "Only employees who have NOT registered on iGOT should submit their details for registration.\n\n" +
        "Please avoid duplicate registration requests."
    );

};
</script>

</asp:Content>




<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">


<div class="container">

<div class="card-box">

<h3>
Employee Registration
</h3>

<hr/>


<div class="row">


<div class="col-md-4 form-group">

<label>
Employee ID
</label>

<asp:TextBox
ID="txtEmpID"
runat="server"
ReadOnly="true"
CssClass="form-control">
</asp:TextBox>

</div>




<div class="col-md-4 form-group">

<label>
Full Name
<span class="red">*</span>
</label>

<asp:TextBox
ID="txtName"
runat="server"
ReadOnly="true"
CssClass="form-control">
</asp:TextBox>

</div>




<div class="col-md-4 form-group">

<label>
DOB
<span class="red">*</span>
</label>

<asp:TextBox
ID="txtDOB"
runat="server"
ReadOnly="true"
CssClass="form-control">
</asp:TextBox>

</div>




<div class="col-md-4 form-group">

<label>
Email
<span class="red">*</span>
</label>

<asp:TextBox
ID="txtEmail"
runat="server"
CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
runat="server"
ControlToValidate="txtEmail"
ErrorMessage="Required"
CssClass="validation"/>

<asp:RegularExpressionValidator
runat="server"
ControlToValidate="txtEmail"
ValidationExpression="^\w+([-.+']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"
ErrorMessage="Invalid Email"
CssClass="validation"/>

</div>




<div class="col-md-4 form-group">

<label>
Mobile
<span class="red">*</span>
</label>

<asp:TextBox
ID="txtMobile"
runat="server"
MaxLength="10"
CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
runat="server"
ControlToValidate="txtMobile"
ErrorMessage="Required"
CssClass="validation"/>

<asp:RegularExpressionValidator
runat="server"
ControlToValidate="txtMobile"
ValidationExpression="^[0-9]{10}$"
ErrorMessage="Invalid Mobile"
CssClass="validation"/>

</div>




<div class="col-md-4 form-group">

<label>
Group
<span class="red">*</span>
</label>

<asp:DropDownList
ID="ddlGroup"
runat="server"
CssClass="form-control">

<asp:ListItem Value="">
--Select--
</asp:ListItem>

<asp:ListItem>Group A</asp:ListItem>
<asp:ListItem>Group B</asp:ListItem>
<asp:ListItem>Group C</asp:ListItem>
<asp:ListItem>Group D</asp:ListItem>

</asp:DropDownList>

<asp:RequiredFieldValidator
runat="server"
ControlToValidate="ddlGroup"
InitialValue=""
ErrorMessage="Select Group"
CssClass="validation"/>

</div>




<div class="col-md-4 form-group">

<label>
Designation
<span class="red">*</span>
</label>

<asp:DropDownList
ID="ddlDesignation"
runat="server"
ClientIDMode="Static"
CssClass="form-control">

</asp:DropDownList>

<asp:RequiredFieldValidator
runat="server"
ControlToValidate="ddlDesignation"
InitialValue=""
ErrorMessage="Select Designation"
CssClass="validation"/>

</div>




<div class="col-md-4 form-group">

<label>
Gender
</label>

<asp:DropDownList
ID="ddlGender"
runat="server"
CssClass="form-control">

<asp:ListItem>Male</asp:ListItem>
<asp:ListItem>Female</asp:ListItem>
<asp:ListItem>Other</asp:ListItem>

</asp:DropDownList>

</div>




<div class="col-md-4 form-group">

<label>
Category
</label>

<asp:TextBox
ID="txtCategory"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-4 form-group">

<label>
Mother Tongue
</label>

<asp:TextBox
ID="txtMother"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-4 form-group">

<label>
Office Pin Code
</label>

<asp:TextBox
ID="txtPin"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-4 form-group">

<label>
External System ID
</label>

<asp:TextBox
ID="txtExternalID"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-4 form-group">

<label>
External System Name
</label>

<asp:TextBox
ID="txtExternalName"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-12 form-group">

<label>
Tags
</label>

<asp:TextBox
ID="txtTags"
runat="server"
CssClass="form-control"/>

</div>




<div class="col-md-12">

<asp:Button
ID="btnSave"
runat="server"
Text="Save"
CssClass="btn btn-success"
OnClick="btnSave_Click"/>

</div>



<div class="col-md-12">

<br/>

<asp:Label
ID="lblMsg"
runat="server">
</asp:Label>

</div>



<div class="col-md-12">

<div class="note">

<b>Note:</b>
If any detail is unavailable then leave it blank.

</div>

</div>

</div>

</div>

</div>




<script>

function pageLoad()
{
    setTimeout(function(){

        try
        {
            if($('#ddlDesignation')
            .hasClass(
            "select2-hidden-accessible"))
            {
                $('#ddlDesignation')
                .select2(
                'destroy');
            }


            $('#ddlDesignation')
            .select2({

                width:'100%',

                placeholder:
                'Search Designation',

                allowClear:true

            });

        }
        catch(ex)
        {
            console.log(ex);
        }

    },500);
}

</script>


</asp:Content>