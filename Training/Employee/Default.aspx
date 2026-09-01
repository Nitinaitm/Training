<%@ Page Title=""
Language="C#"
MasterPageFile="~/EmpMaster.Master"
AutoEventWireup="true"
CodeBehind="Default.aspx.cs"
Inherits="Training.Employee.Default" %>


<asp:Content ID="Content1"
ContentPlaceHolderID="head"
runat="server">

<style>

.welcome-box{
background:#ffffff;
margin:40px auto;
padding:35px;
border-radius:15px;
box-shadow:0px 0px 15px rgba(0,0,0,0.1);
border-left:6px solid #17a2b8;
}

.welcome-title{
font-size:30px;
font-weight:bold;
color:#17a2b8;
margin-bottom:20px;
}

.welcome-text{
font-size:18px;
line-height:1.9;
color:#555;
text-align:justify;
}

.highlight{
font-weight:bold;
color:#007bff;
}

</style>

</asp:Content>



<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container">

<div class="welcome-box">

<div class="welcome-title">

Welcome to Training Portal

</div>


<div class="welcome-text">

Welcome to the
<span class="highlight">
Training Portal
</span>.

Employees can participate in various programs such as
<span class="highlight">
Induction Training
</span>,
<span class="highlight">
Refresher Training
</span>,
<span class="highlight">
Skill Development Programs
</span>,
<span class="highlight">
Workshops
</span>
and other official learning initiatives.

<br/><br/>

Employees can submit training-related feedback through this portal. Those who have not yet completed their iGOT registration can also provide their details for registration and further processing. Please keep your information updated and regularly check training-related notifications and instructions.
</div>

</div>

</div>

</asp:Content>