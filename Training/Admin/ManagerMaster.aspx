<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ManagerMaster.aspx.cs" Inherits="Training.Admin.ManagerMaster" ClientIDMode="Static" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        body { background:#f5f5f5; }
        .main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }
        .page-heading { font-size:28px; font-weight:bold; color:darkcyan; margin-bottom:20px; }
        .validation { color:red; font-size:13px; }
        .select2-container { width:100% !important; }
        .select2-container--default .select2-selection--single { height:38px !important; border:1px solid #ced4da !important; }
        .select2-selection__rendered { line-height:36px !important; }
        .select2-selection__arrow { height:36px !important; }
        .readonly-field { background:#f8f9fa; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">Manager Master</div>

            <div class="row">
                <div class="col-lg-4 mb-3">
                    <label class="form-label">EmpID *</label>
                    <asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtEmpID_TextChanged"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmpID" runat="server" ControlToValidate="txtEmpID" ValidationGroup="SaveGroup" CssClass="validation" ErrorMessage="Enter EmpID"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="card border-0 shadow-sm mb-4">
                <div class="card-header bg-primary text-white">Employee Details (from EmpBasicMaster)</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-lg-4 mb-3"><label class="form-label">Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">DOB</label><asp:TextBox ID="txtDOB" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">DOJ</label><asp:TextBox ID="txtDOJ" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">Mobile No</label><asp:TextBox ID="txtMobileNo" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">Email ID</label><asp:TextBox ID="txtEmailID" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">Place of Posting</label><asp:TextBox ID="txtPlaceOfPosting" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                        <div class="col-lg-4 mb-3"><label class="form-label">Designation</label><asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control readonly-field" ReadOnly="true"></asp:TextBox></div>
                    </div>
                </div>
            </div>

            <div class="card border-0 shadow-sm mb-4">
                <div class="card-header bg-primary text-white">Location Mapping</div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-lg-6 mb-3">
                            <label class="form-label">Map for Location *</label>
                            <asp:DropDownList ID="ddlMapForLocation" runat="server" CssClass="form-select searchable-dropdown"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvMapForLocation" runat="server" ControlToValidate="ddlMapForLocation" InitialValue="" ValidationGroup="SaveGroup" CssClass="validation" ErrorMessage="Select Map for Location"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-lg-6 mb-3">
                            <label class="form-label">Training Location *</label>
                            <asp:DropDownList ID="ddlTrainingLocation" runat="server" CssClass="form-select searchable-dropdown"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvTrainingLocation" runat="server" ControlToValidate="ddlTrainingLocation" InitialValue="" ValidationGroup="SaveGroup" CssClass="validation" ErrorMessage="Select Training Location"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Button ID="btnSave" runat="server" Text="Save Manager" CssClass="btn btn-success" ValidationGroup="SaveGroup" OnClick="btnSave_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary ms-2" CausesValidation="false" OnClick="btnClear_Click" />
            <br /><br />
            <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>

            <hr />

            <div class="row mb-3">
                <div class="col-lg-4">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search EmpID / Name / Location"></asp:TextBox>
                </div>
                <div class="col-lg-2">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" CausesValidation="false" OnClick="btnSearch_Click" />
                </div>
            </div>

            <asp:GridView ID="gvManager" runat="server" CssClass="table table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="ID"
                OnRowEditing="gvManager_RowEditing" OnRowUpdating="gvManager_RowUpdating" OnRowCancelingEdit="gvManager_RowCancelingEdit" OnRowDeleting="gvManager_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="Manager ID"><ItemTemplate><%# Eval("ID") %></ItemTemplate></asp:TemplateField>
                    <asp:BoundField DataField="EmpID" HeaderText="EmpID" ReadOnly="true" />
                    <asp:BoundField DataField="EmpName" HeaderText="Name" ReadOnly="true" />
                    <asp:BoundField DataField="Designation" HeaderText="Designation" ReadOnly="true" />
                    <asp:BoundField DataField="PlaceOfPosting" HeaderText="Place of Posting" ReadOnly="true" />
                    <asp:BoundField DataField="MapForLocation" HeaderText="Map for Location" />
                    <asp:BoundField DataField="TrainingLocation" HeaderText="Training Location" ReadOnly="true" />
                    <asp:BoundField DataField="CreatedOn" HeaderText="Created On" ReadOnly="true" DataFormatString="{0:dd-MM-yyyy}" />
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" ButtonType="Button" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <script>
        function initManagerSelect2() {
            $('.searchable-dropdown').each(function () {
                if ($(this).hasClass('select2-hidden-accessible')) {
                    $(this).select2('destroy');
                }
                $(this).select2({ width: '100%', placeholder: 'Select', allowClear: true });
            });
        }
        $(document).ready(function () { initManagerSelect2(); });
    </script>
</asp:Content>