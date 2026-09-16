
<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="BulkExcelUpload.aspx.cs"
    Inherits="Training.SuperAdmin.BulkExcelUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>

        * {
            box-sizing: border-box;
        }

        .main-container {
            width: 100%;
            padding: 20px;
        }

        .card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
        }

        .title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 25px;
            color: #1e293b;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 20px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            margin-bottom: 8px;
            font-weight: 600;
        }

        .textbox,
        select {
            width: 100%;
            padding: 12px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
        }

        .checkbox-container {
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            padding: 15px;
            max-height: 250px;
            overflow-y: auto;
        }

        .btn {
            padding: 12px 25px;
            border: none;
            border-radius: 8px;
            color: white;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-load {
            background: #2563eb;
        }

        .btn-upload {
            background: #059669;
        }

        .btn-reset {
            background: #64748b;
        }

        .button-container {
            margin-top: 25px;
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }

        .summary {
            margin-top: 20px;
            padding: 15px;
            border-radius: 8px;
            background: #f8fafc;
            line-height: 28px;
            font-weight: 600;
        }

        .grid-card {
            overflow-x: auto;
        }

        .gridview {
            width: 100%;
            min-width: 1000px;
            border-collapse: collapse;
        }

        .gridview th {
            background: #2563eb;
            color: white;
            padding: 12px;
        }

        .gridview td {
            padding: 10px;
            border-bottom: 1px solid #e2e8f0;
        }

        @media screen and (max-width: 992px) {

            .grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        @media screen and (max-width: 576px) {

            .grid {
                grid-template-columns: 1fr;
            }

            .btn {
                width: 100%;
            }
        }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main-container">

        <div class="card">

            <div class="title">
                Bulk Excel Upload
            </div>

            <div class="grid">

                <div class="form-group">
                    <label>Select Table</label>

                    <asp:DropDownList ID="ddlTable"
                        runat="server"
                        CssClass="textbox"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlTable_SelectedIndexChanged">

                        <asp:ListItem Value="">-- Select Table --</asp:ListItem>
                        <asp:ListItem Value="EmpBasicMaster">EmpBasicMaster</asp:ListItem>
                        <asp:ListItem Value="TrainingDetails">TrainingDetails</asp:ListItem>
                        <asp:ListItem Value="FeedbackReport">FeedbackReport</asp:ListItem>
                        <asp:ListItem Value="FeedbackTrainingRelated">FeedbackTrainingRelated</asp:ListItem>

                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Operation</label>

                    <asp:RadioButtonList ID="rblOperation"
                        runat="server"
                        RepeatDirection="Horizontal">

                        <asp:ListItem Value="Insert">Insert</asp:ListItem>
                        <asp:ListItem Value="Update" Selected="True">Update</asp:ListItem>

                    </asp:RadioButtonList>
                </div>

                <div class="form-group">
                    <label>Upload Excel</label>

                    <asp:FileUpload ID="fuExcel"
                        runat="server"
                        CssClass="textbox" />
                </div>

            </div>

            <br />

            <div class="form-group">

                <label>Select Fields</label>

                <div class="checkbox-container">

                    <asp:CheckBoxList ID="chkFields"
                        runat="server">
                    </asp:CheckBoxList>

                </div>

            </div>

            <div class="button-container">

                <asp:Button ID="btnUpload"
                    runat="server"
                    Text="Upload & Process"
                    CssClass="btn btn-upload"
                    OnClick="btnUpload_Click" />

                <asp:Button ID="btnReset"
                    runat="server"
                    Text="Reset"
                    CssClass="btn btn-reset"
                    OnClick="btnReset_Click" />

            </div>

            <div class="summary">

                <asp:Label ID="lblSummary"
                    runat="server"></asp:Label>

            </div>

        </div>

        <div class="card grid-card">

            <asp:GridView ID="gvPreview"
                runat="server"
                CssClass="gridview">
            </asp:GridView>

        </div>

    </div>

</asp:Content>
