<%@ Page Title="My Feedback Report"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="FeedbackReport.aspx.cs"
    Inherits="Training.Trainer.FeedbackReport" %>

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
            background-color: #ffffff;
            border: 1px solid #e5e5e5;
            border-radius: 8px;
            box-shadow: 0 2px 6px rgba(0,0,0,.08);
            padding: 18px;
            text-align: center;
            margin-bottom: 15px;
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

        .rating-value {
            font-weight: bold;
            color: #198754;
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
        }

        .report-table td {
            vertical-align: middle;
        }

        .section-title {
            font-size: 18px;
            font-weight: 600;
        }

        .empty-data {
            padding: 25px;
            text-align: center;
            color: #6c757d;
        }

        .comment-box {
            padding: 12px;
            margin-bottom: 10px;
            border: 1px solid #e5e5e5;
            border-radius: 6px;
            background-color: #f8f9fa;
        }

        .comment-question {
            display: block;
            font-weight: 600;
            margin-bottom: 5px;
            color: #495057;
        }

        .comment-text {
            display: block;
            color: #212529;
        }

        .message-area {
            display: block;
            margin-bottom: 15px;
            font-weight: bold;
        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <!-- Page Heading -->

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">
                    My Feedback Report
                </span>

            </div>

        </div>


        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="message-area">
        </asp:Label>


        <!-- Trainer Information -->

        <div class="card report-card">

            <div class="card-header bg-dark text-white">

                <b>
                    Trainer Details
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-4">

                        <b>
                            Trainer ID:
                        </b>

                        <asp:Label
                            ID="lblTrainerID"
                            runat="server">
                        </asp:Label>

                    </div>


                    <div class="col-md-4">

                        <b>
                            Trainer Name:
                        </b>

                        <asp:Label
                            ID="lblTrainerName"
                            runat="server">
                        </asp:Label>

                    </div>


                    <div class="col-md-4">

                        <b>
                            Trainer Type:
                        </b>

                        <asp:Label
                            ID="lblTrainerType"
                            runat="server">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </div>


        <!-- Filters -->

        <div class="card report-card">

            <div class="card-header bg-primary text-white">

                <b>
                    Search Feedback
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Training -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Training
                        </label>

                        <asp:DropDownList
                            ID="ddlTraining"
                            runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>

                    </div>


                    <!-- From Date -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Submitted From
                        </label>

                        <asp:TextBox
                            ID="txtFromDate"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:TextBox>

                    </div>


                    <!-- To Date -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Submitted To
                        </label>

                        <asp:TextBox
                            ID="txtToDate"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-2 mb-3">

                        <label class="filter-label">
                            &nbsp;
                        </label>

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
                            CausesValidation="false"
                            OnClick="btnReset_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- Summary -->

        <asp:Panel
            ID="pnlSummary"
            runat="server"
            Visible="false">

            <div class="row">

                <div class="col-md-4">

                    <div class="summary-card">

                        <span class="summary-title">
                            Trainings With Feedback
                        </span>

                        <asp:Label
                            ID="lblTrainingCount"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <div class="col-md-4">

                    <div class="summary-card">

                        <span class="summary-title">
                            Total Feedback Responses
                        </span>

                        <asp:Label
                            ID="lblResponseCount"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <div class="col-md-4">

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

        </asp:Panel>


        <!-- Training Wise Summary -->

        <div class="card report-card">

            <div class="card-header bg-success text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">
                            Training-wise Feedback
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:Button
                            ID="btnExportTraining"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-light btn-sm"
                            CausesValidation="false"
                            OnClick="btnExportTraining_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvTrainingSummary"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover report-table"
                        GridLines="None"
                        OnRowCommand="gvTrainingSummary_RowCommand">

                        <Columns>

                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Width="65px" />

                            </asp:TemplateField>


                            <asp:BoundField
                                DataField="TrainingID"
                                HeaderText="Training ID" />


                            <asp:BoundField
                                DataField="CourseName"
                                HeaderText="Course" />


                            <asp:BoundField
                                DataField="Batch"
                                HeaderText="Batch" />


                            <asp:BoundField
                                DataField="TrainingDuration"
                                HeaderText="Training Duration" />


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
                                        Text='<%# Eval("AverageRating") %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>


                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnView"
                                        runat="server"
                                        Text="View Details"
                                        CssClass="btn btn-primary btn-sm"
                                        CausesValidation="false"
                                        CommandName="ViewTraining"
                                        CommandArgument='<%# Eval("TrainingID") %>'>
                                    </asp:LinkButton>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No feedback is available.
                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </div>

        </div>


        <!-- Question Wise -->

        <asp:Panel
            ID="pnlQuestionDetail"
            runat="server"
            Visible="false">

            <div class="card report-card">

                <div class="card-header bg-info text-white">

                    <div class="row">

                        <div class="col-md-8">

                            <span class="section-title">

                                Question-wise Feedback -

                                <asp:Label
                                    ID="lblSelectedTraining"
                                    runat="server">
                                </asp:Label>

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

                    <div class="table-responsive">

                        <asp:GridView
                            ID="gvQuestionSummary"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover report-table"
                            GridLines="None">

                            <Columns>

                                <asp:TemplateField
                                    HeaderText="Sl. No.">

                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
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
                                    DataField="TotalResponses"
                                    HeaderText="Responses">

                                    <ItemStyle
                                        HorizontalAlign="Center" />

                                </asp:BoundField>


                                <asp:TemplateField
                                    HeaderText="Average Rating">

                                    <ItemTemplate>

                                        <asp:Label
                                            ID="lblQuestionRating"
                                            runat="server"
                                            CssClass="rating-value"
                                            Text='<%# Eval("AverageRating") %>'>
                                        </asp:Label>

                                    </ItemTemplate>

                                    <ItemStyle
                                        HorizontalAlign="Center" />

                                </asp:TemplateField>

                            </Columns>


                            <EmptyDataTemplate>

                                <div class="empty-data">
                                    No question-wise rating found.
                                </div>

                            </EmptyDataTemplate>

                        </asp:GridView>

                    </div>

                </div>

            </div>


            <!-- Anonymous Comments -->

            <div class="card report-card">

                <div class="card-header bg-secondary text-white">

                    <span class="section-title">
                        Feedback Comments
                    </span>

                </div>


                <div class="card-body">

                    <asp:Repeater
                        ID="rptComments"
                        runat="server">

                        <ItemTemplate>

                            <div class="comment-box">

                                <span class="comment-question">

                                    <%#
                                        Eval("QuestionText")
                                    %>

                                </span>

                                <span class="comment-text">

                                    <%#
                                        Eval("Answer")
                                    %>

                                </span>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>


                    <asp:Label
                        ID="lblNoComments"
                        runat="server"
                        Text="No comments available."
                        ForeColor="#6c757d"
                        Visible="false">
                    </asp:Label>

                </div>

            </div>

        </asp:Panel>

    </div>

</asp:Content>