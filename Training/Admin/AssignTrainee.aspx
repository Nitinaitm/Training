<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="AssignTrainee.aspx.cs" MaintainScrollPositionOnPostback="true"
    Inherits="Training.Admin.AssignTrainee"
    ClientIDMode="Static" %>

<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/css/bootstrap-multiselect.min.css" />

    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/js/bootstrap-multiselect.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>



    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            margin-top: 20px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px;
        }



        .section-title {
            font-size: 20px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 15px;
        }

        .gridview th {
            background: #198754;
            color: #fff;
            text-align: center;
        }

        .gridview td {
            vertical-align: middle;
        }

        .mode-buttons {
            margin-bottom: 25px;
        }

        .btn-group {
            width: 100% !important;
        }

        .multiselect {
            text-align: left !important;
        }

        .multiselect-container {
            max-height: 300px;
            overflow-y: auto;
            width: 100% !important;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Assign Trainee

            </div>
            <uc:TrainingSummary ID="TrainingSummary1" runat="server" />
            <!-- ================= TRAINING SUMMARY ================= -->



            <!-- ================= MODE BUTTONS ================= -->

            <div class="mode-buttons">

                <asp:Button
                    ID="btnEmpWise"
                    runat="server"
                    Text="EmpID Wise"
                    CssClass="btn btn-primary"
                    OnClick="btnEmpWise_Click" />

                &nbsp;

                <asp:Button
                    ID="btnCompanyWise"
                    runat="server"
                    Text="Company Wise"
                    CssClass="btn btn-warning"
                    OnClick="btnCompanyWise_Click" />

                &nbsp;

                <asp:Button
                    ID="btnBulkWise"
                    runat="server"
                    Text="Bulk Upload"
                    CssClass="btn btn-success"
                    OnClick="btnBulkWise_Click" />

            </div>

            <!-- ================= EMPLOYEE WISE PANEL ================= -->

            <asp:Panel
                ID="pnlEmpWise"
                runat="server">

                <div class="section-title">
                    Employee Wise Assignment

                </div>

                <div class="row">

                    <div class="col-md-4">

                        <label class="form-label">
                            Employee ID

                        </label>

                        <asp:TextBox
                            ID="txtEmpID"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Enter Employee ID">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2">

                        <br />

                        <asp:Button
                            ID="btnAddEmployee"
                            runat="server"
                            Text="Add Employee"
                            CssClass="btn btn-success"
                            OnClick="btnAddEmployee_Click" />

                    </div>

                    <div class="col-md-6">

                        <br />

                        <asp:Label
                            ID="lblEmpMessage"
                            runat="server"
                            Font-Bold="true">
                        </asp:Label>

                    </div>

                </div>

                <hr />

             

            </asp:Panel>

            <!-- ================= BULK UPLOAD PANEL ================= -->

            <asp:Panel
                ID="pnlBulk"
                runat="server"
                Visible="false">

                <div class="section-title">
                    Bulk Upload Assignment

                </div>

                <div class="row">

                    <div class="col-md-3">

                        <asp:Button
                            ID="btnDownloadFormat"
                            runat="server"
                            Text="Download Sample Format"
                            CssClass="btn btn-info"
                            OnClick="btnDownloadFormat_Click" />

                    </div>

                </div>

                <br />

                <div class="row">

                    <div class="col-md-5">

                        <label class="form-label">
                            Select Excel File

                        </label>

                        <asp:FileUpload
                            ID="fuExcel"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-2">

                        <br />

                        <asp:Button
                            ID="btnUploadExcel"
                            runat="server"
                            Text="Upload & Assign"
                            CssClass="btn btn-success"
                            OnClick="btnUploadExcel_Click" />

                    </div>

                    <div class="col-md-5">

                        <br />

                        <asp:Label
                            ID="lblBulkMessage"
                            runat="server"
                            Font-Bold="true" />

                    </div>

                </div>

            </asp:Panel>

            <!-- ================= COMPANY WISE PANEL ================= -->


            <asp:Panel
                ID="pnlCompany"
                runat="server"
                Visible="false">

                <div class="section-title">
                    Company Wise Assignment

                </div>

                <div class="row">

                    <div class="col-md-4">

                        <label>
                            Company
            <span style="color: red">*</span>
                        </label>

                        <asp:ListBox
                            ID="lstCompany"
                            runat="server"
                            SelectionMode="Multiple"
                            CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="lstCompany_SelectedIndexChanged"></asp:ListBox>

                    </div>
                    <%-- <div class="col-md-4">
                        <asp:Button
                            ID="btnLoadFilter"
                            runat="server"
                            Text="Refresh Designation / Posting Place"
                            CssClass="btn btn-secondary"
                            OnClick="btnLoadFilter_Click" />

                        &nbsp;
                    </div>--%>
                </div>
                <hr />



                <div class="row">
                    <div class="col-md-6">

                        <label>
                            Designation
                        </label>

                        <asp:ListBox
                            ID="lstDesignation"
                            runat="server"
                            SelectionMode="Multiple"
                            CssClass="form-control" Style="height: 600px"></asp:ListBox>

                    </div>

                    <div class="col-md-6">

                        <label>
                            Posting Place
                        </label>

                        <asp:ListBox
                            ID="lstPostingPlace"
                            runat="server"
                            SelectionMode="Multiple"
                            CssClass="form-control" Style="height: 600px"></asp:ListBox>

                    </div>

                </div>

                <hr />

                <div class="row">
                    <div class="col-md-12 text-center">
                        <asp:Button
                            ID="btnLoadEmployee"
                            runat="server"
                            Text="Load Employees"
                            CssClass="btn btn-primary"
                            OnClick="btnLoadEmployee_Click" />

                        &nbsp;

        <asp:Button
            ID="btnSelectAll"
            runat="server"
            Text="Select All"
            CssClass="btn btn-info"
            OnClientClick="SelectAllEmployee();return false;" />

                        &nbsp;

        <asp:Button
            ID="btnClearSelection"
            runat="server"
            Text="Clear Selection"
            CssClass="btn btn-warning"
            OnClientClick="ClearEmployeeSelection();return false;" />

                        &nbsp;

        <asp:Button
            ID="btnAssignSelected"
            runat="server"
            Text="Assign Selected"
            CssClass="btn btn-success"
            OnClick="btnAssignSelected_Click" />

                        <br />
                        <br />

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Font-Bold="true"
                            ForeColor="Green">
                        </asp:Label>
                    </div>

                </div>

                
                <asp:GridView
                    ID="gvCompanyEmployee"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped gridview"
                    EmptyDataText="No Employee Found">

                    <Columns>

                        <asp:TemplateField HeaderText="Select">

                            <HeaderTemplate>

                                <input
                                    type="checkbox"
                                    onclick="ToggleAll(this);" />

                            </HeaderTemplate>

                            <ItemTemplate>

                                <asp:CheckBox
                                    ID="chkSelect"
                                    runat="server" />

                            </ItemTemplate>

                            <ItemStyle
                                HorizontalAlign="Center"
                                Width="70px" />

                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate>
                                <%#Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Emp ID">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblEmpID"
                                    runat="server"
                                    Text='<%# Eval("EmpID") %>' />

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:BoundField
                            DataField="EmpName"
                            HeaderText="Employee Name" />

                        <asp:BoundField
                            DataField="EmpDesignation"
                            HeaderText="Designation" />

                        <asp:BoundField
                            DataField="EmpCompany"
                            HeaderText="Company" />

                        <asp:BoundField
                            DataField="EmpPostingPlace"
                            HeaderText="Posting Place" />

                    </Columns>

                </asp:GridView>

            </asp:Panel>

            <hr />
              <div class="section-title" runat ="server" id="messageTotalAssigned">
                    Total Trainee Assigned 

                </div>
               <asp:GridView
                    ID="gvAssignedEmployee"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped gridview"
                    EmptyDataText="No Employee Assigned"
                    OnRowCommand="gvAssignedEmployee_RowCommand">

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">

                            <ItemTemplate>

                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>

                            <ItemStyle Width="60px"
                                HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:BoundField
                            DataField="EmpID"
                            HeaderText="Employee ID">

                            <ItemStyle Width="120px" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="EmpName"
                            HeaderText="Employee Name" />

                        <asp:BoundField
                            DataField="EmpDesignation"
                            HeaderText="Designation" />

                        <asp:BoundField
                            DataField="EmpCompany"
                            HeaderText="Company" />

                        <asp:BoundField
                            DataField="EmpPostingPlace"
                            HeaderText="Posting Place" />

                        <asp:TemplateField
                            HeaderText="Assignment Status">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblStatus"
                                    runat="server"
                                    Text='<%# Eval("AssignmentStatus") %>'
                                    CssClass="badge bg-success">
                                </asp:Label>

                            </ItemTemplate>

                            <ItemStyle HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:TemplateField
                            HeaderText="Action">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="lnkRemove"
                                    runat="server"
                                    Text="Remove"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="RemoveEmployee"
                                    CommandArgument='<%# Eval("AssignmentID") %>'
                                    OnClientClick="return confirm('Are you sure you want to remove this employee?');">
                                </asp:LinkButton>

                            </ItemTemplate>

                            <ItemStyle Width="100px"
                                HorizontalAlign="Center" />

                        </asp:TemplateField>

                    </Columns>

                    <HeaderStyle
                        HorizontalAlign="Center" />

                    <EmptyDataRowStyle
                        HorizontalAlign="Center"
                        ForeColor="Red"
                        Font-Bold="true" />

                </asp:GridView>
            <div class="row">

                <div class="col-md-12 text-center">
                     <asp:Button
                         ID="btnUpdateBatch"
                         runat="server"
                         Text="Update Batch"
                         CssClass="btn btn-secondary"
                         OnClick="btnUpdateBatch_Click" />
                    &nbsp;
                    <asp:Button
                        ID="btnPrevious"
                        runat="server"
                        Text="Update Sessions & Trainers"
                        CssClass="btn btn-secondary"
                        OnClick="btnPrevious_Click" />

                    
                    


                    &nbsp;

        <asp:Button
            ID="btnFinish"
            runat="server"
            Text="Finish Trainee Assignment"
            CssClass="btn btn-success"
            OnClick="btnFinish_Click" />



                </div>

            </div>

        </div>

    </div>

    <script>
        function InitializeMultiSelect() {

            if ($('#lstCompany').hasClass('select2-hidden-accessible'))
                $('#lstCompany').select2('destroy');

            $('#lstCompany').select2({
                placeholder: 'Select Company',
                width: '100%'
            });

            if ($('#lstDesignation').hasClass('select2-hidden-accessible'))
                $('#lstDesignation').select2('destroy');

            $('#lstDesignation').select2({
                placeholder: 'Select Designation',
                width: '100%'
            });

            if ($('#lstPostingPlace').hasClass('select2-hidden-accessible'))
                $('#lstPostingPlace').select2('destroy');

            $('#lstPostingPlace').select2({
                placeholder: 'Select Posting Place',
                width: '100%'
            });
        }

        $(document).ready(function () {
            InitializeMultiSelect();
        });

        if (typeof (Sys) !== "undefined") {
            Sys.Application.add_load(function () {
                InitializeMultiSelect();
            });
        }





        function ToggleAll(source) {
            var chk =
                document.querySelectorAll(
                    "#gvCompanyEmployee input[type=checkbox]");

            for (var i = 0; i < chk.length; i++) {
                chk[i].checked =
                    source.checked;
            }
        }

        function SelectAllEmployee() {
            $('#gvCompanyEmployee input[type=checkbox]')
                .prop('checked', true);
        }

        function ClearEmployeeSelection() {
            $('#gvCompanyEmployee input[type=checkbox]')
                .prop('checked', false);
        }

    </script>

</asp:Content>
