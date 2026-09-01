<%@ Page Title="Trainer Dashboard"
    Language="C#"
    MasterPageFile="~/TrainerMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Training.Trainer.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .main-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px;
        }

        .summary-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 8px #d9d9d9;
            padding: 18px;
            text-align: center;
            margin-bottom: 20px;
            transition: .3s;
        }

            .summary-card:hover {
                transform: translateY(-3px);
            }

        .summary-value {
            font-size: 30px;
            font-weight: bold;
            color: #0d6efd;
        }

        .summary-label {
            color: #666;
            font-size: 14px;
        }

        .search-card {
            background: #f8f9fa;
            border-radius: 10px;
            padding: 18px;
            margin-top: 15px;
            margin-bottom: 20px;
        }

        .grid-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 8px #d9d9d9;
            padding: 20px;
        }

        .grid-title {
            font-size: 22px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 15px;
        }

         .gridview th {
            background: #198754;
            color: #ffffff;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
        }

        .btn-search {
            min-width: 110px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Trainer Dashboard

            </div>

            <div class="row">

                <div class="col-lg-3 col-md-6">

                    <div class="summary-card">

                        <div class="summary-value">

                            <asp:Label
                                ID="lblTodaySession"
                                runat="server"
                                Text="0" />

                        </div>

                        <div class="summary-label">
                            Today's Sessions

                        </div>

                    </div>

                </div>

                <div class="col-lg-3 col-md-6">

                    <div class="summary-card">

                        <div class="summary-value">

                            <asp:Label
                                ID="lblPendingAttendance"
                                runat="server"
                                Text="0" />

                        </div>

                        <div class="summary-label">
                            Pending Attendance

                        </div>

                    </div>

                </div>

                <div class="col-lg-3 col-md-6">

                    <div class="summary-card">

                        <div class="summary-value">

                            <asp:Label
                                ID="lblPendingPreTest"
                                runat="server"
                                Text="0" />

                        </div>

                        <div class="summary-label">
                            Pending Pre Test

                        </div>

                    </div>

                </div>

                <div class="col-lg-3 col-md-6">

                    <div class="summary-card">

                        <div class="summary-value">

                            <asp:Label
                                ID="lblPendingPostTest"
                                runat="server"
                                Text="0" />

                        </div>

                        <div class="summary-label">
                            Pending Post Test

                        </div>

                    </div>

                </div>

            </div>

            <div class="search-card">

                <div class="row">

                    <div class="col-md-3">

                        <label>
                            Course

                        </label>

                        <asp:DropDownList
                            ID="ddlCourse"
                            runat="server"
                            CssClass="form-select" />

                    </div>

                    <div class="col-md-2">

                        <label>
                            Batch

                        </label>

                        <asp:DropDownList
                            ID="ddlBatch"
                            runat="server"
                            CssClass="form-select" />

                    </div>

                    <div class="col-md-2">

                        <label>
                            From Date

                        </label>

                        <asp:TextBox
                            ID="txtFromDate"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-2">

                        <label>
                            To Date

                        </label>

                        <asp:TextBox
                            ID="txtToDate"
                            runat="server"
                            CssClass="form-control" />

                    </div>

                    <div class="col-md-3">

                        <label>&nbsp;</label>

                        <br />

                        <asp:Button
                            ID="btnSearch"
                            runat="server"
                            Text="Search"
                            CssClass="btn btn-success btn-search"
                            OnClick="btnSearch_Click" />

                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-secondary btn-search"
                            OnClick="btnReset_Click" />

                    </div>

                </div>

            </div>

            <div class="grid-card">

                <div class="grid-title">
                    Assigned Sessions

                </div>
                <asp:GridView
                    ID="gvSession"
                    runat="server"
                    AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover gridview"
                    Width="100%"
                    EmptyDataText="No Session Assigned"
                    ShowHeaderWhenEmpty="true"
                    DataKeyNames="SessionID,TrainingID" OnRowCommand="gvSession_RowCommand">

                    <HeaderStyle
                        BackColor="#198754"
                        ForeColor="White"
                        Font-Bold="true" />

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="Center" />
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
                            DataField="SessionNo"
                            HeaderText="Session No" />

                        <asp:BoundField
                            DataField="SessionName"
                            HeaderText="Session Name" />

                        <asp:BoundField
                            DataField="TopicName"
                            HeaderText="Topic" />

                        <asp:BoundField
                            DataField="SessionDate"
                            HeaderText="Session Date" />

                        <asp:BoundField
                            DataField="StartTime"
                            HeaderText="Start Time" />

                        <asp:BoundField
                            DataField="EndTime"
                            HeaderText="End Time" />

                       

                        <asp:TemplateField HeaderText="Attendance">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblAttendance"
                                    runat="server"
                                    Text='<%# Eval("AttendanceStatus") %>'
                                    CssClass="badge bg-success" />

                            </ItemTemplate>

                            <ItemStyle Width="110px" HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">

                            <ItemTemplate>

                                <asp:Button
                                    ID="btnView"
                                    runat="server"
                                    Text="View"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="View"
                                    CommandArgument='<%# Eval("SessionID") %>' />

                            </ItemTemplate>

                            <ItemStyle
                                Width="90px"
                                HorizontalAlign="Center" />

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>
