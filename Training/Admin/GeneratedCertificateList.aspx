<%@ Page Title="Generated Certificates"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="GeneratedCertificateList.aspx.cs"
    Inherits="Training.Admin.GeneratedCertificateList" %>

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
    <style>
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #0d6efd;
        }

        .filter-card {
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,.10);
            margin-bottom: 20px;
        }

        .certificate-card {
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,.10);
            margin-bottom: 20px;
        }

        .certificate-table {
            width: 100%;
        }

            .certificate-table th {
                background-color: #0d6efd;
                color: #ffffff;
                font-weight: 600;
                text-align: center;
                vertical-align: middle;
                white-space: nowrap;
            }

            .certificate-table td {
                vertical-align: middle;
            }

        .certificate-no {
            font-weight: 600;
            color: #0d6efd;
        }

        .status-active {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            background-color: #198754;
            color: #ffffff;
            font-size: 12px;
            font-weight: 600;
        }

        .status-inactive {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            background-color: #dc3545;
            color: #ffffff;
            font-size: 12px;
            font-weight: 600;
        }

        .certificate-actions {
            white-space: nowrap;
        }

        .empty-certificate {
            padding: 30px;
            text-align: center;
            font-size: 16px;
            color: #6c757d;
        }

        .filter-label {
            font-weight: 600;
            margin-bottom: 5px;
            display: block;
        }

        @media (max-width: 768px) {

            .page-title {
                font-size: 20px;
            }

            .certificate-table {
                min-width: 1300px;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <!-- Page Title -->

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">Generated Certificates
                </span>

            </div>

        </div>


        <!-- Message -->

        <asp:Label
            ID="lblMessage"
            runat="server"
            Font-Bold="true">
        </asp:Label>


        <!-- Search / Filter -->

        <div class="card filter-card">

            <div class="card-header bg-secondary text-white">

                <b>Search Certificate
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Training
                        </label>

                        <asp:DropDownList
                            ID="ddlTraining"
                            runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Course
                        </label>

                        <asp:DropDownList
                            ID="ddlCourse"
                            runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Course Title
                        </label>

                        <asp:TextBox
                            ID="txtCourseTitle"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="500"
                            placeholder="Course Title">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Certificate No.
                        </label>

                        <asp:TextBox
                            ID="txtCertificateNo"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="50"
                            placeholder="Certificate No.">
                        </asp:TextBox>

                    </div>

                </div>


                <div class="row">

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Employee / Trainee
                        </label>

                        <asp:TextBox
                            ID="txtEmployee"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="100"
                            placeholder="ID or Name">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Text="All"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Active"
                                Value="A">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Inactive"
                                Value="I">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Generated From
                        </label>

                        <asp:TextBox
                            ID="txtFromDate"
                            runat="server"
                            CssClass="form-control"
                            placeholder="dd-MM-yyyy">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Generated To
                        </label>

                        <asp:TextBox
                            ID="txtToDate"
                            runat="server"
                            CssClass="form-control"
                            placeholder="dd-MM-yyyy">
                        </asp:TextBox>

                    </div>

                </div>


                <div class="row">

                    <div class="col-md-12">

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
                CausesValidation="false"
                OnClick="btnReset_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- Certificate Grid -->

        <div class="card certificate-card">

            <div class="card-header bg-primary text-white">

                <b>Certificate List
                </b>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvCertificate"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover certificate-table"
                        GridLines="None"
                        EmptyDataText="No generated certificate found."
                        OnRowCommand="gvCertificate_RowCommand">

                        <Columns>


                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>

                                    <%#
                                        Container.DataItemIndex + 1
                                    %>
                                </ItemTemplate>

                                <ItemStyle
                                    Width="65px"
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="CertificateNo"
                                HeaderText="Certificate No.">

                                <ItemStyle
                                    CssClass="certificate-no" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TrainingID"
                                HeaderText="Training ID">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="CourseName"
                                HeaderText="Course" />



                            <asp:BoundField
                                DataField="EmpID"
                                HeaderText="Employee ID">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TraineeName"
                                HeaderText="Trainee Name" />



                            <asp:BoundField
                                DataField="TrainingDuration"
                                HeaderText="Training Duration">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="GeneratedOn"
                                HeaderText="Generated On"
                                DataFormatString="{0:dd-MM-yyyy hh:mm tt}">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Status">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblStatus"
                                        runat="server"
                                        Text='<%#
                                            Eval("CertificateStatus").ToString() == "A"
                                            ? "Active"
                                            : "Inactive"
                                        %>'
                                        CssClass='<%#
                                            Eval("CertificateStatus").ToString() == "A"
                                            ? "status-active"
                                            : "status-inactive"
                                        %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>


                         

                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>

                                    <div class="certificate-actions">

                                        <asp:LinkButton
                                            ID="btnView"
                                            runat="server"
                                            Text="View"
                                            CssClass="btn btn-primary btn-sm"
                                            CommandName="ViewCertificate"
                                            CommandArgument='<%# Eval("CertificateID") %>'
                                            CausesValidation="false">
                                        </asp:LinkButton>

                                        &nbsp;

                                        <asp:LinkButton
                                            ID="btnDownload"
                                            runat="server"
                                            Text="Download"
                                            CssClass="btn btn-success btn-sm"
                                            CommandName="DownloadCertificate"
                                            CommandArgument='<%# Eval("CertificateID") %>'
                                            CausesValidation="false">
                                        </asp:LinkButton>

                                    </div>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-certificate">
                                No generated certificate found.

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
