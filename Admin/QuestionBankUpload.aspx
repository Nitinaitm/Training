<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AdminMaster.Master" CodeBehind="QuestionBankUpload.aspx.cs" Inherits="Training.Admin.QuestionBankUpload" %>

<asp:Content    id="Content1"
    contentplaceholderid="head"
    runat="server">

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

<asp:Content    ID="Content2"    ContentPlaceHolderID="ContentPlaceHolder1"    runat="server">

    <div class="container-fluid">

        <div class="page-title">
            Bulk Question Upload

        </div>

        <div class="card-box">

            <div class="section-title">
                Instructions

            </div>

            <ul>

                <li>Download the sample Excel file.</li>

                <li>Do not change the column names.</li>

                <li>One question per row.</li>

                <li>Course and Topic should already exist.</li>

                <li>Correct Answer must be A, B, C or D.</li>

                <li>Difficulty should be Easy, Medium or Hard.</li>

                <li>Marks must be numeric.</li>

            </ul>

            <asp:Button
                ID="btnDownloadSample"
                runat="server"
                Text="Download Sample Excel"
                CssClass="btn btn-success"
                OnClick="btnDownloadSample_Click" />

        </div>

        <div class="card-box">

            <div class="section-title">
                Upload Excel

            </div>

            <div class="row">

                <div class="col-md-8">

                    <asp:FileUpload
                        ID="fuExcel"
                        runat="server"
                        CssClass="form-control" />

                </div>

                <div class="col-md-4">

                    <asp:Button
                        ID="btnUpload"
                        runat="server"
                        Text="Upload Questions"
                        CssClass="btn btn-primary w-100"
                        OnClick="btnUpload_Click" />

                </div>

            </div>

            <br />

            <asp:Label
                ID="lblMessage"
                runat="server"
                Font-Bold="true">
            </asp:Label>

        </div>

        <div class="card-box">

            <div class="section-title">
                Upload Result

            </div>

            <asp:GridView
                ID="gvResult"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                EmptyDataText="No data available.">

                <Columns>

                    <asp:BoundField
                        DataField="RowNo"
                        HeaderText="Row No" />

                    <asp:BoundField
                        DataField="Course"
                        HeaderText="Course" />

                    <asp:BoundField
                        DataField="Topic"
                        HeaderText="Topic" />

                    <asp:BoundField
                        DataField="Question"
                        HeaderText="Question" />

                    <asp:BoundField
                        DataField="Status"
                        HeaderText="Status" />

                    <asp:BoundField
                        DataField="Message"
                        HeaderText="Message" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
