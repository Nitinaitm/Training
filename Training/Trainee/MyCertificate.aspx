<%@ Page Title="My Certificates"
    Language="C#"
    MasterPageFile="~/TraineeMaster.Master"
    AutoEventWireup="true"
    CodeBehind="MyCertificate.aspx.cs"
    Inherits="Training.Trainee.MyCertificate" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #0d6efd;
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

                .certificate-table td.text-center {
                    text-align: center;
                }

        .certificate-no {
            font-weight: 600;
            color: #0d6efd;
        }

        .empty-certificate {
            padding: 30px;
            text-align: center;
            font-size: 16px;
            color: #6c757d;
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

        .certificate-actions {
            white-space: nowrap;
        }

        @media (max-width: 768px) {

            .page-title {
                font-size: 20px;
            }

            .certificate-table {
                min-width: 950px;
            }
        }
    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">My Certificates
                </span>

            </div>

        </div>


        <asp:label
            id="lblMessage"
            runat="server"
            font-bold="true">
        </asp:label>


        <div class="card certificate-card">

            <div class="card-header bg-primary text-white">

                <b>Generated Certificates
                </b>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvCertificate"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover certificate-table"
                        gridlines="None"
                        emptydatatext="No certificate has been generated yet."
                        onrowcommand="gvCertificate_RowCommand">

                        <Columns>

                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>

                                    <%#
                                        Container.DataItemIndex + 1
                                    %>

                                </ItemTemplate>

                                <ItemStyle
                                    Width="70px"
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
                                DataField="CourseTitle"
                                HeaderText="Course Title" />


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

                                    <span class="status-active">
                                        Generated
                                    </span>

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
                                            Text="View Certificate"
                                            CssClass="btn btn-primary btn-sm"
                                            CommandName="ViewCertificate"
                                            CommandArgument='<%# Eval("CertificateID") %>'
                                            CausesValidation="false">
                                        </asp:LinkButton>

                                        &nbsp;

                                        <asp:LinkButton
                                            ID="btnDownload"
                                            runat="server"
                                            Text="Download PDF"
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

                                No certificate has been generated yet.

                                <br />

                                Your certificate will appear here after
                                completion of the required training activities.

                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>
                
            </div>
             <div class="row">

                    <div class="col-md-12 text-center">
             <asp:Button
                            ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-secondary"
                            CausesValidation="false"
                            PostBackUrl="~/Trainee/MyTrainings.aspx" />
                        </div></div>
        </div>

    </div>

</asp:Content>
