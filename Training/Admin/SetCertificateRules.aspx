<%@ Page Title="Certificate Rules" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SetCertificateRules.aspx.cs" Inherits="Training.Admin.SetCertificateRules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px}.heading{font-size:26px;font-weight:700;margin-bottom:20px}.rule-card{border:1px solid #dee2e6;border-radius:10px;padding:18px;margin-bottom:20px}.table td,.table th{vertical-align:middle}.rule-note{font-size:14px;color:#6c757d}.btn-action{margin:2px 4px 2px 0}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="heading">Certificate Rules</div>
<asp:Label ID="lblTraining" runat="server" CssClass="fw-bold" />
<div class="rule-card mt-3">
<h5>Attendance Certificate Rule</h5>
<p class="rule-note">Minimum attendance is calculated as Present sessions divided by all attendance-required and non-skipped sessions. Example: 10 applicable sessions and 30% rule means at least 3 Present sessions.</p>
<div class="row align-items-end">
<div class="col-md-4">
<label class="form-label">Minimum Attendance Percentage</label>
<asp:TextBox ID="txtMinimumAttendance" runat="server" CssClass="form-control" TextMode="Number" />
</div>
<div class="col-md-4">
<asp:Button ID="btnSaveAttendance" runat="server" Text="Save Attendance Rule" CssClass="btn btn-primary btn-action" OnClick="btnSaveAttendance_Click" />
</div>
<div class="col-md-4">
<asp:Label ID="lblAttendanceStatus" runat="server" CssClass="fw-bold" />
</div>
</div>
</div>

<div class="rule-card">
<h5>Session-wise Pre-Test / Post-Test Certificate Rules</h5>
<p class="rule-note">Only Required and non-skipped sessions need a rule. PASS means the trainee must pass that session test. ALL means the trainee may pass or fail after submitting the test.</p>
<asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" DataKeyNames="SessionID" OnRowCommand="gvSessions_RowCommand">
<Columns>
<asp:BoundField DataField="SessionID" HeaderText="Session" />
<asp:BoundField DataField="SessionNo" HeaderText="Session No." />
<asp:BoundField DataField="SessionName" HeaderText="Session Name" />
<asp:TemplateField HeaderText="Pre-Test Rule">
<ItemTemplate>
<asp:Label ID="lblPreState" runat="server" Text='<%# Eval("PreState") %>' CssClass="badge bg-secondary" />
<asp:DropDownList ID="ddlPreRule" runat="server" CssClass="form-select mt-2" Visible='<%# Convert.ToBoolean(Eval("PreApplicable")) %>'>
<asp:ListItem Value="">Select Rule</asp:ListItem>
<asp:ListItem Value="PASS">PASS - Certificate only if passed</asp:ListItem>
<asp:ListItem Value="ALL">ALL - Pass or Fail accepted</asp:ListItem>
</asp:DropDownList>
<asp:Button ID="btnSavePre" runat="server" Text="Save Pre Rule" CommandName="SavePre" CommandArgument='<%# Eval("SessionID") %>' CssClass="btn btn-sm btn-outline-primary mt-2" Visible='<%# Convert.ToBoolean(Eval("PreApplicable")) %>' />
</ItemTemplate>
</asp:TemplateField>
<asp:TemplateField HeaderText="Post-Test Rule">
<ItemTemplate>
<asp:Label ID="lblPostState" runat="server" Text='<%# Eval("PostState") %>' CssClass="badge bg-secondary" />
<asp:DropDownList ID="ddlPostRule" runat="server" CssClass="form-select mt-2" Visible='<%# Convert.ToBoolean(Eval("PostApplicable")) %>'>
<asp:ListItem Value="">Select Rule</asp:ListItem>
<asp:ListItem Value="PASS">PASS - Certificate only if passed</asp:ListItem>
<asp:ListItem Value="ALL">ALL - Pass or Fail accepted</asp:ListItem>
</asp:DropDownList>
<asp:Button ID="btnSavePost" runat="server" Text="Save Post Rule" CommandName="SavePost" CommandArgument='<%# Eval("SessionID") %>' CssClass="btn btn-sm btn-outline-primary mt-2" Visible='<%# Convert.ToBoolean(Eval("PostApplicable")) %>' />
</ItemTemplate>
</asp:TemplateField>
</Columns>
</asp:GridView>
</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true" />
</div>
</div>
</asp:Content>