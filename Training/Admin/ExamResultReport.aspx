<%@ Page Title="Exam Result Report"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="ExamResultReport.aspx.cs"
    Inherits="Training.Admin.ExamResultReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

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

        .summary-box {
            background: #ffffff;
            border: 1px solid #e5e5e5;
            border-radius: 8px;
            padding: 18px;
            text-align: center;
            min-height: 110px;
            margin-bottom: 15px;
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

        .passed-value {
            color: #198754;
        }

        .failed-value {
            color: #dc3545;
        }

        .average-value {
            color: #6f42c1;
        }

        .result-pass {
            color: #198754;
            font-weight: bold;
        }

        .result-fail {
            color: #dc3545;
            font-weight: bold;
        }

        .improvement-positive {
            color: #198754;
            font-weight: bold;
        }

        .improvement-negative {
            color: #dc3545;
            font-weight: bold;
        }

        .report-table {
            width: 100%;
        }

            .report-table th {
                background-color: #0d6efd;
                color: #ffffff;
                text-align: center;
                vertical-align: middle;
                white-space: nowrap;
                font-weight: 600;
            }

            .report-table td {
                vertical-align: middle;
            }

        .section-title {
            font-size: 18px;
            font-weight: 600;
        }

        .empty-data {
            text-align: center;
            padding: 25px;
            color: #6c757d;
        }

        .message-area {
            display: block;
            margin-bottom: 15px;
            font-weight: bold;
        }

        .percentage-text {
            font-weight: bold;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <!-- ================================================= -->
        <!-- PAGE TITLE                                        -->
        <!-- ================================================= -->

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">Exam Result Report
                </span>

            </div>

        </div>


        <asp:label
            id="lblMessage"
            runat="server"
            cssclass="message-area">
        </asp:label>


        <!-- ================================================= -->
        <!-- FILTERS                                           -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="card-header bg-primary text-white">

                <b>Search Result
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Training -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Training
                        </label>

                        <asp:dropdownlist
                            id="ddlTraining"
                            runat="server"
                            cssclass="form-control"
                            autopostback="true"
                            onselectedindexchanged="ddlTraining_SelectedIndexChanged">
                        </asp:dropdownlist>

                    </div>


                    <!-- Course -->

                    <div class="col-md-4 mb-3">

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

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Batch
                        </label>

                        <asp:textbox
                            id="txtBatch"
                            runat="server"
                            cssclass="form-control"
                            maxlength="100"
                            placeholder="Batch">
                        </asp:textbox>

                    </div>

                </div>


                <div class="row">

                    <!-- Test Type -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Test Type
                        </label>

                        <asp:dropdownlist
                            id="ddlTestType"
                            runat="server"
                            cssclass="form-control"
                            autopostback="true"
                            onselectedindexchanged="ddlTestType_SelectedIndexChanged">

                            <asp:ListItem
                                Text="-- All Test Types --"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Pre Training Exam"
                                Value="Pre">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Post Training Exam"
                                Value="Post">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>


                    <!-- Test -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Test
                        </label>

                        <asp:dropdownlist
                            id="ddlTest"
                            runat="server"
                            cssclass="form-control">
                        </asp:dropdownlist>

                    </div>


                    <!-- Trainee -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Trainee ID / Name
                        </label>

                        <asp:textbox
                            id="txtTrainee"
                            runat="server"
                            cssclass="form-control"
                            maxlength="150"
                            placeholder="Search trainee">
                        </asp:textbox>

                    </div>


                    <!-- Result -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Result Status
                        </label>

                        <asp:dropdownlist
                            id="ddlResultStatus"
                            runat="server"
                            cssclass="form-control">

                            <asp:ListItem
                                Text="-- All Results --"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Pass"
                                Value="PASS">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Fail"
                                Value="FAIL">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>

                </div>


                <div class="row">

                    <!-- Attempt -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Attempt
                        </label>

                        <asp:dropdownlist
                            id="ddlAttempt"
                            runat="server"
                            cssclass="form-control">

                            <asp:ListItem
                                Text="Final Attempt"
                                Value="Final"
                                Selected="True">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="All Attempts"
                                Value="All">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>


                    <!-- From Date -->

                    <div class="col-md-3 mb-3">

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

                    <div class="col-md-3 mb-3">

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


                    <!-- Buttons -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            &nbsp;
                        </label>

                        <asp:button
                            id="btnSearch"
                            runat="server"
                            text="Search"
                            cssclass="btn btn-primary"
                            onclick="btnSearch_Click" />

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


        <!-- ================================================= -->
        <!-- SUMMARY                                           -->
        <!-- ================================================= -->

        <asp:panel
            id="pnlSummary"
            runat="server"
            visible="false">

            <div class="row">

                <!-- Appeared -->

                <div class="col-md-3">

                    <div class="summary-box">

                        <span class="summary-title">
                            Total Results
                        </span>

                        <asp:Label
                            ID="lblTotalResults"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Passed -->

                <div class="col-md-3">

                    <div class="summary-box">

                        <span class="summary-title">
                            Passed
                        </span>

                        <asp:Label
                            ID="lblPassed"
                            runat="server"
                            Text="0"
                            CssClass="summary-value passed-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Failed -->

                <div class="col-md-3">

                    <div class="summary-box">

                        <span class="summary-title">
                            Failed
                        </span>

                        <asp:Label
                            ID="lblFailed"
                            runat="server"
                            Text="0"
                            CssClass="summary-value failed-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Average -->

                <div class="col-md-3">

                    <div class="summary-box">

                        <span class="summary-title">
                            Average Percentage
                        </span>

                        <asp:Label
                            ID="lblAveragePercentage"
                            runat="server"
                            Text="0.00 %"
                            CssClass="summary-value average-value">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </asp:panel>


        <!-- ================================================= -->
        <!-- RESULT GRID                                       -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="card-header bg-success text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Trainee Exam Results
                        </span>

                    </div>


                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportResult"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportResult_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvResult"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowdatabound="gvResult_RowDataBound">

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



                            <asp:TemplateField
                                HeaderText="Exam">

                                <ItemTemplate>

                                    <%#
                                        Eval("TestType").ToString() == "Pre"
                                        ? "Pre Training"
                                        :
                                        Eval("TestType").ToString() == "Post"
                                        ? "Post Training"
                                        : Eval("TestType").ToString()
                                    %>

                                </ItemTemplate>

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="TestTitle"
                                HeaderText="Test Title" />



                            <asp:BoundField
                                DataField="EmpID"
                                HeaderText="Trainee ID" />



                            <asp:BoundField
                                DataField="TraineeName"
                                HeaderText="Trainee Name" />



                            <asp:BoundField
                                DataField="AttemptNo"
                                HeaderText="Attempt">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TotalQuestions"
                                HeaderText="Questions">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="AttemptedQuestions"
                                HeaderText="Attempted">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="CorrectAnswers"
                                HeaderText="Correct">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="WrongAnswers"
                                HeaderText="Wrong">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TotalMarks"
                                HeaderText="Total Marks">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="ObtainedMarks"
                                HeaderText="Obtained">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Percentage">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblPercentage"
                                        runat="server"
                                        CssClass="percentage-text"
                                        Text='<%# Eval("Percentage", "{0:0.00}") + " %" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>



                            <asp:TemplateField
                                HeaderText="Result">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblResult"
                                        runat="server"
                                        Text='<%# Eval("ResultStatus") %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="RankNo"
                                HeaderText="Rank">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Time Taken">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblTimeTaken"
                                        runat="server"
                                        Text='<%# FormatTimeTaken(Eval("TimeTaken")) %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Wrap="false" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="SubmittedOn"
                                HeaderText="Submitted On"
                                DataFormatString="{0:dd-MM-yyyy hh:mm tt}">

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Wrap="false" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Final">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblFinalAttempt"
                                        runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("IsFinalAttempt")) ? "Yes" : "No" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No exam result found.
                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- PRE VS POST COMPARISON                            -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="card-header bg-info text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Pre vs Post Training Comparison
                        </span>

                    </div>


                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportComparison"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportComparison_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:gridview
                        id="gvComparison"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowdatabound="gvComparison_RowDataBound">

    <Columns>

        <asp:TemplateField HeaderText="Sl. No.">

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
            DataField="EmpID"
            HeaderText="Trainee ID" />


        <asp:BoundField
            DataField="TraineeName"
            HeaderText="Trainee Name" />


        <asp:TemplateField HeaderText="Pre %">

            <ItemTemplate>

                <asp:Label
                    ID="lblPre"
                    runat="server"
                    Text='<%# FormatPercentage(Eval("PrePercentage")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>


        <asp:TemplateField HeaderText="Post %">

            <ItemTemplate>

                <asp:Label
                    ID="lblPost"
                    runat="server"
                    Text='<%# FormatPercentage(Eval("PostPercentage")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>


        <asp:TemplateField HeaderText="Improvement">

            <ItemTemplate>

                <asp:Label
                    ID="lblImprovement"
                    runat="server"
                    Text='<%# FormatImprovement(Eval("Improvement")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>

    </Columns>


    <EmptyDataTemplate>

        <div class="empty-data">

            No Pre/Post comparison data found.

        </div>

    </EmptyDataTemplate>

</asp:gridview>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
