<%@ Page Title="Feedback Master"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="FeedbackMaster.aspx.cs"
    Inherits="Training.Admin.FeedbackMaster" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>
        body {
            background: #f5f5f5;
        }

        .main-card {
            background: #ffffff;
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

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">Feedback Master

                </span>

            </div>

        </div>

        <asp:Label ID="lblMessage"
            runat="server"
            Font-Bold="true">
        </asp:Label>

        <div class="card mb-4">

            <div class="card-header bg-primary text-white">

                <b>Add / Update Feedback Question</b>

            </div>

            <div class="card-body">

                <asp:HiddenField ID="hfQuestionID"
                    runat="server" />

                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label>
                            Feedback Category

                            <span class="required">*</span>

                        </label>

                        <asp:DropDownList ID="ddlCategory"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator
                            ID="rfvCategory"
                            runat="server"
                            ControlToValidate="ddlCategory"
                            InitialValue=""
                            ErrorMessage="Required"
                            ForeColor="Red"
                            ValidationGroup="Save">
                        </asp:RequiredFieldValidator>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label>
                            Display Order

                        </label>

                        <asp:TextBox ID="txtDisplayOrder"
                            runat="server"
                            CssClass="form-control"
                            Text="1">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label>
                            Mandatory

                        </label>

                        <br />

                        <asp:CheckBox ID="chkMandatory"
                            runat="server"
                            Text=" Yes" />

                    </div>

                    <div class="col-md-2 mb-3">

                        <label>
                            Active

                        </label>

                        <br />

                        <asp:CheckBox ID="chkActive"
                            runat="server"
                            Checked="true"
                            Text=" Yes" />

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-12 mb-3">

                        <label>
                            Question

                            <span class="required">*</span>

                        </label>

                        <asp:TextBox ID="txtQuestion"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3"
                            MaxLength="500">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvQuestion"
                            runat="server"
                            ControlToValidate="txtQuestion"
                            ErrorMessage="Required"
                            ForeColor="Red"
                            ValidationGroup="Save">
                        </asp:RequiredFieldValidator>

                    </div>

                </div>
                <div class="col-md-3 mb-3">

                    <label>
                        Answer Type
        <span class="required">*</span>

                    </label>

                    <asp:DropDownList
                        ID="ddlAnswerType"
                        runat="server"
                        CssClass="form-select">

                        <asp:ListItem
                            Value="Rating">
            Rating (1-5)
                        </asp:ListItem>

                        <asp:ListItem
                            Value="YesNo">
            Yes / No
                        </asp:ListItem>

                        <asp:ListItem
                            Value="Text">
            Single Line Text
                        </asp:ListItem>

                        <asp:ListItem
                            Value="TextArea">
            Multi Line Text
                        </asp:ListItem>

                        <asp:ListItem
                            Value="Number">
            Number
                        </asp:ListItem>

                    </asp:DropDownList>

                </div>

                <div class="row">

                    <div class="col-md-12">

                        <asp:Button ID="btnSave"
                            runat="server"
                            Text="Save"
                            CssClass="btn btn-success"
                            ValidationGroup="Save"
                            OnClick="btnSave_Click" />

                        <asp:Button ID="btnClear"
                            runat="server"
                            Text="Clear"
                            CssClass="btn btn-secondary"
                            CausesValidation="false"
                            OnClick="btnClear_Click" />

                    </div>

                </div>

            </div>

        </div>

        <div class="card">

            <div class="card-header bg-primary text-white">

                <div class="row">

                    <div class="col-md-6">

                        <b>Feedback Questions</b>

                    </div>

                    <div class="col-md-6 text-end">

                        <asp:TextBox ID="txtSearch"
                            runat="server"
                            CssClass="form-control"
                            Width="320px"
                            AutoPostBack="true"
                            placeholder="Search Question..."
                            OnTextChanged="txtSearch_TextChanged">
                        </asp:TextBox>

                    </div>

                </div>

            </div>

            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView ID="gvQuestion"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover grid"
                        DataKeyNames="QuestionID"
                        OnRowCommand="gvQuestion_RowCommand">

                        <Columns>

                            <asp:TemplateField HeaderText="#">

                                <ItemTemplate>

                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>

                                <ItemStyle Width="50"
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="QuestionID"
                                HeaderText="Question ID" />

                            <asp:BoundField
                                DataField="CategoryName"
                                HeaderText="Category" />

                            <asp:BoundField
                                DataField="QuestionText"
                                HeaderText="Question" />

                            <asp:BoundField
                                DataField="AnswerType"
                                HeaderText="Answer Type" />

                            <asp:BoundField
                                DataField="DisplayOrder"
                                HeaderText="Order">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>

                            <asp:TemplateField
                                HeaderText="Mandatory">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblMandatory"
                                        runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("Mandatory")) ? "Yes" : "No" %>'
                                        CssClass='<%# Convert.ToBoolean(Eval("Mandatory")) ? "badge bg-success" : "badge bg-secondary" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle HorizontalAlign="Center" />

                            </asp:TemplateField>

                            <asp:TemplateField
                                HeaderText="Status">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblStatus"
                                        runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("Active")) ? "Active" : "Inactive" %>'
                                        CssClass='<%# Convert.ToBoolean(Eval("Active")) ? "badge bg-success" : "badge bg-danger" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle HorizontalAlign="Center" />

                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="CreatedOn"
                                HeaderText="Created On"
                                DataFormatString="{0:dd-MM-yyyy HH:mm}" />

                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemStyle Width="120"
                                    HorizontalAlign="Center" />

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="lnkEdit"
                                        runat="server"
                                        CssClass="btn btn-warning btn-sm action-btn"
                                        CommandName="EditRow"
                                        CommandArgument='<%# Eval("QuestionID") %>'>

                                        <i class="fa fa-edit"></i>

                                    </asp:LinkButton>

                                    <asp:LinkButton
                                        ID="lnkDelete"
                                        runat="server"
                                        CssClass="btn btn-danger btn-sm action-btn"
                                        CommandName="DeleteRow"
                                        CommandArgument='<%# Eval("QuestionID") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Are you sure to delete this question?');">

                                        <i class="fa fa-trash"></i>

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <EmptyDataTemplate>

                            <div class="alert alert-warning">
                                No Record Found.

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
