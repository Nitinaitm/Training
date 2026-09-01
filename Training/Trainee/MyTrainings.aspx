<%@ Page title="My Trainings"
    language="C#"
    masterpagefile="~/TraineeMaster.Master"
    autoeventwireup="true"
    codebehind="MyTrainings.aspx.cs"
    inherits="Training.Trainee.MyTrainings" %>

<%@ Register src="~/Trainee/TraineeTrainingSummary.ascx"
    tagprefix="uc1"
    tagname="TraineeTrainingSummary" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        .search-card {
            border: 0;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
        }
         .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle
        }

        .gridview td {
            vertical-align: middle
        }

        .grid-card {
            border: 0;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
        }

        .badge-status {
            font-size: 13px;
            padding: 6px 10px;
        }

        .progress {
            height: 20px;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
        }

        .table td,
        .table th {
            vertical-align: middle !important;
        }

        .btn-group .btn {
            margin-right: 4px;
        }

        .btn-group {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
        }

        .progress {
            min-width: 120px;
        }

        .badge-status {
            min-width: 110px;
            display: inline-block;
            text-align: center;
        }

        .btn[disabled],
        .btn.disabled {
            cursor: not-allowed;
            opacity: .55;
        }

    </style>

</asp:Content>

<asp:content id="Content2"
    contentplaceholderid="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="row mb-3">

            <div class="col-md-12">

                <h3 class="page-title">My Trainings
                </h3>

            </div>

        </div>

        <!-- SEARCH -->

        <div class="card search-card mb-3">

            <div class="card-body">

                <div class="row">

                    <div class="col-md-3 mb-2">

                        <label>
                            Training ID
                        </label>

                        <asp:TextBox
                            ID="txtTrainingID"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Training ID">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3 mb-2">

                        <label>
                            Course
                        </label>

                        <asp:DropDownList
    ID="ddlCourse"
    runat="server"
    CssClass="form-control">
</asp:DropDownList>

                    </div>

                    <div class="col-md-3 mb-2">

                        <label>
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="All" Value=""></asp:ListItem>

                            <asp:ListItem Text="Pending" Value="P"></asp:ListItem>

                            <asp:ListItem Text="In Progress" Value="I"></asp:ListItem>

                            <asp:ListItem Text="Completed" Value="C"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3">

                        <label>&nbsp;</label>

                        <div>

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
                                OnClick="btnReset_Click" />

                        </div>

                    </div>

                </div>

            </div>

        </div>

        <!-- GRID -->

        <div class="card grid-card">

            <div class="card-body table-responsive">

                <asp:GridView
                    ID="gvTraining"
                    runat="server"
                    CssClass="table table-bordered table-hover gridview"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    AllowSorting="True"
                    PageSize="10"
                    DataKeyNames="TrainingID"
                    EmptyDataText="No Training Assigned."
                    OnPageIndexChanging="gvTraining_PageIndexChanging"
                    OnSorting="gvTraining_Sorting"
OnRowCommand="gvTraining_RowCommand"
OnRowDataBound="gvTraining_RowDataBound">
                    <Columns>

                        <asp:BoundField
                            HeaderText="Training ID"
                            DataField="TrainingID"
                            />

                        <asp:BoundField
                            HeaderText="Course"
                            DataField="CourseName"
                           />

                        <asp:BoundField
                            HeaderText="Training Type"
                            DataField="TrainingType"
                            />

                        <asp:BoundField
                            HeaderText="Organizer"
                            DataField="TrainingOrganizer"
                            />

                        <asp:BoundField
                            HeaderText="Batch"
                            DataField="Batch"
                            />

                        <asp:BoundField
                            HeaderText="From"
                            DataField="DateFrom"
                            DataFormatString="{0:dd-MMM-yyyy}" />

                        <asp:BoundField
                            HeaderText="To"
                            DataField="DateTo"
                            DataFormatString="{0:dd-MMM-yyyy}" />

                       

                        <asp:TemplateField HeaderText="Status">

                            <ItemTemplate>

                              <asp:Label
    ID="lblStatus"
    runat="server"
  
    Text='<%# Eval("StatusText") %>'>
</asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>

                       <asp:TemplateField HeaderText="Action">

    <ItemTemplate>

        <div class="btn-group">

            <asp:LinkButton
                ID="lnkView"
                runat="server"
                CssClass="btn btn-success btn-sm"
                CommandName="ViewTraining"
                CommandArgument='<%# Eval("TrainingID") %>'>

                <i class="fa fa-eye"></i>
                View

            </asp:LinkButton>

            <asp:LinkButton
                ID="lnkAttendance"
                runat="server"
                CssClass="btn btn-primary btn-sm"
                CommandName="Attendance"
                CommandArgument='<%# Eval("TrainingID") %>'>

                <i class="fa fa-calendar-check-o"></i>
                Attendance

            </asp:LinkButton>

            <asp:LinkButton
                ID="lnkFeedback"
                runat="server"
                CssClass="btn btn-warning btn-sm"
                CommandName="BatchFeedback"
                CommandArgument='<%# Eval("TrainingID") %>'
                Enabled='<%# Convert.ToBoolean(Eval("CanBatchFeedback")) %>'>

                <i class="fa fa-comments"></i>
                Feedback

            </asp:LinkButton>

            <asp:LinkButton
                ID="lnkCertificate"
                runat="server"
                CssClass="btn btn-info btn-sm"
                CommandName="Certificate"
                CommandArgument='<%# Eval("TrainingID") %>'
                Enabled='<%# Convert.ToBoolean(Eval("CanCertificate")) %>'>

                <i class="fa fa-certificate"></i>
                Certificate

            </asp:LinkButton>

        </div>

    </ItemTemplate>

</asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:content>
