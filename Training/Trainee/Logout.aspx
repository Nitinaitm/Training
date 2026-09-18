<%@ Page Title="" Language="C#" MasterPageFile="~/TraineeMaster.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="Training.Trainee.Logout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script.js" type="text/javascript"></script>
     <script type="text/javascript">
         function DisableBackButton() {
             window.history.forward()
         }
         DisableBackButton();
         window.onload = DisableBackButton;
         window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
         window.onunload = function () { void (0) }
    </script>
     <script>
  function preventBack(){window.history.forward();}
  setTimeout("preventBack()", 0);
  window.onunload=function(){null};
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
