<%@ Page Title="Feedback Report"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="FeedbackReport.aspx.cs"
    Inherits="Training.Admin.FeedbackReport" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style type="text/css">
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #0d6efd;
        }

        .report-card {
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,.10);
            margin-bottom: 20px;
        }

        .filter-label {
            display: block;
            font-weight: 600;
            margin-bottom: 5px;
        }

        .summary-card {
            border-radius: 8px;
            padding: 18px;
            margin-bottom: 15px;
            background-color: #ffffff;
            border: 1px solid #e5e5e5;
            box-shadow: 0 2px 6px rgba(0,0,0,.08);
            text-align: center;
            min-height: 110px;
        }

        .summary-title {
            display: block;
            font-size: 14px;
            font-weight: 600;
            color: #6c757d;
            margin-bottom: 8px;
        }

        .summary-value {
            display: block;
            font-size: 26px;
            font-weight: bold;
            color: #0d6efd;
        }

        .report-table {
            width: 100%;
        }

            .report-table th {
                background-color: #0d6efd;
                color: #ffffff;
                font-weight: 600;
                text-align: center;
                vertical-align: middle;
                white-space: nowrap;
            }

            .report-table td {
                vertical-align: middle;
            }

        .section-title {
            font-size: 18px;
            font-weight: 600;
        }

        .rating-value {
            font-weight: bold;
            color: #198754;
        }

        .empty-data {
            padding: 25px;
            text-align: center;
            color: #6c757d;
        }

        .action-area {
            white-space: nowrap;
        }

        .status-submitted {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            background-color: #198754;
            color: #ffffff;
            font-size: 12px;
            font-weight: 600;
        }

        .status-pending {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            background-color: #ffc107;
            color: #212529;
            font-size: 12px;
            font-weight: 600;
        }

        .message-area {
            display: block;
            margin-bottom: 15px;
            font-weight: bold;
        }

        @media (max-width: 768px) {

            .page-title {
                font-size: 20px;
            }

            .report-table {
                min-width: 1000px;
            }
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <!-- Page Title -->

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">Training Feedback Report
                </span>

            </div>

        </div>


        <!-- Message -->

        <asp:label
            id="lblMessage"
            runat="server"
            cssclass="message-area">
        </asp:label>


        <!-- Filter -->

        <div class="card report-card">

            <div class="card-header bg-primary text-white">

                <b>Search Feedback
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Training -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Training
                        </label>

                        <asp:dropdownlist
                            id="ddlTraining"
                            runat="server"
                            cssclass="form-control">
                        </asp:dropdownlist>

                    </div>


                    <!-- Course -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Course
                        </label>

                        <asp:dropdownlist
                            id="ddlCourse"
                            runat="server"
                            cssclass="form-control">
                        </asp:dropdownlist>

                    </div>


                    <!-- Batch -->

                    <div class="col-md-2 mb-3">

                        <label class="filter-label">
                            Batch
                        </label>

                        <asp:textbox
                            id="txtBatch"
                            runat="server"
                            cssclass="form-control"
                            maxlength="50"
                            placeholder="Batch">
                        </asp:textbox>

                    </div>


                    <!-- From Date -->

                    <div class="col-md-2 mb-3">

                        <label class="filter-label">
                            Submitted From
                        </label>

                        <asp:textbox
                            id="txtFromDate"
                            runat="server"
                            cssclass="form-control"
                            maxlength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:textbox>

                    </div>


                    <!-- To Date -->

                    <div class="col-md-2 mb-3">

                        <label class="filter-label">
                            Submitted To
                        </label>

                        <asp:textbox
                            id="txtToDate"
                            runat="server"
                            cssclass="form-control"
                            maxlength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:textbox>

                    </div>

                </div>


                <div class="row">

                    <div class="col-md-12">

                        <asp:button
                            id="btnSearch"
                            runat="server"
                            text="Search"
                            cssclass="btn btn-primary"
                            onclick="btnSearch_Click" />

                        &nbsp;

                        <asp:button
                            id="btnReset"
                            runat="server"
                            text="Reset"
                            cssclass="btn btn-secondary"
                            causesvalidation="false"
                            onclick="btnReset_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- Summary -->

        <asp:panel
            id="pnlSummary"
            runat="server"
            visible="false">

            <div class="row">

                <div class="col-md-3">

                    <div class="summary-card">

                        <span class="summary-title">
                            Assigned Trainees
                        </span>

                        <asp:Label
                            ID="lblAssignedTrainees"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <div class="col-md-3">

                    <div class="summary-card">

                        <span class="summary-title">
                            Feedback Submitted
                        </span>

                        <asp:Label
                            ID="lblFeedbackSubmitted"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <div class="col-md-3">

                    <div class="summary-card">

                        <span class="summary-title">
                            Feedback Pending
                        </span>

                        <asp:Label
                            ID="lblFeedbackPending"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <div class="col-md-3">

                    <div class="summary-card">

                        <span class="summary-title">
                            Overall Average Rating
                        </span>

                        <asp:Label
                            ID="lblAverageRating"
                            runat="server"
                            Text="0.00 / 5"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </asp:panel>


        <!-- Question Wise Feedback -->

        <div class="card report-card">

            <div class="card-header bg-success text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Question-wise Feedback Summary
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportQuestion"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportQuestion_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvQuestionSummary"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None">

                        <Columns>

                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>

                                    <%#
                                        Container.DataItemIndex + 1
                                    %>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Width="65px" />

                            </asp:TemplateField>


                            <asp:BoundField
                                DataField="CategoryName"
                                HeaderText="Category" />


                            <asp:BoundField
                                DataField="QuestionText"
                                HeaderText="Question" />


                            <asp:BoundField
                                DataField="AnswerType"
                                HeaderText="Answer Type">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="TotalResponses"
                                HeaderText="Responses">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:TemplateField
                                HeaderText="Average Rating">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblRating"
                                        runat="server"
                                        CssClass="rating-value"
                                        Text='<%#
                                            Eval("AverageRating")
                                        %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No question-wise feedback found.
                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>

            </div>

        </div>


        <!-- Trainer Wise Feedback -->

        <div class="card report-card">

            <div class="card-header bg-info text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Trainer-wise Feedback Summary
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportTrainer"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportTrainer_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvTrainerSummary"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowcommand="gvTrainerSummary_RowCommand">

                        <Columns>

                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>

                                    <%#
                                        Container.DataItemIndex + 1
                                    %>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Width="65px" />

                            </asp:TemplateField>


                            <asp:BoundField
                                DataField="TrainerID"
                                HeaderText="Trainer ID">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="TrainerName"
                                HeaderText="Trainer Name" />


                            <asp:BoundField
                                DataField="TrainerType"
                                HeaderText="Trainer Type">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="TotalResponses"
                                HeaderText="Responses">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:TemplateField
                                HeaderText="Average Rating">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblTrainerRating"
                                        runat="server"
                                        CssClass="rating-value"
                                        Text='<%#
                                            Eval("AverageRating")
                                        %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>


                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnViewTrainer"
                                        runat="server"
                                        Text="View Details"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="ViewTrainer"
                                        CommandArgument='<%#
                                            Eval("TrainerID")
                                        %>'
                                        CausesValidation="false">
                                    </asp:LinkButton>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No trainer feedback found.
                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>

            </div>

        </div>


        <!-- Trainee Wise Feedback -->

        <div class="card report-card">

            <div class="card-header bg-secondary text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Trainee-wise Feedback
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportTrainee"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportTrainee_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvTraineeFeedback"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowcommand="gvTraineeFeedback_RowCommand">

                        <Columns>

                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>

                                    <%#
                                        Container.DataItemIndex + 1
                                    %>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Width="65px" />

                            </asp:TemplateField>


                            <asp:BoundField
                                DataField="TrainingID"
                                HeaderText="Training ID">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="EmpID"
                                HeaderText="Trainee ID">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="TraineeName"
                                HeaderText="Trainee Name" />


                            <asp:BoundField
                                DataField="CourseName"
                                HeaderText="Course" />


                            <asp:BoundField
                                DataField="Batch"
                                HeaderText="Batch">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="SubmittedOn"
                                HeaderText="Submitted On"
                                DataFormatString="{0:dd-MM-yyyy hh:mm tt}">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:TemplateField
                                HeaderText="Status">

                                <ItemTemplate>

                                    <span class="status-submitted">
                                        Submitted
                                    </span>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>


                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>

                                    <div class="action-area">

                                        <asp:LinkButton
                                            ID="btnViewFeedback"
                                            runat="server"
                                            Text="View Feedback"
                                            CssClass="btn btn-primary btn-sm"
                                            CommandName="ViewFeedback"
                                            CommandArgument='<%#
                                                Eval("FeedbackID")
                                            %>'
                                            CausesValidation="false">
                                        </asp:LinkButton>

                                    </div>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No submitted feedback found.
                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>

            </div>

        </div>


        <!-- Feedback Detail -->

        <asp:panel
            id="pnlFeedbackDetail"
            runat="server"
            visible="false">

            <div class="card report-card">

                <div class="card-header bg-dark text-white">

                    <div class="row">

                        <div class="col-md-8">

                            <span class="section-title">
                                Feedback Details
                            </span>

                        </div>

                        <div class="col-md-4 text-right">

                            <asp:Button
                                ID="btnCloseDetail"
                                runat="server"
                                Text="Close"
                                CssClass="btn btn-light btn-sm"
                                CausesValidation="false"
                                OnClick="btnCloseDetail_Click" />

                        </div>

                    </div>

                </div>


                <div class="card-body">

                    <div class="row mb-3">

                        <div class="col-md-4">

                            <b>
                                Training:
                            </b>

                            <asp:Label
                                ID="lblDetailTraining"
                                runat="server">
                            </asp:Label>

                        </div>


                        <div class="col-md-4">

                            <b>
                                Trainee:
                            </b>

                            <asp:Label
                                ID="lblDetailTrainee"
                                runat="server">
                            </asp:Label>

                        </div>


                        <div class="col-md-4">

                            <b>
                                Submitted:
                            </b>

                            <asp:Label
                                ID="lblDetailSubmittedOn"
                                runat="server">
                            </asp:Label>

                        </div>

                    </div>


                    <div class="table-responsive">

                        <asp:GridView
                            ID="gvFeedbackDetail"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover report-table"
                            GridLines="None">

                            <Columns>

                                <asp:TemplateField
                                    HeaderText="Sl. No.">

                                    <ItemTemplate>

                                        <%#
                                            Container.DataItemIndex + 1
                                        %>

                                    </ItemTemplate>

                                    <ItemStyle
                                        HorizontalAlign="Center"
                                        Width="65px" />

                                </asp:TemplateField>


                                <asp:BoundField
                                    DataField="CategoryName"
                                    HeaderText="Category" />


                                <asp:BoundField
                                    DataField="QuestionText"
                                    HeaderText="Question" />


                                <asp:BoundField
                                    DataField="TrainerName"
                                    HeaderText="Trainer" />


                                <asp:BoundField
                                    DataField="AnswerType"
                                    HeaderText="Type">

                                    <ItemStyle
                                        HorizontalAlign="Center" />

                                </asp:BoundField>


                                <asp:BoundField
                                    DataField="Response"
                                    HeaderText="Response" />

                            </Columns>


                            <EmptyDataTemplate>

                                <div class="empty-data">
                                    No feedback details found.
                                </div>

                            </EmptyDataTemplate>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </asp:panel>

    </div>

</asp:Content>
