<%@ Page Title="Exam Result Report"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="ExamResultReport.aspx.cs"
    Inherits="Training.Trainer.ExamResultReport" %>

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

        .trainer-info {
            font-size: 14px;
            color: #6c757d;
        }

        .trainer-name {
            font-weight: 600;
            color: #212529;
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


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <!-- ================================================= -->
        <!-- PAGE TITLE                                        -->
        <!-- ================================================= -->

        <div class="row mb-3">

            <div class="col-md-7">

                <span class="page-title">
                    Exam Result Report
                </span>

            </div>

            <div class="col-md-5 text-right">

                <span class="trainer-info">

                    Trainer:

                    <asp:Label
                        ID="lblTrainerName"
                        runat="server"
                        CssClass="trainer-name">
                    </asp:Label>

                    &nbsp;|&nbsp;

                    ID:

                    <asp:Label
                        ID="lblTrainerID"
                        runat="server"
                        CssClass="trainer-name">
                    </asp:Label>

                </span>

            </div>

        </div>


        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="message-area">
        </asp:Label>


        <!-- ================================================= -->
        <!-- FILTER                                            -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="card-header bg-primary text-white">

                <b>
                    Search Result
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
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged">
                        </asp:DropDownList>

                    </div>


                    <!-- Test Type -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Test Type
                        </label>

                        <asp:DropDownList
                            ID="ddlTestType"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlTestType_SelectedIndexChanged">

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

                        </asp:DropDownList>

                    </div>


                    <!-- Test -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Test
                        </label>

                        <asp:DropDownList
                            ID="ddlTest"
                            runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>

                    </div>

                </div>


                <div class="row">

                    <!-- Trainee -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Trainee ID / Name
                        </label>

                        <asp:TextBox
                            ID="txtTrainee"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="150"
                            placeholder="Search trainee">
                        </asp:TextBox>

                    </div>


                    <!-- Result -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Result Status
                        </label>

                        <asp:DropDownList
                            ID="ddlResultStatus"
                            runat="server"
                            CssClass="form-control">

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

                        </asp:DropDownList>

                    </div>


                    <!-- Attempt -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Attempt
                        </label>

                        <asp:DropDownList
                            ID="ddlAttempt"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Text="Final Attempt"
                                Value="Final"
                                Selected="True">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="All Attempts"
                                Value="All">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- Batch -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Batch
                        </label>

                        <asp:TextBox
                            ID="txtBatch"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="100"
                            placeholder="Batch">
                        </asp:TextBox>

                    </div>

                </div>


                <div class="row">

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


                    <!-- Buttons -->

                    <div class="col-md-6 mb-3">

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


        <!-- ================================================= -->
        <!-- SUMMARY                                           -->
        <!-- ================================================= -->

        <asp:Panel
            ID="pnlSummary"
            runat="server"
            Visible="false">

            <div class="row">

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

        </asp:Panel>


        <!-- ================================================= -->
        <!-- RESULT GRID                                       -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="card-header bg-success text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">
                            Trainee Exam Results
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:Button
                            ID="btnExportResult"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-light btn-sm"
                            CausesValidation="false"
                            OnClick="btnExportResult_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvResult"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover report-table"
                        GridLines="None"
                        OnRowDataBound="gvResult_RowDataBound">

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

                    </asp:GridView>

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

                        <span class="section-title">
                            Pre vs Post Training Comparison
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:Button
                            ID="btnExportComparison"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-light btn-sm"
                            CausesValidation="false"
                            OnClick="btnExportComparison_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvComparison"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover report-table"
                        GridLines="None"
                        OnRowDataBound="gvComparison_RowDataBound">

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
                                DataField="EmpID"
                                HeaderText="Trainee ID" />


                            <asp:BoundField
                                DataField="TraineeName"
                                HeaderText="Trainee Name" />


                            <asp:TemplateField
                                HeaderText="Pre %">

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


                            <asp:TemplateField
                                HeaderText="Post %">

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


                            <asp:TemplateField
                                HeaderText="Improvement">

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

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>