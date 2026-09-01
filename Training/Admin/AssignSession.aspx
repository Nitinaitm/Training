<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="AssignSession.aspx.cs" MaintainScrollPositionOnPostback="true"
    Inherits="Training.Admin.AssignSession"
    ClientIDMode="Static" %>

<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>


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
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .sub-heading {
            font-size: 18px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 15px;
            border-bottom: 1px solid #ddd;
            padding-bottom: 6px;
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

        .readonly-box {
            background: #eef3f8 !important;
            font-weight: bold;
        }

        .select2-container {
            width: 100% !important;
        }

            .select2-container .select2-selection--single {
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
                Session Management
            </div>

            <!--==========================================================
            TRAINING SUMMARY
        ===========================================================-->

            <uc:TrainingSummary ID="TrainingSummary1" runat="server" />

            <div class="col-md-3" runat="server" visible="false">
                <span class="summary-label">Training ID :</span><br />
                <asp:Label ID="lblTrainingID" runat="server" CssClass="summary-value" />
            </div>

            <div class="col-md-3" runat="server" visible="false">
                <span class="summary-label">Planned Hours :</span><br />
                <asp:Label ID="lblTrainingHours" runat="server" CssClass="summary-value" />
            </div>




            <!--==========================================================
            SESSION ENTRY
        ===========================================================-->

            <div class="card shadow-sm border-0">

                <div class="card-header bg-success text-white fw-bold">
                    <i class="fa fa-clock"></i>
                    Session Details
                </div>

                <div class="card-body">

                    <div class="row">

                        <!-- Session ID -->

                        <div class="col-lg-3 mb-3">

                            <label class="form-label fw-bold">
                                Session ID
                            </label>

                            <asp:TextBox
                                ID="txtSessionID"
                                runat="server"
                                CssClass="form-control readonly-box"
                                ReadOnly="true">
                            </asp:TextBox>

                        </div>

                        <!-- Session No -->

                        <div class="col-lg-2 mb-3">

                            <label class="form-label fw-bold">
                                Session No.
                            </label>

                            <asp:TextBox
                                ID="txtSessionNo"
                                runat="server"
                                CssClass="form-control readonly-box"
                                ReadOnly="true">
                            </asp:TextBox>

                        </div>

                        <!-- Session Name -->

                        <div class="col-lg-7 mb-3">

                            <label class="form-label fw-bold">
                                Session Name
                            </label>

                            <asp:TextBox
                                ID="txtSessionName"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator
                                ID="rfvSessionName"
                                runat="server"
                                ControlToValidate="txtSessionName"
                                ValidationGroup="SaveGroup"
                                CssClass="validation"
                                ErrorMessage="Required">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- Session Date -->

                        <div class="col-lg-3 mb-3">

                            <label class="form-label fw-bold">
                                Session Date
                            </label>

                            <asp:TextBox
                                ID="txtSessionDate"
                                runat="server"
                                CssClass="form-control flatpickr"
                                placeholder="dd-mm-yyyy"
                                autocomplete="off"
                                onkeydown="return false;">

                            </asp:TextBox>

                        </div>


                        <!-- Start -->

                        <div class="col-lg-3 mb-3">

                            <label class="form-label fw-bold">
                                Start Time
                            </label>

                            <asp:DropDownList
                                ID="ddlStartTime"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                        </div>

                        <!-- End -->

                        <div class="col-lg-3 mb-3">

                            <label class="form-label fw-bold">
                                End Time
                            </label>

                            <asp:DropDownList
                                ID="ddlEndTime"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                        </div>

                        <!-- Hours -->

                        <div class="col-lg-3 mb-3">

                            <label class="form-label fw-bold">
                                Session Hours
                            </label>

                            <asp:TextBox
                                ID="txtTotalHours"
                                runat="server"
                                CssClass="form-control readonly-box"
                                ReadOnly="true">
                            </asp:TextBox>
                            <asp:HiddenField ID="hfTotalHours" runat="server" />
                        </div>

                        <!-- Topic -->

                        <div class="col-lg-6 mb-3">

                            <label class="form-label fw-bold">
                                Topic
                            </label>

                            <asp:DropDownList
                                ID="ddlTopic"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator
                                ID="rfvTopic"
                                runat="server"
                                ControlToValidate="ddlTopic"
                                InitialValue=""
                                ErrorMessage="Select Topic"
                                CssClass="validation"
                                ValidationGroup="SaveGroup">
                            </asp:RequiredFieldValidator>

                        </div>

                        <!-- Trainer -->

                        <div class="col-lg-6 mb-3">

                            <label class="form-label fw-bold">
                                Trainer
                            </label>

                            <asp:DropDownList
                                ID="ddlTrainer"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator
                                ID="rfvTrainer"
                                runat="server"
                                ControlToValidate="ddlTrainer"
                                InitialValue=""
                                ErrorMessage="Select Trainer"
                                CssClass="validation"
                                ValidationGroup="SaveGroup">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-lg-12 mb-3">

                            <label class="form-label fw-bold">
                                Area of Expertise
                            </label>

                            <asp:Label
                                ID="lblTrainerExpertise"
                                runat="server"
                                CssClass="form-control readonly-box">
                            </asp:Label>

                        </div>
                        <!-- Remarks -->

                        <div class="col-lg-12 mb-3">

                            <label class="form-label fw-bold">
                                Remarks
                            </label>

                            <asp:TextBox
                                ID="txtRemarks"
                                runat="server"
                                CssClass="form-control"
                                Rows="3"
                                TextMode="MultiLine">
                            </asp:TextBox>

                        </div>

                        <!-- Buttons -->

                        <div class="col-12 mt-2 text-center">

                            <asp:Button
                                ID="btnSave"
                                runat="server"
                                CssClass="btn btn-success"
                                Text="Add Session"
                                ValidationGroup="SaveGroup"
                                OnClick="btnSave_Click" />

                            <asp:Button
                                ID="btnUpdate"
                                runat="server"
                                CssClass="btn btn-warning"
                                Text="Update"
                                Visible="false"
                                ValidationGroup="SaveGroup"
                                OnClick="btnUpdate_Click" />

                            <asp:Button
                                ID="btnDelete"
                                runat="server"
                                CssClass="btn btn-danger"
                                Text="Delete"
                                Visible="false"
                                CausesValidation="false"
                                OnClick="btnDelete_Click" />

                            <asp:Button
                                ID="btnClear"
                                runat="server"
                                CssClass="btn btn-secondary"
                                Text="Clear"
                                CausesValidation="false"
                                OnClick="btnClear_Click" />



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

            <!-- Session Grid starts in Part-1B -->

            <!--==========================================================
            SESSION LIST
        ===========================================================-->

            <div class="card shadow-sm border-0 mt-4">

                <div class="card-header bg-info text-white fw-bold">
                    <i class="fa fa-list"></i>
                    Session List
                </div>

                <div class="card-body p-0">

                    <div class="table-responsive">

                        <asp:GridView
                            ID="gvSession"
                            runat="server"
                            CssClass="table table-bordered table-hover table-striped mb-0"
                            AutoGenerateColumns="False"
                            DataKeyNames="SessionID"
                            OnRowCommand="gvSession_RowCommand">

                            <Columns>

                                <asp:TemplateField HeaderText="#">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Session">

                                    <ItemTemplate>

                                        <span class="badge bg-dark">S-<%# Eval("SessionNo") %>

                                        </span>

                                    </ItemTemplate>

                                    <ItemStyle HorizontalAlign="Center" Width="90px" />

                                </asp:TemplateField>

                                <asp:BoundField DataField="SessionName" HeaderText="Session Name" />

                                <asp:TemplateField HeaderText="Schedule">

                                    <ItemTemplate>

                                        <div style="font-weight: 600">

                                            <%# Eval("SessionDate") %>
                                        </div>

                                        <small class="text-muted">

                                            <%# Eval("StartTime") %>
            -
            <%# Eval("EndTime") %>

                                        </small>

                                    </ItemTemplate>

                                    <ItemStyle Width="170px" />

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Hours">

                                    <ItemTemplate>

                                        <span class="badge bg-warning text-dark">

                                            <%# Eval("TotalHours") %> Hr

                                        </span>

                                    </ItemTemplate>

                                    <ItemStyle HorizontalAlign="Center" Width="80px" />

                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Topic">

                                    <ItemTemplate>

                                        <span class="badge bg-primary px-3 py-2"
                                            style="font-size: 13px; white-space: normal;">

                                            <%# Eval("TopicName") %>

                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Trainer">

                                    <ItemTemplate>

                                        <div style="font-weight: bold; color: #0d6efd;">
                                            <%# Eval("DisplayTrainerID") %>
                                        </div>

                                        <div>
                                            <%# Eval("TrainerName") %>
                                        </div>

                                        <small class="text-muted">
                                            <%# Eval("Designation") %>
                                        </small>

                                        <br />

                                        <span class='<%# Eval("TrainerType").ToString()=="Internal"
                        ? "badge bg-success"
                        : "badge bg-warning text-dark" %>'>

                                            <%# Eval("TrainerType") %>

                                        </span>

                                    </ItemTemplate>

                                    <ItemStyle Width="250px" />

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Edit">

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            ID="lnkEdit"
                                            runat="server"
                                            CssClass="btn btn-sm btn-primary"
                                            CommandName="EditRecord"
                                            CommandArgument='<%# Eval("SessionID") %>'>

                                        <i class="fa fa-edit"></i>

                                        </asp:LinkButton>

                                    </ItemTemplate>

                                    <ItemStyle HorizontalAlign="Center" Width="70px" />

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Delete">

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            ID="lnkDelete"
                                            runat="server"
                                            CssClass="btn btn-sm btn-danger"
                                            CommandName="DeleteRecord"
                                            CommandArgument='<%# Eval("SessionID") %>'
                                            OnClientClick="return confirm('Delete this Session ?');">

                                        <i class="fa fa-trash"></i>

                                        </asp:LinkButton>

                                    </ItemTemplate>

                                    <ItemStyle HorizontalAlign="Center" Width="80px" />

                                </asp:TemplateField>

                            </Columns>

                            <HeaderStyle
                                BackColor="#0d6efd"
                                ForeColor="White"
                                Font-Bold="true" />

                            <EmptyDataTemplate>

                                <div class="text-center p-4 text-danger fw-bold">
                                    No Session Created Yet.

                                </div>

                            </EmptyDataTemplate>

                        </asp:GridView>

                    </div>

                </div>

            </div>


            <!--==========================================================
            SESSION SUMMARY
        ===========================================================-->

            <div class="row mt-4">

                <div class="col-lg-4">

                    <div class="card border-0 shadow-sm text-center">

                        <div class="card-body">

                            <h6 class="text-muted">Total Sessions
                            </h6>

                            <h2 class="text-primary fw-bold">

                                <asp:Label
                                    ID="lblTotalSessions"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </h2>

                        </div>

                    </div>

                </div>

                <div class="col-lg-4">

                    <div class="card border-0 shadow-sm text-center">

                        <div class="card-body">

                            <h6 class="text-muted">Total Session Hours
                            </h6>

                            <h2 class="text-success fw-bold">

                                <asp:Label
                                    ID="lblUsedHours"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </h2>

                        </div>

                    </div>

                </div>

                <div class="col-lg-4">

                    <div class="card border-0 shadow-sm text-center">

                        <div class="card-body">

                            <h6 class="text-muted">Remaining Hours
                            </h6>

                            <h2 class="text-danger fw-bold">

                                <asp:Label
                                    ID="lblRemainingHours"
                                    runat="server"
                                    Text="0">
                                </asp:Label>

                            </h2>

                        </div>

                    </div>

                </div>

            </div>
            <hr />
            <div class="row">
                <div class="col-md-12 text-center">
                    <asp:Button
                        ID="btnUpdateBatch"
                        runat="server"
                        Text="Update Batch"
                        CssClass="btn btn-secondary"
                        OnClick="btnUpdateBatch_Click" />
                    <asp:Button
                        ID="btnUpdateTrainee"
                        runat="server"
                        Text="Update Trainee"
                        CssClass="btn btn-secondary"
                        OnClick="btnUpdateTrainee_Click" />
                    
                    <asp:Button
                        ID="btnFinishSession"
                        runat="server"
                        Text="Finish Session & Trainer Assignment"
                        CssClass="btn btn-success"
                        OnClick="btnFinishSession_Click" />
                </div>

            </div>

        </div>

    </div>


    <script>

        function initControls() {

            document.querySelectorAll(".flatpickr").forEach(function (el) {

                if (el._flatpickr) {
                    el._flatpickr.destroy();
                }

            });


            flatpickr(".flatpickr",
                {
                    dateFormat: "d-m-Y",
                    allowInput: false,
                    clickOpens: true
                });

            $("#ddlStartTime").select2({ width: "100%" });

            $("#ddlEndTime").select2({ width: "100%" });

            $("#ddlTopic").select2({
                width: "100%",
                placeholder: "Select Topic"
            });


            $("#ddlTrainer").select2({
                width: "100%",
                placeholder: "Search Trainer...",
                allowClear: true,
                minimumInputLength: 0
            });
        }

        $(document).on("change", "#ddlTrainer", function () {

    var trainerID = $(this).val();

    if (trainerID == "") {

        $("#lblTrainerExpertise").text("");

        return;
    }

    $.ajax({

        type: "POST",

        url: "AssignSession.aspx/GetTrainerExpertise",

        contentType: "application/json; charset=utf-8",

        dataType: "json",

        data: JSON.stringify({
            trainerID: trainerID
        }),

        success: function (response) {

            $("#lblTrainerExpertise").text(response.d);

        },

        error: function () {

            $("#lblTrainerExpertise").text("");

        }

    });

});
        $(document)
            .ready(function () {

                initControls();

            });


        if (typeof (Sys) !== "undefined") {
            Sys.Application.add_load(
                function () {

                    initControls();

                });
        }

        $(document).on("change", "#ddlStartTime,#ddlEndTime", function () {

            calculateHours();

        });


        function convertToMinutes(time) {

            if (!time)
                return 0;

            var p = time.split(' ');

            var hm = p[0].split(':');

            var h = parseInt(hm[0]);

            var m = parseInt(hm[1]);

            if (p[1] == "PM" && h != 12)
                h += 12;

            if (p[1] == "AM" && h == 12)
                h = 0;

            return h * 60 + m;

        }


        function calculateHours() {
            var s = $("#ddlStartTime").val();

            var e = $("#ddlEndTime").val();

            if (s == "" || e == "") {

                $("#txtTotalHours").val("");

                return;

            }

            var start = convertToMinutes(s);

            var end = convertToMinutes(e);

            if (end <= start) {

                $("#txtTotalHours").val("");

                return;

            }

            var hrs = ((end - start) / 60).toFixed(2);

            $("#txtTotalHours").val(hrs);

            $("#hfTotalHours").val(hrs);

        }

    </script>

</asp:Content>
