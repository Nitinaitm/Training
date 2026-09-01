<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="CertificateTemplate.aspx.cs"
    Inherits="Training.Admin.CertificateTemplate"
    MasterPageFile="~/AdminMaster.Master" %>

<%@ Register
    Src="~/Admin/TrainingSummary.ascx"
    TagPrefix="uc"
    TagName="TrainingSummary" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css"
        rel="stylesheet" />

    <style>
        body {
            background: #f4f7fb;
        }

        .main-card {
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0,0,0,.08);
            padding: 25px;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-title {
            font-size: 30px;
            font-weight: 700;
            color: #198754;
            margin-bottom: 20px;
        }

        .section-card {
            background: #ffffff;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
            margin-top: 20px;
        }

        .section-title {
            font-size: 20px;
            color: #0d6efd;
            font-weight: 600;
            margin-bottom: 20px;
        }

        .required {
            color: red;
        }

        .config-box {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px;
        }

        .config-label {
            color: #0d6efd;
            font-weight: 600;
        }

        .config-value {
            color: #212529;
            word-break: break-word;
        }

        .preview-img {
            width: 180px;
            height: 90px;
            border: 1px solid #d9d9d9;
            background: #ffffff;
            object-fit: contain;
            padding: 5px;
        }

        .radio-big input {
            margin-right: 6px;
        }

        .radio-big label {
            margin-right: 35px;
            font-weight: 600;
            cursor: pointer;
        }

        .border-dashed {
            border: 2px dashed #dee2e6;
            border-radius: 10px;
            padding: 20px;
        }

        @media(max-width:768px) {

            .page-title {
                font-size: 24px;
            }

            .preview-img {
                width: 100%;
                height: 100px;
            }
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-title">

                <i class="bi bi-award-fill"></i>

                Training Certificate Configuration

            </div>

            <asp:hiddenfield
                id="hfTrainingID"
                runat="server" />

            <asp:hiddenfield
                id="hfTrainingTemplateID"
                runat="server" />

            <asp:hiddenfield
                id="hfSelectedConfigurationID"
                runat="server" />

            <!------------------------------------------------------>
            <!-- Training Summary -->
            <!------------------------------------------------------>

            <uc:trainingsummary
                id="TrainingSummary1"
                runat="server" />

            <!------------------------------------------------------>
            <!-- Configuration Mode -->
            <!------------------------------------------------------>

            <div class="section-card">

                <div class="section-title">

                    <i class="bi bi-sliders"></i>

                    Configuration Mode

                </div>

                <asp:radiobuttonlist
                    id="rblMode"
                    runat="server"
                    cssclass="radio-big"
                    repeatdirection="Horizontal"
                    autopostback="true"
                    onselectedindexchanged="rblMode_SelectedIndexChanged">

    <asp:ListItem

        Selected="True"

        Value="Existing">

        Use Existing Configuration

    </asp:ListItem>

    <asp:ListItem

        Value="New">

        Create New Configuration

    </asp:ListItem>

</asp:radiobuttonlist>

            </div>
            <!-------------------------------------------------------------->
            <!-- USE EXISTING CONFIGURATION -->
            <!-------------------------------------------------------------->

            <asp:panel
                id="pnlExisting"
                runat="server">

    <div class="section-card">

        <div class="section-title">

            <i class="bi bi-collection-fill"></i>

            Use Existing Certificate Configuration

        </div>

        <div class="row">

            <div class="col-lg-9 col-md-8 col-sm-12">

                <label>

                    Existing Configuration

                    <span class="required">*</span>

                </label>

                <asp:DropDownList
                    ID="ddlConfiguration"
                    runat="server"
                    CssClass="form-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlConfiguration_SelectedIndexChanged">

                </asp:DropDownList>

                <small class="text-muted">

                    Select a reusable certificate configuration.

                </small>

            </div>

            <div class="col-lg-3 col-md-4 col-sm-12 d-grid">

                <label>&nbsp;</label>

                <asp:Button
                    ID="btnApplyConfiguration"
                    runat="server"
                    Text="Apply"
                    CssClass="btn btn-success"
                    Enabled="false"
                    OnClick="btnApplyConfiguration_Click" />

            </div>

        </div>

        <br />

        <asp:Panel
            ID="pnlExistingDetails"
            runat="server"
            Visible="false">

            <div class="config-box">

                <div class="row">

                    <div class="col-lg-4 col-md-6 mb-4">

                        <span class="config-label">

                            Configuration Name

                        </span>

                        <br />

                        <asp:Label
                            ID="lblConfigurationName"
                            runat="server"
                            CssClass="config-value" />

                    </div>

                    <div class="col-lg-4 col-md-6 mb-4">

                        <span class="config-label">

                            Certificate Template

                        </span>

                        <br />

                        <asp:Label
                            ID="lblTemplateName"
                            runat="server"
                            CssClass="config-value" />

                    </div>

                    <div class="col-lg-4 col-md-6 mb-4">

                        <span class="config-label">

                            Course Title

                        </span>

                        <br />

                        <asp:Label
                            ID="lblCourseTitle"
                            runat="server"
                            CssClass="config-value" />

                    </div>

                </div>

                <div class="row">

                    <div class="col-lg-12 mb-4">

                        <span class="config-label">

                            Description

                        </span>

                        <br />

                        <asp:Label
                            ID="lblConfigurationDescription"
                            runat="server"
                            CssClass="config-value" />

                    </div>

                </div>

                <hr />

                <div class="row">

                    <div class="col-lg-6">

                        <div class="card">

                            <div class="card-header bg-primary text-white">

                                Left Signatory

                            </div>

                            <div class="card-body">

                                <div class="text-center mb-3">

                                    <asp:Image
                                        ID="imgPreviewLeft"
                                        runat="server"
                                        CssClass="preview-img" />

                                </div>

                                <table class="table table-sm table-borderless">

                                    <tr>

                                        <td width="35%">

                                            <b>Name</b>

                                        </td>

                                        <td>

                                            <asp:Label
                                                ID="lblPreviewLeftName"
                                                runat="server" />

                                        </td>

                                    </tr>

                                    <tr>

                                        <td>

                                            <b>Designation</b>

                                        </td>

                                        <td>

                                            <asp:Label
                                                ID="lblPreviewLeftDesignation"
                                                runat="server" />

                                        </td>

                                    </tr>

                                </table>

                            </div>

                        </div>

                    </div>

                    <div class="col-lg-6">

                        <div class="card">

                            <div class="card-header bg-success text-white">

                                Right Signatory

                            </div>

                            <div class="card-body">

                                <div class="text-center mb-3">

                                    <asp:Image
                                        ID="imgPreviewRight"
                                        runat="server"
                                        CssClass="preview-img" />

                                </div>

                                <table class="table table-sm table-borderless">

                                    <tr>

                                        <td width="35%">

                                            <b>Name</b>

                                        </td>

                                        <td>

                                            <asp:Label
                                                ID="lblPreviewRightName"
                                                runat="server" />

                                        </td>

                                    </tr>

                                    <tr>

                                        <td>

                                            <b>Designation</b>

                                        </td>

                                        <td>

                                            <asp:Label
                                                ID="lblPreviewRightDesignation"
                                                runat="server" />

                                        </td>

                                    </tr>

                                </table>

                            </div>

                        </div>

                    </div>

                </div>

                <div class="text-end mt-3">

                    <asp:Button
                        ID="btnPreviewConfiguration"
                        runat="server"
                        Text="Preview Configuration"
                        CssClass="btn btn-outline-primary"
                        OnClick="btnPreviewConfiguration_Click" />

                </div>

            </div>

        </asp:Panel>

    </div>

</asp:panel>
            <!-------------------------------------------------------------->
            <!-- CREATE NEW CONFIGURATION -->
            <!-------------------------------------------------------------->

            <asp:panel
                id="pnlNew"
                runat="server"
                visible="false">

    <div class="section-card">

        <div class="section-title">

            <i class="bi bi-plus-circle-fill"></i>

            Create New Certificate Configuration

        </div>

        <!-------------------------------------------------------------->
        <!-- Save As Reusable -->
        <!-------------------------------------------------------------->

        <div class="border-dashed mb-4">

            <div class="row">

                <div class="col-md-12">

                    <asp:CheckBox
                        ID="chkReusable"
                        runat="server"
                        AutoPostBack="true"
                        Text=" Save this configuration for future trainings"
                        OnCheckedChanged="chkReusable_CheckedChanged" />

                </div>

            </div>

            <br />

            <asp:Panel
                ID="pnlReusable"
                runat="server"
                Visible="false">

                <div class="row">

                    <div class="col-lg-6">

                        <label>

                            Configuration Name

                            <span class="required">*</span>

                        </label>

                        <asp:TextBox
                            ID="txtConfigurationName"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="250">
                        </asp:TextBox>

                    </div>

                    <div class="col-lg-6">

                        <label>

                            Description

                        </label>

                        <asp:TextBox
                            ID="txtDescription"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3"
                            MaxLength="1000">
                        </asp:TextBox>

                    </div>

                </div>

            </asp:Panel>

        </div>

        <!-------------------------------------------------------------->
        <!-- Template -->
        <!-------------------------------------------------------------->

        <div class="row">

            <div class="col-lg-6 mb-4">

                <label>

                    Certificate Template

                    <span class="required">*</span>

                </label>

                <asp:DropDownList
                    ID="ddlTemplate"
                    runat="server"
                    CssClass="form-select">

                </asp:DropDownList>

            </div>

            <div class="col-lg-6 mb-4">

                <label>

                    Certificate Course Title

                    <span class="required">*</span>

                </label>

                <asp:TextBox
                    ID="txtCourseTitle"
                    runat="server"
                    CssClass="form-control"
                    TextMode="MultiLine"
                    Rows="2"
                    MaxLength="500">

                </asp:TextBox>

            </div>

        </div>

        <hr />

        <!-------------------------------------------------------------->
        <!-- LEFT SIGNATORY -->
        <!-------------------------------------------------------------->

        <h5 class="text-primary mb-3">

            <i class="bi bi-person-badge"></i>

            Left Signatory

        </h5>

        <div class="row">

            <div class="col-lg-3 mb-3">

                <label>

                    Signature

                </label>

                <asp:FileUpload
                    ID="fuLeftSignature"
                    runat="server"
                    CssClass="form-control" />

            </div>

            <div class="col-lg-3 mb-3 text-center">

                <label>

                    Preview

                </label>

                <br />

                <asp:Image
                    ID="imgLeftSignature"
                    runat="server"
                    CssClass="preview-img" />

            </div>

            <div class="col-lg-3 mb-3">

                <label>

                    Name

                    <span class="required">*</span>

                </label>

                <asp:TextBox
                    ID="txtLeftName"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

            <div class="col-lg-3 mb-3">

                <label>

                    Designation

                    <span class="required">*</span>

                </label>

                <asp:TextBox
                    ID="txtLeftDesignation"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

        </div>

        <hr />

        <!-------------------------------------------------------------->
        <!-- RIGHT SIGNATORY -->
        <!-------------------------------------------------------------->

        <h5 class="text-success mb-3">

            <i class="bi bi-person-badge-fill"></i>

            Right Signatory

        </h5>

        <div class="row">

            <div class="col-lg-3 mb-3">

                <label>

                    Signature

                </label>

                <asp:FileUpload
                    ID="fuRightSignature"
                    runat="server"
                    CssClass="form-control" />

            </div>

            <div class="col-lg-3 mb-3 text-center">

                <label>

                    Preview

                </label>

                <br />

                <asp:Image
                    ID="imgRightSignature"
                    runat="server"
                    CssClass="preview-img" />

            </div>

            <div class="col-lg-3 mb-3">

                <label>

                    Name

                    <span class="required">*</span>

                </label>

                <asp:TextBox
                    ID="txtRightName"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

            <div class="col-lg-3 mb-3">

                <label>

                    Designation

                    <span class="required">*</span>

                </label>

                <asp:TextBox
                    ID="txtRightDesignation"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

        </div>
                <hr />

        <!-------------------------------------------------------------->
        <!-- ACTION BUTTONS -->
        <!-------------------------------------------------------------->

        <div class="section-card">

            <div class="row">

                <div class="col-md-12 text-center">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="💾 Save"
                        CssClass="btn btn-success btn-lg me-2"
                        Width="180"
                        OnClick="btnSave_Click" />

                    <asp:Button
                        ID="btnPreview"
                        runat="server"
                        Text="👁 Preview"
                        CssClass="btn btn-primary btn-lg me-2"
                        Width="180"
                        OnClick="btnPreview_Click" />

                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="↻ Reset"
                        CssClass="btn btn-secondary btn-lg"
                        Width="180"
                        OnClick="btnReset_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:panel>

            <!-------------------------------------------------------------->
            <!-- MESSAGE -->
            <!-------------------------------------------------------------->

            <asp:panel
                id="pnlMessage"
                runat="server"
                visible="false"
                cssclass="alert alert-success mt-3">

    <asp:Label
        ID="lblMessage"
        runat="server">
    </asp:Label>

</asp:panel>

        </div>

    </div>

</asp:Content>
