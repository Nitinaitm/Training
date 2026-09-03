<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="CreateBatch.aspx.cs" MaintainScrollPositionOnPostback="true"
    Inherits="Training.Admin.CreateBatch"
    ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


    <style>
        body {
            background: #f5f5f5;
        }

        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0px 0px 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .btn-save {
            background: darkcyan;
            color: white;
            border: none;
        }

            .btn-save:hover {
                background: teal;
                color: white;
            }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--multiple {
            min-height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .form-select {
            height: 38px !important;
        }

        .select2-container
        .select2-selection--single {
            height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .select2-selection__rendered {
            line-height: 36px !important;
        }

        .select2-selection__arrow {
            height: 36px !important;
        }
    </style>

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Create Batch

            </div>

            <div class="row">


                <!-- Training ID -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Batch ID

                    </label>

                    <asp:TextBox
                        ID="txtTrainingID"
                        runat="server"
                        CssClass="form-control"
                        ReadOnly="true">
                    </asp:TextBox>

                </div>


                <!-- Course -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Select Course *
                    </label>

                    <asp:DropDownList
                        ID="ddlCourse"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator
                        ID="rfvCourse"
                        runat="server"
                        ControlToValidate="ddlCourse"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Course">
                    </asp:RequiredFieldValidator>
                </div>

                <!-- Training Category -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Training Category *
                    </label>

                    <asp:DropDownList
                        ID="ddlTrainingCategory"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator
                        ID="rfvTrainingCategory"
                        runat="server"
                        ControlToValidate="ddlTrainingCategory"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Training Category">
                    </asp:RequiredFieldValidator>
                </div>


                <!-- Type -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Training Type *

                    </label>

                    <asp:DropDownList
                        ID="ddlTrainingType"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>


                    <asp:RequiredFieldValidator
                        ID="rfvType"
                        runat="server"
                        ControlToValidate="ddlTrainingType"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Training Type">

                    </asp:RequiredFieldValidator>

                </div>





                <!-- Organizer -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Training Organizer *

                    </label>

                    <asp:DropDownList
                        ID="ddlTrainingOrganizer"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>


                    <asp:RequiredFieldValidator
                        ID="rfvOrganizer"
                        runat="server"
                        ControlToValidate="ddlTrainingOrganizer"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Organizer">

                    </asp:RequiredFieldValidator>

                </div>





                <!-- Location -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Training Location *

                    </label>

                    <asp:DropDownList
                        ID="ddlTrainingLocation"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>


                    <asp:RequiredFieldValidator
                        ID="rfvLocation"
                        runat="server"
                        ControlToValidate="ddlTrainingLocation"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Location">

                    </asp:RequiredFieldValidator>

                </div>





                <!-- Designation -->
                <%--
                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Employee Designation *

                    </label>

                    <asp:ListBox
                        ID="lstDesignation"
                        runat="server"
                        CssClass="form-control"
                        SelectionMode="Multiple"></asp:ListBox>

                </div>--%>





                <!-- Batch -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Batch No*

                    </label>

                    <asp:TextBox
                        ID="txtBatch"
                        runat="server"
                        CssClass="form-control">

                    </asp:TextBox>


                    <asp:RequiredFieldValidator
                        ID="rfvBatch"
                        runat="server"
                        ControlToValidate="txtBatch"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Batch required">

                    </asp:RequiredFieldValidator>

                </div>







                <!-- Date From -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Date From *

                    </label>

                    <asp:TextBox
                        ID="txtDateFrom"
                        runat="server"
                        CssClass="form-control flatpickr"
                        placeholder="dd-mm-yyyy"
                        autocomplete="off"
                        onkeydown="return false;">
                    </asp:TextBox>




                </div>





                <!-- Date To -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Date To *

                    </label>

                    <asp:TextBox
                        ID="txtDateTo"
                        runat="server"
                        CssClass="form-control flatpickr"
                        placeholder="dd-mm-yyyy"
                        autocomplete="off"
                        onkeydown="return false;">
                    </asp:TextBox>




                </div>




                <!-- No Of Days -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        No. Of Days
                    </label>

                    <asp:TextBox
                        ID="txtNoOfDays"
                        runat="server"
                        CssClass="form-control"
                        ReadOnly="true">
                    </asp:TextBox>

                </div>


                <!-- Hours -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Total Training Hours
                    </label>

                    <asp:TextBox
                        ID="txtHours"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>
                    <asp:RegularExpressionValidator
                        ID="revHours"
                        runat="server"
                        ControlToValidate="txtHours"
                        ValidationGroup="SaveGroup"
                        ValidationExpression="^\d+(\.\d{1,2})?$"
                        CssClass="validation"
                        ErrorMessage="Enter valid Hours">
                    </asp:RegularExpressionValidator>
                </div>


                <!-- Start Time -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Start Time
                    </label>

                    <div class="row">

                        <div class="col-8">
                            <asp:DropDownList
                                ID="ddlStartTime"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                            <asp:RequiredFieldValidator
                                ID="rfvStartTime"
                                runat="server"
                                ControlToValidate="ddlStartTime"
                                InitialValue=""
                                ValidationGroup="SaveGroup"
                                CssClass="validation"
                                ErrorMessage="Select Start Time">
                            </asp:RequiredFieldValidator>
                        </div>



                    </div>

                </div>

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Batch Strength
                    </label>

                    <asp:TextBox
                        ID="txtStrength"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="3"
                        onkeypress="return isNumber(event);">
                    </asp:TextBox>

                    <asp:RegularExpressionValidator
                        ID="revStrength"
                        runat="server"
                        ControlToValidate="txtStrength"
                        ValidationGroup="SaveGroup"
                        ValidationExpression="^\d+$"
                        ErrorMessage="Enter valid Batch Strength"
                        CssClass="validation">
                    </asp:RegularExpressionValidator>

                </div>

                <!-- Training Requirements -->

                <div class="col-12 mb-3">
                    <label class="form-label">Training Requirements</label>

                    <div class="row">
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkAttendanceRequired" runat="server" Text="Attendance" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkPreTrainingAssessment" runat="server" Text="Pre-Training Assessment" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkPostTrainingAssessment" runat="server" Text="Post-Training Assessment" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkFeedbackRequired" runat="server" Text="Feedback" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkCertificateRequired" runat="server" Text="Certificate" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkTrainerHostelRequired" runat="server" Text="Trainer Hostel" Checked="true" />
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6 mb-2">
                            <asp:CheckBox ID="chkTraineeHostelRequired" runat="server" Text="Trainee Hostel" Checked="true" />
                        </div>
                    </div>
                </div>

                <!-- Remarks -->

                <div class="col-lg-4 mb-3">

                    <label class="form-label">
                        Remarks
                    </label>

                    <asp:TextBox
                        ID="txtRemarks"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="3">
                    </asp:TextBox>

                </div>



                <div class="col-12 mt-3 text-center">
                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Create Batch"
                        CssClass="btn btn-save"
                        ValidationGroup="SaveGroup"
                        OnClick="btnSave_Click" />

                    <asp:Button
                        ID="btnUpdate"
                        runat="server"
                        Text="Update Batch"
                        CssClass="btn btn-warning"
                        ValidationGroup="SaveGroup"
                        OnClick="btnUpdate_Click" />

                    &nbsp;

                    <asp:Button
                        ID="btnCreateSessions"
                        runat="server"
                        Text="Assign Sessions & Trainers"
                        CssClass="btn btn-success"
                        OnClick="btnCreateSessions_Click" />

                    &nbsp;

                    <asp:Button
                        ID="btnAssignTrainee"
                        runat="server"
                        Text="Assign Trainee"
                        CssClass="btn btn-success"
                        OnClick="btnAssignTrainee_Click"
                        Enabled="false" />

                </div>



                <div class="col-12 mt-3">

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Font-Bold="true">
                    </asp:Label>

                </div>

            </div>

        </div>

    </div>



    <script>

        function initControls() {
            // Flatpickr destroy + recreate

            document
                .querySelectorAll(".flatpickr")
                .forEach(function (el) {

                    if (el._flatpickr) {
                        el._flatpickr.destroy();
                    }

                });


            flatpickr(".flatpickr", {

                dateFormat: "d-m-Y",

                allowInput: false,

                clickOpens: true,

                onChange: function () {

                    calculateDays();

                }

            });

            if ($('#ddlCourse').length) {
                if ($('#ddlCourse')
                    .hasClass(
                        'select2-hidden-accessible')) {
                    $('#ddlCourse')
                        .select2('destroy');
                }

                $('#ddlCourse')
                    .select2({
                        width: '100%'
                    });
            }

            if ($('#ddlTrainingType').length) {
                if ($('#ddlTrainingType')
                    .hasClass(
                        'select2-hidden-accessible')) {
                    $('#ddlTrainingType')
                        .select2('destroy');
                }

                $('#ddlTrainingType')
                    .select2({
                        width: '100%'
                    });
            }

            if ($('#ddlTrainingCategory').length) {
                if ($('#ddlTrainingCategory')
                    .hasClass(
                        'select2-hidden-accessible')) {
                    $('#ddlTrainingCategory')
                        .select2('destroy');
                }

                $('#ddlTrainingCategory')
                    .select2({
                        width: '100%'
                    });
            }



            if ($('#ddlTrainingOrganizer').length) {
                if ($('#ddlTrainingOrganizer')
                    .hasClass(
                        'select2-hidden-accessible')) {
                    $('#ddlTrainingOrganizer')
                        .select2('destroy');
                }

                $('#ddlTrainingOrganizer')
                    .select2({
                        width: '100%'
                    });
            }



            if ($('#ddlTrainingLocation').length) {
                if ($('#ddlTrainingLocation')
                    .hasClass(
                        'select2-hidden-accessible')) {
                    $('#ddlTrainingLocation')
                        .select2('destroy');
                }

                $('#ddlTrainingLocation')
                    .select2({
                        width: '100%'
                    });
            }

        }

        function calculateDays() {
            var from = $('#txtDateFrom').val();
            var to = $('#txtDateTo').val();

            if (!from || !to) {
                $('#txtNoOfDays').val('');
                return;
            }

            var p1 = from.split('-');
            var p2 = to.split('-');

            var d1 = new Date(p1[2], p1[1] - 1, p1[0]);
            var d2 = new Date(p2[2], p2[1] - 1, p2[0]);

            var diff = (d2 - d1) / (1000 * 60 * 60 * 24);

            if (diff >= 0) {
                $('#txtNoOfDays').val(diff + 1);
            }
            else {
                $('#txtNoOfDays').val('');
            }
        }

        $(document).ready(function () {
            initControls();
        });

    </script>

</asp:Content>