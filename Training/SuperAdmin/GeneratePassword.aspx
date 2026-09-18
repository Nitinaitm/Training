<%@ Page Title="" Language="C#"
    MasterPageFile="~/SuperAdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="GeneratePassword.aspx.cs"
    Inherits="Training.SuperAdmin.GeneratePassword" %>


<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        .password-card{
height:100%;
background:white;
padding:30px;
border-radius:15px;
box-shadow:0px 2px 12px rgba(0,0,0,.15);
margin-bottom:20px;
}

        .title {
            font-size: 28px;
            font-weight: 600;
            text-align: center;
            color: #0d6efd;
            margin-bottom: 25px;
        }

        .result {
            font-size: 20px;
            font-weight: bold;
            color: green;
            padding: 10px;
            display: block;
            margin-top: 15px;
        }
    </style>

</asp:Content>



<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

   <div class="container mt-4">

<div class="row">

<!-- Generate Password -->

<div class="col-md-6">

<div class="password-card">

<div class="title">

Password Generator

</div>


<div class="form-group">

<label>

Enter Password

</label>

<asp:TextBox
ID="txtPassword"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>

<br/>


<div class="text-center">

<asp:Button
ID="btnGenerate"
runat="server"
Text="Generate"
CssClass="btn btn-primary"
OnClick="btnGenerate_Click"/>

</div>


<br/>


<div class="text-center">

<asp:Label
ID="lblPassword"
runat="server"
CssClass="result">
</asp:Label>

</div>

</div>

</div>



<!-- Decrypt Password -->

<div class="col-md-6">

<div class="password-card">

<div class="title">

Password Decrypt

</div>


<div class="form-group">

<label>

Enter Encrypted Password

</label>

<asp:TextBox
ID="txtEncPassword"
runat="server"
CssClass="form-control">
</asp:TextBox>

</div>


<br/>


<div class="text-center">

<asp:Button
ID="btnDecrypt"
runat="server"
Text="Decrypt"
CssClass="btn btn-success"
OnClick="btnDecrypt_Click"/>

</div>


<br/>


<div class="text-center">

<asp:Label
ID="lblDecryptedPassword"
runat="server"
CssClass="result">
</asp:Label>

</div>

</div>

</div>

</div>

</div>


</asp:Content>
