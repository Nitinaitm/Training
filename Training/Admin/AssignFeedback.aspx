<%@ Page Title="Assign Feedback" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AssignFeedback.aspx.cs" Inherits="Training.Admin.AssignFeedback" %>
<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>body{background:#f5f5f5}.page-title{font-size:24px;font-weight:600;color:#0d6efd}.card{border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,.10)}.chkCategory label{margin-left:8px;font-weight:500}.chkCategory td{padding:8px 12px}.info{background:#eef7ff;border-left:5px solid #0d6efd;padding:12px;border-radius:4px}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="row mb-3"><div class="col-md-12"><span class="page-title">Assign Feedback</span></div></div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="card mb-3"><div class="card-header bg-primary text-white"><b>Training Details</b></div><div class="card-body"><uc:TrainingSummary ID="ucTrainingSummary" runat="server" /></div></div>
<div class="card"><div class="card-header bg-primary text-white"><b>Select Feedback Categories</b></div><div class="card-body">
<div class="info mb-3">Select the feedback categories applicable for this training batch.<br/>During trainee feedback submission only these categories will be displayed.<br/>If <b>Trainer</b> category is selected, all assigned trainers will automatically appear.</div>
<asp:CheckBoxList ID="chkCategory" runat="server" CssClass="chkCategory" RepeatColumns="2" RepeatDirection="Vertical"></asp:CheckBoxList>
<hr />
<asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
<asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary ms-2" Visible="false" CausesValidation="false" OnClick="btnNext_Click" />
<asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="btn btn-secondary ms-2" CausesValidation="false" OnClick="btnCancel_Click" />
</div></div></div>
</asp:Content>