<%@ Page Title="Attendance"
    Language="C#"
    MasterPageFile="~/TraineeMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Attendance.aspx.cs"
    Inherits="Training.Trainee.Attendance" %>

<%@ Register
    Src="~/Trainee/TraineeTrainingSummary.ascx"
    TagPrefix="uc1"
    TagName="TraineeTrainingSummary" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        body {
            background: #f5f7fb;
        }

        .page-title {
            font-size: 28px;
            font-weight: 600;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .card-box {
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,.08);
            margin-bottom: 20px;
        }

        .card-header-custom {
            background: #0d6efd;
            color: #ffffff;
            padding: 12px 18px;
            font-size: 18px;
            font-weight: 600;
            border-radius: 10px 10px 0 0;
        }

        .card-body-custom {
            padding: 20px;
        }

        .label-title {
            font-weight: 600;
            color: #666666;
        }

        .label-value {
            font-size: 16px;
            font-weight: 500;
            color: #222222;
        }

        .status-present {
            background: #198754;
            color: #ffffff;
            padding: 5px 10px;
            border-radius: 4px;
        }

        .status-absent {
            background: #dc3545;
            color: #ffffff;
            padding: 5px 10px;
            border-radius: 4px;
        }

        .status-pending {
            background: #ffc107;
            color: #000000;
            padding: 5px 10px;
            border-radius: 4px;
        }

        .grid {
            width: 100%;
        }

            .grid th {
                background: #0d6efd;
                color: #ffffff;
                text-align: center;
            }

            .grid td {
                vertical-align: middle;
            }

        .btn-complete {
            width: 220px;
            font-weight: 600;
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="row">

            <div class="col-md-12">

                <h3 class="page-title">Attendance

                </h3>

            </div>

        </div>
                <uc1:TraineeTrainingSummary
    ID="TraineeTrainingSummary1"
    runat="server" />

       

        <div class="card-box">

            <div class="card-header-custom">
                Session Attendance

            </div>

            <div class="card-body-custom">

                <asp:GridView
                    ID="gvAttendance"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-bordered table-hover grid"
                    GridLines="None"
                    EmptyDataText="No session found.">

                    <Columns>

                        <asp:BoundField
                            DataField="SessionNo"
                            HeaderText="Session No">

                            <ItemStyle
                                HorizontalAlign="Center"
                                Width="90px" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="SessionName"
                            HeaderText="Session Name" />

                        <asp:BoundField
                            DataField="SessionDate"
                            HeaderText="Date"
                            DataFormatString="{0:dd-MMM-yyyy}">

                            <ItemStyle
                                Width="130px"
                                HorizontalAlign="Center" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="StartTime"
                            HeaderText="Start Time">

                            <ItemStyle
                                Width="120px"
                                HorizontalAlign="Center" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="EndTime"
                            HeaderText="End Time">

                            <ItemStyle
                                Width="120px"
                                HorizontalAlign="Center" />

                        </asp:BoundField>
                        <asp:TemplateField
                            HeaderText="Attendance Status">

                            <ItemStyle
                                Width="150px"
                                HorizontalAlign="Center" />

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblAttendanceStatus"
                                    runat="server"
                                    Text='<%# Eval("AttendanceStatus") %>'
                                    CssClass='<%#
        Eval("AttendanceStatus").ToString()=="Present"
        ?
        "status-present"
        :
        Eval("AttendanceStatus").ToString()=="Absent"
        ?
        "status-absent"
        :
        "status-pending"
        %>'>
                                </asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:BoundField
                            DataField="MarkedOn"
                            HeaderText="Marked On"
                            DataFormatString="{0:dd-MMM-yyyy HH:mm}">

                            <ItemStyle
                                Width="170px"
                                HorizontalAlign="Center" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="MarkedBy"
                            HeaderText="Marked By">

                            <ItemStyle
                                Width="150px"
                                HorizontalAlign="Center" />

                        </asp:BoundField>

                        <asp:BoundField
                            DataField="Remarks"
                            HeaderText="Remarks" />

                    </Columns>

                    <HeaderStyle
                        HorizontalAlign="Center" />

                    <RowStyle
                        HorizontalAlign="Center" />

                    <EmptyDataRowStyle
                        CssClass="text-center text-danger"
                        Font-Bold="true" />

                </asp:GridView>

            </div>

        </div>

        <div class="card-box">

            <div class="card-header-custom">
                Attendance Completion

            </div>

            <div class="card-body-custom">

                <div class="row">

                    <div class="col-md-12 text-center">

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Font-Bold="true">
                        </asp:Label>
                        <asp:BulletedList
    ID="blPending"
    runat="server"
    BulletStyle="Disc"
    CssClass="text-danger">
</asp:BulletedList>
                    </div>

                </div>

                <br />

                <div class="row">

                    <div class="col-md-12 text-center">

                       
                        <asp:Button
                            ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-secondary"
                            CausesValidation="false"
                            PostBackUrl="~/Trainee/MyTrainings.aspx" />

                    </div>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
