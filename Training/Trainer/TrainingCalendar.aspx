<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="TrainingCalendar.aspx.cs" Inherits="Training.Trainer.TrainingCalendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .calendar-table {
            width: 100%;
            border-collapse: collapse
        }

            .calendar-table th {
                background: #198754;
                color: white;
                padding: 10px;
                text-align: center
            }

            .calendar-table td {
                height: 80px;
                vertical-align: top;
                padding: 5px;
                border: 1px solid #ddd
            }

            .calendar-table .day-number {
                font-weight: bold;
                font-size: 16px
            }

            .calendar-table .event {
                background: #e8f5e9;
                border-radius: 4px;
                padding: 2px 5px;
                margin: 2px 0;
                font-size: 12px;
                cursor: pointer
            }

                .calendar-table .event:hover {
                    background: #c8e6c9
                }

            .calendar-table .today {
                background: #fff3cd
            }

        .nav-arrows {
            font-size: 24px;
            cursor: pointer
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Training Calendar</div>
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-4">
                    <h4>
                        <asp:Label ID="lblMonthYear" runat="server" /></h4>
                </div>
                <div class="col-md-8 text-end">
                    <asp:LinkButton ID="lnkPrev" runat="server" CssClass="nav-arrows" OnClick="lnkPrev_Click">◀</asp:LinkButton><asp:LinkButton ID="lnkNext" runat="server" CssClass="nav-arrows ms-3" OnClick="lnkNext_Click">▶</asp:LinkButton><asp:Button ID="btnToday" runat="server" Text="Today" CssClass="btn btn-primary ms-3" OnClick="btnToday_Click" /></div>
            </div>
            <asp:Table ID="tblCalendar" runat="server" CssClass="calendar-table" />
            <div class="mt-3">
                <h6>Legend:</h6>
                <span class="badge bg-success">Completed</span><span class="badge bg-warning ms-2">Pending</span><span class="badge bg-info ms-2">Upcoming</span></div>
        </div>
    </div>
</asp:Content>
