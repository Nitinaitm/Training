<%@ Page Title="Question Bank" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="QuestionBank.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Training.Trainer.QuestionBank" ClientIDMode="Static" %>

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



        <div class="card">

            <div class="card-header bg-success text-white">
                Question Details

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label>
                            Course
                            <span class="required">*</span>

                        </label>

                        <asp:DropDownList
                            ID="ddlCourse"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label>
                            Topic
                            <span class="required">*</span>

                        </label>

                        <asp:DropDownList
                            ID="ddlTopic"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label>
                            Difficulty

                        </label>

                        <asp:DropDownList
                            ID="ddlDifficulty"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem>Easy</asp:ListItem>
                            <asp:ListItem>Medium</asp:ListItem>
                            <asp:ListItem>Hard</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                </div>

                <div class="row">

                    <div class="col-md-8 mb-3">

                        <label>
                            Question
                            <span class="required">*</span>

                        </label>

                        <asp:TextBox
                            ID="txtQuestion"
                            runat="server"
                            CssClass="form-control question-box"
                            TextMode="MultiLine"
                            Rows="5"
                            MaxLength="2000">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-4">

                        <div class="mb-3">

                            <label>
                                Question Type

                            </label>

                            <asp:DropDownList
                                ID="ddlQuestionType"
                                runat="server"
                                CssClass="form-control">

                                <asp:ListItem>MCQ</asp:ListItem>

                            </asp:DropDownList>

                        </div>

                        <div class="mb-3">

                            <label>
                                Language

                            </label>

                            <asp:DropDownList
                                ID="ddlLanguage"
                                runat="server"
                                CssClass="form-control">

                                <asp:ListItem>English</asp:ListItem>
                                <asp:ListItem>Hindi</asp:ListItem>

                            </asp:DropDownList>

                        </div>

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label>
                            Option A

                        </label>

                        <asp:TextBox
                            ID="txtOptionA"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-6 mb-3">

                        <label>
                            Option B

                        </label>

                        <asp:TextBox
                            ID="txtOptionB"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-6 mb-3">

                        <label>
                            Option C

                        </label>

                        <asp:TextBox
                            ID="txtOptionC"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-6 mb-3">

                        <label>
                            Option D

                        </label>

                        <asp:TextBox
                            ID="txtOptionD"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500">

                        </asp:TextBox>

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-3 mb-3">

                        <label>
                            Correct Option

                        </label>

                        <asp:DropDownList
                            ID="ddlCorrectOption"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem>A</asp:ListItem>
                            <asp:ListItem>B</asp:ListItem>
                            <asp:ListItem>C</asp:ListItem>
                            <asp:ListItem>D</asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Marks

                        </label>

                        <asp:TextBox
                            ID="txtMarks"
                            runat="server"
                            CssClass="form-control"
                            Text="1">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Negative Marks

                        </label>

                        <asp:TextBox
                            ID="txtNegativeMarks"
                            runat="server"
                            CssClass="form-control"
                            Text="0">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Status

                        </label>

                        <asp:DropDownList
                            ID="ddlStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem Value="1">Active</asp:ListItem>
                            <asp:ListItem Value="0">Inactive</asp:ListItem>

                        </asp:DropDownList>

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-6 mb-3">

                        <label>
                            Explanation

                        </label>

                        <asp:TextBox
                            ID="txtExplanation"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="4">

                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Question Image

                        </label>

                        <asp:FileUpload
                            ID="fuQuestionImage"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Explanation Image

                        </label>

                        <asp:FileUpload
                            ID="fuExplanationImage"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                </div>

                <div class="text-end">

                    <asp:HiddenField
                        ID="hfQuestionID"
                        runat="server" />

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save"
                        CssClass="btn btn-success"
                        OnClick="btnSave_Click" />

                    <asp:Button
                        ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="btn btn-secondary"
                        OnClick="btnClear_Click" />

                </div>

            </div>

        </div>

        <div class="card">

            <div class="card-header bg-primary text-white">
                Search Question

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-3 mb-3">

                        <label>
                            Course
                            <span class="required">*</span>

                        </label>

                        <asp:DropDownList
                            ID="ddlSearchCourse"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSearchCourse_SelectedIndexChanged">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label>
                            Topic

                        </label>

                        <asp:DropDownList
                            ID="ddlSearchTopic"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label>
                            Difficulty

                        </label>

                        <asp:DropDownList
                            ID="ddlSearchDifficulty"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem Value="">All</asp:ListItem>
                            <asp:ListItem>Easy</asp:ListItem>
                            <asp:ListItem>Medium</asp:ListItem>
                            <asp:ListItem>Hard</asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-4 mb-3">

                        <label>
                            Question

                        </label>

                        <asp:TextBox
                            ID="txtSearchQuestion"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500">

                        </asp:TextBox>

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-12 text-end">

                        <asp:Button
                            ID="btnSearch"
                            runat="server"
                            Text="Search"
                            CssClass="btn btn-primary"
                            OnClick="btnSearch_Click" />

                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-secondary"
                            OnClick="btnReset_Click" />
                        <asp:Button
                            ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-success" OnClick="btnBack_Click" />
                    </div>

                </div>

            </div>

        </div>


        <div class="card">

            <div class="card-header bg-dark text-white">
                Question List

            </div>

            <div class="card-body">

                <asp:GridView
                    ID="gvQuestion"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-hover"
                    AllowPaging="true"
                    PageSize="20" DataKeyNames="QuestionID"
                    OnPageIndexChanging="gvQuestion_PageIndexChanging"
                    OnRowCommand="gvQuestion_RowCommand">

                    <Columns>

                        <asp:BoundField DataField="QuestionID" HeaderText="Question ID" />

                        <asp:BoundField DataField="CourseName" HeaderText="Course" />

                        <asp:BoundField DataField="TopicName" HeaderText="Topic" />

                        <asp:BoundField DataField="DifficultyLevel" HeaderText="Difficulty" />

                        <asp:BoundField DataField="Marks" HeaderText="Marks" />

                        <asp:BoundField DataField="QuestionType" HeaderText="Type" />

                        <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />

                        <asp:TemplateField HeaderText="Action">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="lnkEdit"
                                    runat="server"
                                    CommandName="EditRecord"
                                    CommandArgument='<%# Eval("QuestionID") %>'
                                    CssClass="btn btn-sm btn-warning">

Edit

                                </asp:LinkButton>

                                <asp:LinkButton
                                    ID="lnkDelete"
                                    runat="server"
                                    CommandName="DeleteRecord"
                                    CommandArgument='<%# Eval("QuestionID") %>'
                                    OnClientClick="return confirm('Delete this question?');"
                                    CssClass="btn btn-sm btn-danger">

Delete

                                </asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </div>

    </div>


    <script>
        function initControls() {

            var ddl = $("#<%= ddlCourse.ClientID %>");

            if (ddl.hasClass("select2-hidden-accessible")) {
                ddl.select2("destroy");
            }

            ddl.select2({
                width: "100%"
            });
        }

        if ($('#ddlTopic').length) {

            if ($('#ddlTopic').hasClass('select2-hidden-accessible')) {

                $('#ddlTopic').select2('destroy');
            }

            $('#ddlTopic').select2({
                width: '100%'
            });
        }
        if ($('#ddlSearchTopic').length) {

            if ($('#ddlSearchTopic').hasClass('select2-hidden-accessible')) {

                $('#ddlSearchTopic').select2('destroy');
            }

            $('#ddlSearchTopic').select2({
                width: '100%'
            });
        }
        if ($('#ddlSearchCourse').length) {

            if ($('#ddlSearchCourse').hasClass('select2-hidden-accessible')) {

                $('#ddlSearchCourse').select2('destroy');
            }

            $('#ddlSearchCourse').select2({
                width: '100%'
            });
        }

        $(function () {
            initControls();
        });

        if (typeof (Sys) != "undefined") {
            Sys.Application.add_load(function () {
                initControls();
            });
        }
    </script>
    <%--<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>--%>

    <script>
        $(function () {
            initControls();
        });
    </script>
</asp:Content>
