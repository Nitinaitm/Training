<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CertificatePreview.aspx.cs" Inherits="Training.Admin.CertificatePreview" MasterPageFile="~/AdminMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body{background:#f5f5f5}.toolbar{text-align:center;margin-bottom:20px}.certificate{position:relative;width:1123px;height:794px;margin:auto;background:#fff;background-repeat:no-repeat;background-position:center;background-size:100% 100%;border:1px solid #ccc;box-shadow:0 0 10px #999;overflow:hidden}.logo,.header,.title,.body,.footer,.signature{position:absolute}.logo{width:90px;height:90px;object-fit:contain}.header{left:80px;right:80px;text-align:center;font-weight:bold}.title{left:60px;right:60px;text-align:center;font-weight:bold}.body{left:90px;right:90px;text-align:center;line-height:35px}.footer{left:60px;right:60px;text-align:center}.signature{bottom:110px;width:220px;text-align:center}.signature img{width:180px;height:70px;object-fit:contain}.left{left:80px}.right{right:80px}.sign-name{font-weight:bold;margin-top:5px}.sign-designation{font-size:15px}.trainee-name{font-size:36px;font-weight:bold;display:block;margin-top:15px;margin-bottom:15px}.course-title{font-size:24px;font-weight:bold}@media print{body{background:#fff}.toolbar{display:none}.certificate{border:none;box-shadow:none;margin:0}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="toolbar"><asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" /></div>
<div id="divCertificate" runat="server" class="certificate">
<asp:Image ID="imgLogo" runat="server" CssClass="logo" />
<asp:Label ID="lblHeader" runat="server" CssClass="header" />
<asp:Label ID="lblTitle" runat="server" CssClass="title" />
<div id="divBody" runat="server" class="body">
This Certificate is proudly presented to<br /><br />
<asp:Label ID="lblEmployee" runat="server" CssClass="trainee-name" />
for successfully completing<br /><br />
<asp:Label ID="lblCourse" runat="server" CssClass="course-title" /><br /><br />
conducted from <asp:Label ID="lblDuration" runat="server" />
</div>
<div id="divLeftSignature" runat="server" class="signature left">
<asp:Image ID="imgLeftSignature" runat="server" /><div class="sign-name"><asp:Label ID="lblLeftName" runat="server" /></div><div class="sign-designation"><asp:Label ID="lblLeftDesignation" runat="server" /></div>
</div>
<div id="divRightSignature" runat="server" class="signature right">
<asp:Image ID="imgRightSignature" runat="server" /><div class="sign-name"><asp:Label ID="lblRightName" runat="server" /></div><div class="sign-designation"><asp:Label ID="lblRightDesignation" runat="server" /></div>
</div>
<asp:Label ID="lblFooter" runat="server" CssClass="footer" />
</div></div>
</asp:Content>