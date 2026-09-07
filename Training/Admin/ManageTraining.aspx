<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="ManageTraining.aspx.cs" Inherits="Training.Admin.ManageTraining" %>
<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px}.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.info-box{margin-bottom:12px}.action-card{margin-top:20px;background:#fff;border:1px solid #dee2e6;border-radius:10px;padding:20px}.btn-action{min-width:180px;margin-right:10px;margin-bottom:10px}.status-badge{font-size:16px;padding:8px 15px}
.lifecycle{margin:20px 0 25px;padding:20px;border:1px solid #dee2e6;border-radius:12px;background:#f8f9fa}.lifecycle-title{font-size:20px;font-weight:700;margin-bottom:18px;text-align:center}.stage-scroll{overflow-x:auto;padding:8px 0 12px}.stage-line{display:flex;align-items:flex-start;min-width:1050px}.stage-item{flex:1;position:relative;text-align:center}.stage-item:not(:last-child):after{content:"";position:absolute;top:17px;left:50%;width:100%;height:4px;background:#dc3545;z-index:0}.stage-item.done:not(:last-child):after{background:#198754}.stage-bubble{position:relative;z-index:1;width:36px;height:36px;line-height:36px;border-radius:50%;margin:0 auto 8px;background:#dc3545;color:#fff;font-weight:700;border:3px solid #fff;box-shadow:0 0 0 1px #dc3545}.stage-item.done .stage-bubble{background:#198754;box-shadow:0 0 0 1px #198754}.stage-item.na .stage-bubble{background:#adb5bd;box-shadow:0 0 0 1px #adb5bd}.stage-label{font-size:12px;font-weight:600;line-height:1.25;padding:0 4px}.stage-state{font-size:10px;margin-top:3px;color:#dc3545}.stage-item.done .stage-state{color:#198754}.stage-item.na .stage-state{color:#6c757d}@media(max-width:768px){.main-card{padding:15px}.stage-scroll{margin-left:-5px;margin-right:-5px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Training Management</div>
<uc:trainingsummary id="TrainingSummary1" runat="server" />
<div class="col-md-3 info-box" runat="server" visible="false"><span class="summary-label">Status :</span><br /><asp:Label ID="lblStatus" runat="server" CssClass="badge bg-primary status-badge" /></div>
<div class="lifecycle"><div class="lifecycle-title">Batch Life Cycle</div><div class="stage-scroll"><asp:Literal ID="litBatchLifecycle" runat="server" /></div></div>
<div class="action-card text-center"><h5>Actions</h5><hr />
<asp:Button ID="btnUpdateTraining" runat="server" Text="Update Batch" CssClass="btn btn-warning btn-action" BackColor="Gray" BorderColor="Gray" OnClick="btnUpdateTraining_Click" />
<asp:Button ID="btnAssignSession" runat="server" Text="Assign Sessions & Trainers" CssClass="btn btn-warning btn-action" OnClick="btnAssignSession_Click" />
<asp:Button ID="btnAssignTrainee" runat="server" Text="Assign Trainee" CssClass="btn btn-primary btn-action" OnClick="btnAssignTrainee_Click" />
<asp:Button ID="btnCertificateTemplate" runat="server" Text="Certificate Template" CssClass="btn btn-primary btn-action" Enabled="false" OnClick="btnCertificateTemplate_Click" />
<asp:Button ID="btnAssignHostel" runat="server" Text="Assign Hostel" CssClass="btn btn-info btn-action" Visible="false" OnClick="btnAssignHostel_Click" />
<asp:Button ID="btnStartTraining" runat="server" Text="Start Training" CssClass="btn btn-success btn-action" Visible="false" OnClick="btnStartTraining_Click" />
<asp:Button ID="btnAttendance" runat="server" Text="Attendance" CssClass="btn btn-info btn-action" Visible="false" OnClick="btnAttendance_Click" />
</div>
<asp:Panel ID="pnlHostelConfirmation" runat="server" Visible="false" CssClass="card mt-3"><div class="card-header bg-warning text-dark"><b>Hostel Requirement</b></div><div class="card-body"><p>Is hostel accommodation required for trainees?</p><asp:Button ID="btnHostelYes" runat="server" Text="Yes" CssClass="btn btn-primary" CausesValidation="false" OnClick="btnHostelYes_Click" />&nbsp;<asp:Button ID="btnHostelNo" runat="server" Text="No" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnHostelNo_Click" /></div></asp:Panel>
<div class="mt-3"><asp:Label ID="lblMessage" runat="server" Font-Bold="true" /></div>
</div></div>
</asp:Content>
