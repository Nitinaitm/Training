<%@ page title="My Exam Result"
    language="C#"
    masterpagefile="~/TraineeMaster.Master"
    autoeventwireup="true"
    codebehind="MyExamResult.aspx.cs"
    inherits="Training.Trainee.MyExamResult" %>

<%@ Register
    Src="~/Trainee/SessionSummary.ascx"
    TagPrefix="uc1"
    TagName="SessionSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #0d6efd;
        }

        .result-card {
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,.10);
            margin-bottom: 20px;
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
            font-size: 25px;
            font-weight: bold;
            color: #0d6efd;
        }

        .pre-value {
            color: #0d6efd;
        }

        .post-value {
            color: #198754;
        }

        .improvement-positive {
            color: #198754;
        }

        .improvement-negative {
            color: #dc3545;
        }

        .result-pass {
            color: #198754;
            font-weight: bold;
        }

        .result-fail {
            color: #dc3545;
            font-weight: bold;
        }

        .result-table {
            width: 100%;
        }

            .result-table th {
                background-color: #0d6efd;
                color: #ffffff;
                text-align: center;
                vertical-align: middle;
                font-weight: 600;
                white-space: nowrap;
            }

            .result-table td {
                vertical-align: middle;
            }

        .section-title {
            font-size: 18px;
            font-weight: 600;
        }

        .empty-result {
            text-align: center;
            padding: 25px;
            color: #6c757d;
        }

        .filter-label {
            display: block;
            font-weight: 600;
            margin-bottom: 5px;
        }

        .message-area {
            display: block;
            font-weight: bold;
            margin-bottom: 15px;
        }

        .percentage-text {
            font-weight: bold;
        }
        .info-label {
    font-weight: 600;
    color: #6c757d;
    margin-bottom: 4px;
}

.status-badge {
    font-size: 13px;
    padding: 6px 12px;
}
    </style>

</asp:Content>


<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <!-- Page Title -->

       


        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="message-area">
        </asp:Label>


        <!-- Training Details -->

        <div class="card result-card">

            <div class="card-header bg-primary text-white">

                <b>Training Details
                </b>

            </div>

            <div class="card-body">

                <uc1:SessionSummary
    ID="SessionSummary1"
    runat="server" />

            </div>
            <div class="card shadow-sm mb-3">

    <div class="card-header bg-info text-white">

        <i class="fa fa-book"></i>
        Session Details

    </div>

    <div class="card-body">

        <div class="row">

            <div class="col-md-4">

                <div class="info-label">
                    Session No
                </div>

                <asp:Label
                    ID="lblResultSessionNo"
                    runat="server"
                    CssClass="font-weight-bold">
                </asp:Label>

            </div>

            <div class="col-md-4">

                <div class="info-label">
                    Session Name
                </div>

                <asp:Label
                    ID="lblResultSessionName"
                    runat="server"
                    CssClass="font-weight-bold">
                </asp:Label>

            </div>

            <div class="col-md-4">

                <div class="info-label">
                    Result Type
                </div>

                <asp:Label
                    ID="lblResultTestType"
                    runat="server"
                    ForeColor="Black"
                    CssClass="badge badge-primary status-badge">
                </asp:Label>

            </div>

        </div>

    </div>

</div>
        </div>



        <!-- Filters -->

        <div class="card result-card">

            <div class="card-header bg-dark text-white">

                <b>Result Filter
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Test Type -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Test Type
                        </label>

                        <asp:DropDownList
                            ID="ddlTestType"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Text="All Tests"
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


                    <!-- Attempt -->

                    <div class="col-md-4 mb-3">

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


                    <!-- Buttons -->

                    <div class="col-md-4 mb-3">

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


        <!-- Pre vs Post Summary -->

        <asp:Panel
            ID="pnlComparison"
            runat="server"
            Visible="false">

            <div class="card result-card">

                <div class="card-header bg-success text-white">

                    <span class="section-title">Pre vs Post Training Performance
                    </span>

                </div>

                <div class="card-body">

                    <div class="row">

                        <!-- Pre -->

                        <div class="col-md-3">

                            <div class="summary-box">

                                <span class="summary-title">Pre Training %
                                </span>

                                <asp:Label
                                    ID="lblPrePercentage"
                                    runat="server"
                                    Text="-"
                                    CssClass="summary-value pre-value">
                                </asp:Label>

                            </div>

                        </div>


                        <!-- Post -->

                        <div class="col-md-3">

                            <div class="summary-box">

                                <span class="summary-title">Post Training %
                                </span>

                                <asp:Label
                                    ID="lblPostPercentage"
                                    runat="server"
                                    Text="-"
                                    CssClass="summary-value post-value">
                                </asp:Label>

                            </div>

                        </div>


                        <!-- Improvement -->

                        <div class="col-md-3">

                            <div class="summary-box">

                                <span class="summary-title">Improvement
                                </span>

                                <asp:Label
                                    ID="lblImprovement"
                                    runat="server"
                                    Text="-"
                                    CssClass="summary-value">
                                </asp:Label>

                            </div>

                        </div>


                        <!-- Post Result -->

                        <div class="col-md-3">

                            <div class="summary-box">

                                <span class="summary-title">Post Training Result
                                </span>

                                <asp:Label
                                    ID="lblPostResult"
                                    runat="server"
                                    Text="-"
                                    CssClass="summary-value">
                                </asp:Label>

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </asp:Panel>


        <!-- Result Grid -->

        <div class="card result-card">

            <div class="card-header bg-primary text-white">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Exam Results
                        </span>

                    </div>

                    <div class="col-md-4 text-right">

                        <asp:Button
                            ID="btnExport"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-light btn-sm"
                            CausesValidation="false"
                            OnClick="btnExport_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvResult"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="table table-bordered table-hover result-table"
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



                            <asp:TemplateField
                                HeaderText="Exam">

                                <ItemTemplate>

                                    <%#
                                        Eval("TestType").ToString() == "Pre"
                                        ? "Pre Training"
                                        : "Post Training"
                                    %>
                                </ItemTemplate>

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="TestTitle"
                                HeaderText="Test Title" />



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
                                    HorizontalAlign="Center" />

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

                            <div class="empty-result">
                                No exam result is available for this training.

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

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
         
    </div>

</asp:content>
