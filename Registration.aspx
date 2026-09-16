<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Training.Registration" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>BSPHCL Seminar Registration</title>

    <meta charset="utf-8" />

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />

    <!-- Bootstrap -->

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <!-- Bootstrap Icons -->

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css"
        rel="stylesheet" />

    <style>

        body {

            background:#eef2f7;

            font-family:'Segoe UI';

        }

        .top-header{

            background:#0d6efd;

            color:white;

            padding:18px;

            text-align:center;

            font-size:28px;

            font-weight:600;

        }

        .sub-title{

            font-size:15px;

            opacity:.9;

        }

        .main-card{

            background:white;

            border-radius:15px;

            box-shadow:0 5px 20px rgba(0,0,0,.15);

            margin-top:30px;

            margin-bottom:30px;

            padding:30px;

        }

        .section-title{

            color:#0d6efd;

            font-size:22px;

            font-weight:600;

            margin-bottom:20px;

        }

        .form-label{

            font-weight:600;

        }

        .btn-main{

            background:#0d6efd;

            color:white;

            border:none;

            border-radius:8px;

            padding:10px 30px;

        }

        .btn-main:hover{

            background:#084298;

            color:white;

        }

        .btn-success2{

            background:#198754;

            color:white;

            border:none;

            border-radius:8px;

            padding:10px 30px;

        }
        .rblType label{
    margin-right:30px;
    font-weight:600;
    cursor:pointer;
}

.rblType input[type=radio]{
    margin-right:8px;
}
        .btn-success2:hover{

            background:#157347;

            color:white;

        }

        .readonly{

            background:#f8f9fa !important;

        }

        .message{

            font-weight:600;

            font-size:16px;

        }

        .card-title2{

            background:#0d6efd;

            color:white;

            padding:10px;

            border-radius:8px;

            margin-bottom:20px;

            font-size:18px;

        }

        @media(max-width:768px){

            .main-card{

                padding:20px;

                margin:10px;

            }

            .top-header{

                font-size:22px;

            }

            .btn-main,.btn-success2{

                width:100%;

                margin-top:10px;

            }

        }

    </style>

</head>

<body>

<form id="form1" runat="server">

<div class="top-header">

    Seminar Registration

    <div class="sub-title">

        Bihar State Power Holding Company Limited

    </div>

</div>

<div class="container">

<div class="main-card">

<div class="card-title2">

    Select Trainee Type

</div>

<div class="row mb-4">



<asp:RadioButtonList
    ID="rblType"
    runat="server"
    CssClass="rblType"
    RepeatDirection="Horizontal"
    AutoPostBack="true"
    OnSelectedIndexChanged="rblType_SelectedIndexChanged">

    <asp:ListItem
        Value="Internal"
        Selected="True">
        BSPHCL &amp; Subsidiary Employee
    </asp:ListItem>
  
    <asp:ListItem
        Value="External">
        External Trainee
    </asp:ListItem>

</asp:RadioButtonList>



</div>

<!-- INTERNAL PANEL START -->

<asp:Panel
    ID="pnlInternal"
    runat="server">

<div class="card-title2">

Internal Employee

</div>

<div class="row">

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Employee ID

</label>

<asp:TextBox
    ID="txtSearchEmpID"
    runat="server"
    CssClass="form-control"
    placeholder="Enter Employee ID">
</asp:TextBox>

</div>

<div class="col-lg-2 col-md-6 mb-3 d-flex align-items-end">

<asp:Button
    ID="btnView"
    runat="server"
    Text="View"
    CssClass="btn btn-main"
    OnClick="btnView_Click"/>

</div>

</div>
    </asp:Panel>
<asp:Panel
    ID="pnlEmployeeDetails"
    runat="server"
    Visible="false">

<hr />


<div class="row">

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Employee ID

</label>

<asp:TextBox
    ID="txtIEmpID"
    runat="server"
    ReadOnly="true"
    CssClass="form-control readonly">
</asp:TextBox>

</div>

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Employee Name

</label>

<asp:TextBox
    ID="txtIName"
    runat="server"
    ReadOnly="true"
    CssClass="form-control readonly">
</asp:TextBox>

</div>

    <div class="col-lg-4 col-md-6 mb-3">

    <label class="form-label">

        Designation

    </label>

    <asp:TextBox
        ID="txtIDesignation"
        runat="server"
        ReadOnly="true"
        CssClass="form-control readonly">
    </asp:TextBox>

</div>

<div class="col-lg-4 col-md-6 mb-3">

    <label class="form-label">

        Organization

    </label>

    <asp:TextBox
        ID="txtICompany"
        runat="server"
        ReadOnly="true"
        CssClass="form-control readonly">
    </asp:TextBox>

</div>

<div class="col-lg-4 col-md-6 mb-3">

    <label class="form-label">

        Mobile Number

    </label>

    <asp:TextBox
        ID="txtIMobile"
        runat="server"
        ReadOnly="true"
        CssClass="form-control readonly">
    </asp:TextBox>

</div>

<div class="col-lg-4 col-md-6 mb-3">

    <label class="form-label">

        Email ID

    </label>

    <asp:TextBox
        ID="txtIEmail"
        runat="server"
        ReadOnly="true"
        CssClass="form-control readonly">
    </asp:TextBox>

</div>

</div>

<div class="row mt-3">

    <div class="col-md-12 text-center">

        <asp:Button
            ID="btnAttendance"
            runat="server"
            Text="Register"
            CssClass="btn btn-success2 btn-lg"
            OnClick="btnAttendance_Click" />

    </div>

</div>

</asp:Panel>

<!-- ========================= -->

<!-- EXTERNAL PANEL -->

<!-- ========================= -->

<asp:Panel
    ID="pnlExternal"
    runat="server"
    Visible="false">

<div class="card-title2">

External Trainee Registration

</div>

    <div class="col-lg-4 col-md-6 mb-3">

<label class="form-label" style="visibility:hidden">

Employee ID

</label>

<asp:TextBox
    ID="txtEmpID"
    runat="server"
    Enabled="false" Visible="false"
    CssClass="form-control">
</asp:TextBox>

</div>
<div class="row">



<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Employee Name <span class="text-danger">*</span>

</label>

<asp:TextBox
    ID="txtEmpName"
    runat="server"
    CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
    ID="rfvEmpName"
    runat="server"
    ControlToValidate="txtEmpName"
    CssClass="text-danger"
    ErrorMessage="Required"
    ValidationGroup="Ext">
</asp:RequiredFieldValidator>

</div>

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Designation <span class="text-danger">*</span>

</label>

<asp:TextBox
    ID="txtDesignation"
    runat="server"
    CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
    ID="rfvDesignation"
    runat="server"
    ControlToValidate="txtDesignation"
    CssClass="text-danger"
    ErrorMessage="Required"
    ValidationGroup="Ext">
</asp:RequiredFieldValidator>

</div>

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Organization <span class="text-danger">*</span>

</label>

<asp:TextBox
    ID="txtOrganization"
    runat="server"
    CssClass="form-control">
</asp:TextBox>

<asp:RequiredFieldValidator
    ID="rfvOrganization"
    runat="server"
    ControlToValidate="txtOrganization"
    CssClass="text-danger"
    ErrorMessage="Required"
    ValidationGroup="Ext">
</asp:RequiredFieldValidator>

</div>

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Mobile Number <span class="text-danger">*</span>

</label>

<asp:TextBox
    ID="txtMobileNo"
    runat="server"
    CssClass="form-control"
    MaxLength="10">
</asp:TextBox>

<asp:RequiredFieldValidator
    ID="rfvMobile"
    runat="server"
    ControlToValidate="txtMobileNo"
    CssClass="text-danger"
    ErrorMessage="Required"
    ValidationGroup="Ext">
</asp:RequiredFieldValidator>

<asp:RegularExpressionValidator
    ID="revMobile"
    runat="server"
    ControlToValidate="txtMobileNo"
    ValidationExpression="^[0-9]{10}$"
    ErrorMessage="Invalid Mobile"
    CssClass="text-danger"
    ValidationGroup="Ext">
</asp:RegularExpressionValidator>

</div>

<div class="col-lg-4 col-md-6 mb-3">

<label class="form-label">

Email ID

</label>

<asp:TextBox
    ID="txtEmailId"
    runat="server"
    CssClass="form-control">
</asp:TextBox>
    <asp:RegularExpressionValidator
    ID="revEmail"
    runat="server"
    ControlToValidate="txtEmailId"
    ValidationGroup="Ext"
    CssClass="text-danger"
    Display="Dynamic"
    ValidationExpression="^$|^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,})+$"
    ErrorMessage="Please enter a valid Email ID.">
</asp:RegularExpressionValidator>
</div>

</div>
   

    <div class="row mt-4">

    <div class="col-md-12 text-center">

        <asp:Button
            ID="btnSave"
            runat="server"
            Text="Register"
            CssClass="btn btn-success2 btn-lg"
            ValidationGroup="Ext"
            OnClick="btnSave_Click" />

    </div>

</div>

</asp:Panel>

     <hr class="mt-4" />

<div class="row mt-3">

    <div class="col-md-12 text-center">

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="message">
        </asp:Label>

    </div>

</div>

<asp:ValidationSummary
    ID="ValidationSummary1"
    runat="server"
    ValidationGroup="Ext"
    CssClass="alert alert-danger mt-3"
    ShowMessageBox="false"
    ShowSummary="true" />

</div>

</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</form>

</body>

</html>