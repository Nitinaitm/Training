<%@ Page Title="Training Material"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingMaterial.aspx.cs"
    Inherits="Training.Trainer.TrainingMaterial" %>

<%@ Register Src="~/Trainer/TrainerSummary.ascx"
    TagPrefix="uc1"
    TagName="TrainerSummary" %>

<%@ Register Src="~/Trainer/SessionSummary.ascx"
    TagPrefix="uc2"
    TagName="SessionSummary" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px;
        }

        .dashboard-card {
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px;
        }

        .summary-box {
            background: #f8f9fa;
            border-left: 5px solid #198754;
            border-radius: 8px;
            padding: 15px;
            text-align: center;
            margin-bottom: 15px;
        }

        .summary-title {
            font-size: 14px;
            color: #666666;
        }

        .summary-value {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
        }

        .upload-box {
            background: #fcfcfc;
            border: 2px dashed #cccccc;
            border-radius: 10px;
            padding: 20px;
        }

            .upload-box:hover {
                border-color: #198754;
            }

        .btn-action {
            min-width: 180px;
            margin-right: 10px;
            margin-bottom: 10px;
        }

        .section-title {
            font-size: 18px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 15px;
        }

        .gridview th {
            background: #198754;
            color: #ffffff;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">

        <div class="page-heading">
            Training Material Management

        </div>

        <uc1:trainersummary id="TrainerSummary1" runat="server" />

        <br />

        <uc2:sessionsummary id="SessionSummary1" runat="server" />

        <br />

        <div class="row">

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        Total

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblTotal"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        Documents

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblDocument"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        PDF

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblPDF"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        PPT

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblPPT"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        Video

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblVideo"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

            <div class="col-lg-2 col-md-4">

                <div class="summary-box">

                    <div class="summary-title">
                        Others

                    </div>

                    <div class="summary-value">

                        <asp:Label
                            ID="lblOther"
                            runat="server"
                            Text="0" />

                    </div>

                </div>

            </div>

        </div>

        <div class="dashboard-card">

            <div class="section-title">
                Upload Training Material

            </div>

            <div class="upload-box">

                <div class="row">

                    <div class="col-md-4">

                        <label>
                            Title *

                        </label>

                        <asp:TextBox
                            ID="txtTitle"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-3">

                        <label>
                            Material Type *

                        </label>

                        <asp:DropDownList
                            ID="ddlType"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Value="">
        Select Type
                            </asp:ListItem>

                            <asp:ListItem
                                Value="PDF">
        PDF
                            </asp:ListItem>

                            <asp:ListItem
                                Value="PPT">
        PPT
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Document">
        Document
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Video">
        Video
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Other">
        Other
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>
                    <div class="col-md-3">

                        <label>
                            Select File *

                        </label>

                        <asp:FileUpload
                            ID="fuMaterial"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-2">

                        <label>
                            &nbsp;

                        </label>

                        <asp:Button
                            ID="btnUpload"
                            runat="server"
                            Text="Upload"
                            CssClass="btn btn-success w-100"
                            OnClick="btnUpload_Click" />

                    </div>

                </div>

                <br />

                <div class="row">

                    <div class="col-md-12">

                        <label>
                            Description

                        </label>

                        <asp:TextBox
                            ID="txtDescription"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3" />

                        <div class="row mt-3">

                            <div class="col-md-3">

                                <asp:CheckBox
                                    ID="chkVisibleToTrainee"
                                    runat="server"
                                    Text="Visible To Trainee"
                                    Checked="true" />

                            </div>

                            <div class="col-md-3">

                                <asp:CheckBox
                                    ID="chkDownloadAllowed"
                                    runat="server"
                                    Text="Download Allowed"
                                    Checked="true" />

                            </div>

                        </div>

                    </div>

                </div>

                <br />

                <div class="alert alert-info">

                    <b>Allowed File Types :

                    </b>

                    PDF,

PPT,

PPTX,

DOC,

DOCX,

XLS,

XLSX,

MP4,

ZIP,

RAR,

PNG,

JPG,

JPEG

                    <br />

                    <b>Maximum File Size :

                    </b>

                    100 MB

                </div>
                <div class="col-md-12 text-center">

                    <asp:Button
                        ID="btnBack"
                        runat="server"
                        Text="Back"
                        CssClass="btn btn-primary btn-action "
                        OnClick="btnBack_Click" />
                </div>
                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    Font-Bold="true" />

            </div>

        </div>

        <div class="dashboard-card">

            <div class="section-title">
                Search Material

            </div>

            <div class="row">

                <div class="col-md-4">

                    <label>
                        Search Title

                    </label>

                    <asp:TextBox
                        ID="txtSearch"
                        runat="server"
                        CssClass="form-control"
                        placeholder="Enter Title" />

                </div>

                <div class="col-md-3">

                    <label>
                        Material Type

                    </label>

                    <asp:DropDownList
                        ID="ddlFilterType"
                        runat="server"
                        CssClass="form-select">

                        <asp:ListItem
                            Value="All">

All

                        </asp:ListItem>

                        <asp:ListItem
                            Value="PDF">
    PDF
                        </asp:ListItem>

                        <asp:ListItem
                            Value="PPT">
    PPT
                        </asp:ListItem>

                        <asp:ListItem
                            Value="Document">
    Document
                        </asp:ListItem>

                        <asp:ListItem
                            Value="Video">
    Video
                        </asp:ListItem>

                        <asp:ListItem
                            Value="Other">
    Other
                        </asp:ListItem>

                    </asp:DropDownList>

                </div>

                <div class="col-md-5">

                    <br />

                    <asp:Button
                        ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />

                    &nbsp;

                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Reset"
                        CssClass="btn btn-secondary"
                        OnClick="btnReset_Click" />



                </div>

            </div>
        </div>
        <div class="dashboard-card">

            <div class="section-title">
                Uploaded Training Materials

            </div>

            <asp:GridView
                ID="gvMaterial"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover gridview"
                EmptyDataText="No Training Material Uploaded."
                ShowHeaderWhenEmpty="true"
                DataKeyNames="MaterialID"
                OnRowCommand="gvMaterial_RowCommand">

                <Columns>

                    <asp:TemplateField HeaderText="Sl No">

                        <ItemTemplate>

                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>

                        <ItemStyle
                            Width="60px"
                            HorizontalAlign="Center" />

                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="Title"
                        HeaderText="Title" />

                    <asp:BoundField DataField="MaterialType" HeaderText="Type" />

                    <asp:BoundField
                        DataField="FileName"
                        HeaderText="File Name" />



                    <asp:BoundField
                        DataField="Description"
                        HeaderText="Description" />

                    <asp:BoundField
                        DataField="CreatedOn"
                        HeaderText="Uploaded On"
                        DataFormatString="{0:dd-MM-yyyy HH:mm}" />

                    <asp:TemplateField HeaderText="Visible">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblVisible"
                                runat="server"
                                Text='<%# Convert.ToBoolean(Eval("VisibleToTrainee")) ? "Yes" : "No" %>' />

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Download">

                        <ItemTemplate>

                            <asp:Label
                                ID="lblDownload"
                                runat="server"
                                Text='<%# Convert.ToBoolean(Eval("DownloadAllowed")) ? "Allowed" : "Not Allowed" %>' />

                        </ItemTemplate>

                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Download">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkDownload"
                                runat="server"
                                Text="Download"
                                CommandName="DownloadMaterial"
                                CommandArgument='<%# Eval("MaterialID") %>'
                                CausesValidation="false" />

                        </ItemTemplate>

                        <ItemStyle
                            Width="120px"
                            HorizontalAlign="Center" />

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Delete">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkDelete"
                                runat="server"
                                Text="Delete"
                                CommandName="DeleteMaterial"
                                CommandArgument='<%# Eval("MaterialID") %>'
                                CausesValidation="false"
                                OnClientClick="return confirm('Are you sure?');" />

                        </ItemTemplate>

                        <ItemStyle
                            Width="100px"
                            HorizontalAlign="Center" />

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
